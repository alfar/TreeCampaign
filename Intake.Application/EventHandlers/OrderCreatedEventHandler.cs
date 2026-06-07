using System.Threading.Channels;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Domain.Orders.Events;

namespace Intake.Application.EventHandlers;

public class OrderReceivedEventHandler : OrderToValidateEventHandlerBase<OrderReceived>
{
    public OrderReceivedEventHandler(ChannelWriter<ValidationSignalBase> writer) : base(writer)
    {
    }
}