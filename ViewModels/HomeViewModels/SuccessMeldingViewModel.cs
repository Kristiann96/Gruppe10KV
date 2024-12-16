namespace ViewModels.HomeViewModels;

public class SuccessMeldingViewModel
{
    public string? Message { get; set; }
    public bool ShowSuccess => !string.IsNullOrEmpty(Message);
}