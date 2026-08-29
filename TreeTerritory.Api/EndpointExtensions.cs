using Common.Infrastructure.Auth;
using TreeTerritory.Application;
using TreeTerritory.Infrastructure;
using TreeTerritory.Api.Streets;
using TreeTerritory.Api.Territories;
using TreeTerritory.Api.Neighborhoods;
using TreeTerritory.Api.StreetSections;
using TreeTerritory.Api.JsonConverters;

namespace TreeTerritory.Api;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapTreeTerritoryEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapStreetEndpoints();
        app.MapTerritoryEndpoints();
        app.MapGroup("/Territories/{territoryId:guid}")
            .MapNeighborhoodEndpoints()
            .MapStreetSectionEndpoints()
            .MapPost("/import-street-sections", ImportStreetSectionsEndpoint.Handle)
            .WithTags("Territories")
            .RequireAuthorization(AuthPolicies.ScoutGroupMember)
            .DisableAntiforgery();

        return app;
    }

    public static IServiceCollection AddTreeTerritory(this IServiceCollection services)
    {
        services.AddTreeTerritoryRepository();
        services.AddTreeTerritoryServices();

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new NeighborhoodIdJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetIdJsonConverter());
            options.SerializerOptions.Converters.Add(new StreetSectionIdJsonConverter());
            options.SerializerOptions.Converters.Add(new ZipCodeJsonConverter());
            options.SerializerOptions.Converters.Add(new TerritoryIdJsonConverter());
            options.SerializerOptions.Converters.Add(new HouseNumberJsonConverter());
            options.SerializerOptions.Converters.Add(new ScoutGroupRefJsonConverter());
        });

        return services;
    }
}