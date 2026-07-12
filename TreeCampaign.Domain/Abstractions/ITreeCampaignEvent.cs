using Common.Domain.Abstractions;
using TreeCampaign.Domain.Campaigns.ValueObjects;

namespace TreeCampaign.Domain.Abstractions;

public interface ITreeCampaignEvent : IDomainEvent
{
    CampaignId CampaignId { get; }
}
