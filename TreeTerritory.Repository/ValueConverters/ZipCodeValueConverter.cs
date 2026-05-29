
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TreeTerritory.Domain.Streets.ValueObjects;

public class ZipCodeValueConverter : ValueConverter<ZipCode, string>
{
    public ZipCodeValueConverter()
        : base(zip => zip.Value, value => ZipCode.From(value)) { }
}