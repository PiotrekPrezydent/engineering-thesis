using Dara.Server.Modules.Clients.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Clients.Infrastructure;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.ClientId);
        
        builder.Property(c => c.ClientId)
            .HasConversion(
                id => id.Value,
                value => new ClientId(value)
            );
    }
}