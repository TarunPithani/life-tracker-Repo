namespace LifeTracker.Application.DTOs.Auth;

public class ChangeEmailRequest
{
    public string NewEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
