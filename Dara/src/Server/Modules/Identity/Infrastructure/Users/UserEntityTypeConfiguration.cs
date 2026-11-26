using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Identity.Domain;
using Dara.Server.Modules.Identity.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(User.DbTableName);

        builder
            .HasKey(u => u.Id);

        builder
            .Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value)
                );

        builder.Property(u => u.Login).HasMaxLength(32).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(EmailMustHaveValidFormatRule.MaxLength).IsRequired();
        builder.Property(u => u.NormalizedEmail).HasMaxLength(EmailMustHaveValidFormatRule.MaxLength).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();

        // a relational provider keeps both unique; the in-memory one does not, so the domain checks them too
        builder.HasIndex(u => u.Login).IsUnique();
        builder.HasIndex(u => u.NormalizedEmail).IsUnique();
    }
}
