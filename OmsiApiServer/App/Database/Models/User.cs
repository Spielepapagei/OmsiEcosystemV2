namespace OmsiApiServer.App.Database.Models;

public class User
{
    public int Id { get; set; }
    public int DriverId { get; set; }
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime PasswordChangedAt { get; set; } = DateTime.UtcNow.AddHours(-1);
    public DateTime LastLoggedIn { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}