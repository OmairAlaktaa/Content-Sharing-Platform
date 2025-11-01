using ContentShare.Application.DTOs.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Auth;

public sealed class LoginRequestExample : IExamplesProvider<LoginRequest>
{
    public LoginRequest GetExamples() => new()
    {
        Email = "john@test.com",
        Password = "P@ssw0rd1!"
    };
}