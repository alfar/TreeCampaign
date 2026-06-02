using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeCampaign.Domain.Stops;

internal class ReasonTextValueConverter : ValueConverter<ReasonText, string>
{
    public ReasonTextValueConverter()
        : base(reason => reason.Text, value => new ReasonText(value)) { }
}

internal class NullableReasonTextValueConverter : ValueConverter<ReasonText?, string?>
{
    public NullableReasonTextValueConverter()
        : base(
            reason => reason == null ? null : reason.Text,
            value => value == null ? null : ReasonText.From(value)
        ) { }
}
