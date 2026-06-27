using AuthApi.Controllers;
using AuthApi.DTOs;
using AuthApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuthAPI.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _controller = new AuthController(_mockAuthService.Object);
    }

    // ── Register Tests ──

    [Fact]
    public async Task Register_WithValidData_ShouldReturn200()
    {
        // Arrange
        var request = new RegisterDto
        {
            Username = "nader",
            Email = "nader@test.com",
            Password = "Password123!"
        };

        var tokenResponse = new TokenResponseDto
        {
            AccessToken = "fake-access-token",
            RefreshToken = "fake-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _mockAuthService
            .Setup(s => s.RegisterAsync(request))
            .ReturnsAsync(tokenResponse);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturn400()
    {
        // Arrange
        var request = new RegisterDto
        {
            Username = "nader",
            Email = "nader@test.com",
            Password = "Password123!"
        };

        _mockAuthService
            .Setup(s => s.RegisterAsync(request))
            .ReturnsAsync((TokenResponseDto?)null);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }

    // ── Login Tests ──

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturn200()
    {
        // Arrange
        var request = new LoginDto
        {
            Email = "nader@test.com",
            Password = "Password123!"
        };

        var tokenResponse = new TokenResponseDto
        {
            AccessToken = "fake-access-token",
            RefreshToken = "fake-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _mockAuthService
            .Setup(s => s.LoginAsync(request))
            .ReturnsAsync(tokenResponse);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturn401()
    {
        // Arrange
        var request = new LoginDto
        {
            Email = "nader@test.com",
            Password = "WrongPassword!"
        };

        _mockAuthService
            .Setup(s => s.LoginAsync(request))
            .ReturnsAsync((TokenResponseDto?)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    // ── Refresh Token Tests ──

    [Fact]
    public async Task Refresh_WithValidToken_ShouldReturn200()
    {
        // Arrange
        var refreshToken = "valid-refresh-token";

        var tokenResponse = new TokenResponseDto
        {
            AccessToken = "new-access-token",
            RefreshToken = "new-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _mockAuthService
            .Setup(s => s.RefreshTokenAsync(refreshToken))
            .ReturnsAsync(tokenResponse);

        // Act
        var result = await _controller.Refresh(refreshToken);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Refresh_WithInvalidToken_ShouldReturn401()
    {
        // Arrange
        _mockAuthService
            .Setup(s => s.RefreshTokenAsync("invalid-token"))
            .ReturnsAsync((TokenResponseDto?)null);

        // Act
        var result = await _controller.Refresh("invalid-token");

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(401, unauthorizedResult.StatusCode);
    }

    // ── Logout Tests ──

    [Fact]
    public async Task Logout_WithValidToken_ShouldReturn200()
    {
        // Arrange
        _mockAuthService
            .Setup(s => s.RevokeTokenAsync("valid-token"))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Logout("valid-token");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Logout_WithInvalidToken_ShouldReturn400()
    {
        // Arrange
        _mockAuthService
            .Setup(s => s.RevokeTokenAsync("invalid-token"))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Logout("invalid-token");

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequest.StatusCode);
    }
}