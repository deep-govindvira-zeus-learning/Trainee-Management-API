using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Ocsp;
using Serilog;
using TraineeManagementApi.Data;
using TraineeManagementApi.DTOs;
using TraineeManagementApi.Models;


namespace TraineeManagementApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
        {
            _logger.LogWarning("Authentication failed. Username: {Username} not found", request.Username);
            throw new Exception("User is not authorized");
        }

        bool validPassword = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash
        );

        if (!validPassword)
        {
            _logger.LogWarning("Authentication failed. Invalid password for username: {Username}", request.Username);
            throw new Exception("User is not authorized");
        }

        var token = GenerateJwtToken(user);

        _logger.LogInformation("User: {Username} successfully logged in.", user.Username);

        return new LoginResponse
        {
            Token = token,
            ExpiresIn = 3600,
            User = new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            }
        };
    }

    private string GenerateJwtToken(User user)
    {
        var jwt = _configuration.GetSection("jwt");

        var claims = new[]
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim("Username", user.Username),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToInt32(jwt["ExpiryMinutes"])),
                signingCredentials: credentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
