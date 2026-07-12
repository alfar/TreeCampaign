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

builder.Services.AddOpenApi();

builder.Services.AddDomainEventServices();
builder.Services.AddTreeCampaign();
builder.Services.AddTreeTerritory();
builder.Services.AddIntake();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await services.GetRequiredService<StoredDomainEventContext>().Database.MigrateAsync();
    await services.GetRequiredService<TreeCampaignContext>().Database.MigrateAsync();
    await services.GetRequiredService<TreeTerritoryContext>().Database.MigrateAsync();
    await services.GetRequiredService<IntakeContext>().Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

var apiGroup =app.MapGroup("/api");
apiGroup.MapTreeCampaignEndpoints();
apiGroup.MapTreeTerritoryEndpoints();
apiGroup.MapIntakeEndpoints();

app.Run();
