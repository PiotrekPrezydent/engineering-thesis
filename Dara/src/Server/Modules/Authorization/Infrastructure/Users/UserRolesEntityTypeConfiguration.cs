using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.Modules.Authorization.Domain.Users;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Authorization.Infrastructure.Users;

// the roles of the clients are owned by the account, so loading the account loads them too
public class UserRolesEntityTypeConfiguration : IEntityTypeConfiguration<UserRoles>
{
    public void Configure(EntityTypeBuilder<UserRoles> builder)
    {
        builder.ToTable(UserRoles.DbTableName);

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever();
        builder.HasIndex(u => u.UserId).IsUnique();

        builder.PrimitiveCollection(u => u.AccountRoleCodes)
            .HasField("_accountRoleCodes")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(u => u.Clients, clients =>
        {
            clients.ToTable(ClientRoles.DbTableName);
            clients.WithOwner().HasForeignKey("UserRolesId");
            clients.HasKey(c => c.Id);
            clients.Property(c => c.Id).ValueGeneratedNever();
            clients.HasIndex(c => c.ClientId).IsUnique();

            clients.PrimitiveCollection(c => c.RoleCodes)
                .HasField("_roleCodes")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });
        builder.Navigation(u => u.Clients).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
