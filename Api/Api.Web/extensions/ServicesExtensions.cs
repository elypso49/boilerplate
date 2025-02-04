using Api.Common.Options;
using Api.Services;

namespace Api.Web.extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddCustonAutoMapper(this IServiceCollection services)
        => services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

    public static IServiceCollection ConfigureCustomOptions(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<ApiOptions>(configuration.GetSection(nameof(ApiOptions)));

    public static IServiceCollection AddCustomDependencies(this IServiceCollection services)
        => services.GetDependencyInjection();
}