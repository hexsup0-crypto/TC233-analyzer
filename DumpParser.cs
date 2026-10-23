namespace TC233Analyzer;

public static class DumpParser
{
    static readonly byte[] StartMarker = { 0x65, 0xAF, 0xF5, 0xF5 };
    const int ServiceBytes = 3;

    public static List<DumpBlock> Parse(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException($"Dump file not found: {path}");
        var data = File.ReadAllBytes(path);
        var name = Path.GetFileName(path);
        var blocks = new List<DumpBlock>();
        int pos = 0;
        while (pos + StartMarker.Length + 4 + DumpBlock.TypeLength <= data.Length)
        {
            if (!Match(data, pos, StartMarker)) { pos++; continue; }
            var typeBytes = data.AsSpan(pos + DumpBlock.TypeOffset, DumpBlock.TypeLength).ToArray();
            string type = Convert.ToHexString(typeBytes);
            int end = FindEndMarker(data, type, pos + DumpBlock.TypeOffset + DumpBlock.TypeLength);
            if (end < 0) { pos++; continue; }
            int length = Math.Min(end + 2 + DumpBlock.TypeLength + ServiceBytes, data.Length) - pos;
            blocks.Add(new DumpBlock(name, pos, data.AsSpan(pos, length).ToArray()));
            pos += length;
        }
        return blocks;
    }

    /// <summary>Finds "50 0a" followed by the given type; returns the offset of 50 or -1.</summary>
    public static int FindEndMarker(byte[] data, string type, int from = 0)
    {
        var t = Convert.FromHexString(type);
        for (int i = Math.Max(from, 0); i + 2 + t.Length <= data.Length; i++)
        {
            if (data[i] == 0x50 && data[i + 1] == 0x0A &&
                data.AsSpan(i + 2, t.Length).SequenceEqual(t)) return i;
        }
        return -1;
    }

    static bool Match(byte[] data, int pos, byte[] pattern) =>
        data.AsSpan(pos, pattern.Length).SequenceEqual(pattern);
}
