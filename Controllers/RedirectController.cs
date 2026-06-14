using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Services;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("")]
public class RedirectController : ControllerBase
{
    private readonly UrlStorageService _storage;

    public RedirectController(UrlStorageService storage)
    {
        _storage = storage;
    }

    [HttpGet("{shortCode}")]
    public IActionResult RedirectToOriginalUrl(string shortCode)
    {
        var mapping = _storage.GetByShortCode(shortCode);

        if (mapping == null)
        {
            return NotFound("Short URL not found");
        }

        if (mapping.ExpiryDate.HasValue &&
            mapping.ExpiryDate.Value < DateTime.UtcNow)
        {
            return BadRequest("This short URL has expired");
        }

        mapping.VisitCount++;
        mapping.LastAccessedAt = DateTime.UtcNow;

        _storage.Update(mapping);

        return Redirect(mapping.OriginalUrl);
    }
}