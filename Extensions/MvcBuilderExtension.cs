using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TianyiNetwork.Web.AppsApi.Logging;

namespace TianyiNetwork.Web.AppsApi.Extensions
{
    public static class MvcBuilderExtension
    {
        public static string GetResourceDirectory(this WebApplicationBuilder builder)
        {
            return Path.Combine(builder.Environment.ContentRootPath, "Resources");
        }

        public static string GetResourceByName(this WebApplicationBuilder builder, string name)
        {
            return Path.Combine(builder.GetResourceDirectory(), name);
        }

        public static IMvcBuilder AddLocalizedProblem(this IMvcBuilder builder)
        {
            builder.ConfigureApiBehaviorOptions(options =>
            {
                options.ClientErrorMapping.Clear();
                options.ClientErrorMapping.Add(400, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/400", Title = "请求错误"});
                options.ClientErrorMapping.Add(401, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/401", Title = "需要授权"});
                options.ClientErrorMapping.Add(403, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/403", Title = "禁止访问"});
                options.ClientErrorMapping.Add(404, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/404", Title = "找不到相关资源"});
                options.ClientErrorMapping.Add(405, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/405", Title = "不允许该 HTTP 方法"});
                options.ClientErrorMapping.Add(406, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/406", Title = "无法接受请求"});
                options.ClientErrorMapping.Add(408, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/408", Title = "请求超时"});
                options.ClientErrorMapping.Add(409, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/409", Title = "请求冲突"});
                options.ClientErrorMapping.Add(412, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/412", Title = "请求不符合前提条件"});
                options.ClientErrorMapping.Add(415, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/415", Title = "不支持的内容类型"});
                options.ClientErrorMapping.Add(422, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/422", Title = "数据验证错误"});
                options.ClientErrorMapping.Add(429, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/429", Title = "请求过多"});
                options.ClientErrorMapping.Add(451, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/451", Title = "请求由于法律原因被屏蔽"});
                options.ClientErrorMapping.Add(500, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/500", Title = "服务器错误"});
                options.ClientErrorMapping.Add(502, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/502", Title = "网关错误"});
                options.ClientErrorMapping.Add(503, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/503", Title = "服务器维护中"});
                options.ClientErrorMapping.Add(504, new ClientErrorData { Link = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/504", Title = "网关超时"});
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problem = new ValidationProblemDetails(context.ModelState)
                    {
                        Title = "数据验证错误",
                        Type = "https://developer.mozilla.org/zh-CN/docs/Web/HTTP/Status/422",
                        Detail = string.Join("; ", context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                    };
                    problem.Extensions.Add("traceId", Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
                    return new UnprocessableEntityObjectResult(problem);
                };
            });
            return builder;
        }

        public static IServiceCollection AddLocalizedLogging(this IServiceCollection services)
        {
            services.AddLogging(loggingOptions =>
            {
                loggingOptions.ClearProviders();
                loggingOptions.AddConsole(consoleLoggingOptions =>
                {
                    consoleLoggingOptions.FormatterName = "localized";
                });
                loggingOptions.AddConsoleFormatter<LoggerFormatter, LoggerOptions>();
            });
            return services;
        }
    }
}
