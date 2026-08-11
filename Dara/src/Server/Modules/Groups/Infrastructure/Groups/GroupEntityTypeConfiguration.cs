using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Groups.Infrastructure.Groups;

public class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Group>
{
    
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .HasConversion(id => id.Value, value => new GroupId(value));
        
        builder.HasMany(g => g.Members)
            .WithOne()
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(g => g.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Property<GroupMemberId>("_ownerId")
            .HasColumnName("OwnerId")
            .HasConversion(id => id.Value, value => new GroupMemberId(value))
            .IsRequired();
        
        builder.Property<string>("_name")
            .HasColumnName("Name")
            .IsRequired();
        
        builder.Property<string>("_joinCode")
            .HasColumnName("JoinCode");
    }
}