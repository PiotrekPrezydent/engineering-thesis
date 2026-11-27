using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Authorization.Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Authorization.Infrastructure.Permissions;

public class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable(Permission.DbTableName);

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Code).IsRequired();
        builder.Property(p => p.ScopeCode).IsRequired();
        builder.Property(p => p.Module).IsRequired();
        // a relational provider keeps the natural key unique; the synchronization never adds a code twice
        builder.HasIndex(p => p.Code).IsUnique();
    }
}
