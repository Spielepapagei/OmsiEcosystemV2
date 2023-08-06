using DiscordLogging.App.Database.Models;
using dotnet_rpg.Models;
using Microsoft.EntityFrameworkCore;
using OmsiApiServer.App.Database;
using OmsiApiServer.App.Models;

namespace OmsiApiServer.App.Services.Auth;

public class IdentityService
{
    private readonly Repository<User> UserRepo;
    
    public IdentityService(Repository<User> userRepo)
    {
        UserRepo = userRepo;
    }

    public async Task Login(UserLoginDto loginDto)
    {
        
    }

    public async Task<ServiceResponse<int>> Register(User user, string password)
    {
        
        
        ServiceResponse<int> response = new ServiceResponse<int>();
        if (await UserExist(user.Username))
        {
            response.Success = false;
            response.Message = "User already exists.";
            return response;
        }
        
        CreatePasswordHash(password, out byte[] passwordHash,out byte[] passwordSalt);

        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;
        user.Username = user.Username.ToLower();
        
        UserRepo.Add(user);
        return response;
    }

    public async Task<bool> UserExist(string username)
    {
        return await UserRepo.Get().AnyAsync(x => x.Username.ToLower() == username.ToLower());
    }
    
    
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }
}