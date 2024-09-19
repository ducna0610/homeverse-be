using Homeverse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Homeverse.UnitTest.Mocks;

public static class MockDbContext
{
    public static HomeverseDbContext CreateMockDbContext()
    {
        var options = new DbContextOptionsBuilder<HomeverseDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        var dbContext = new HomeverseDbContext(options);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}
