using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.ClientRegistrationCodes;
using Dara.Server.Modules.Identity.Domain.Users.Clients;
using Dara.Server.Modules.Identity.Domain.Users.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

// the clients and the open registration codes are owned by the account, so loading a user loads them too
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

        builder.OwnsMany(u => u.Clients, clients =>
        {
            clients.ToTable(UserClient.DbTableName);
            clients.WithOwner().HasForeignKey(c => c.UserId);
            clients.HasKey(c => c.Id);
            // the domain sets the id, so a new client is an insert, not an update of an unknown row
            clients.Property(c => c.Id).ValueGeneratedNever();

            clients.Property(c => c.Name).HasMaxLength(ClientNameMustBeValidRule.MaxLength).IsRequired();
            clients.Property(c => c.Type).HasConversion<string>();
            clients.Property(c => c.Context).HasJsonConversion().IsRequired();
            clients.Ignore(c => c.IsRevoked);

            // at most one open code, in the row of the client
            clients.OwnsOne(c => c.LoginCode, code =>
            {
                code.Property(c => c.Id).HasColumnName("LoginCodeId");
                code.Property(c => c.Hash).HasColumnName("LoginCodeHash").IsRequired();
                code.Property(c => c.IssuedAt).HasColumnName("LoginCodeIssuedAt");
                code.Property(c => c.ExpiresAt).HasColumnName("LoginCodeExpiresAt");
                code.HasIndex(c => c.Hash);
            });
        });
        builder.Navigation(u => u.Clients).UsePropertyAccessMode(PropertyAccessMode.Field);

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
