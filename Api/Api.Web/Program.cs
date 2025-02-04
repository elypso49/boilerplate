using Api.Common.Consts;
using Api.Web.auth;
using Api.Web.extensions;
using Api.Web.swagger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json", true, true).AddEnvironmentVariables();

builder
    .Services.AddEndpointsApiExplorer()
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddVersionedApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    })
    .AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition(EnvironmentVariables.AuthUser,
            new OpenApiSecurityScheme
            {
                Name = EnvironmentVariables.AuthUser,
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Description = $"Custom header for {EnvironmentVariables.AuthUser}"
            });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme, Id = EnvironmentVariables.AuthUser
                    }
                },
                []
            }
        });
    })
    .ConfigureOptions<ConfigureSwaggerOptions>()
    .AddSwaggerGenNewtonsoftSupport()
    .AddCors(options => { options.AddPolicy("AllowAll", x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()); })
    .AddCustonAutoMapper()
    .ConfigureCustomOptions(builder.Configuration)
    .AddCustomDependencies()
    .AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Converters.Add(new StringEnumConverter());
        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
    });

builder.WebHost.ConfigureKestrel(serverOptions => { serverOptions.Limits.MaxRequestBodySize = 52428800; });

var app = builder.Build();

app
    .UseCors("AllowAll")
    .UseSwagger()
    .UseSwaggerUI(options =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        foreach (var description in provider.ApiVersionDescriptions)
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());

        options.RoutePrefix = string.Empty;
    })
    .UseMiddleware<AuthUserMiddleware>();

app.MapControllers();
app.Run();