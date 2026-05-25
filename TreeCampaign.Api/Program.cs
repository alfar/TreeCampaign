using Scalar.AspNetCore;
using TreeCampaign.Api.Campaigns;
using TreeCampaign.Api.Stops;
using TreeCampaign.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddTreeCampaign();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new TreeCountJsonConverter());
    options.SerializerOptions.Converters.Add(new StopIdJsonConverter());
    options.SerializerOptions.Converters.Add(new TeamIdJsonConverter());
    options.SerializerOptions.Converters.Add(new CollectionCampaignIdJsonConverter());
    options.SerializerOptions.Converters.Add(new CampaignSeasonJsonConverter());
    options.SerializerOptions.Converters.Add(new ReasonTextJsonConverter());
    options.SerializerOptions.Converters.Add(new TeamNameJsonConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGroup("/api")
    .MapCampaignEndpoints()
    .MapGroup("/{campaignId}")
    .MapStopEndpoints()
    .MapTeamEndpoints();

app.Run();
