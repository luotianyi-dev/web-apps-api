using System.ComponentModel.DataAnnotations;
using TianyiNetwork.Web.AppsApi.Models.Validators;
using TianyiNetwork.Web.AppsApi.Services;

namespace TianyiNetwork.Web.AppsApi.Models.Transfer
{
    [CardDateOfBirthValidator]
    public class CardApplication : ICardDateOfBirthValidatable
    {
        [Required(ErrorMessage = "必须填写姓")]
        [RegularExpression(@"^([a-zA-Z\u4e00-\u9fff]{1,3})$", ErrorMessage = "姓应该是 1-3 个中英文字符，且为常用汉字 (U+4E00-U+9FFF)")]
        public string Surname { get; set; } = "";

        [Required(ErrorMessage = "必须填写名")]
        [RegularExpression(@"^([a-zA-Z\u4e00-\u9fff]{1,3})$", ErrorMessage = "名应该是 1-3 个中英文字符，且为常用汉字 (U+4E00-U+9FFF)")]
        public string GivenName { get; set; } = "";

        [Required(ErrorMessage = "必须填写姓拼音")]
        [RegularExpression(@"^([a-zA-Z]{1,12})$", ErrorMessage = "姓拼音必须是 1-12 个英文字母")]
        public string RomanizedSurname { get; set; } = "";

        [Required(ErrorMessage = "必须填写名拼音")]
        [RegularExpression(@"^([a-zA-Z]{1,12})$", ErrorMessage = "名拼音必须是 1-12 个英文字母")]
        public string RomanizedGivenName { get; set; } = "";

        // ReSharper disable once StringLiteralTypo
        [Required(ErrorMessage = "必须填写性别")]
        [RegularExpression(@"^[MFXmfx]{1}$", ErrorMessage = "性别必须是以下值之一：M, F, X")]
        public string Sex { get; set; } = "";

        [Required(ErrorMessage = "必须填写生日")]
        [RegularExpression(@"^(\d{4})-(\d{2})-(\d{2})$", ErrorMessage = "生日必须是 YYYY-MM-DD 格式")]
        public string DateOfBirth { get; set; } = "";

        public void TrimFields()
        {
            Surname = Surname.Trim();
            GivenName = GivenName.Trim();
            RomanizedSurname = RomanizedSurname.Trim().ToUpperInvariant();
            RomanizedGivenName = RomanizedGivenName.Trim().ToUpperInvariant();
            Sex = Sex.Trim().ToUpperInvariant();
        }

        public bool HasBlockedWords(BlockedWordService blockedWordService)
        {
            string[] fieldsToCheck = [
                Surname,
                GivenName,
                RomanizedSurname,
                RomanizedGivenName,
                Surname + GivenName,
                GivenName + Surname,
                RomanizedSurname + RomanizedGivenName,
                RomanizedGivenName + RomanizedSurname,
                Surname + " " + GivenName,
                GivenName + " " + Surname,
                RomanizedSurname + " " + RomanizedGivenName,
                RomanizedGivenName + " " + RomanizedSurname
            ];
            return fieldsToCheck.Any(blockedWordService.HasBlockedWord);
        }
    }
}
