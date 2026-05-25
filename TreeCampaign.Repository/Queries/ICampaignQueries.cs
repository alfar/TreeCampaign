using TreeCampaign.Domain.Campaigns;

public interface ICampaignQueries
{
    Task<IReadOnlyCollection<ProjectionContext.CampaignProjection>> GetCampaignsAsync(
        CancellationToken cancellationToken
    );
}
