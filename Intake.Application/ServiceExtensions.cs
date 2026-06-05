using System.Threading.Channels;
using Common.InfraStructure.Abstractions;
using Intake.Application.BackgroundWorkers;
using Intake.Application.BackgroundWorkers.Signals;
using Intake.Application.EventHandlers;
using Intake.Application.Services;
using Intake.Domain.Orders.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Intake.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddIntakeServices(this IServiceCollection services)
    {
        services.AddSingleton(_ => Channel.CreateUnbounded<ValidationSignalBase>());
        services.AddSingleton(sp => sp.GetRequiredService<Channel<ValidationSignalBase>>().Reader);
        services.AddSingleton(sp => sp.GetRequiredService<Channel<ValidationSignalBase>>().Writer);

        services.AddHostedService<OrderValidationWorker>();

        services.AddScoped<IAddressValidationService, AddressValidationService>();

        services.AddScoped<OrderReceivedEventHandler>();
        return services;
    }
}
