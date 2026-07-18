using System.Globalization;
using Intake.Domain.Orders.ValueObjects;

namespace Intake.Domain.Orders.Services;

public class CsvPaymentParser : ICsvPaymentParser
{
    private const string AmountColumn = "Amount";
    private const string TimestampColumn = "Timestamp";
    private const string MessageColumn = "Message";
    private const string UserNameColumn = "User Name";
    private const string UserPhoneNumberColumn = "User Phone Number";
    private const string TransactionIdColumn = "Payment Transaction ID";

    public IReadOnlyList<PaymentParsingResult> Parse(string csvContent)
    {
        var lines = csvContent.ReplaceLineEndings("\n").Split('\n');
        if (lines.Length < 3)
            return [];

        var columnIndex = BuildColumnIndex(lines[0]);

        var results = new List<PaymentParsingResult>();
        for (var i = 2; i < lines.Length; i++)
        {
            var lineNumber = i + 1;
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var row = line.Split(';');
            if (row.Length < columnIndex.Count)
            {
                results.Add(new PaymentParsingFailed(lineNumber, "Uventet antal kolonner"));
                continue;
            }

            var amountText = row[columnIndex[AmountColumn]].Replace(",", ".");
            if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out var amountValue) || amountValue <= 0)
            {
                results.Add(new PaymentParsingFailed(lineNumber, $"Kunne ikke læse beløbet '{row[columnIndex[AmountColumn]]}'"));
                continue;
            }

            var timestampText = row[columnIndex[TimestampColumn]];
            if (!DateTimeOffset.TryParse(timestampText, CultureInfo.InvariantCulture, DateTimeStyles.None, out var orderDate))
            {
                results.Add(new PaymentParsingFailed(lineNumber, $"Kunne ikke læse tidsstemplet '{timestampText}'"));
                continue;
            }

            var senderName = row[columnIndex[UserNameColumn]].Trim();
            if (senderName.Length == 0)
            {
                results.Add(new PaymentParsingFailed(lineNumber, "Mangler afsendernavn"));
                continue;
            }

            var phoneNumber = row[columnIndex[UserPhoneNumberColumn]].Trim();
            var message = row[columnIndex[MessageColumn]].Trim();
            var transactionIdText = row[columnIndex[TransactionIdColumn]].Trim();

            results.Add(new ParsedPayment(
                new Sender(senderName, phoneNumber.Length == 0 ? null : phoneNumber),
                MoneyAmount.From(amountValue),
                orderDate,
                message,
                transactionIdText.Length == 0 ? null : TransactionId.From(transactionIdText)));
        }

        return results;
    }

    private static Dictionary<string, int> BuildColumnIndex(string headerLine)
    {
        var headers = headerLine.Split(';');
        var columnIndex = new Dictionary<string, int>();
        for (var i = 0; i < headers.Length; i++)
        {
            columnIndex[headers[i].Trim()] = i;
        }

        foreach (var required in new[] { AmountColumn, TimestampColumn, MessageColumn, UserNameColumn, UserPhoneNumberColumn, TransactionIdColumn })
        {
            if (!columnIndex.ContainsKey(required))
                throw new InvalidOperationException($"CSV-filen mangler kolonnen '{required}'");
        }

        return columnIndex;
    }
}
