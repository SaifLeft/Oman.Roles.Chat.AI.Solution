using Application.Common.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register all services from the Infrastructure layer
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}