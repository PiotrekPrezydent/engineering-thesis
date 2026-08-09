using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.Clients;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder
            .HasKey(u => u.ClientId);

        builder
            .Property(u => u.ClientId)
            .HasConversion(
                id => id.Value,
                value => new ClientId(value)
                );
    }
}