namespace UrlShortener.API.DTOs;

public class UrlStatsResponse
{
    public string OriginalUrl { get; set; } = string.Empty;

    public string ShortCode { get; set; } = string.Empty;

    public int VisitCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastAccessedAt { get; set; }
}