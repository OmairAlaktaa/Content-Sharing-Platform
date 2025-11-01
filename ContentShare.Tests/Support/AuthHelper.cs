using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace ContentShare.Tests.Support;

public static class AuthHelper
{
    public record RegisterRequest(string Username, string Email, string Password);
    public record LoginRequest(string Email, string Password);
    public record AuthResponse(string Token, DateTime ExpiresAt);

    public static async Task<string> RegisterAndLoginAsync(HttpClient client, string username, string email, string password)
    {
        var reg = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(username, email, password));
        if (reg.StatusCode == HttpStatusCode.BadRequest)
        {
            var loginExisting = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password));
            loginExisting.EnsureSuccessStatusCode();
            var existingAuth = await loginExisting.Content.ReadFromJsonAsync<AuthResponse>();
            existingAuth!.Token.Should().NotBeNullOrWhiteSpace();
            return existingAuth.Token;
        }

        reg.EnsureSuccessStatusCode();
        var auth = await reg.Content.ReadFromJsonAsync<AuthResponse>();
        auth!.Token.Should().NotBeNullOrWhiteSpace();
        return auth.Token;
    }
}
