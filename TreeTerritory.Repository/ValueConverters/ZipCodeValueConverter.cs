
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.Streets.ValueObjects;

public class ZipCodeValueConverter : ValueConverter<ZipCode, string>
{
    public ZipCodeValueConverter()
        : base(zip => zip.Value, value => ZipCode.From(value)) { }
}

public class NullableZipCodeValueConverter : ValueConverter<ZipCode?, string?>
{
    public NullableZipCodeValueConverter()
        : base(
            zipCode => zipCode != null ? zipCode.Value : null,
            value => value != null ? ZipCode.From(value) : null
        )
    { }
}
