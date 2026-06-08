namespace UrlShortener.API.DTOs;

public class DashboardResponse
{
    public int TotalUrls { get; set; }

    public int TotalVisits { get; set; }

    public string? MostVisitedShortCode { get; set; }

    public int ActiveUrls { get; set; }

    public int ExpiredUrls { get; set; }
}