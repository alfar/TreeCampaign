using Access.Api.Auth;
using Access.Api.JsonConverters;
using Access.Api.ScoutGroups;
using Access.Api.Users;
using Access.Domain.Users;
using Access.Infrastructure;
using Common.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Access.Api;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapAccessEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapScoutGroupEndpoints();
        app.MapUserEndpoints();

        return app;
    }

    public static IServiceCollection AddAccess(this IServiceCollection services)
    {
        services.AddAccessRepository();
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "TreeCampaign.Auth";
                options.LoginPath = "/api/auth/login";
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });
        services
            .AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.ScoutGroupMember, policy => policy.RequireClaim(AccessClaimTypes.ScoutGroupId));

        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(new ScoutGroupIdJsonConverter());
            options.SerializerOptions.Converters.Add(new UserIdJsonConverter());
            options.SerializerOptions.Converters.Add(new EmailJsonConverter());
        });

        return services;
    }
}
