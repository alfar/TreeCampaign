using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Services;

public abstract record PaymentParsingResult;

public sealed record ParsedPayment(
    Sender Sender,
    MoneyAmount Amount,
    DateTimeOffset OrderDate,
    string Message,
    TransactionId? TransactionId) : PaymentParsingResult;

public sealed record PaymentParsingFailed(int LineNumber, string Reason) : PaymentParsingResult;
