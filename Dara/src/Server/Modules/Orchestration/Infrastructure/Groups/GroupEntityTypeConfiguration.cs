using System.Text.Json;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Orchestration.Domain.Groups;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Dara.Server.Modules.Orchestration.Infrastructure.Groups;

public class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable(Group.DbTableName);
        
        builder.HasKey(x => x.Id);
        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value, 
                value => new GroupId(value));

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
        
        // settings in columns of the group table, the api key in a column of its own
        builder.OwnsOne(x => x.AgentSettings, settings =>
        {
            settings.Property(s => s.PromptMessagePrefix).HasColumnName(nameof(GroupAgentSettings.PromptMessagePrefix));
            settings.Property(s => s.ChatClientProviderName).HasColumnName(nameof(GroupAgentSettings.ChatClientProviderName));
            settings.Property(s => s.ClientModelName).HasColumnName(nameof(GroupAgentSettings.ClientModelName));
        });

        builder.Navigation(x => x.AgentSettings).IsRequired();
    }
}