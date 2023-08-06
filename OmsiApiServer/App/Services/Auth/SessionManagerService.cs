using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OmsiApiServer.App.Database.Models;
using OmsiApiServer.App.Services.Configuration;

namespace OmsiApiServer.App.Services.Auth;

public class SessionManagerService
{
    private readonly string Secret = "";
    private readonly int Expires;

    public SessionManagerService(ConfigService configService)
    {
        var config = configService
            .Get()
            .OmsiClient.Security;

        Secret = config.Secret;
        Expires = config.Expires;
    }


    public async Task<string> CreateToken(User user)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        
        if (Secret == null)
        {
            throw new NoNullAllowedException();
        }

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(Expires == 0 ? 14 : Expires),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = creds
        };
        
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}