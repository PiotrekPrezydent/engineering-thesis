using Microsoft.EntityFrameworkCore;

namespace Dara.Server.BuildingBlocks.UnitTests;

// two internal commands on one aggregate (for example AgentRun.Version) collide on its concurrency token;
// the second save throws, so its command is retried instead of overwriting the first change
public class InMemoryConcurrencyTests
{
    [Fact]
    public async Task SecondSaveOfAStaleVersionThrows()
    {
        var options = new DbContextOptionsBuilder<VersionedContext>().UseInMemoryDatabase($"versioned-{Guid.NewGuid():N}").Options;
        var id = Guid.NewGuid();
        await using (var setup = new VersionedContext(options))
        {
            setup.Items.Add(new VersionedItem { Id = id, Version = 1 });
            await setup.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        await using var first = new VersionedContext(options);
        await using var second = new VersionedContext(options);
        var firstItem = await first.Items.SingleAsync(i => i.Id == id, TestContext.Current.CancellationToken);
        var secondItem = await second.Items.SingleAsync(i => i.Id == id, TestContext.Current.CancellationToken);

        firstItem.Version++;
        await first.SaveChangesAsync(TestContext.Current.CancellationToken);

        secondItem.Version++;
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync(TestContext.Current.CancellationToken));
    }

    public class VersionedItem
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
    }

    public class VersionedContext(DbContextOptions<VersionedContext> options) : DbContext(options)
    {
        public DbSet<VersionedItem> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VersionedItem>().HasKey(i => i.Id);
            modelBuilder.Entity<VersionedItem>().Property(i => i.Version).IsConcurrencyToken();
        }
    }
}
