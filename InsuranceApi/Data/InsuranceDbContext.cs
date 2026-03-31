using InsuranceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Data;

public class InsuranceDbContext(DbContextOptions<InsuranceDbContext> options) : DbContext(options)
{
    public DbSet<Policy> Policies => Set<Policy>();
}