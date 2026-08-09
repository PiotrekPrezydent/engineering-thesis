using Dara.Server.Modules.Groups.Domain.GroupMessages;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Groups.Infrastructure.GroupMessages;

public class GroupMessageEntityTypeConfiguration : IEntityTypeConfiguration<GroupMessage>
{
    public void Configure(EntityTypeBuilder<GroupMessage> builder)
    {
        builder.HasKey(e => e.MessageId);
        
        builder.Property(e => e.MessageId)
            .HasConversion(id => id.Value, value => new GroupMessageId(value));

        builder.Property(e => e.GroupId).HasConversion(e => e.Value, value => new GroupId(value));
        builder.Property(e=>e.Sender).HasConversion(e=>e.Value,value=>new GroupMemberId(value));
    }
}