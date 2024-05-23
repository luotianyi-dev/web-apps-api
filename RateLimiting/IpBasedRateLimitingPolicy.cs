using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace TianyiNetwork.Web.AppsApi.RateLimiting
{
    public class IpBasedRateLimitingPolicy(string policyName, int permitLimit, TimeSpan window) : IRateLimiterPolicy<string>
    {
        private readonly FixedWindowRateLimiterOptions _options = new() { PermitLimit = permitLimit, Window = window };

        public Func<OnRejectedContext, CancellationToken, ValueTask>? OnRejected => (context, token) =>
        {
            int? retryAfterSecond = null;
            if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            {
                retryAfterSecond = (int)retryAfter.TotalSeconds;
            }

            if (retryAfterSecond != null) context.HttpContext.Response.Headers.RetryAfter = retryAfterSecond.ToString();
            context.HttpContext.Response.Headers["X-Rate-Limit-Policy"] = policyName;
            context.HttpContext.Response.Headers["X-Rate-Limit-Algorithm"] = nameof(FixedWindowRateLimiter);
            context.HttpContext.Response.Headers["X-Rate-Limit-Allowed-Request-Per-Window"] = _options.PermitLimit.ToString();
            context.HttpContext.Response.Headers["X-Rate-Limit-Window-Length-Seconds"] = _options.Window.TotalSeconds.ToString(CultureInfo.InvariantCulture);
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return ValueTask.CompletedTask;
        };

        public RateLimitPartition<string> GetPartition(HttpContext httpContext)
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            return RateLimitPartition.GetFixedWindowLimiter(ip, configure => _options);
        }
    }
}
