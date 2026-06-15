using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.DTOs;
using UrlShortener.API.Models;
using UrlShortener.API.Services;

namespace UrlShortener.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;

    public AuthController(
        UserService userService,
        JwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterRequest request)
    {
        if (_userService.UsernameExists(request.Username))
        {
            return BadRequest("Username already exists");
        }

        var user = new User
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _userService.Save(user);

        return Ok(new RegisterResponse
        {
            Message = "User registered successfully"
        });
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = _userService.GetUserByUsername(request.Username);

        if (user == null)
        {
            return Unauthorized("Invalid username or password");
        }

        bool passwordValid =
            BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            return Unauthorized("Invalid username or password");
        }

        var token = _jwtService.GenerateToken(user);

        return Ok(new LoginResponse
        {
            Message = "Login successful",
            Token = token
        });
    }
}