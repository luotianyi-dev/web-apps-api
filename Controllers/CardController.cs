using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using TianyiNetwork.Web.AppsApi.Services;
using TianyiNetwork.Web.AppsApi.Models.Entities;
using TianyiNetwork.Web.AppsApi.Models.Transfer;

namespace TianyiNetwork.Web.AppsApi.Controllers
{
    [ApiController]
    [Route("apps/api/card")]
    public class CardController(
        #pragma warning disable CS9113
        [SuppressMessage("ReSharper", "InconsistentNaming")] ILogger<Controller> logger,
        [SuppressMessage("ReSharper", "InconsistentNaming")] BlockedWordService BlockedWord,
        [SuppressMessage("ReSharper", "InconsistentNaming")] CardMorseCodeService MorseCode,
        [SuppressMessage("ReSharper", "InconsistentNaming")] CardImageService Image,
        [SuppressMessage("ReSharper", "InconsistentNaming")] FileCacheService<CardController> FileCache,
        [SuppressMessage("ReSharper", "InconsistentNaming")] IMapper mapper,
        [SuppressMessage("ReSharper", "InconsistentNaming")] Entity db) : ControllerBase
        #pragma warning restore CS9113
    {
        [HttpPost]
        [EnableRateLimiting(nameof(CreateCard))]
        public async Task<ActionResult> CreateCard([FromBody] CardApplication application)
        {
            application.TrimFields();
            if (application.HasBlockedWords(BlockedWord)) return Problem(statusCode: 451, detail: "您的姓名中包含敏感词，已被屏蔽。请更换后重试。");
            var card = mapper.Map<CardEntity>(application);
            var serial = await db.Cards
                .OrderByDescending(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync() + 1;

            card.MorseSurname = MorseCode.GetReadableMorseCode(card.Surname);
            card.MorseGivenName = MorseCode.GetReadableMorseCode(card.GivenName);
            card.IssuedAt = DateOnly.FromDateTime(DateTime.UtcNow);
            card.ExpiresAt = card.IssuedAt.AddYears(10);
            card.CardNo = GenerateCardNo(serial, card.IssuedAt);
            card.Key = GenerateCardKey(card);
            db.Cards.Add(card);
            logger.LogInformation("Created card: {CardNo}", card.CardNo);
            await db.SaveChangesAsync();
            return Ok(card);
        }

        [HttpGet]
        public async Task<ActionResult> GetCardByQuery([FromQuery] CardQuery query)
        {
            var card = await db.Cards
                .Where(x =>
                    x.CardNo == query.CardNo &&
                    x.Surname == query.Surname &&
                    x.GivenName == query.GivenName &&
                    x.DateOfBirth == DateOnly.ParseExact(query.DateOfBirth, "yyyy-MM-dd"))
                .FirstOrDefaultAsync();
            return card == null
                ? Problem("请确保您输入了正确的姓名、出生日期和卡号。", statusCode: (int)HttpStatusCode.NotFound)
                : Ok(card);
        }

        [HttpGet("{key:length(40)}")]
        public async Task<ActionResult> GetCardByKey(string key)
        {
            var card = await db.Cards
                .Where(x => x.Key == key)
                .FirstOrDefaultAsync();
            return card == null
                ? Problem("身份密钥无效", statusCode: (int)HttpStatusCode.NotFound)
                : Ok(card);
        }

        [HttpGet("{key:length(40)}.{format}")]
        public async Task<ActionResult> GetCardImage(string key,
            [RegularExpression("^(png|webp)$")] string format,
            [FromQuery] bool download = false) {
            var card = await db.Cards
                .Where(x => x.Key == key)
                .FirstOrDefaultAsync();
            if (card == null) return NotFound();

            var etag = $"\"{GenerateCardKey(card)}\"";
            if (etag != card.Key) logger.LogWarning(
                "Card key and etag mismatch, maybe changed in database: etag({key}) != card.Key({card.Key})",
                key, card.Key);
            Response.Headers.ETag = etag;
            if (Request.Headers.IfNoneMatch == etag)
            {
                return StatusCode((int)HttpStatusCode.NotModified);
            }

            var cacheName = $"{card.CardNo}-{key}";
            var cachedImage = await FileCache.GetAsync($"{cacheName}.{format}");
            if (download) Response.Headers.ContentDisposition = $"attachment; filename=\"{card.CardNo}.{format}\"";
            if (cachedImage != null) return File(cachedImage, $"image/{format}");

            logger.LogInformation("Generating card image: {CardNo}", card.CardNo);
            var mrz = new CardMrz(card);
            var image = await Image.Draw(card, mrz);
            await FileCache.SetAsync($"{cacheName}.png", image.Png);
            logger.LogInformation("Generated and cached card image: {filename}", $"{cacheName}.png");
            await FileCache.SetAsync($"{cacheName}.webp", image.Webp);
            logger.LogInformation("Generated and cached card image: {filename}", $"{cacheName}.webp");
            return File(image.GetFormat(format), $"image/{format}");
        }

        private static string GenerateCardNo(int serial, DateOnly issuedAt)
        {
            const string cardIdPrefix = "LA";
            var cardIdYear = issuedAt.Year.ToString()[2..];
            var cardIdMonth = issuedAt.Month.ToString("X");
            var cardIdSerial = serial.ToString().PadLeft(4, '0');
            return $"{cardIdPrefix}{cardIdYear}{cardIdMonth}{cardIdSerial}";
        }

        private static string GenerateCardKey(CardEntity card)
        {
            var cardDigest = $"CardNo={card.CardNo};" +
                             $"Surname={card.Surname};GivenName={card.GivenName};" +
                             $"RomanizedSurname={card.RomanizedSurname};RomanizedGivenName={card.RomanizedGivenName};" +
                             $"MorseSurname={card.MorseSurname};MorseGivenName={card.MorseGivenName};" +
                             $"IssuedAt={card.IssuedAt};ExpiresAt={card.ExpiresAt};";
            var hash = SHA1.HashData(Encoding.UTF8.GetBytes(cardDigest)).ToList().ConvertAll(b => b.ToString("x2"));
            return string.Concat(hash);
        }
    }
}
