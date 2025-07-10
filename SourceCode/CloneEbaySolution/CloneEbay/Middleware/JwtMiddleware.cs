using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace CloneEbay.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Cookies["JWTToken"];

        if (!string.IsNullOrEmpty(token))
        {
            // Add the token to the Authorization header
            context.Request.Headers["Authorization"] = $"Bearer {token}";
        }

        await _next(context);
    }
}

// Extension method for easy middleware registration
public static class JwtMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }
} 