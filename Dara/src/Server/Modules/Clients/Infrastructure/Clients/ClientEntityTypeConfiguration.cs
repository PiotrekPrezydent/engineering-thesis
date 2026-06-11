using System.Text.Json;
using Dara.Server.Modules.Clients.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Clients.Infrastructure.Clients;

public class ClientEntityTypeConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => new ClientId(value)
            );
        
        builder.Property(c => c.ClientSupervisors)
            .HasField("_clientSupervisors") 
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(
                list => JsonSerializer.Serialize(list, (JsonSerializerOptions)null),
                json => JsonSerializer.Deserialize<List<ClientId>>(json, (JsonSerializerOptions)null) ?? new List<ClientId>()
            )
            .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyList<ClientId>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList() 
            ));
    }
}