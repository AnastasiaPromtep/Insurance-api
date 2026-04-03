using System.Data.Common;
using InsuranceApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InsuranceApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DbConnection _connection;

    public CustomWebApplicationFactory()
    {
            // Create in-memory SQLite connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var DbOptionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<InsuranceDbContext>));
            var DbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(InsuranceDbContext));

            if (DbOptionsDescriptor != null)
                services.Remove(DbOptionsDescriptor);
            if (DbContextDescriptor != null)
                services.Remove(DbContextDescriptor);

            services.AddDbContext<InsuranceDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            // Build provider and apply migrations
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<InsuranceDbContext>();
            db.Database.EnsureCreated();
        });
    }
}