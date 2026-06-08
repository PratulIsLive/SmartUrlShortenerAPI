using Microsoft.AspNetCore.Mvc;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Status = "Application Running",
            Project = "SmartUrlShortener",
            Developer = "Pratul Sharma",
            Time = DateTime.UtcNow
        });
    }
}