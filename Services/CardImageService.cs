using System.Numerics;
using System.Globalization;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using TianyiNetwork.Web.AppsApi.Models.Entities;
using TianyiNetwork.Web.AppsApi.Extensions;
using TianyiNetwork.Web.AppsApi.Models.Transfer;

namespace TianyiNetwork.Web.AppsApi.Services
{
    public class CardImageService(byte[] background, FontFamily enFont, FontFamily cnFont) : ISingletonService<CardImageService>
    {
        private readonly Dictionary<string, CardImageStyle> _imageStyles = new()
        {
            ["CardNo"] = new CardImageStyle { FontFamily = enFont, FontSize = 37.5F, Space = 22, Color = Color.FromRgba(0xc8, 0x00, 0x00, 0xff) },
            ["NameZH"] = new CardImageStyle { FontFamily = cnFont, FontSize = 38.4F, Space = 45, Color = Color.FromRgba(0x1e, 0x1e, 0x1e, 0xe0) },
            ["LongEN"] = new CardImageStyle { FontFamily = enFont, FontSize = 26.0F, Space = 16, Color = Color.FromRgba(0x1e, 0x1e, 0x1e, 0xe0) },
            ["CodeEN"] = new CardImageStyle { FontFamily = enFont, FontSize = 38.5F, Space = 25, Color = Color.FromRgba(0x1e, 0x1e, 0x1e, 0xe0) },
            ["MRZSum"] = new CardImageStyle { FontFamily = enFont, FontSize = 60.0F, Space = 43, Color = Color.FromRgba(0xff, 0xff, 0xff, 0xf0) },
        };

        public static CardImageService Create(WebApplicationBuilder builder)
        {
            return new CardImageService(
                File.ReadAllBytes(builder.GetResourceByName("Card/Background.png")),
                new FontCollection().Add(builder.GetResourceByName("Card/JetBrainsMono.ttf")),
                new FontCollection().Add(builder.GetResourceByName("Card/FangZhengXiaoBiaoSong.ttf")));
        }

        public async Task<ImageGenerated> Draw(CardEntity card, CardMrz mrz)
        {
            using var image = Image.Load(background);
            var col = new float[] { 105, 150, 595, 1052, 1675 };
            var row = new float[] { 53, 240, 285, 320, 460, 620, 660, 790, 920, 980 };

            image.Mutate(context =>
            {
                AddTextToImage(context, col[4], row[0], "CardNo", card.CardNo);
                AddTextToImage(context, col[1], row[1], "NameZH", card.Surname);
                AddTextToImage(context, col[1], row[2], "LongEN", card.RomanizedSurname);
                AddTextToImage(context, col[1], row[3], "LongEN", card.MorseSurname);
                AddTextToImage(context, col[2], row[1], "NameZH", card.GivenName);
                AddTextToImage(context, col[2], row[2], "LongEN", card.RomanizedGivenName);
                AddTextToImage(context, col[2], row[3], "LongEN", card.MorseGivenName);
                AddTextToImage(context, col[3], row[1], "CodeEN", card.Sex.ToString());
                AddTextToImage(context, col[1], row[4], "CodeEN", mrz.HolderNationalityIso3166);
                AddTextToImage(context, col[2], row[4], "CodeEN", "Tianyi City");
                AddTextToImage(context, col[1], row[5], "LongEN", "Border Protection Center,");
                AddTextToImage(context, col[1], row[6], "LongEN", "Tianyi City Police Department");
                AddTextToImage(context, col[3], row[4], "CodeEN", card.DateOfBirth.ToString("ddMMMyyyy", new CultureInfo("en-US")).ToUpper());
                AddTextToImage(context, col[3], row[5], "CodeEN", card.IssuedAt.ToString("ddMMMyyyy", new CultureInfo("en-US")).ToUpper());
                AddTextToImage(context, col[3], row[7], "CodeEN", card.ExpiresAt.ToString("ddMMMyyyy", new CultureInfo("en-US")).ToUpper());
                AddTextToImage(context, col[0], row[8], "MRZSum", mrz.Line1);
                AddTextToImage(context, col[0], row[9], "MRZSum", mrz.Line2);
            });

            return await ImageGenerated.Create(image);
        }

        private void AddTextToImage(IImageProcessingContext context, float x, float y, string styleName, string text)
        {
            var style = _imageStyles[styleName];
            for (var i = 0; i < text.Length; i++)
            {
                context.DrawText(
                    new RichTextOptions(new Font(style.FontFamily, style.FontSize))
                    {
                        Origin = new Vector2(x + style.Space * i, y),
                    },
                    text: text[i].ToString(),
                    brush: Brushes.Solid(style.Color)
                );
            }
        }
    }
}
