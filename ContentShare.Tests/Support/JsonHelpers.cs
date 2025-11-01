using System.Text.Json;

namespace ContentShare.Tests.Support;

public static class JsonHelpers
{
    public static bool TryGetPropertyCI(this JsonElement el, string name, out JsonElement value)
    {
        if (el.TryGetProperty(name, out value)) return true;

        foreach (var n in new[]
        {
            name, name.ToLowerInvariant(), name.ToUpperInvariant(),
            char.ToLowerInvariant(name[0]) + name[1..],
            char.ToUpperInvariant(name[0]) + name[1..]
        })
        {
            if (el.TryGetProperty(n, out value)) return true;
        }

        foreach (var prop in el.EnumerateObject())
        {
            if (string.Equals(prop.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = prop.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
}
