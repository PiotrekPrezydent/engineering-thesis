using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Groups.Domain.GroupMessages;
using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Groups.Infrastructure.GroupMessages;

public class GroupMessageEntityTypeConfiguration : IEntityTypeConfiguration<GroupMessage>
{
    public void Configure(EntityTypeBuilder<GroupMessage> builder)
    {
        builder.ToTable(GroupMessage.DbTableName);
        
        builder.HasKey(e => e.Id);
        
        builder.Property(x => x.Id).HasConversion(id => id.Value, v => new GroupMessageId(v));
        
        builder.Property(x => x.GroupId).HasConversion(id => id.Value, v => new GroupId(v));
        builder.Property(x => x.AuthorId).HasConversion(id => id.Value, v => new MemberId(v));
        
        builder.HasOne<Group>()
            .WithMany()
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}