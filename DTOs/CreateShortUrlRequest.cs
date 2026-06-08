namespace UrlShortener.API.DTOs;

public class CreateShortUrlRequest
{
    public string OriginalUrl { get; set; } = string.Empty;

    public string? CustomCode { get; set; }

    public DateTime? ExpiryDate { get; set; }
}