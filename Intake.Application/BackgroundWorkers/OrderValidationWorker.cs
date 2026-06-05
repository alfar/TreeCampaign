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
                var incomingToValidated = await TryValidateOrderByParsing(incomingOrder, parser, addressValidationService, stoppingToken);
                if (incomingToValidated is not null)
                    uow.Transition<IncomingOrder, ValidatedOrder, OrderId>(incomingOrder, incomingToValidated);
                else
                    uow.Transition<IncomingOrder, UnwashedOrder, OrderId>(incomingOrder, incomingOrder.MarkUnwashed());
                break;
            case UnwashedOrder unwashed:
                var unwashedToValidated = await TryValidateOrderByParsing(unwashed, parser, addressValidationService, stoppingToken);
                if (unwashedToValidated is not null)
                    uow.Transition<UnwashedOrder, ValidatedOrder, OrderId>(unwashed, unwashedToValidated);
                break;
            case WashedOrder washed:
                var washedToValidated = await TryValidateOrderByReferences(washed, addressValidationService, stoppingToken);
                if (washedToValidated is not null)
                    uow.Transition<WashedOrder, ValidatedOrder, OrderId>(washed, washedToValidated);
                else
                    uow.Transition<WashedOrder, OutOfBoundsOrder, OrderId>(washed, washed.MarkOutOfBounds());
                break;
            case OutOfBoundsOrder outOfBounds:
                var outOfBoundsToValidated = await TryValidateOrderByParsing(outOfBounds, parser, addressValidationService, stoppingToken);
                if (outOfBoundsToValidated is not null)
                    uow.Transition<OutOfBoundsOrder, ValidatedOrder, OrderId>(outOfBounds, outOfBoundsToValidated);
                break;
        }
    }

    private async Task<ValidatedOrder?> TryValidateOrderByParsing(IParseableOrder order, IAddressParser parser, IAddressValidationService addressValidationService, CancellationToken stoppingToken = default)
    {
        var parsed = parser.TryParse(order.Message);

        if (parsed is not null)
        {
            var validationResult = await addressValidationService.ValidateAsync(parsed, order.CampaignId, stoppingToken);
            if (validationResult is ValidationSuccess success)
            {
                _logger.LogInformation("Order {OrderId} validated successfully.", order.Id);
                var validatedOrder = order.Accept(success);
                return validatedOrder;
            }
            else
            {
                _logger.LogWarning("Order {OrderId} failed validation: {Reason}", order.Id, validationResult.GetType().Name);
            }
        }
        _logger.LogWarning("Failed to parse order {OrderId}.", order.Id);

        return null;
    }

    private async Task<ValidatedOrder?> TryValidateOrderByReferences(WashedOrder order, IAddressValidationService addressValidationService, CancellationToken stoppingToken = default)
    {
        var validationResult = await addressValidationService.ValidateRefsAsync(order.StreetId, order.StreetSectionId, order.NeighborhoodId, order.CampaignId, stoppingToken);
        if (validationResult is ValidationSuccess success)
        {
            _logger.LogInformation("Order {OrderId} validated successfully by references.", order.Id);
            var validatedOrder = order.Accept(success);
            return validatedOrder;
        }

        _logger.LogWarning("Order {OrderId} failed reference validation: {Reason}", order.Id, validationResult.GetType().Name);
        return null;
    }
}