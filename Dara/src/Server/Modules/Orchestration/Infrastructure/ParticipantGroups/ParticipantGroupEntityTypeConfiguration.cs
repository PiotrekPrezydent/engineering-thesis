using System.Text.Json;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Orchestration.Domain.ParticipantGroups;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dara.Server.Modules.Orchestration.Infrastructure.ParticipantGroups;

public class ParticipantGroupEntityTypeConfiguration : IEntityTypeConfiguration<ParticipantGroup>
{
    public void Configure(EntityTypeBuilder<ParticipantGroup> builder)
    {
        builder.ToTable(ParticipantGroup.DbTableName);
        
        builder.HasKey(x => x.Id);
        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value, 
                value => new ParticipantGroupId(value));

        var participantListConverter = new ValueConverter<IReadOnlyList<ParticipantId>, string>(
            v => JsonSerializer.Serialize(v.Select(id => id.Value), (JsonSerializerOptions?)null),
            v => (JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>())
                .Select(val => new ParticipantId(val))
                .ToList()
        );
        var participantListComparer = new ValueComparer<IReadOnlyList<ParticipantId>>(
            (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
            c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Value.GetHashCode())),
            c => c.ToList()
        );
        
        builder.Property(x => x.Participants)
            .HasField("_participants")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasConversion(participantListConverter)
            .Metadata.SetValueComparer(participantListComparer);
        

    }
}