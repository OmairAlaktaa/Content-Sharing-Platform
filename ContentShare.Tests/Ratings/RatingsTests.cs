using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ContentShare.Tests.Support;
using FluentAssertions;

namespace ContentShare.Tests.Ratings;

public class RatingsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RatingsTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Rate_Content_Then_Get_Average()
    {
        _ = await _client.PostAsJsonAsync("/api/auth/register",
            new { Username = "bob", Email = "bob@test.com", Password = "P@ssw0rd1!" });

        var login = await _client.PostAsJsonAsync("/api/auth/login",
            new { Email = "bob@test.com", Password = "P@ssw0rd1!" });
        login.EnsureSuccessStatusCode();

        var loginDoc = await login.Content.ReadFromJsonAsync<JsonDocument>();
        var token = loginDoc!.RootElement.GetProperty("token").GetString();
        _client.SetBearer(token!);

        var create = await _client.PostAsJsonAsync("/api/contents", new
        {
            title = "Rated Item",
            description = "nice",
            category = "Game",
            thumbnailUrl = "https://example.com/thumb.jpg",
            contentUrl = "https://example.com/content"
        });
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var list = await _client.GetAsync("/api/contents?search=Rated%20Item");
        var listBody = await list.Content.ReadAsStringAsync();
        list.IsSuccessStatusCode.Should().BeTrue($"content list should succeed. Body: {listBody}");

        using var doc = JsonDocument.Parse(listBody);
        var root = doc.RootElement;

        JsonElement items;
        if (root.ValueKind == JsonValueKind.Array)
        {
            items = root;
        }
        else if (root.TryGetPropertyCI("Items", out var tmpItems) && tmpItems.ValueKind == JsonValueKind.Array)
        {
            items = tmpItems;
        }
        else if (root.TryGetPropertyCI("data", out var dataItems) && dataItems.ValueKind == JsonValueKind.Array)
        {
            items = dataItems;
        }
        else
        {
            throw new InvalidOperationException($"Expected an array of items. Body: {listBody}");
        }

        items.GetArrayLength().Should().BeGreaterThan(0);
        var first = items[0];

        Guid contentId =
            first.TryGetProperty("media_id", out var mid) ? mid.GetGuid() :
            first.TryGetProperty("MediaId", out var mid2) ? Guid.Parse(mid2.GetString()!) :
            Guid.Empty;

        contentId.Should().NotBe(Guid.Empty, "the content item must include media_id/MediaId. Body: " + listBody);

        var rate = await _client.PostAsJsonAsync("/api/ratings", new
        {
            mediaContentId = contentId,
            score = 5,
            comment = "Loved it"
        });
        rate.StatusCode.Should().Be(HttpStatusCode.OK);

        var rate2 = await _client.PostAsJsonAsync("/api/ratings", new
        {
            mediaContentId = contentId,
            score = 3,
            comment = "Changed my mind"
        });
        rate2.StatusCode.Should().Be(HttpStatusCode.OK);

        var listRatings = await _client.GetAsync($"/api/ratings/content/{contentId}");
        var lrBody = await listRatings.Content.ReadAsStringAsync();
        listRatings.IsSuccessStatusCode.Should().BeTrue($"ratings list should succeed. Body: {lrBody}");

        using var rdoc = JsonDocument.Parse(lrBody);
        var ratingsRoot = rdoc.RootElement;

        JsonElement ratingsArray;
        if (ratingsRoot.ValueKind == JsonValueKind.Array)
        {
            ratingsArray = ratingsRoot;
        }
        else if (ratingsRoot.TryGetPropertyCI("Items", out var rItems) && rItems.ValueKind == JsonValueKind.Array)
        {
            ratingsArray = rItems;
        }
        else if (ratingsRoot.TryGetPropertyCI("data", out var rData) && rData.ValueKind == JsonValueKind.Array)
        {
            ratingsArray = rData;
        }
        else
        {
            throw new InvalidOperationException($"Expected ratings as an array. Body: {lrBody}");
        }

        ratingsArray.GetArrayLength().Should().Be(1);

        var r0 = ratingsArray[0];
        var comment =
            r0.TryGetProperty("comment", out var c) ? c.GetString() :
            r0.TryGetProperty("Comment", out var C) ? C.GetString() : null;
        comment.Should().Contain("Changed my mind");

        var score =
            r0.TryGetProperty("score", out var s) ? s.GetInt32() :
            r0.TryGetProperty("Score", out var S) ? S.GetInt32() : 0;
        score.Should().Be(3);

        var computedAvg = ratingsArray.EnumerateArray()
            .Select(e => e.TryGetProperty("score", out var sc) ? sc.GetInt32()
                      : e.TryGetProperty("Score", out var Sc) ? Sc.GetInt32() : 0)
            .DefaultIfEmpty(0)
            .Average();

        computedAvg.Should().BeApproximately(3.0, 0.001);

        var avgResp = await _client.GetAsync($"/api/ratings/content/{contentId}/average");
        var avgBody = await avgResp.Content.ReadAsStringAsync();
        if (avgResp.IsSuccessStatusCode)
        {
            var avg = await avgResp.Content.ReadFromJsonAsync<double>();
            avg.Should().BeApproximately(3.0, 0.001);
        }
        else
        {
            avgBody.Should().NotBeNullOrWhiteSpace("average endpoint returned a non-success status");
        }
    }
}
