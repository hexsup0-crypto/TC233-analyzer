namespace TC233Analyzer;

public static class ConfigParser
{
    /// <summary>Reads "type1 = type2" rules; skips blank lines and '#' comments.</summary>
    public static List<(string From, string To)> ReadMapping(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException($"Mapping file not found: {path}");
        var rules = new List<(string, string)>();
        int lineNo = 0;
        foreach (var raw in File.ReadLines(path))
        {
            lineNo++;
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            var parts = line.Split('=');
            if (parts.Length != 2)
                throw new FormatException($"Invalid mapping at line {lineNo}: '{line}' (expected 'XX XX XX = XX XX XX')");
            rules.Add((ParseType(parts[0], lineNo), ParseType(parts[1], lineNo)));
        }
        return rules;
    }

    static string ParseType(string text, int lineNo)
    {
        var tokens = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length != DumpBlock.TypeLength ||
            tokens.Any(t => t.Length != 2 || !Uri.IsHexDigit(t[0]) || !Uri.IsHexDigit(t[1])))
            throw new FormatException($"Invalid block type '{text.Trim()}' at line {lineNo} (expected 3 hex bytes, e.g. '7B CC BB')");
        return string.Concat(tokens).ToUpperInvariant();
    }
}
