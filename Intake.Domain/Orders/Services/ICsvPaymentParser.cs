namespace Intake.Domain.Orders.Services;

public interface ICsvPaymentParser
{
    IReadOnlyList<PaymentParsingResult> Parse(string csvContent);
}
