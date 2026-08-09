using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Groups.Infrastructure.Groups;

public class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Group>, IEntityTypeConfiguration<GroupMember>
{
    
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        
        builder.HasKey(g => g.GroupId);

        builder.Property(g => g.GroupId)
            .HasConversion(id => id.Value, value => new GroupId(value));
        
        builder.HasMany(g => g.Members)
            .WithOne()
            .HasForeignKey(gm => gm.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(g => g.OwnerId)
            .HasConversion(id => id.Value, value => new GroupMemberId(value))
            .IsRequired();
    }

    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.HasKey(gm => new { gm.GroupId, gm.MemberId });
        
        builder.Property(gm => gm.GroupId)
            .HasConversion(id => id.Value, value => new GroupId(value));
        
        builder.Property(gm => gm.MemberId)
            .HasConversion(id => id.Value, value => new GroupMemberId(value));

    }
}