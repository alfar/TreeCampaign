using Access.Api;
using Access.Infrastructure;
using Common.Infrastructure;
using Common.Infrastructure.Services;
using Common.InfraStructure;
using Intake.Api;
using Intake.Application;
using Intake.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TreeCampaign.Api;
using TreeCampaign.Infrastructure;
using TreeTerritory.Api;
using TreeTerritory.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing 'ConnectionStrings:Default' configuration value.");

builder.Services.AddOpenApi();

builder.Services.AddDomainEventServices(connectionString);
builder.Services.AddCurrentUserAccessor();
builder.Services.AddAccess(connectionString);
builder.Services.AddTreeCampaign(connectionString);
builder.Services.AddTreeTerritory(connectionString);
builder.Services.AddIntake(connectionString);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.GetRequiredService<StoredDomainEventContext>().Database.MigrateAsync();
    await services.GetRequiredService<AccessContext>().Database.MigrateAsync();
    await services.GetRequiredService<TreeCampaignContext>().Database.MigrateAsync();
    await services.GetRequiredService<TreeTerritoryContext>().Database.MigrateAsync();
    await services.GetRequiredService<IntakeContext>().Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

var apiGroup =app.MapGroup("/api");
apiGroup.MapAccessEndpoints();
apiGroup.MapTreeCampaignEndpoints();
apiGroup.MapTreeTerritoryEndpoints();
apiGroup.MapIntakeEndpoints();

app.MapFallbackToFile("index.html");

app.Run();
