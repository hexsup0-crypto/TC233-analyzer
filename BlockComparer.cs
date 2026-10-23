namespace TC233Analyzer;

public sealed class BlockGroup
{
    public List<DumpBlock> First { get; } = new();
    public List<DumpBlock> Second { get; } = new();
    public bool IsDuplicate => First.Count > 0 && Second.Count > 0;
    public long SortAddress => First.Count > 0 ? First[0].Address : Second[0].Address;
}

public sealed class ComparisonResult
{
    public List<BlockGroup> Duplicates { get; } = new();
    public List<DumpBlock> UnpairedFirst { get; } = new();
    public List<DumpBlock> UnpairedSecond { get; } = new();
    public int MappingsApplied { get; set; }
}

public static class BlockComparer
{
    public static ComparisonResult Compare(List<DumpBlock> first, List<DumpBlock> second, TypeMapper mapper)
    {
        var groups = new Dictionary<string, BlockGroup>();
        var order = new List<BlockGroup>();

        void Add(DumpBlock b, bool isSecond)
        {
            var key = mapper.Canonical(b.Type, isSecond) + ":" + Convert.ToHexString(b.NormalizedData());
            if (!groups.TryGetValue(key, out var g)) { g = new BlockGroup(); groups[key] = g; order.Add(g); }
            (isSecond ? g.Second : g.First).Add(b);
        }
        foreach (var b in first) Add(b, false);
        foreach (var b in second) Add(b, true);

        var result = new ComparisonResult();
        foreach (var g in order)
        {
            if (g.IsDuplicate)
            {
                result.Duplicates.Add(g);
                if (g.First[0].Type != g.Second[0].Type) result.MappingsApplied++;
            }
            else
            {
                result.UnpairedFirst.AddRange(g.First);
                result.UnpairedSecond.AddRange(g.Second);
            }
        }
        result.UnpairedFirst.Sort((a, b) => a.Address.CompareTo(b.Address));
        result.UnpairedSecond.Sort((a, b) => a.Address.CompareTo(b.Address));
        return result;
    }
}
