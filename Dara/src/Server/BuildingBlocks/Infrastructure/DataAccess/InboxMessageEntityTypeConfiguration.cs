using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Inbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

public class InboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable(nameof(InboxMessage)+"s");
        
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.Id).ValueGeneratedNever();
    }
}