using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.Clients;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<ClientIdentity>
{
    public void Configure(EntityTypeBuilder<ClientIdentity> builder)
    {
        builder
            .HasKey(u => u.ClientId);

        builder
            .Property(u => u.ClientId)
            .HasConversion(
                id => id.Value,
                value => new ClientIdentityId(value)
                );
        
        builder.ResolvePrivateFields();
    }
}