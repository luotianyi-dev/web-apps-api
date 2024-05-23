using System.ComponentModel.DataAnnotations;
using TianyiNetwork.Web.AppsApi.Models.Transfer;

namespace TianyiNetwork.Web.AppsApi.Models.Validators
{
    public class CardDateOfBirthValidator : ValidationAttribute
    {
        private static readonly DateOnly MaxDateOfBirth = new(2100, 1, 1);
        private static readonly DateOnly MinDateOfBirth = new(1900, 1, 1);

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is not ICardDateOfBirthValidatable application)
            {
                return new ValidationResult("无法验证非 ICardWithDateOfBirth 类型的对象");
            }

            if (!DateOnly.TryParseExact(application.DateOfBirth, "yyyy-MM-dd", out var dateOfBirth))
            {
                return new ValidationResult(
                    "日期无效，请确保您输入了一个有效的出生日期",
                    [nameof(ICardDateOfBirthValidatable.DateOfBirth)]);
            }

            if (dateOfBirth > MaxDateOfBirth || dateOfBirth < MinDateOfBirth)
            {
                return new ValidationResult(
                    "日期超出了合理范围，出生年份必须在 1901-2099 年之间",
                    [nameof(ICardDateOfBirthValidatable.DateOfBirth)]);

            }

            return ValidationResult.Success;
        }
    }
}
