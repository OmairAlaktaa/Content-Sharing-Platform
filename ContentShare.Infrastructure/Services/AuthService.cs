using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ContentShare.Application.DTOs.Auth;
using ContentShare.Application.Interfaces.Services;
using ContentShare.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ContentShare.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _config;

    public AuthService(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest req, CancellationToken ct = default)
    {
        var user = new AppUser
        {
            UserName = req.Username,
            Email = req.Email,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, req.Password);
        if (!result.Succeeded)
        {
            var message = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(message);
        }

        return await IssueTokenAsync(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest req, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user is null)
            throw new UnauthorizedAccessException("Invalid email or password");

        var signIn = await _signInManager.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: false);
        if (!signIn.Succeeded)
            throw new UnauthorizedAccessException("Invalid email or password");

        user.LastLogin = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return await IssueTokenAsync(user);
    }

    private Task<AuthResponse> IssueTokenAsync(AppUser user)
    {
        var keyString = _config["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key missing from configuration");

        if (Encoding.UTF8.GetByteCount(keyString) < 32)
            throw new InvalidOperationException("Jwt:Key must be at least 32 bytes for HS256.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(12);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Task.FromResult(new AuthResponse
        {
            Token = jwt,
            ExpiresAt = expires
        });
    }
}
