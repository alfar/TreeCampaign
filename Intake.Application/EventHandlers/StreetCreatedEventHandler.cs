using System.Threading.Channels;
using Common.Domain.Abstractions;
using Common.InfraStructure.Abstractions;
using Intake.Application.BackgroundWorkers.Signals;
using TreeTerritory.Domain.Streets.Events;

namespace Intake.Application.EventHandlers;

public class StreetCreatedEventHandler : IDomainEventHandler<StreetCreated>
{
    private readonly ChannelWriter<ValidationSignalBase> _writer;

    public StreetCreatedEventHandler(ChannelWriter<ValidationSignalBase> writer)
    {
        _writer = writer;
    }

    public async Task HandleAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _writer.WriteAsync(new EverythingValidationSignal());
    }
}
