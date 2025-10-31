namespace ContentShare.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; init; } = default!;
    public DateTime ExpiresAt { get; init; }
}
