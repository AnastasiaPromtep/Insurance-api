using InsuranceApi.Data;
using InsuranceApi.Models;
using InsuranceApi.Requests;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<InsuranceDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();    
}

app.MapGet("/", () => "Hello World!");

var policiesGroup = app.MapGroup("/policies");

policiesGroup.MapGet("/", async (InsuranceDbContext dbContext) => {
    var policies = await dbContext.Policies.ToListAsync();
    return Results.Ok(policies);
});
policiesGroup.MapGet("/{id}", (int id, InsuranceDbContext dbContext) =>
{
    var policy = dbContext.Policies.FirstOrDefault(p => p.Id == id);

    return policy is not null
        ? Results.Ok(policy)
        : Results.NotFound();
});
policiesGroup.MapPost("/", async (CreatePolicyRequest request, InsuranceDbContext db) =>
{
    var policy = new Policy
    {
        PolicyNumber = request.PolicyNumber,
        SubscriberName = request.SubscriberName,
        PremiumAmount = request.PremiumAmount,
        StartDate = request.StartDate,
        Status = PolicyStatus.Draft
    };

    db.Policies.Add(policy);
    await db.SaveChangesAsync();

    return Results.Created($"/policies/{policy.Id}", policy);
});
policiesGroup.MapPut("/{id:int}", async (int id, UpdatePolicyRequest request, InsuranceDbContext db) =>
{
    var policy = await db.Policies.FindAsync(id);

    if (policy is null)
        return Results.NotFound();

    policy.PolicyNumber = request.PolicyNumber;
    policy.SubscriberName = request.SubscriberName;
    policy.PremiumAmount = request.PremiumAmount;
    policy.StartDate = request.StartDate;
    policy.EndDate = request.EndDate;
    policy.Status = request.Status;

    await db.SaveChangesAsync();

    return Results.Ok(policy);
});
policiesGroup.MapPatch("/{id:int}", async (int id, PatchPolicyRequest request, InsuranceDbContext db) =>
{
    var policy = db.Policies.FirstOrDefault(p => p.Id == id);

    if (policy is null)
        return Results.NotFound();

    if (request.SubscriberName is not null)
        policy.SubscriberName = request.SubscriberName;

    if (request.PremiumAmount.HasValue)
        policy.PremiumAmount = request.PremiumAmount.Value;

    if (request.Status.HasValue)
        policy.Status = request.Status.Value;

    await db.SaveChangesAsync();

    return Results.Ok(policy);
});
policiesGroup.MapDelete("/{id:int}", (int id, InsuranceDbContext db) =>
{
    var policy = db.Policies.FirstOrDefault(p => p.Id == id);

    if (policy is null)
        return Results.NotFound();

    db.Policies.Remove(policy);
    db.SaveChangesAsync();

    return Results.NoContent();
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InsuranceDbContext>();
    db.Database.Migrate();
}

app.Run();

app.Run();
