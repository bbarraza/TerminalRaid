using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using TerminalRaid.Services;
using TerminalRaid.Models;
using System.Threading.Tasks;

namespace TerminalRaid.Controllers;

[Route("api/[Controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly JwtService _jwtService;

    public AuthController(AuthService authService, JwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
    {
        try
        {
            var credentials = await _authService.LoginAsync(request.Username, request.Password);
            if (credentials == null)
                return Unauthorized("Invalid username or password");

            var token = _jwtService.GenerateToken(credentials.Username, credentials.PlayerId);
            return Ok(new AuthResponse { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] AuthRequest request)
    {
        try
        {
            var credentials = await _authService.RegisterAsync(request.Username, request.Password);
            return Ok(new { message = "Registration successful", playerId = credentials.PlayerId });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
    {
        try
        {
            await _authService.LogoutAsync(request.PlayerId);
            return Ok(new { message = "Logout successful" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class AuthRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class AuthResponse
{
    public string Token { get; set; }
}

public class LogoutRequest
{
    public string PlayerId { get; set; }
}