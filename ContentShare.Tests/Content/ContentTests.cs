using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ContentShare.Tests.Support;
using FluentAssertions;

namespace ContentShare.Tests.Content;

public class ContentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ContentTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Create_Then_Get_List_Includes_Item()
    {
        var reg = await _client.PostAsJsonAsync("/api/auth/register",
            new { Username = "alice", Email = "alice@test.com", Password = "P@ssw0rd1!" });
        var login = await _client.PostAsJsonAsync("/api/auth/login",
            new { Email = "alice@test.com", Password = "P@ssw0rd1!" });
        login.EnsureSuccessStatusCode();

        var loginDoc = await login.Content.ReadFromJsonAsync<JsonDocument>();
        var token = loginDoc!.RootElement.GetProperty("token").GetString();
        _client.SetBearer(token!);

        var create = await _client.PostAsJsonAsync("/api/contents", new
        {
            title = "Test Video",
            description = "demo",
            category = "Video",
            thumbnailUrl = "https://example.com/thumb.jpg",
            contentUrl = "https://example.com/content.mp4"
        });
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var list = await _client.GetAsync("/api/contents?search=Test%20Video");
        list.EnsureSuccessStatusCode();

        using var doc = await list.Content.ReadFromJsonAsync<JsonDocument>();
        doc.Should().NotBeNull();
        var root = doc!.RootElement;

        JsonElement items;
        if (root.ValueKind == JsonValueKind.Array)
        {
            items = root;
        }
        else if (!root.TryGetPropertyCI("Items", out items))
        {
            throw new KeyNotFoundException("Response did not contain an 'Items' collection.");
        }

        items.GetArrayLength().Should().BeGreaterThan(0);

        var first = items[0];

        string? firstTitle = null;
        if (first.TryGetProperty("title", out var t)) firstTitle = t.GetString();
        else if (first.TryGetProperty("Title", out var T)) firstTitle = T.GetString();

        firstTitle.Should().Be("Test Video");
    }
}
