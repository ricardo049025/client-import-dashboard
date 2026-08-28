namespace Domain.Domain.Helpers;

public static class UtilityHelper
{
    public static string ReadColumnValue(string[] row, Dictionary<string, int> headerMap, string key)
    {
        if (!headerMap.TryGetValue(key, out var index)) return string.Empty;
        return index >= row.Length ? string.Empty : row[index].Trim();
    }

    public static Dictionary<string, int> BuildHeaderMap(string[] headers)
    {
        var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < headers.Length; i++)
        {
            var value = headers[i].Trim();
            if (!map.ContainsKey(value)) map[value] = i;
        }

        return map;
    }
}
