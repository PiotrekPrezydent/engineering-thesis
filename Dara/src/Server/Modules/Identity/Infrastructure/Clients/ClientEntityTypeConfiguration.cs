using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.Clients;

// the part of the hub and its open login code are kept in the row of the client
public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable(Client.DbTableName);

        builder.HasKey(c => c.Id);
        // the domain sets the id, so a new client is an insert, not an update of an unknown row
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.UserId).IsRequired();
        builder.HasIndex(c => c.UserId);
        builder.Property(c => c.Kind).HasConversion<string>();
        builder.Ignore(c => c.IsRevoked);

        builder.OwnsOne(c => c.Hub, hub =>
        {
            hub.Property(h => h.Name).HasColumnName("Name").HasMaxLength(ClientNameMustBeValidRule.MaxLength).IsRequired();
            hub.Property(h => h.Type).HasColumnName("Type").HasConversion<string>();
            hub.Property(h => h.Context).HasColumnName("Context").HasJsonConversion().IsRequired();

            // at most one open code
            hub.OwnsOne(h => h.LoginCode, code =>
            {
                code.Property(c => c.Id).HasColumnName("LoginCodeId");
                code.Property(c => c.Hash).HasColumnName("LoginCodeHash").IsRequired();
                code.Property(c => c.IssuedAt).HasColumnName("LoginCodeIssuedAt");
                code.Property(c => c.ExpiresAt).HasColumnName("LoginCodeExpiresAt");
                code.HasIndex(c => c.Hash);
            });
        });
    }
}
