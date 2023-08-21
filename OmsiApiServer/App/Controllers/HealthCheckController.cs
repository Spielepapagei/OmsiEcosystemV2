using Microsoft.AspNetCore.Mvc;
using OmsiApiServer.App.Dtos;
using OmsiApiServer.App.Services.Configuration;

namespace OmsiApiServer.App.Controllers;

[ApiController]
[Route("api/")]
public class HealthCheckController : ControllerBase
{
    private readonly string Version;
    
    public HealthCheckController(ConfigService configService)
    {
        var config = configService
            .Get().OmsiClient;

        Version = config.Version;
    }
    
    [HttpGet("health")]
    public async Task<ActionResult<ServiceResponse<string>>> CheckHealth()
    {
        var x = new ServiceResponse<string>();

        x.Data = Version;
        x.Message = "Api is Running and Healthy.";
        
        return Ok(x);
    }
    
    
}