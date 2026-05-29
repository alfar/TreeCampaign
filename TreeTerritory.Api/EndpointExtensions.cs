using TreeTerritory.Repository;

namespace TreeTerritory.Api;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapTreeTerritoryEndpoints(this IEndpointRouteBuilder app)
    {
        // app.MapNeighborhoodEndpoints();

        return app;
    }

    public static IServiceCollection AddTreeTerritory(this IServiceCollection services)
    {
        services.AddTreeTerritoryRepository();

/*        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new TreeCountJsonConverter());
            options.SerializerOptions.Converters.Add(new StopIdJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamIdJsonConverter());
            options.SerializerOptions.Converters.Add(new CollectionCampaignIdJsonConverter());
            options.SerializerOptions.Converters.Add(new CampaignSeasonJsonConverter());
            options.SerializerOptions.Converters.Add(new ReasonTextJsonConverter());
            options.SerializerOptions.Converters.Add(new TeamNameJsonConverter());
        });
*/

        return services;
    }
}