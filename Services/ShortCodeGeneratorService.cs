namespace UrlShortener.API.Services;

public class ShortCodeGeneratorService
{
    public string GenerateCode()
    {
        return Guid.NewGuid()
            .ToString()
            .Replace("-", "")
            .Substring(0, 6);
    }
}