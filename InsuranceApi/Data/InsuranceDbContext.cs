using InsuranceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Data;

public class InsuranceDbContext : DbContext
{
    public InsuranceDbContext(DbContextOptions<InsuranceDbContext> options)
        : base(options)
    {
    }

    public DbSet<Policy> Policies => Set<Policy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Policy>()
            .HasIndex(p => p.PolicyNumber)
            .IsUnique();
    }
}