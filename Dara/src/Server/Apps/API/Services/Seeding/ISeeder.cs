namespace Dara.Server.Apps.API.Services.Seeding;

// seeds data of one module through its api, seeders run in the order of registration
public interface ISeeder
{
    Task SeedAsync(SeedData data, CancellationToken cancellationToken);
}
