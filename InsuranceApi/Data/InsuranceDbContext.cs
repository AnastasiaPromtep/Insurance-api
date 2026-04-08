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
    public DbSet<Quote> Quotes => Set<Quote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(q => q.QuoteId);

            entity.Property(q => q.QuoteStatus)
                .IsRequired();

            entity.Property(q => q.CreationDate)
                .IsRequired();

            entity.Property(q => q.StartDate)
                .IsRequired();
        });

        modelBuilder.Entity<Policy>()
            .HasIndex(p => p.PolicyNumber)
            .IsUnique();
    }
}