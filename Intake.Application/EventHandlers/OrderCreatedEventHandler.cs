using System.Threading.Channels;
using Common.Domain.Abstractions;
using Common.InfraStructure.Abstractions;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Domain.Orders.Events;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Application.EventHandlers;

public class OrderReceivedEventHandler(ChannelWriter<ValidationSignalBase> writer) : IDomainEventHandler<OrderReceived>
{
    public Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent is OrderReceived orderReceived)
        {
            var signal = new OrderValidationSignal(OrderId.From(orderReceived.AggregateId));

            return writer.WriteAsync(signal, cancellationToken).AsTask();
        }

        throw new ArgumentException("Invalid event type", nameof(domainEvent));
    }
}