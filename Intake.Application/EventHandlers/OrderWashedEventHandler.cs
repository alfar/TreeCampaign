using System.Threading.Channels;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Domain.Orders.Events;

namespace Intake.Application.EventHandlers;

public class OrderWashedEventHandler : OrderToValidateEventHandlerBase<OrderWashed>
{
    public OrderWashedEventHandler(ChannelWriter<ValidationSignalBase> writer) : base(writer)
    {
    }
}
