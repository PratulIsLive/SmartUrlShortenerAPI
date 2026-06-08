using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Data;
using UrlShortener.API.Models;

namespace UrlShortener.API.Services;

public class UrlStorageService
{
    private readonly AppDbContext _context;

    public UrlStorageService(AppDbContext context)
    {
        _context = context;
    }

    public void Save(UrlMapping mapping)
    {
        _context.UrlMappings.Add(mapping);
        _context.SaveChanges();
    }

    public UrlMapping? GetByShortCode(string shortCode)
    {
        return _context.UrlMappings
            .FirstOrDefault(x => x.ShortCode == shortCode);
    }

    public UrlMapping? GetByShortCodeAndUserId(
        string shortCode,
        int userId)
    {
        return _context.UrlMappings
            .FirstOrDefault(x =>
                x.ShortCode == shortCode &&
                x.UserId == userId);
    }

    public bool ShortCodeExists(string shortCode)
    {
        return _context.UrlMappings
            .Any(x => x.ShortCode == shortCode);
    }

    public List<UrlMapping> GetAll()
    {
        return _context.UrlMappings.ToList();
    }

    public List<UrlMapping> GetAllByUserId(int userId)
    {
        return _context.UrlMappings
            .Where(x => x.UserId == userId)
            .ToList();
    }

    public List<UrlMapping> GetFilteredUrls(
        int userId,
        string? search,
        string? sortBy,
        string? sortOrder,
        int page,
        int pageSize)
    {
        var query = _context.UrlMappings
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.OriginalUrl.Contains(search) ||
                x.ShortCode.Contains(search));
        }

        query = (sortBy?.ToLower(), sortOrder?.ToLower()) switch
        {
            ("visitcount", "asc") =>
                query.OrderBy(x => x.VisitCount),

            ("visitcount", "desc") =>
                query.OrderByDescending(x => x.VisitCount),

            ("createdat", "asc") =>
                query.OrderBy(x => x.CreatedAt),

            _ =>
                query.OrderByDescending(x => x.CreatedAt)
        };

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public int GetFilteredCount(
        int userId,
        string? search)
    {
        var query = _context.UrlMappings
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.OriginalUrl.Contains(search) ||
                x.ShortCode.Contains(search));
        }

        return query.Count();
    }

    public void Update(UrlMapping mapping)
    {
        _context.UrlMappings.Update(mapping);
        _context.SaveChanges();
    }

    public UrlMapping? GetById(int id)
    {
        return _context.UrlMappings
            .FirstOrDefault(x => x.Id == id);
    }

    public void Delete(UrlMapping mapping)
    {
        _context.UrlMappings.Remove(mapping);
        _context.SaveChanges();
    }

    // Dashboard Analytics

    public int GetTotalUrls(int userId)
    {
        return _context.UrlMappings
            .Count(x => x.UserId == userId);
    }

    public int GetTotalVisits(int userId)
    {
        return _context.UrlMappings
            .Where(x => x.UserId == userId)
            .Sum(x => x.VisitCount);
    }

    public UrlMapping? GetMostVisitedUrl(int userId)
    {
        return _context.UrlMappings
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.VisitCount)
            .FirstOrDefault();
    }

    public int GetActiveUrls(int userId)
    {
        return _context.UrlMappings
            .Count(x =>
                x.UserId == userId &&
                (!x.ExpiryDate.HasValue ||
                 x.ExpiryDate > DateTime.UtcNow));
    }

    public int GetExpiredUrls(int userId)
    {
        return _context.UrlMappings
            .Count(x =>
                x.UserId == userId &&
                x.ExpiryDate.HasValue &&
                x.ExpiryDate < DateTime.UtcNow);
    }
}