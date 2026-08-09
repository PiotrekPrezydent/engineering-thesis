using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Groups.Infrastructure.Groups;

public class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Group>
{
    
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder
            .HasKey(u => u.GroupId);

        builder
            .Property(u => u.GroupId)
            .HasConversion(
                id => id.Value,
                value => new GroupId(value)
            );
    }
}