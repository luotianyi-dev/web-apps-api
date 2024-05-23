using TianyiNetwork.Web.AppsApi.Logging;
using TianyiNetwork.Web.AppsApi.Models.Entities;

namespace TianyiNetwork.Web.AppsApi.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static WebApplication EnsureDatabaseCreated(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            using var db = scope.ServiceProvider.GetService<Entity>();
            db?.Database.EnsureCreated();
            return app;
        }

        public static WebApplication UseDefaultRequestBodyContentType(this WebApplication app, string contentType = "application/json")
        {
            string[] allowJsonHttpMethods = ["POST", "PUT", "PATCH"];

            app.Use(async (context, next) =>
            {
                var a = allowJsonHttpMethods.Contains(context.Request.Method);
                var b = string.IsNullOrEmpty(context.Request.Headers.ContentType);
                if (allowJsonHttpMethods.Contains(context.Request.Method) && string.IsNullOrEmpty(context.Request.Headers.ContentType))
                {
                    context.Request.Headers.ContentType = contentType;
                }
                await next(context);
            });

            return app;
        }

        public static WebApplication UseRequestLogging(this WebApplication app)
        {
            app.Use(async (context, next) =>
            {
                await next();
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                var httpLogger = new HttpLogger(context);
                logger.LogInformation(httpLogger.GetLogContent());
            });
            return app;
        }
    }
}
