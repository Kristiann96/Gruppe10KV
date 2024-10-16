using System.ComponentModel.DataAnnotations;

namespace Models.InnmelderModels;

public class InnmeldingLoginModel
{
    [Required(ErrorMessage = "E-post er påkrevd.")]
    [EmailAddress(ErrorMessage = "Ugyldig e-postadresse.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Passord er påkrevd.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}