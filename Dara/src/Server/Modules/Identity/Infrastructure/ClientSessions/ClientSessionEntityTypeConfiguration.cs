using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Identity.Domain.ClientSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.ClientSessions;

public class ClientSessionEntityTypeConfiguration : IEntityTypeConfiguration<ClientSession>
{
    public void Configure(EntityTypeBuilder<ClientSession> builder)
    {
        builder.ToTable(ClientSession.DbTableName);

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        // one session per client
        builder.Property(s => s.ClientId).IsRequired();
        builder.HasIndex(s => s.ClientId).IsUnique();

        builder.Property(s => s.UserId).IsRequired();
        builder.HasIndex(s => s.UserId);

        // the domain raises it on every change, a save with an older value fails
        builder.Property(s => s.Version).IsConcurrencyToken();

        builder.OwnsMany(s => s.Tokens, tokens =>
        {
            tokens.ToTable("ClientSessionRefreshTokens");
            tokens.WithOwner().HasForeignKey("SessionId");
            tokens.HasKey(t => t.Id);
            // the domain sets the id, so a new token is an insert, not an update of an unknown row
            tokens.Property(t => t.Id).ValueGeneratedNever();
            tokens.Property(t => t.TokenHash).IsRequired();
            tokens.HasIndex(t => t.TokenHash);
        });

        builder.Navigation(s => s.Tokens).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(s => s.IsEnded);
        builder.Ignore(s => s.IsRevoked);
    }
}
