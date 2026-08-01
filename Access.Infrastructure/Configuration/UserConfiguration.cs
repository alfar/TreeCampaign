using Access.Domain.ScoutGroups;
using Access.Domain.Users;
using Access.Infrastructure.ValueConverters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Access.Infrastructure.Configuration;

internal static class UserConfiguration
{
    public static ModelBuilder AddUsers(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("Users").Configure();
        return modelBuilder;
    }

    public static void Configure(this EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasConversion(new UserIdValueConverter());
        builder.Property(u => u.ScoutGroupId).HasConversion(new ScoutGroupIdValueConverter());
        builder.Property(u => u.Email).HasConversion(new EmailValueConverter()).HasMaxLength(256);
        builder.Property(u => u.DisplayName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasOne<ScoutGroup>().WithMany().HasForeignKey(u => u.ScoutGroupId);
    }
}
