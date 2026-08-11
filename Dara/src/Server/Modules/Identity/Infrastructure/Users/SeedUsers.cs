using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Infrastructure.Users;

public static class SeedUsers
{
    public static object[] SeedAllUsers()
    {
        return
        [
            SeedUser(SharedSeedGuids.User1, "1"),
            SeedUser(SharedSeedGuids.User2, "2"),
            SeedUser(SharedSeedGuids.User3, "3"),
            SeedUser(SharedSeedGuids.User4, "4"),
            SeedUser(SharedSeedGuids.User5, "5")
        ];
    }
    
    public static object SeedUser(Guid id, string identifier)
    {
        return new
        {
            Id = new UserId(id),
            _identifier = identifier
        };
    }
}