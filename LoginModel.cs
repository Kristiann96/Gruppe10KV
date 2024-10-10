using System.ComponentModel.DataAnnotations;

namespace Gruppe10KVprototype.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "E-post er p�krevd.")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Passord er p�krevd.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
