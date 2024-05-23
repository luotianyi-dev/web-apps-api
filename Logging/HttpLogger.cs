using System.Diagnostics;

namespace TianyiNetwork.Web.AppsApi.Logging
{

    public class HttpLogger(HttpContext context)
    {
        public string GetLogContent()
        {
            var client = GetRemoteAddress();
            var method = context.Request.Method.PadLeft(7, ' ');
            var status = GetColoredStatusCode();
            var path = context.Request.Path;
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            var received = (context.Request.ContentLength ?? 0).ToString().PadRight(19, ' ');
            var sent = (context.Response.ContentLength ?? 0).ToString().PadRight(19, ' ');
            var referer = context.Request.Headers.Referer;
            var userAgent = context.Request.Headers.UserAgent;
            return $"{client} - {method} \x1b[33m<{traceId}>\x1b[0m {status} {path}\n" +
                   $"recv={received}  send={sent}  referer={referer}\n" +
                   $"user-agent={userAgent}";
        }

        public string GetRemoteAddress()
        {
            var host = context.Request.HttpContext.Connection.RemoteIpAddress;
            var port = context.Request.HttpContext.Connection.RemotePort;
            var address = $"{host}:{port}";
            return address.PadRight(21, ' ');
        }

        public string GetColoredStatusCode()
        {
            var status = context.Response.StatusCode;
            var color = string.Empty;
            if (status >= 400) color = "\x1b[01;31m";
            if (status >= 300 && status < 400) color = "\x1b[01;33m";
            if (status >= 200 && status < 300) color = "\x1b[01;32m";
            return $"{color}{status}\x1b[0m";
        }
    }
}
