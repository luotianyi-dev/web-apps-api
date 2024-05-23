using System.Globalization;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using TianyiNetwork.Web.AppsApi.Extensions;
using TianyiNetwork.Web.AppsApi.Models.Transfer;

namespace TianyiNetwork.Web.AppsApi.Services
{
    public class PaperboardImageService(byte[] background, Font font) : ISingletonService<PaperboardImageService>
    {
        private static readonly CultureInfo FontLocale = new("zh-CN");
        private static readonly Color FontColor = Color.FromRgba(30, 30, 30, 204);
        private const float FontSize = 160;
        private const float RotateDegree = 10;
        private const float XCenter = 165;
        private const float YCenter = 915;
        private const float XCenterChar = 145;
        private const float YMargin = 20;

        public static PaperboardImageService Create(WebApplicationBuilder builder)
        {
            var background = File.ReadAllBytes(builder.GetResourceByName("Paperboard/Background.jpg"));
            var font = new FontCollection().Add(builder.GetResourceByName("Paperboard/SourceHanSansSC.otf"), FontLocale).CreateFont(FontSize);
            return new PaperboardImageService(background, font);
        }

        public async Task<ImageGenerated> Draw(string line1, string line2)
        {
            using var image = Image.Load(background);
            var singleLineMode = string.IsNullOrWhiteSpace(line1) || string.IsNullOrWhiteSpace(line2);
            var xCenterLine1 = line1.Trim().Length == 2 ? XCenterChar : XCenter;
            var xCenterLine2 = line2.Trim().Length == 2 ? XCenterChar : XCenter;

            image.Mutate(context =>
            {
                if (singleLineMode)
                {
                    var text = $"{line1}{line2}".Trim();
                    context.SetDrawingTransform(Matrix3x2Extensions.CreateRotationDegrees(RotateDegree, new PointF(XCenter, YCenter)))
                        .DrawText(
                            new RichTextOptions(font) { Origin = new PointF(xCenterLine1, YCenter - YMargin - FontSize / 2) },
                            text: text, brush: Brushes.Solid(FontColor));
                }
                else
                {
                    context.SetDrawingTransform(Matrix3x2Extensions.CreateRotationDegrees(10F, new PointF(XCenter, YCenter)))
                        .DrawText(
                            new RichTextOptions(font) { Origin = new PointF(xCenterLine1, YCenter - YMargin - FontSize) },
                    text: line1, brush: Brushes.Solid(FontColor))
                        .DrawText(
                            new RichTextOptions(font) { Origin = new PointF(xCenterLine2, YCenter + YMargin) },
                    text: line2, brush: Brushes.Solid(FontColor));
                }
            });

            return await ImageGenerated.Create(image);
        }
    }
}
