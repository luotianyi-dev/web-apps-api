using System.ComponentModel.DataAnnotations;
using TianyiNetwork.Web.AppsApi.Models.Entities;

namespace TianyiNetwork.Web.AppsApi.Models.Transfer
{
    public class CardMrz
    {
        public CardMrz(CardEntity card, string type, string issuerState, string holderNationality)
        {
            Card = card;
            Icao9303DocumentType = type;
            IssuerStateIso3166 = issuerState;
            HolderNationalityIso3166 = holderNationality;
        }

        public CardMrz(CardEntity card)
        {
            Card = card;
        }

        [RegularExpression(@"^[IPV]{1}$")] public string Icao9303DocumentType = "I";
        [RegularExpression(@"^[A-Z]{3}$")] public string IssuerStateIso3166 = "LTY";
        [RegularExpression(@"^[A-Z]{3}$")] public string HolderNationalityIso3166 = "LTY";

        public CardEntity Card { get; }
        public string Name => $"{Card.RomanizedSurname}<<{Card.RomanizedGivenName}".PadRight(38, '<')[..38];
        public string Line1 => $"{Icao9303DocumentType}<{IssuerStateIso3166}<{Name}";

        public string CardNo => Card.CardNo.PadRight(9, '<')[..9];
        public string CardNoWithChecksum => $"{CardNo}{Icao9303Checksum(CardNo)}";

        public string DateOfBrith => Card.DateOfBirth.ToString("yyMMdd");
        public string DateOfBrithWithChecksum => $"{DateOfBrith}{Icao9303Checksum(DateOfBrith)}";

        public string ExpiresAt => Card.ExpiresAt.ToString("yyMMdd");
        public string ExpiresAtWithChecksum => $"{ExpiresAt}{Icao9303Checksum(ExpiresAt)}";

        public string PersonalNumber => $"{RemoveSpace(Card.MorseSurname)}<<{RemoveSpace(Card.MorseGivenName)}".PadRight(14, '<')[..14];
        public string PersonalNumberWithChecksum => $"{PersonalNumber}{Icao9303Checksum(PersonalNumber)}";

        public string CompositeChecksum => Icao9303Checksum($"{CardNoWithChecksum}{DateOfBrithWithChecksum}{ExpiresAtWithChecksum}{PersonalNumberWithChecksum}");
        public string Line2 => $"{CardNoWithChecksum}{HolderNationalityIso3166}{DateOfBrithWithChecksum}{Card.Sex}{ExpiresAtWithChecksum}{PersonalNumberWithChecksum}{CompositeChecksum}";

        public static string RemoveSpace(string source) => source.Replace(" ", string.Empty);

        public static string Icao9303Checksum(string source)
        {
            var s = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var w = new int[] { 7, 3, 1 };
            var c = 0;
            for (var i = 0; i < source.Length; i++)
            {
                if (source[i] == '<') continue;
                c += s.IndexOf(source[i]) * w[i % 3];
            }
            c %= 10;
            return c.ToString();
        }
    }
}
