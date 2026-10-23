using System.Text;

namespace TC233Analyzer;

public static class OutputFormatter
{
    static string Addr(long a) => $"0x{a:X8}";

    public static string Format(ComparisonResult r, string name1, string name2)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== BLOCKS BY TYPE ===").AppendLine();
        foreach (var g in r.Paired)
        {
            string type = TypeMapper.Format(g.Type);
            if (g.First[0].Type != g.Second[0].Type)
                type += $" / {g.Second[0].TypeText} (mapped)";
            var blocks = g.First.Concat(g.Second).ToList();
            bool listSizes = AppendGroupHeader(sb, type, blocks);
            foreach (var b in g.First) AppendBlock(sb, b, name1, listSizes);
            foreach (var b in g.Second) AppendBlock(sb, b, name2, listSizes);
            sb.AppendLine("---").AppendLine();
        }

        sb.AppendLine("=== UNPAIRED BLOCKS ===").AppendLine();
        AppendUnpairedGroups(sb, r.UnpairedFirst, name1, name2);
        AppendUnpairedGroups(sb, r.UnpairedSecond, name2, name1);
        return sb.ToString();
    }

    static void AppendUnpairedGroups(StringBuilder sb, List<DumpBlock> blocks, string from, string other)
    {
        foreach (var group in blocks.GroupBy(b => b.Type))
        {
            var items = group.ToList();
            bool listSizes = AppendGroupHeader(sb, group.First().TypeText, items);
            sb.AppendLine($"From {from} (not found in {other})");
            sb.AppendLine();
            foreach (var block in items) AppendBlock(sb, block, from, listSizes);
            sb.AppendLine("---").AppendLine();
        }
    }

    static bool AppendGroupHeader(StringBuilder sb, string type, List<DumpBlock> blocks)
    {
        sb.AppendLine($"Type: {type}");
        bool listSizes = blocks.Any(block => block.Size != blocks[0].Size);
        if (!listSizes) sb.AppendLine($"Size: {blocks[0].Size} bytes");
        sb.AppendLine();
        return listSizes;
    }

    static void AppendBlock(StringBuilder sb, DumpBlock block, string name, bool listSize)
    {
        sb.AppendLine($"Dump: {name}");
        sb.AppendLine($"  Address: {Addr(block.Address)}");
        if (listSize) sb.AppendLine($"  Size: {block.Size} bytes");
        for (int offset = 0; offset < block.Data.Length; offset += 50)
        {
            int count = Math.Min(50, block.Data.Length - offset);
            string content = string.Join(" ", block.Data.AsSpan(offset, count).ToArray()
                .Select(value => value.ToString("x2")));
            sb.AppendLine(offset == 0 ? $"  Content: {content}" : $"           {content}");
        }
        sb.AppendLine();
    }

    public static string Summary(ComparisonResult r, string name1, string name2, string outputPath, int total1, int total2) =>
        $"=== ANALYSIS SUMMARY ===\n" +
        $"Total blocks in {name1}: {total1}\n" +
        $"Total blocks in {name2}: {total2}\n" +
        $"Paired block types: {r.Paired.Count}\n" +
        $"Unpaired blocks from {name1}: {r.UnpairedFirst.Count}\n" +
        $"Unpaired blocks from {name2}: {r.UnpairedSecond.Count}\n" +
        $"Type mappings applied: {r.MappingsApplied}\n" +
        $"Output written to: {outputPath}";
}
