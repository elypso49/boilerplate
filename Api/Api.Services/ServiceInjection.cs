using Api.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Services;

public static class ServiceInjection
{
    public static IServiceCollection GetDependencyInjection(this IServiceCollection services)
        => services
            .AddScoped<IBoilerplateService, BoilerplateService>()
            .AddScoped<IBoilerplateRepository, BoilerplateRepository>();
}