using UrlShortener.API.Data;
using UrlShortener.API.Models;

namespace UrlShortener.API.Services;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public void Save(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public User? GetUserByUsername(string username)
    {
        return _context.Users
            .FirstOrDefault(x => x.Username == username);
    }

    public bool UsernameExists(string username)
    {
        return _context.Users
            .Any(x => x.Username == username);
    }
}