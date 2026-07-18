using Intake.Application.Services;
using Intake.Domain.ExternalReferences;
using Microsoft.AspNetCore.Mvc;

namespace Intake.Api.Orders;

internal class ImportPaymentsEndpoint
{
    public static async Task<IResult> Handle(
        [FromRoute] CampaignRef campaignId,
        IFormFile file,
        IPaymentImportService paymentImportService,
        CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var csvContent = await reader.ReadToEndAsync(cancellationToken);

        var summary = await paymentImportService.ImportAsync(campaignId, csvContent, cancellationToken);

        return Results.Ok(summary);
    }
}
