using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Groups.Infrastructure.Groups;

public class GroupMemberEntityTypeConfiguration :  IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.HasKey(gm => new { gm.GroupId, MemberId = gm.Id });
        
        builder.Property(gm => gm.GroupId)
            .HasConversion(id => id.Value, value => new GroupId(value));
        
        builder.Property(gm => gm.Id)
            .HasConversion(id => id.Value, value => new GroupMemberId(value));
    }
}