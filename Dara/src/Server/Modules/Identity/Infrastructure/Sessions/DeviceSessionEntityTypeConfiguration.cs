using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Identity.Domain.Sessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.Sessions;

public class DeviceSessionEntityTypeConfiguration : IEntityTypeConfiguration<DeviceSession>
{
    public void Configure(EntityTypeBuilder<DeviceSession> builder)
    {
        builder.ToTable(DeviceSession.DbTableName);

        // the server gives the device id when the device is created
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.UserId).IsRequired();
        builder.HasIndex(s => s.UserId);

        // the domain raises it on every change, a save with an older value fails
        builder.Property(s => s.Version).IsConcurrencyToken();

        builder.OwnsMany(s => s.Tokens, tokens =>
        {
            tokens.ToTable("DeviceSessionRefreshTokens");
            tokens.WithOwner().HasForeignKey("SessionId");
            tokens.HasKey(t => t.Id);
            // the domain sets the id, so a new token is an insert, not an update of an unknown row
            tokens.Property(t => t.Id).ValueGeneratedNever();
            tokens.Property(t => t.TokenHash).IsRequired();
            tokens.HasIndex(t => t.TokenHash);
        });

        builder.Navigation(s => s.Tokens).UsePropertyAccessMode(PropertyAccessMode.Field);

        // at most one open code, in the row of the device
        builder.OwnsOne(s => s.LoginCode, code =>
        {
            code.Property(c => c.Hash).HasColumnName("LoginCodeHash").IsRequired();
            code.Property(c => c.IssuedAt).HasColumnName("LoginCodeIssuedAt");
            code.Property(c => c.ExpiresAt).HasColumnName("LoginCodeExpiresAt");
            code.HasIndex(c => c.Hash);
        });
    }
}
