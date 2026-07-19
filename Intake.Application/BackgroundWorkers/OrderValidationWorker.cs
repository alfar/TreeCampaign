using System.Threading.Channels;
using Common.Infrastructure.Abstractions;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Intake.Application.BackgroundWorkers;

public class OrderValidationWorker : BackgroundService
{
    private readonly ChannelReader<ValidationSignalBase> _channelReader;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<OrderValidationWorker> _logger;

    public OrderValidationWorker(ChannelReader<ValidationSignalBase> channelReader, IServiceScopeFactory serviceScopeFactory, ILogger<OrderValidationWorker> logger)
    {
        _channelReader = channelReader;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Order Validation Worker is starting.");
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await foreach (var signal in _channelReader.ReadAllAsync(stoppingToken))
                {
                    await using var scope = _serviceScopeFactory.CreateAsyncScope();
                    var uow = scope.ServiceProvider.GetRequiredService<IIntakeUnitOfWork>();
                    var addressParser = scope.ServiceProvider.GetRequiredService<IAddressParser>();
                    var addressValidationService = scope.ServiceProvider.GetRequiredService<IAddressValidationService>();

                    switch (signal)
                    {
                        case CampaignValidationSignal campaignSignal:
                            _logger.LogInformation("Received CampaignValidationSignal for CampaignId: {CampaignId}", campaignSignal.CampaignId);
                            foreach (var campaignOrder in await uow.GetUnvalidatedOrdersByCampaign(campaignSignal.CampaignId).ToListAsync(stoppingToken))
                            {
                                await ValidateOrder(campaignOrder, uow, addressParser, addressValidationService, stoppingToken);
                            }
                            break;

                        case OrderValidationSignal orderSignal:
                            _logger.LogInformation("Received OrderValidationSignal for OrderId: {OrderId}", orderSignal.OrderId);
                            var singleOrder = await uow.FindOrderByIdAsync(orderSignal.OrderId, stoppingToken);
                            if (singleOrder != null)
                                await ValidateOrder(singleOrder, uow, addressParser, addressValidationService, stoppingToken);
                            break;

                        case EverythingValidationSignal:
                            _logger.LogInformation("Received EverythingValidationSignal");
                            foreach (var everyOrder in await uow.GetUnvalidatedOrders().ToListAsync(stoppingToken))
                            {
                                await ValidateOrder(everyOrder, uow, addressParser, addressValidationService, stoppingToken);
                            }
                            break;

                        default:
                            _logger.LogWarning("Received unknown signal type: {SignalType}", signal?.GetType().Name);
                            break;
                    }

                    await uow.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing validation signal.");
            }
        }

        _logger.LogInformation("Order Validation Worker is stopping.");
    }

    private async Task ValidateOrder(OrderBase order, IIntakeUnitOfWork uow, IAddressParser parser, IAddressValidationService addressValidationService, CancellationToken stoppingToken = default)
    {
        switch (order)
        {
            case IncomingOrder incomingOrder:
                switch (await TryValidateOrderByParsing(incomingOrder, parser, addressValidationService, stoppingToken))
                {
                    case ValidationSuccess success:
                        uow.Transition<IncomingOrder, ValidatedOrder, OrderId>(incomingOrder, incomingOrder.Accept(success));
                        break;
                    case AddressLookupFailed lookupFailed:
                        uow.Transition<IncomingOrder, UnwashedOrder, OrderId>(incomingOrder, incomingOrder.MarkUnwashed(lookupFailed.Reason));
                        break;
                    default:
                        uow.Transition<IncomingOrder, UnwashedOrder, OrderId>(incomingOrder, incomingOrder.MarkUnwashed());
                        break;
                }
                break;
            case UnwashedOrder unwashed:
                switch (await TryValidateOrderByParsing(unwashed, parser, addressValidationService, stoppingToken))
                {
                    case ValidationSuccess success:
                        uow.Transition<UnwashedOrder, ValidatedOrder, OrderId>(unwashed, unwashed.Accept(success));
                        break;
                    case AddressLookupFailed lookupFailed:
                        unwashed.UpdateErrorMessage(lookupFailed.Reason);
                        break;
                }
                break;
            case WashedOrder washed:
                switch (await TryValidateOrderByReferences(washed, addressValidationService, stoppingToken))
                {
                    case ValidationSuccess success:
                        uow.Transition<WashedOrder, ValidatedOrder, OrderId>(washed, washed.Accept(success));
                        break;
                    case AddressLookupFailed lookupFailed:
                        uow.Transition<WashedOrder, UnwashedOrder, OrderId>(washed, washed.MarkUnwashed(lookupFailed.Reason));
                        break;
                    default:
                        uow.Transition<WashedOrder, OutOfBoundsOrder, OrderId>(washed, washed.MarkOutOfBounds());
                        break;
                }
                break;
            case OutOfBoundsOrder outOfBounds:
                switch (await TryValidateOrderByStreetAndHouseNumber(outOfBounds, addressValidationService, stoppingToken))
                {
                    case ValidationSuccess success:
                        uow.Transition<OutOfBoundsOrder, ValidatedOrder, OrderId>(outOfBounds, outOfBounds.Accept(success));
                        break;
                    case AddressLookupFailed lookupFailed:
                        uow.Transition<OutOfBoundsOrder, UnwashedOrder, OrderId>(outOfBounds, outOfBounds.MarkUnwashed(lookupFailed.Reason));
                        break;
                }

                break;
        }
    }

    private async Task<AddressValidationResult?> TryValidateOrderByParsing(IParseableOrder order, IAddressParser parser, IAddressValidationService addressValidationService, CancellationToken stoppingToken = default)
    {
        var parsed = parser.TryParse(order.Message);

        if (parsed is null)
        {
            _logger.LogWarning("Failed to parse order {OrderId}.", order.Id);
            return null;
        }

        var validationResult = await addressValidationService.ValidateAsync(parsed, order.CampaignId, stoppingToken);
        if (validationResult is ValidationSuccess)
            _logger.LogInformation("Order {OrderId} validated successfully.", order.Id);
        else
            _logger.LogWarning("Order {OrderId} failed validation: {Reason}", order.Id, validationResult.GetType().Name);

        return validationResult;
    }

    private async Task<AddressValidationResult> TryValidateOrderByReferences(WashedOrder order, IAddressValidationService addressValidationService, CancellationToken stoppingToken = default)
    {
        var validationResult = await addressValidationService.ValidateRefsAsync(order.StreetId, order.StreetSectionId, order.NeighborhoodId, order.HouseNumber, order.CampaignId, stoppingToken);
        if (validationResult is ValidationSuccess)
            _logger.LogInformation("Order {OrderId} validated successfully by references.", order.Id);
        else
            _logger.LogWarning("Order {OrderId} failed reference validation: {Reason}", order.Id, validationResult.GetType().Name);

        return validationResult;
    }

    private async Task<AddressValidationResult> TryValidateOrderByStreetAndHouseNumber(OutOfBoundsOrder order, IAddressValidationService addressValidationService, CancellationToken stoppingToken = default)
    {
        var validationResult = await addressValidationService.ValidateStreetAsync(order.StreetId, order.HouseNumber, order.CampaignId, stoppingToken);
        if (validationResult is ValidationSuccess)
            _logger.LogInformation("Order {OrderId} validated successfully by street and house number.", order.Id);
        else
            _logger.LogWarning("Order {OrderId} failed street/house number validation: {Reason}", order.Id, validationResult.GetType().Name);

        return validationResult;
    }
}