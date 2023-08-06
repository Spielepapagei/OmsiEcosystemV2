using DiscordLogging.App.Database.Models;
using dotnet_rpg.Models;
using Microsoft.AspNetCore.Mvc;
using OmsiApiServer.App.Models;
using OmsiApiServer.App.Services;
using OmsiApiServer.App.Services.Auth;

namespace OmsiApiServer.App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public static User user = new();
    private readonly ConfigService ConfigService;
    private readonly IdentityService IdentityService;
    
    public AuthController(ConfigService configService, IdentityService identityService)
    {
        ConfigService = configService;
        IdentityService = identityService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ServiceResponse<int>>> Register(UserRegisterDto request)
    {
        var repose = await IdentityService.Register(
            new User()
            {
                Username = request.Username
            },
            request.Password);

        if (!repose.Success)
        {
            return BadRequest(repose);
        }

        return Ok(repose);
    }
    

}