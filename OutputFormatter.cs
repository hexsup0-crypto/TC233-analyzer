using System.Text;

namespace TC233_analyzer;

internal static class OutputFormatter
{
    public static void Write(string outputPath, IReadOnlyList<DuplicateGroup> groups, string dump1Path, string dump2Path)
    {
        using var writer = new StreamWriter(outputPath, false, Encoding.UTF8);
        writer.WriteLine("=== DUPLICATE BLOCKS ===");
        writer.WriteLine();

        if (groups.Count == 0)
        {
            writer.WriteLine("No duplicate blocks found");
            return;
        }

        var dump1Name = Path.GetFileName(dump1Path);
        var dump2Name = Path.GetFileName(dump2Path);
        for (var i = 0; i < groups.Count; i++)
        {
            var group = groups[i];
            writer.WriteLine($"Block Group {i + 1}");
            writer.WriteLine($"Size: {group.Content.Length} bytes");
            writer.WriteLine($"Type: {FormatBytes(group.Dump1Blocks[0].Type)}");
            writer.WriteLine();

            WriteBlocks(writer, group.Dump1Blocks, dump1Name);
            WriteBlocks(writer, group.Dump2Blocks, dump2Name);

            if (i < groups.Count - 1)
            {
                writer.WriteLine("---");
                writer.WriteLine();
            }
        }
    }

    private static void WriteBlocks(StreamWriter writer, IEnumerable<DumpBlock> blocks, string dumpName)
    {
        foreach (var block in blocks)
        {
            writer.WriteLine($"Dump: {dumpName}");
            writer.WriteLine($"  Address: 0x{block.Address:X8}");
            writer.WriteLine($"  Content: {FormatBytes(block.Content)}");
            writer.WriteLine();
        }
    }

    private static string FormatBytes(byte[] bytes)
    {
        var result = new StringBuilder(bytes.Length * 3);
        for (var i = 0; i < bytes.Length; i++)
        {
            if (i > 0)
            {
                result.Append(' ');
            }

            result.Append(bytes[i].ToString("x2"));
        }

        return result.ToString();
    }
}
