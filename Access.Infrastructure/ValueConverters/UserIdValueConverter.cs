using Access.Domain.Users.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Access.Infrastructure.ValueConverters;

internal class UserIdValueConverter : ValueConverter<UserId, Guid>
{
    public UserIdValueConverter()
        : base(id => id.Value, value => UserId.From(value)) { }
}
