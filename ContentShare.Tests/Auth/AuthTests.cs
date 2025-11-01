using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ContentShare.Tests.Support;
using FluentAssertions;

namespace ContentShare.Tests.Auth;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Register_Then_Login_Returns_Token()
    {
        var reg = await _client.PostAsJsonAsync("/api/auth/register",
            new { Username = "joe", Email = "joe@test.com", Password = "P@ssw0rd1!" });
        if (reg.StatusCode != HttpStatusCode.BadRequest)
        {
            reg.EnsureSuccessStatusCode();
        }

        var login = await _client.PostAsJsonAsync("/api/auth/login",
            new { Email = "joe@test.com", Password = "P@ssw0rd1!" });
        login.EnsureSuccessStatusCode();

        var doc = await login.Content.ReadFromJsonAsync<JsonDocument>();
        doc.Should().NotBeNull();

        var root = doc!.RootElement;
        root.TryGetProperty("token", out var tokenProp).Should().BeTrue("response contains token");
        var token = tokenProp.GetString();
        token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Login_With_Wrong_Password_Returns_NonSuccess()
    {
        var login = await _client.PostAsJsonAsync("/api/auth/login",
            new { Email = "noone@test.com", Password = "wrong" });

        login.IsSuccessStatusCode.Should().BeFalse();

        var body = await login.Content.ReadAsStringAsync();
        body.Should().NotBeNullOrWhiteSpace();
    }
}
