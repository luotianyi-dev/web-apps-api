using System.Text;
using System.Security.Cryptography;
using TianyiNetwork.Web.AppsApi.Extensions;
using TianyiNetwork.Web.AppsApi.Models.Transfer;

namespace TianyiNetwork.Web.AppsApi.Services
{
    public class BlockedWordService : ISingletonService<BlockedWordService>
    {
        public string[] Words { get; }
        public BlockedWordGroup[] Groups { get; }

        public static BlockedWordService Create(WebApplicationBuilder builder)
        {
            var words = File.ReadAllLines(builder.GetResourceByName("BlockedWord/BlockedWords.txt"))
                .ToList()
                .ConvertAll(x => x.Trim().ToUpperInvariant())
                .FindAll(x => !string.IsNullOrEmpty(x) && !x.StartsWith('#'))
                .ToArray();
            return new BlockedWordService(words);
        }

        public BlockedWordService(string[] words)
        {
            Words = words;
            var hashes = new Dictionary<int, List<string>>();
            Words.ToList().ForEach(x => hashes[x.Length] = []);
            Words.ToList().ForEach(x =>
            {
                var hash = SHA1.HashData(Encoding.UTF8.GetBytes(x))
                    .ToList()
                    .ConvertAll(b => b.ToString("x2"));
                hashes[x.Length].Add(string.Join("", hash));
            });
            Groups = hashes
                .ToList()
                .ConvertAll(x => new BlockedWordGroup
                {
                    WordLength = x.Key,
                    Sha1Hashes = x.Value
                })
                .ToArray();
        }

        public bool HasBlockedWord(string text)
        {
            return Words.Any(s => text.ToUpperInvariant().Contains(s));
        }
    }
}
