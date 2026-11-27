using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Authorization.Infrastructure.RoleAssignments;

public class RoleAssignmentEntityTypeConfiguration : IEntityTypeConfiguration<RoleAssignment>
{
    public void Configure(EntityTypeBuilder<RoleAssignment> builder)
    {
        builder.ToTable(RoleAssignment.DbTableName);

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.RoleCode).IsRequired();
        builder.Property(a => a.ScopeCode).IsRequired();

        // one role once per subject and resource; the checks of the access read by subject and by resource
        builder.HasIndex(a => new { a.SubjectId, a.RoleCode, a.ScopeId }).IsUnique();
        builder.HasIndex(a => a.ScopeId);
    }
}
