using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Authorization.Infrastructure.Roles;

public class RoleEntityTypeConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable(Role.DbTableName);

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedNever();

        builder.Property(r => r.Code).IsRequired();
        builder.Property(r => r.ScopeCode).IsRequired();
        builder.Property(r => r.Module).IsRequired();
        builder.HasIndex(r => r.Code).IsUnique();

        builder.PrimitiveCollection(r => r.PermissionCodes)
            .HasField("_permissionCodes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.PrimitiveCollection(r => r.ManageableRoleCodes)
            .HasField("_manageableRoleCodes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.PrimitiveCollection(r => r.CompanionRoleCodes)
            .HasField("_companionRoleCodes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
