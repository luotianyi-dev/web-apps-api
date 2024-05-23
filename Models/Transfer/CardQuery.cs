using System.ComponentModel.DataAnnotations;
using TianyiNetwork.Web.AppsApi.Models.Validators;

namespace TianyiNetwork.Web.AppsApi.Models.Transfer
{
    [CardDateOfBirthValidator]
    public class CardQuery : ICardDateOfBirthValidatable
    {
        [Required(ErrorMessage = "必须填写卡号")]
        [RegularExpression(@"^([0-9a-zA-Z]+)$", ErrorMessage = "卡号必须由数字或字母组成")]
        public string CardNo { get; set; } = "";

        [Required(ErrorMessage = "必须填写姓")]
        public string Surname { get; set; } = "";

        [Required(ErrorMessage = "必须填写名")]
        public string GivenName { get; set; } = "";

        [Required(ErrorMessage = "必须填写生日")]
        [RegularExpression(@"^(\d{4})-(\d{2})-(\d{2})$", ErrorMessage = "生日必须是 YYYY-MM-DD 格式")]
        public string DateOfBirth { get; set; } = "";
    }
}
