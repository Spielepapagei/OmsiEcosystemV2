using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmsiApiServer.App.Database;
using OmsiApiServer.App.Database.Models;
using OmsiApiServer.App.Dtos;
using OmsiApiServer.App.Dtos.User;
using OmsiApiServer.App.Services.Auth;
using OmsiApiServer.App.Services.Configuration;

namespace OmsiApiServer.App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    public static User User = new();
    private readonly ConfigService ConfigService;
    private readonly IdentityService IdentityService;
    private readonly Repository<User> UserRepo;
    
    public AuthController(ConfigService configService, IdentityService identityService, Repository<User> userRepo)
    {
        ConfigService = configService;
        IdentityService = identityService;
        UserRepo = userRepo;
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<ServiceResponse<int>>> Login(UserLoginDto request)
    {
        var response = await IdentityService.Login(request.Username, request.Password);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
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

    [Authorize]
    [HttpGet("check")]
    public async Task<ActionResult<ServiceResponse<string>>> CheckSession()
    {
        var x = new ServiceResponse<string>();
        ClaimsPrincipal httpUser = HttpContext.User;

        x.Data = "Undefined";
        x.Message = "Token is Valid";
        
        string identifierValue = httpUser.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        if (identifierValue != null)
        {
            var user = UserRepo.Get().FirstOrDefault(x => x.Id == int.Parse(identifierValue));
            x.Data = $"{user.Username}";
        }
        
        return Ok(x);
    }
    

}