using AuthApi.Data;
using AuthApi.DTOs;
using AuthApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AuthAPI.Tests.Services;

public class AuthServiceTests
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        // دیتابیس موقت توی RAM
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        // mock کردن configuration
        var inMemorySettings = new Dictionary<string, string>
        {
            { "JwtSettings:SecretKey", "L3zxcgxCHUyC0WeqKYTjwEQhicVF/0sInBopq6oQdY5Lx86Lxd1pwzWXg7WYOM4jWtaYlfkBWblz" },
            { "JwtSettings:Issuer", "AuthAPI" },
            { "JwtSettings:Audience", "AuthAPIUsers" },
            { "JwtSettings:AccessTokenExpirationMinutes", "15" },
            { "JwtSettings:RefreshTokenExpirationDays", "7" }
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _authService = new AuthService(_context, _configuration);
    }

    // ── Register Tests ──
    [Fact]
    public async Task Register_WithValidData_ShouldReturnTokens()
    {
        // Arrange
        var request = new RegisterDto
        {
            Username = "nader",
            Email = "nader@test.com",
            Password = "Password123!"
        };

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnNull()
    {
        // Arrange
        var request = new RegisterDto
        {
            Username = "nader",
            Email = "nader@test.com",
            Password = "Password123!"
        };

        await _authService.RegisterAsync(request);

        // Act — همون email دوباره
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.Null(result);
    }

    // ── Login Tests ──
    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var registerRequest = new RegisterDto
        {
            Username = "nader",
            Email = "nader@test.com",
            Password = "Password123!"
        };
        await _authService.RegisterAsync(registerRequest);

        var loginRequest = new LoginDto
        {
            Email = "nader@test.com",
            Password = "Password123!"
        };

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturnNull()
    {
        // Arrange
        var registerRequest = new RegisterDto
        {
            Username = "nader",
            Email = "nader@test.com",
            Password = "Password123!"
        };
        await _authService.RegisterAsync(registerRequest);

        var loginRequest = new LoginDto
        {
            Email = "nader@test.com",
            Password = "WrongPassword!"
        };

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ShouldReturnNull()
    {
        // Arrange
        var loginRequest = new LoginDto
        {
            Email = "nobody@test.com",
            Password = "Password123!"
        };

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        Assert.Null(result);
    }

    // ── Refresh Token Tests ──
    [Fact]
    public async Task RefreshToken_WithValidToken_ShouldReturnNewTokens()
    {
        // Arrange
        var registerRequest = new RegisterDto
        {
            Username = "nader",
            Email = "nader@test.com",
            Password = "Password123!"
        };
        var registerResult = await _authService.RegisterAsync(registerRequest);

        // Act
        var result = await _authService
            .RefreshTokenAsync(registerResult!.RefreshToken);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ShouldReturnNull()
    {
        // Act
        var result = await _authService.RefreshTokenAsync("invalid-token");

        // Assert
        Assert.Null(result);
    }

    // ── Revoke Token Tests ──
    [Fact]
    public async Task RevokeToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var registerRequest = new RegisterDto
        {
            Username = "nader",
            Email = "nader@test.com",
            Password = "Password123!"
        };
        var registerResult = await _authService.RegisterAsync(registerRequest);

        // Act
        var result = await _authService
            .RevokeTokenAsync(registerResult!.RefreshToken);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task RevokeToken_AfterRevoke_ShouldNotRefresh()
    {
        // Arrange
        var registerRequest = new RegisterDto
        {
            Username = "nader",
            Email = "nader@test.com",
            Password = "Password123!"
        };
        var registerResult = await _authService.RegisterAsync(registerRequest);
        await _authService.RevokeTokenAsync(registerResult!.RefreshToken);

        // Act — بعد از revoke، refresh نباید کار کنه
        var result = await _authService
            .RefreshTokenAsync(registerResult.RefreshToken);

        // Assert
        Assert.Null(result);
    }
}