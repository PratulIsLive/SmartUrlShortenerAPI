using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.DTOs;
using UrlShortener.API.Models;
using UrlShortener.API.Services;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UrlController : ControllerBase
{
    private readonly ShortCodeGeneratorService _generator;
    private readonly UrlStorageService _storage;
    private readonly QrCodeService _qrCodeService;

    public UrlController(
        ShortCodeGeneratorService generator,
        UrlStorageService storage,
        QrCodeService qrCodeService)
    {
        _generator = generator;
        _storage = storage;
        _qrCodeService = qrCodeService;
    }

    [HttpPost("shorten")]
    public IActionResult Shorten(CreateShortUrlRequest request)
    {
        if (!Uri.TryCreate(
                request.OriginalUrl,
                UriKind.Absolute,
                out _))
        {
            return BadRequest("Invalid URL");
        }

        string shortCode;

        if (!string.IsNullOrWhiteSpace(request.CustomCode))
        {
            if (_storage.ShortCodeExists(request.CustomCode))
            {
                return BadRequest("Custom code already exists");
            }

            shortCode = request.CustomCode;
        }
        else
        {
            shortCode = _generator.GenerateCode();
        }

        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var mapping = new UrlMapping
        {
            OriginalUrl = request.OriginalUrl,
            ShortCode = shortCode,
            UserId = userId,
            ExpiryDate = request.ExpiryDate
        };

        _storage.Save(mapping);

        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        return Ok(new
        {
            OriginalUrl = mapping.OriginalUrl,
            ShortCode = mapping.ShortCode,
            ShortUrl = $"{baseUrl}/{mapping.ShortCode}",
            QrCodeUrl = $"{baseUrl}/api/url/qrcode/{mapping.ShortCode}",
            QrCodeDownloadUrl = $"{baseUrl}/api/url/qrcode/{mapping.ShortCode}/download"
        });
    }

    [HttpGet("all")]
    public IActionResult GetAll(
        string? search,
        string? sortBy,
        string? sortOrder,
        int page = 1,
        int pageSize = 5)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var urls = _storage.GetFilteredUrls(
            userId,
            search,
            sortBy,
            sortOrder,
            page,
            pageSize);

        var totalCount =
            _storage.GetFilteredCount(
                userId,
                search);

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            Search = search,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Data = urls
        });
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var mostVisited =
            _storage.GetMostVisitedUrl(userId);

        var response = new DashboardResponse
        {
            TotalUrls = _storage.GetTotalUrls(userId),
            TotalVisits = _storage.GetTotalVisits(userId),
            MostVisitedShortCode = mostVisited?.ShortCode,
            ActiveUrls = _storage.GetActiveUrls(userId),
            ExpiredUrls = _storage.GetExpiredUrls(userId)
        };

        return Ok(response);
    }

    [HttpGet("stats/{shortCode}")]
    public IActionResult GetStats(string shortCode)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var mapping = _storage.GetByShortCodeAndUserId(
            shortCode,
            userId);

        if (mapping == null)
        {
            return NotFound("Short URL not found");
        }

        var response = new UrlStatsResponse
        {
            OriginalUrl = mapping.OriginalUrl,
            ShortCode = mapping.ShortCode,
            VisitCount = mapping.VisitCount,
            CreatedAt = mapping.CreatedAt,
            LastAccessedAt = mapping.LastAccessedAt
        };

        return Ok(response);
    }

    [HttpPut("{shortCode}")]
    public IActionResult Update(
        string shortCode,
        UpdateUrlRequest request)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var mapping = _storage.GetByShortCodeAndUserId(
            shortCode,
            userId);

        if (mapping == null)
        {
            return NotFound("Short URL not found");
        }

        if (!Uri.TryCreate(
                request.OriginalUrl,
                UriKind.Absolute,
                out _))
        {
            return BadRequest("Invalid URL");
        }

        mapping.OriginalUrl = request.OriginalUrl;

        _storage.Update(mapping);

        return Ok(new
        {
            Message = "Short URL updated successfully"
        });
    }

    [HttpDelete("{shortCode}")]
    public IActionResult Delete(string shortCode)
    {
        var userId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var mapping = _storage.GetByShortCodeAndUserId(
            shortCode,
            userId);

        if (mapping == null)
        {
            return NotFound("Short URL not found");
        }

        _storage.Delete(mapping);

        return Ok(new
        {
            Message = "Short URL deleted successfully"
        });
    }

    [AllowAnonymous]
    [HttpGet("qrcode/{shortCode}")]
    public IActionResult GenerateQrCode(string shortCode)
    {
        var mapping = _storage.GetByShortCode(shortCode);

        if (mapping == null)
        {
            return NotFound("Short URL not found");
        }

        var shortUrl =
            $"{Request.Scheme}://{Request.Host}/{shortCode}";

        var qrCodeBytes =
            _qrCodeService.GenerateQrCode(shortUrl);

        return File(
            qrCodeBytes,
            "image/png");
    }

    [AllowAnonymous]
    [HttpGet("qrcode/{shortCode}/download")]
    public IActionResult DownloadQrCode(string shortCode)
    {
        var mapping = _storage.GetByShortCode(shortCode);

        if (mapping == null)
        {
            return NotFound("Short URL not found");
        }

        var shortUrl =
            $"{Request.Scheme}://{Request.Host}/{shortCode}";
            
        var qrCodeBytes =
            _qrCodeService.GenerateQrCode(shortUrl);

        return File(
            qrCodeBytes,
            "image/png",
            $"{shortCode}-qrcode.png");
    }

    [AllowAnonymous]
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