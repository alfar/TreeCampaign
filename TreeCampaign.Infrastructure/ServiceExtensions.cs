using System;
using Common.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TreeCampaign.Infrastructure.Queries;

namespace TreeCampaign.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddTreeCampaignRepository(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TreeCampaignContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddDbContext<ProjectionContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<ITreeCampaignUnitOfWork>(sp => sp.GetRequiredService<TreeCampaignContext>());
        services.AddScoped<IStopQueries, StopQueries>();
        services.AddScoped<ICampaignQueries, CampaignQueries>();
        services.AddScoped<ITeamQueries, TeamQueries>();

        return services;
    }
}
