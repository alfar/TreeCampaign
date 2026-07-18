using Intake.Domain.ExternalReferences;
using Intake.Domain.Orders;
using Intake.Domain.Orders.Services;
using Intake.Domain.Orders.ValueObjects;
using Intake.Infrastructure;

namespace Intake.Application.Services;

public record PaymentImportFailure(int LineNumber, string Reason);

public record PaymentImportSummary(int ImportedCount, IReadOnlyList<string> SkippedDuplicateTransactionIds, IReadOnlyList<PaymentImportFailure> Failures);

public interface IPaymentImportService
{
    Task<PaymentImportSummary> ImportAsync(CampaignRef campaignId, string csvContent, CancellationToken cancellationToken);
}

public class PaymentImportService(ICsvPaymentParser parser, IIntakeUnitOfWork unitOfWork) : IPaymentImportService
{
    public async Task<PaymentImportSummary> ImportAsync(CampaignRef campaignId, string csvContent, CancellationToken cancellationToken)
    {
        var parsed = parser.Parse(csvContent);

        var payments = parsed.OfType<ParsedPayment>().ToList();
        var failures = parsed.OfType<PaymentParsingFailed>()
            .Select(f => new PaymentImportFailure(f.LineNumber, f.Reason))
            .ToList();

        var transactionIds = payments
            .Where(p => p.TransactionId is not null)
            .Select(p => p.TransactionId!)
            .ToList();

        var seenTransactionIds = new HashSet<TransactionId>(await unitOfWork.GetExistingTransactionIdsAsync(transactionIds, cancellationToken));

        var repository = unitOfWork.GetRepository<IncomingOrder, OrderId>();
        var skippedDuplicates = new List<string>();
        var importedCount = 0;

        foreach (var payment in payments)
        {
            if (payment.TransactionId is not null && !seenTransactionIds.Add(payment.TransactionId))
            {
                skippedDuplicates.Add(payment.TransactionId.Value);
                continue;
            }

            var order = IncomingOrder.Create(campaignId, payment.Sender, payment.Amount, payment.OrderDate, payment.Message, payment.TransactionId);
            repository.Add(order);
            importedCount++;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new PaymentImportSummary(importedCount, skippedDuplicates, failures);
    }
}
