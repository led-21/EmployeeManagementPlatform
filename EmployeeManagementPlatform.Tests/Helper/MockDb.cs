using Microsoft.EntityFrameworkCore;
using EmployeeManagementPlatform.Api.Data;

namespace EmployeeManagementPlatform.Tests.Helpers;

public static class TestDbContextFactory
{
    public static AppDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? $"TestDb-{Guid.NewGuid()}")
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}