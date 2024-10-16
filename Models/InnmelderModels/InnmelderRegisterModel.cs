﻿using System.ComponentModel.DataAnnotations;

namespace Models.InnmelderModels;

public class InnmelderRegisterModel
{
    [Required(ErrorMessage = "Brukernavn er påkrevd.")]
    [StringLength(100)]
    public string Username { get; set; }

    [Required(ErrorMessage = "E-post er påkrevd.")]
    [EmailAddress(ErrorMessage = "Ugyldig e-postadresse.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Passord er påkrevd.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Passordene samsvarer ikke.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passordene samsvarer ikke.")]
    public string BekreftPassord { get; set; }
}