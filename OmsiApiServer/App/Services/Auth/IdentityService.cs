using Microsoft.EntityFrameworkCore;
using OmsiApiServer.App.Database;
using OmsiApiServer.App.Database.Models;
using OmsiApiServer.App.Dtos;

namespace OmsiApiServer.App.Services.Auth;

public class IdentityService
{
    private readonly Repository<User> UserRepo;
    private readonly SessionManagerService SessionManager;
    
    public IdentityService(Repository<User> userRepo, SessionManagerService sessionManager)
    {
        UserRepo = userRepo;
        SessionManager = sessionManager;
    }

    public async Task<ServiceResponse<string>> Login(string username, string password)
    {
        var response = new ServiceResponse<string>();
        var user = await UserRepo.Get().FirstOrDefaultAsync(x => x.Username.ToLower().Equals(username.ToLower()));

        if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            response.Success = false;
            response.Message = "User Password combination is wrong.";
        }
        else
        {
            response.Data = await SessionManager.CreateToken(user);
        }
        return response;
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
        return await UserRepo.Get().AnyAsync(x => x.Username.ToLower().Equals(username.ToLower()));
    }
    
    
    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
        {
            var comuteHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return comuteHash.SequenceEqual(passwordHash);
        }
    }
}