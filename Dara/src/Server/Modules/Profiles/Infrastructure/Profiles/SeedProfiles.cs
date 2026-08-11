using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Profiles.Domain;

namespace Dara.Server.Modules.Profiles.Infrastructure.Profiles;

public static class SeedProfiles
{
    public static object[] SeedAllProfiles()
    {
        return
        [
            SeedProfile(SharedSeedGuids.User1, "p1"),
            SeedProfile(SharedSeedGuids.User2, "p2"),
            SeedProfile(SharedSeedGuids.User3, "p3"),
            SeedProfile(SharedSeedGuids.User4, "p4"),
            SeedProfile(SharedSeedGuids.User5, "p5"),
        ];
    }
    
    
    public static object SeedProfile(Guid profileId, string name)
    {
        return new
        {
            Id = new ProfileId(profileId),
            _name = name
        };
    }
}