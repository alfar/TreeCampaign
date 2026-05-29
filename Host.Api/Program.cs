using Scalar.AspNetCore;
using TreeCampaign.Api;
using TreeTerritory.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddTreeCampaign();
builder.Services.AddTreeTerritory();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapTreeCampaignEndpoints();
app.MapTreeTerritoryEndpoints();

app.Run();
