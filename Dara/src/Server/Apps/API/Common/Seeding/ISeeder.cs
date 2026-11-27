namespace Dara.Server.Apps.API.Common.Seeding;

// seeds data of one module through its api, seeders run in the order of registration
public interface ISeeder
{
    Task SeedAsync(SeedData data, CancellationToken cancellationToken);
}
