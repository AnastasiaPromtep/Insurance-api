using InsuranceApi.Data;
using InsuranceApi.Models;
using InsuranceApi.Requests;
using InsuranceApi.Services.Policies;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<InsuranceDbContext>(options =>
{
    if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

builder.Services.AddControllers();

builder.Services.AddScoped<IPolicyRepository, PolicyRepository>();
builder.Services.AddScoped<PolicyService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<InsuranceDbContext>();
    db.Database.Migrate();
}

app.Run();

public partial class Program { }