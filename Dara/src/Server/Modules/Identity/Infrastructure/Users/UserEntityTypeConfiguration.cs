using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.ClientRegistrationCodes;
using Dara.Server.Modules.Identity.Domain.Users.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

// the open registration codes are owned by the account; its clients are their own aggregate (Client)
public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(User.DbTableName);

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();

        builder.Property(u => u.Login).HasMaxLength(32).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(EmailMustHaveValidFormatRule.MaxLength).IsRequired();
        builder.Property(u => u.NormalizedEmail).HasMaxLength(EmailMustHaveValidFormatRule.MaxLength).IsRequired();
        builder.Property(u => u.PasswordHash).IsRequired();

        // a relational provider keeps both unique; the in-memory one does not, so the domain checks them too
        builder.HasIndex(u => u.Login).IsUnique();
        builder.HasIndex(u => u.NormalizedEmail).IsUnique();

        builder.OwnsMany(u => u.ClientRegistrationCodes, codes =>
        {
            codes.ToTable(ClientRegistrationCode.DbTableName);
            codes.WithOwner().HasForeignKey("UserId");
            codes.HasKey(c => c.Id);
            codes.Property(c => c.Id).ValueGeneratedNever();
            codes.Property(c => c.Hash).IsRequired();
            codes.HasIndex(c => c.Hash);
        });
        builder.Navigation(u => u.ClientRegistrationCodes).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
