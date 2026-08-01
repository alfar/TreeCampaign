using Access.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Access.Infrastructure.ValueConverters;

internal class EmailValueConverter : ValueConverter<Email, string>
{
    public EmailValueConverter()
        : base(email => email.Value, value => Email.Create(value)) { }
}
