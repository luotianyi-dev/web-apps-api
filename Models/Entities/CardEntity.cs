using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TianyiNetwork.Web.AppsApi.Models.Entities
{
    [Table("cards")]
    [Index(nameof(CardNo), IsUnique = true)]
    [Index(nameof(Key), IsUnique = true)]
    public class CardEntity
    {
        [Column("id", TypeName = "int unsigned")]
        public int Id { get; set; }

        [Column("card_no", TypeName = "char(9)")]
        public string CardNo { get; set; } = "";

        [Column("surname", TypeName = "varchar(6)")]
        public string Surname { get; set; } = "";

        [Column("given_name", TypeName = "varchar(6)")]
        public string GivenName { get; set; } = "";

        [Column("romanized_surname", TypeName = "varchar(16)")]
        public string RomanizedSurname { get; set; } = "";

        [Column("romanized_given_name", TypeName = "varchar(16)")]
        public string RomanizedGivenName { get; set; } = "";

        [Column("morse_surname", TypeName = "varchar(24)")]
        public string MorseSurname { get; set; } = "";

        [Column("morse_given_name", TypeName = "varchar(24)")]
        public string MorseGivenName { get; set; } = "";

        [Column("sex", TypeName = "char(1)")]
        public char Sex { get; set; }

        [Column("date_of_birth", TypeName = "date")]
        public DateOnly DateOfBirth { get; set; }

        [Column("issued_at", TypeName = "date")]
        public DateOnly IssuedAt { get; set; }

        [Column("expires_at", TypeName = "date")]
        public DateOnly ExpiresAt { get; set; }

        [Column("key", TypeName = "char(40)")]
        public string Key { get; set; } = "";
    }
}
