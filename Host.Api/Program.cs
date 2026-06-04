using Intake.Api;
using Scalar.AspNetCore;
using TreeCampaign.Api;
using TreeTerritory.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddTreeCampaign();
builder.Services.AddTreeTerritory();
builder.Services.AddIntake();

var app = builder.Build();

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
