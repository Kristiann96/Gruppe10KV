using System.ComponentModel.DataAnnotations;

namespace ViewModels;
public class LoggInnViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

