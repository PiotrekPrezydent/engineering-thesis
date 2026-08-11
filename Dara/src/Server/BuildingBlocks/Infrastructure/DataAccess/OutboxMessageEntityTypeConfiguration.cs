using Dara.Server.BuildingBlocks.Infrastructure.Messaging.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

public class OutboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable(nameof(OutboxMessage)+"s");
        
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.Id).ValueGeneratedNever();
    }
}