using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace CloneEbay.Middleware
{
    public class ApiAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private const string AuthTokenHeader = "X-Auth-Token";
        private const string SecuredKeyHeader = "X-Secured-Key";
        private const string ValidToken = "demo-token";
        private const string ValidKey = "demo-key";

        public ApiAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Request.Headers;
            if (!headers.ContainsKey(AuthTokenHeader) || !headers.ContainsKey(SecuredKeyHeader) ||
                headers[AuthTokenHeader] != ValidToken || headers[SecuredKeyHeader] != ValidKey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized: Invalid token or key.");
                return;
            }
            await _next(context);
        }
    }
} 