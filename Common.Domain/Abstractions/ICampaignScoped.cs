namespace Common.Domain.Abstractions;

public interface ICampaignScoped
{
    Guid CampaignId { get; }
}
