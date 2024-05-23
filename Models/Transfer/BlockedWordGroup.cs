namespace TianyiNetwork.Web.AppsApi.Models.Transfer
{
    public class BlockedWordGroup
    {
        public int WordLength { get; set; }
        public IEnumerable<string> Sha1Hashes { get; set; } = new List<string>();
    }
}
