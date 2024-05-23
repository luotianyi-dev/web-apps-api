using System.ComponentModel.DataAnnotations;

namespace TianyiNetwork.Web.AppsApi.Models.Transfer
{
    public class PaperboardGenerateRequest
    {
        [Required(ErrorMessage = "必须填写第一行文字"), MinLength(1, ErrorMessage = "第一行文字最短为 1 个字"), MaxLength(2, ErrorMessage = "第一行文字最长为 2 个字")]
        public string Line1 { get; set; } = "";

        [MaxLength(2, ErrorMessage = "第二行文字最长为 2 个字")]
        public string Line2 { get; set; } = "";

        [RegularExpression("^(jpeg|webp)$", ErrorMessage = "格式必须为 jpeg 或 webp")]
        public string Format { get; set; } = "jpeg";
    }
}
