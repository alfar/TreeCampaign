using Common.Domain.Abstractions;
using Intake.Domain.ExternalReferences;

namespace Intake.Domain.Abstractions;

public interface IIntakeEvent : IDomainEvent
{
    CampaignRef CampaignId { get; }
}
