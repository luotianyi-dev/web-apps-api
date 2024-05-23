using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using TianyiNetwork.Web.AppsApi.Controllers;
using TianyiNetwork.Web.AppsApi.Extensions;
using TianyiNetwork.Web.AppsApi.Services;
using TianyiNetwork.Web.AppsApi.RateLimiting;
using TianyiNetwork.Web.AppsApi.Models.Entities;

namespace TianyiNetwork.Web.AppsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services
                .AddControllers()
                .AddNewtonsoftJson()
                .AddLocalizedProblem()
                .Services
                .AddLocalizedLogging()
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddDbContext<Entity>(options => options.UseMySql(
                    builder.Configuration.GetConnectionString("MySql"),
                    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))))
                .AddRateLimiter(options =>
                {
                    options.AddPolicy<string, CardCreateRateLimitingPolicy>(CardCreateRateLimitingPolicy.PolicyName);
                })
                .AddSingleton(CardMorseCodeService.Create(builder))
                .AddSingleton(BlockedWordService.Create(builder))
                .AddSingleton(CardImageService.Create(builder))
                .AddSingleton(PaperboardImageService.Create(builder))
                .AddSingleton(FileCacheService<CardController>.Create(builder))
                .AddHealthChecks();

            var app = builder.Build();
            app.EnsureDatabaseCreated();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedHost |
                                   ForwardedHeaders.XForwardedProto
            });
            app.UseRequestLogging();
            app.UseDefaultRequestBodyContentType();
            app.UseRateLimiter();
            app.MapControllers();
            app.MapHealthChecks("/healthz");
            app.Run();
        }
    }
}
