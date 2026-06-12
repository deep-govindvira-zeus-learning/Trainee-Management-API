using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<LoginResponse> Login(LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            _logger.LogWarning("Login failed: Request body or required credentials are null/empty.");
            throw new ArgumentException("Username and password are required.");
        }

        try
        {
            _logger.LogInformation("Processing login attempt for Username: {Username}", request.Username);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                _logger.LogWarning("Authentication failed: Username '{Username}' not found.", request.Username);
                throw new InvalidCredentialException("Invalid username or password.");
            }

            bool validPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!validPassword)
            {
                _logger.LogWarning("Authentication failed: Invalid password provided for username '{Username}'.", request.Username);
                throw new InvalidCredentialException("Invalid username or password.");
            }

            var tokenExpiryMinutes = Convert.ToInt32(_configuration["jwt:ExpiryMinutes"] ?? "60");
            var token = GenerateJwtToken(user, tokenExpiryMinutes);

            _logger.LogInformation("User '{Username}' (ID: {UserId}) successfully authenticated.", user.Username, user.Id);

            return new LoginResponse
            {
                Token = token,
                ExpiresIn = tokenExpiryMinutes * 60, // Synchronized dynamic expiry duration
                User = new UserResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Role = user.Role
                }
            };
        }
        catch (InvalidCredentialException)
        {
            throw; // Propagate business logic/auth exceptions cleanly
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during the login process for Username: {Username}", request.Username);
            throw new ApplicationException("An internal error occurred while processing your request.", ex);
        }
    }

    private string GenerateJwtToken(User user, int expiryMinutes)
    {
        try
        {
            var jwtSection = _configuration.GetSection("jwt");
            var keyStr = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            if (string.IsNullOrWhiteSpace(keyStr) || string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException("JWT configuration fields ('Key', 'Issuer', 'Audience') are missing or misconfigured.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token for User ID: {UserId}.", user.Id);
            throw new TokenGenerationException("Failed to generate security credentials.", ex);
        }
    }
}

// Custom internal exception class for tracking specific failures
public class TokenGenerationException : Exception
{
    public TokenGenerationException(string message, Exception innerException) : base(message, innerException) { }
}
