using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.BuildingBlocks.Infrastructure.DataAccess;

public class InternalCommandEntityTypeConfiguration : IEntityTypeConfiguration<InternalCommand>
{
    public void Configure(EntityTypeBuilder<InternalCommand> builder)
    {
        builder.ToTable(nameof(InternalCommand) + "s");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.Data).IsRequired();
        builder.Property(x => x.LastError).HasMaxLength(RetryableMessage.MaxErrorLength);

        builder.HasIndex(x => new { x.ProcessedDate, x.EnqueuedAt });
    }
}
