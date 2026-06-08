namespace UrlShortener.API.Models;

public class UrlMapping
{
    public int Id { get; set; }

    public string OriginalUrl { get; set; } = string.Empty;

    public string ShortCode { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int VisitCount { get; set; } = 0;

    public DateTime? LastAccessedAt { get; set; }

    public DateTime? ExpiryDate { get; set; }

    // Ownership
    public int UserId { get; set; }

    public User? User { get; set; }
}