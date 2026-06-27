using AuthApi.DTOs;
using AuthApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService _authService) : ControllerBase
{
    // ── Register ──
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        var result = await _authService.RegisterAsync(request);

        if (result is null)
            return BadRequest("Email already exists.");

        return Ok(result);
    }

    // ── Login ──
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
            return Unauthorized("Invalid credentials.");

        return Ok(result);
    }

    // ── Refresh Token ──
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string refreshToken)
    {
        var result = await _authService.RefreshTokenAsync(refreshToken);

        if (result is null)
            return Unauthorized("Invalid or expired refresh token.");

        return Ok(result);
    }

    // ── Logout ──
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        var result = await _authService.RevokeTokenAsync(refreshToken);

        if (!result)
            return BadRequest("Invalid token.");

        return Ok("Logged out successfully.");
    }

    // ── Test Endpoint ──
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var userId = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(
            System.Security.Claims.ClaimTypes.Email)?.Value;
        var role = User.FindFirst(
            System.Security.Claims.ClaimTypes.Role)?.Value;

        return Ok(new { userId, email, role });
    }
}