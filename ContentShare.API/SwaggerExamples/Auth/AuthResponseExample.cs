using ContentShare.Application.DTOs.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Auth;

public sealed class AuthResponseExample : IExamplesProvider<AuthResponse>
{
    public AuthResponse GetExamples() => new()
    {
        Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.ey...signature",
        ExpiresAt = DateTime.UtcNow.AddHours(12)
    };
}