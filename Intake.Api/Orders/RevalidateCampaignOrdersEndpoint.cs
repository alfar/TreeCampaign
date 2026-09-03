using System.Threading.Channels;
using Common.Infrastructure.Auth;
using Intake.Api.Helpers;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders.Services;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

public static class RevalidateCampaignOrdersEndpoint
{
    public static async Task<IResult> Handle(
        [FromRoute] CampaignRef campaignId,
        ChannelWriter<ValidationSignalBase> signalWriter,
        ICampaignOwnershipService campaignOwnershipService,
        ICurrentUserAccessor currentUser,
        CancellationToken cancellationToken)
    {
        if (!await campaignOwnershipService.IsOwnedByCurrentScoutGroupAsync(campaignId, currentUser, cancellationToken))
        {
            return Results.NotFound();
        }

        await signalWriter.WriteAsync(new CampaignValidationSignal(campaignId), cancellationToken);

        return Results.Accepted();
    }
}
