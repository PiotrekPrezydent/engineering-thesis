using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasKey(u => u.UserId);

        builder
            .Property(u => u.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value)
                );
    }
}