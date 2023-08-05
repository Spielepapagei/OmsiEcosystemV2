using Microsoft.AspNetCore.Mvc;
using OmsiApiServer.App.Models;
using OmsiApiServer.App.Services;

namespace OmsiApiServer.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionController : ControllerBase
{
    private readonly SessionManagerService SessionManagerService;
    
    public SessionController(SessionManagerService sessionManagerService)
    {
        SessionManagerService = sessionManagerService;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginModel request)
    {
        var userLogin = await SessionManagerService.UserLogin(request);
        
        if (userLogin is { Token: not null })
        {
            return Ok(new { Token = userLogin.Token });
        }

        return Unauthorized(new { Message = "Invalid credentials" });
    }
    
    
}