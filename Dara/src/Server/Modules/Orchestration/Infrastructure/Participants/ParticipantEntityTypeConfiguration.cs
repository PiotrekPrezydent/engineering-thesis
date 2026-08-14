using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Orchestration.Infrastructure.Participants;

public class ParticipantEntityTypeConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable(Participant.DbTableName);
        
        builder.HasKey(x => x.Id);
        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value, 
                value => new ParticipantId(value));
        
        builder.HasMany(x => x.Functions)
            .WithOne(x => x.Participant)
            .HasForeignKey(x => x.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(x => x.Functions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        
    }
}