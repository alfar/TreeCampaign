using System;
using Common.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TreeCampaign.Infrastructure.Queries;

namespace TreeCampaign.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddTreeCampaignRepository(this IServiceCollection services)
    {
        services.AddDbContext<TreeCampaignContext>(options =>
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");

            options.UseSqlite($"Data Source={dbPath}");
        });

        services.AddDbContext<ProjectionContext>(options =>
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "app.db");

            options.UseSqlite($"Data Source={dbPath}");
        });

        services.AddScoped<ITreeCampaignUnitOfWork>(sp => sp.GetRequiredService<TreeCampaignContext>());
        services.AddScoped<IStopQueries, StopQueries>();
        services.AddScoped<ICampaignQueries, CampaignQueries>();
        services.AddScoped<ITeamQueries, TeamQueries>();

        return services;
    }
}
