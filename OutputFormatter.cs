using System.Text;

namespace TC233Analyzer;

public static class OutputFormatter
{
    static string Addr(long a) => $"0x{a:X8}";

    public static string Format(ComparisonResult r, string name1, string name2)
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== DUPLICATE BLOCKS ===").AppendLine();
        int n = 0;
        foreach (var g in r.Duplicates)
        {
            sb.AppendLine($"Block Group {++n}");
            sb.AppendLine($"Size: {g.First[0].Size} bytes");
            sb.AppendLine($"Type: {g.First[0].TypeText}");
            if (g.First[0].Type != g.Second[0].Type) sb.AppendLine($"Mapped type in {name2}: {g.Second[0].TypeText}");
            sb.AppendLine();
            foreach (var (name, list) in new[] { (name1, g.First), (name2, g.Second) })
                foreach (var b in list)
                {
                    sb.AppendLine($"Dump: {name}");
                    sb.AppendLine($"  Address: {Addr(b.Address)}");
                    sb.AppendLine($"  Content: {b.ContentHex}").AppendLine();
                }
            sb.AppendLine("---").AppendLine();
        }

        sb.AppendLine("=== UNPAIRED BLOCKS ===").AppendLine();
        foreach (var b in r.UnpairedFirst) Unpaired(sb, b, name1, name2);
        foreach (var b in r.UnpairedSecond) Unpaired(sb, b, name2, name1);
        return sb.ToString();
    }

    static void Unpaired(StringBuilder sb, DumpBlock b, string from, string other)
    {
        sb.AppendLine($"Block from {from} (not found in {other})");
        sb.AppendLine($"Type: {b.TypeText}");
        sb.AppendLine($"Address: {Addr(b.Address)}");
        sb.AppendLine($"Size: {b.Size} bytes");
        sb.AppendLine($"Content: {b.ContentHex}").AppendLine();
        sb.AppendLine("---").AppendLine();
    }

    public static string Summary(ComparisonResult r, string name1, string name2, int total1, int total2) =>
        $"=== ANALYSIS SUMMARY ===\n" +
        $"Total blocks in {name1}: {total1}\n" +
        $"Total blocks in {name2}: {total2}\n" +
        $"Duplicate blocks found: {r.Duplicates.Count} groups\n" +
        $"Unpaired blocks from {name1}: {r.UnpairedFirst.Count}\n" +
        $"Unpaired blocks from {name2}: {r.UnpairedSecond.Count}\n" +
        $"Type mappings applied: {r.MappingsApplied}";
}
