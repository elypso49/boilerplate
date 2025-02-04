using System.Text.Json;
using Api.Common.Consts;
using Api.Common.Options;
using Microsoft.Extensions.Options;

namespace Api.Web.auth;

public class AuthUserMiddleware(RequestDelegate next, IOptions<ApiOptions> options)
{
    private readonly string _googleUserId = options.Value.AuthUser;

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(EnvironmentVariables.AuthUser, out var authUser))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var errorResponse = new { message = $"Missing {EnvironmentVariables.AuthUser} header" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));

            return;
        }

        if (!IsValidAuthUser(authUser))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var errorResponse = new { message = $"Invalid {EnvironmentVariables.AuthUser} header" };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));

            return;
        }

        await next(context);
    }

    private bool IsValidAuthUser(string? authUser)
        => authUser == _googleUserId;
}