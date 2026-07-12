using System.Threading.Channels;
using Common.Domain.Abstractions;
using Common.InfraStructure.Abstractions;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Application.EventHandlers;

public class OrderToValidateEventHandlerBase<T>(ChannelWriter<ValidationSignalBase> writer) : IDomainEventHandler<T> where T : IDomainEvent
{
    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent is T orderEvent)
        {
            var signal = new OrderValidationSignal(OrderId.From(orderEvent.AggregateId));

            await writer.WriteAsync(signal, cancellationToken);
        }
    }
}
