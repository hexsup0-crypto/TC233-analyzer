namespace TC233Analyzer;

public sealed class TypeMapper
{
    readonly Dictionary<string, string> forward = new();
    readonly Dictionary<string, string> reverse = new();

    public static TypeMapper Empty => new();

    public static TypeMapper FromFile(string path)
    {
        var mapper = new TypeMapper();
        foreach (var (from, to) in ConfigParser.ReadMapping(path)) mapper.Add(from, to);
        mapper.CheckCycles();
        return mapper;
    }

    void Add(string from, string to)
    {
        if (forward.ContainsKey(from))
            throw new InvalidDataException($"Duplicate mapping rule for type {Format(from)}");
        if (reverse.ContainsKey(to))
            throw new InvalidDataException($"Duplicate mapping rule: type {Format(to)} is already the target of {Format(reverse[to])}");
        forward[from] = to;
        reverse[to] = from;
    }

    void CheckCycles()
    {
        foreach (var start in forward.Keys)
        {
            var seen = new HashSet<string> { start };
            var cur = start;
            while (forward.TryGetValue(cur, out var next) && next != cur)
            {
                if (next == start || !seen.Add(next))
                    throw new InvalidDataException($"Cyclic mapping detected involving type {Format(start)}");
                cur = next;
            }
        }
    }

    /// <summary>Canonical type for a block: dump1 types stay as is, dump2 types map back to their dump1 type.</summary>
    public string Canonical(string type, bool isSecondDump) =>
        isSecondDump && reverse.TryGetValue(type, out var src) ? src : type;

    public static string Format(string hex) =>
        string.Join(' ', Enumerable.Range(0, hex.Length / 2).Select(i => hex.Substring(i * 2, 2).ToLowerInvariant()));
}
