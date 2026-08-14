using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Domain.Members;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Groups.Infrastructure.Groups;

public class GroupEntityTypeConfiguration : IEntityTypeConfiguration<Group>
{
    
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable(Group.DbTableName);
        
        builder.HasKey(g => g.Id);

        //builder.Property(x => x.Id).HasConversion(id => id.Value, v => new GroupId(v));
        //builder.Property(x => x.CreatorId).HasConversion(id => id.Value, v => new MemberId(v));
        
        builder.HasMany(x => x.Members)
            .WithOne(m => m.Group)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Navigation(x => x.Members).UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasOne(x => x.Creator)
            .WithMany()
            .HasForeignKey(x => new { x.Id, x.CreatorId })
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}