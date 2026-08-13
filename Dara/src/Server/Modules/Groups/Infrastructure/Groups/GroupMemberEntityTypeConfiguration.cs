using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Groups.Infrastructure.Groups;

public class GroupMemberEntityTypeConfiguration :  IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.ToTable(GroupMember.DbTableName);
        
        builder.HasKey(x => new { x.GroupId, x.MemberId });
        
        builder.Property(x => x.GroupId).HasConversion(id => id.Value, v => new GroupId(v));
        builder.Property(x => x.MemberId).HasConversion(id => id.Value, v => new MemberId(v));
        
        builder.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}