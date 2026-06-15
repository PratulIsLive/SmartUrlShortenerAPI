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

    private int CurrentUserId =>
        int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

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
    public IActionResult Shorten(
        [FromBody] CreateShortUrlRequest request)
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

        var mapping = new UrlMapping
        {
            OriginalUrl = request.OriginalUrl,
            ShortCode = shortCode,
            UserId = CurrentUserId,
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
        var urls = _storage.GetFilteredUrls(
            CurrentUserId,
            search,
            sortBy,
            sortOrder,
            page,
            pageSize);

        var totalCount =
            _storage.GetFilteredCount(
                CurrentUserId,
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
        var mostVisited =
            _storage.GetMostVisitedUrl(CurrentUserId);

        var response = new DashboardResponse
        {
            TotalUrls = _storage.GetTotalUrls(CurrentUserId),
            TotalVisits = _storage.GetTotalVisits(CurrentUserId),
            MostVisitedShortCode = mostVisited?.ShortCode,
            ActiveUrls = _storage.GetActiveUrls(CurrentUserId),
            ExpiredUrls = _storage.GetExpiredUrls(CurrentUserId)
        };

        return Ok(response);
    }

    [HttpGet("stats/{shortCode}")]
    public IActionResult GetStats(string shortCode)
    {
        var mapping = _storage.GetByShortCodeAndUserId(
            shortCode,
            CurrentUserId);

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
        [FromBody] UpdateUrlRequest request)
    {
        var mapping = _storage.GetByShortCodeAndUserId(
            shortCode,
            CurrentUserId);

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

        return Ok(new UpdateResponse
        {
            Message = "Short URL updated successfully"
        });
    }

    [HttpDelete("{shortCode}")]
    public IActionResult Delete(string shortCode)
    {
        var mapping = _storage.GetByShortCodeAndUserId(
            shortCode,
            CurrentUserId);

        if (mapping == null)
        {
            return NotFound("Short URL not found");
        }

        _storage.Delete(mapping);

        return Ok(new DeleteResponse
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

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok(new
        {
            Message = "Authorized successfully",
            UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Username = User.FindFirst(ClaimTypes.Name)?.Value
        });
    }
}