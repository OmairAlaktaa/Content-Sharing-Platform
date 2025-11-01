using ContentShare.Application.DTOs.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace ContentShare.API.SwaggerExamples.Auth;

public sealed class RegisterRequestExample : IExamplesProvider<RegisterRequest>
{
    public RegisterRequest GetExamples() => new()
    {
        Username = "john",
        Email = "john@test.com",
        Password = "P@ssw0rd1!"
    };
}