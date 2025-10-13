namespace RageRealm.Shared.Configuration;

public class SmtpConfiguration
{
    public string Host { get; set; } = "smtp.example.com";
    public int Port { get; set; } = 587;
    public bool EnableSsl { get; set; } = false;
    public string Email { get; set; } = "test@example.com";
    public string Password { get; set; } = "password";
}