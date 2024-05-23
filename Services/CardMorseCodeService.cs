using TianyiNetwork.Web.AppsApi.Extensions;

namespace TianyiNetwork.Web.AppsApi.Services
{
    public class CardMorseCodeService(Dictionary<string, int> codebook) : ISingletonService<CardMorseCodeService>
    {
        public Dictionary<string, int> Codebook { get; } = codebook;


        public static CardMorseCodeService Create(WebApplicationBuilder builder)
        {
            var codebook = new Dictionary<string, int>();
            var lines = File
                .ReadAllLines(builder.GetResourceByName("Card/Morse.txt"))
                .ToList()
                .FindAll(x => !string.IsNullOrEmpty(x))
                .ConvertAll(x => x.Trim());
            lines.ForEach(line =>
            {
                codebook.TryAdd(line[4..].Trim(), int.Parse(line[..4]));
            });
            return new CardMorseCodeService(codebook);
        }

        public int GetMorseCode(char c)
        {
            Codebook.TryGetValue(c.ToString(), out var result);
            return result;
        }

        public int[] GetMorseCode(string text)
        {
            return text
                .ToCharArray()
                .ToList()
                .ConvertAll(GetMorseCode)
                .ToArray();
        }

        public string GetReadableMorseCode(char c)
        {
            return GetMorseCode(c).ToString().PadLeft(4, '0');
        }

        public string GetReadableMorseCode(string text)
        {
            var codes = text
                .ToCharArray()
                .ToList()
                .ConvertAll(GetReadableMorseCode);
            return string.Join(" ", codes);
        }
    }
}
