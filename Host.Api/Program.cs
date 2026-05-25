var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapTreeCampaignEndpoints();

app.Run();
