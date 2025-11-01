using System.Net.Http.Headers;

namespace ContentShare.Tests.Support;

public static class HttpClientExtensions
{
    public static void SetBearer(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
