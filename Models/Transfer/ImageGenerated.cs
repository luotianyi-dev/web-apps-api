using SixLabors.ImageSharp;

namespace TianyiNetwork.Web.AppsApi.Models.Transfer
{
    public struct ImageGenerated
    {
        public byte[] Webp { get; set; }
        public byte[] Png { get; set; }
        public byte[] Jpeg { get; set; }

        public byte[] GetFormat(string format) => format switch
        {
            "webp" => Webp,
            "png" => Png,
            "jpg" => Jpeg,
            "jpeg" => Jpeg,
            _ => throw new System.ArgumentException("Invalid format", nameof(format))
        };

        public static async Task<ImageGenerated> Create(Image image)
        {
            using var webp = new MemoryStream();
            await image.SaveAsWebpAsync(webp);

            using var png = new MemoryStream();
            await image.SaveAsPngAsync(png);

            using var jpeg = new MemoryStream();
            await image.SaveAsJpegAsync(jpeg);

            return new ImageGenerated
            {
                Png = png.ToArray(),
                Webp = webp.ToArray(),
                Jpeg = jpeg.ToArray(),
            };
        }
    }
}
