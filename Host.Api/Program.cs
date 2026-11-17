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

// CORS for the embeddable public intake form (TreeCampaign.UI build:embed) on third-party sites.
// No credentials are allowed, so cookie-authenticated endpoints stay unreachable cross-origin.
var publicIntakeOrigins = builder.Configuration.GetSection("PublicIntake:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("PublicIntake", policy => policy
    .WithOrigins(publicIntakeOrigins)
    .WithMethods("GET", "POST")
    .WithHeaders("Content-Type")));

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

// Only the endpoints PublicIntakeForm calls: streets by zip, campaign summary, territory streets,
// street-section resolve, and creating a pending order.
app.UseWhen(
    context => PublicIntakePaths.IsMatch(context.Request.Path),
    branch => branch.UseCors("PublicIntake"));

app.UseAuthentication();
app.UseAuthorization();

var apiGroup =app.MapGroup("/api");
apiGroup.MapAccessEndpoints();
apiGroup.MapTreeCampaignEndpoints();
apiGroup.MapTreeTerritoryEndpoints();
apiGroup.MapIntakeEndpoints();

app.MapFallbackToFile("index.html");

app.Run();

static partial class PublicIntakePaths
{
    [System.Text.RegularExpressions.GeneratedRegex(
        "^/api/(streets/[^/]+|campaigns/[^/]+/(summary|orders/pending/?)|territories/[^/]+/(streets|street-sections/resolve))/?$",
        System.Text.RegularExpressions.RegexOptions.IgnoreCase)]
    private static partial System.Text.RegularExpressions.Regex Pattern();

    public static bool IsMatch(PathString path) => Pattern().IsMatch(path.Value ?? "");
}
