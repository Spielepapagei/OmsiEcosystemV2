namespace OmsiApiServer.App.Models;

public class UserLoginModel
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Token { get; set; } = null;
}