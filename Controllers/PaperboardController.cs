using System.Text;
using System.Security.Cryptography;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using TianyiNetwork.Web.AppsApi.Services;
using TianyiNetwork.Web.AppsApi.Models.Entities;
using TianyiNetwork.Web.AppsApi.Models.Transfer;
using System.Net;

namespace TianyiNetwork.Web.AppsApi.Controllers
{
    [ApiController]
    [Route("apps/api/paperboard")]
    public class PaperboardController(
        #pragma warning disable CS9113
        [SuppressMessage("ReSharper", "InconsistentNaming")] ILogger<Controller> logger,
        [SuppressMessage("ReSharper", "InconsistentNaming")] BlockedWordService BlockedWord,
        [SuppressMessage("ReSharper", "InconsistentNaming")] PaperboardImageService Image,
        [SuppressMessage("ReSharper", "InconsistentNaming")] FileCacheService<CardController> FileCache,
        [SuppressMessage("ReSharper", "InconsistentNaming")] Entity db) : ControllerBase
        #pragma warning restore CS9113
    {
        [HttpGet]
        public async Task<ActionResult> GetOrCreatePaperboardImage([FromQuery] PaperboardGenerateRequest request)
        {
            var (line1, line2) = (request.Line1.Trim(), request.Line2.Trim());
            if (BlockedWord.HasBlockedWord(line1 + line2) || BlockedWord.HasBlockedWord(line2 + line1))
            {
                logger.LogWarning("Blocked paperboard generation with word: '{Line1}', '{Line2}'",
                    line1, line2);
                return Problem(statusCode: 451, detail: "您提交文字中包含敏感词，已被屏蔽。请更换后重试。");
            }

            var hash = string.Concat(SHA1.HashData(Encoding.UTF8.GetBytes($"{request.Line1}\n{request.Line2}"))
                .ToList()
                .ConvertAll(b => b.ToString("x2")));
            var etag = $"\"{hash}\"";
            var cacheName = $"{hash}.{request.Format}";

            Response.Headers.ETag = etag;
            if (Request.Headers.IfNoneMatch == etag)
            {
                logger.LogInformation("Paperboard cache hit: {Etag} (Browser HTTP)", etag);
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            var file = await FileCache.GetAsync(cacheName);
            if (file != null)
            {
                logger.LogInformation("Paperboard cache hit: {Etag} (Server FS)", etag);
                return File(file, $"image/{request.Format}");
            }

            logger.LogInformation("Paperboard cache miss: {Etag}", etag);
            logger.LogInformation("Generating paperboard image: '{Line1}', '{Line2}'",
                line1, line2);
            var image = await Image.Draw(line1, line2);
            await FileCache.SetAsync($"{hash}.jpeg", image.Jpeg);
            logger.LogInformation("Cached paperboard image: {hash}.jpeg", hash);
            await FileCache.SetAsync($"{hash}.webp", image.Webp);
            logger.LogInformation("Cached paperboard image: {hash}.webp", hash);
            return File(image.GetFormat(request.Format), $"image/{request.Format}");
        }
    }
}
