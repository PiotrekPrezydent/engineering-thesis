using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Identity.Domain.UserSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.UserSessions;

public class UserSessionEntityTypeConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable(UserSession.DbTableName);

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        // one session per client
        builder.Property(s => s.ClientId).IsRequired();
        builder.HasIndex(s => s.ClientId).IsUnique();

        builder.Property(s => s.UserId).IsRequired();
        builder.HasIndex(s => s.UserId);

        builder.Property(s => s.Kind).HasConversion<string>();

        // the domain raises it on every change of the refresh tokens, a save with an older value fails
        builder.Property(s => s.Version).IsConcurrencyToken();

        builder.OwnsMany(s => s.Tokens, tokens =>
        {
            tokens.ToTable("UserSessionRefreshTokens");
            tokens.WithOwner().HasForeignKey("SessionId");
            tokens.HasKey(t => t.Id);
            // the domain sets the id, so a new token is an insert, not an update of an unknown row
            tokens.Property(t => t.Id).ValueGeneratedNever();
            tokens.Property(t => t.TokenHash).IsRequired();
            tokens.HasIndex(t => t.TokenHash);
        });
        builder.Navigation(s => s.Tokens).UsePropertyAccessMode(PropertyAccessMode.Field);

        // no concurrency token of their own: parallel requests of the panel only move LastUsedAt, and ef saves only
        // changed columns, so a sign-out is never lost
        builder.OwnsMany(s => s.PanelSignIns, signIns =>
        {
            signIns.ToTable("UserSessionPanelSignIns");
            signIns.WithOwner().HasForeignKey("SessionId");
            signIns.HasKey(s => s.Id);
            signIns.Property(s => s.Id).ValueGeneratedNever();
            signIns.Ignore(s => s.IsEnded);
        });
        builder.Navigation(s => s.PanelSignIns).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(s => s.IsEnded);
        builder.Ignore(s => s.IsRevoked);
    }
}
