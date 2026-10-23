namespace TC233Analyzer;

public sealed class DumpBlock
{
    public const int TypeOffset = 8;
    public const int TypeLength = 3;

    public DumpBlock(string dumpName, long address, byte[] data)
    {
        DumpName = dumpName;
        Address = address;
        Data = data;
        Type = Convert.ToHexString(data, TypeOffset, TypeLength);
    }

    public string DumpName { get; }
    public long Address { get; }
    public byte[] Data { get; }

    /// <summary>Block type as 6 upper-case hex digits (no spaces).</summary>
    public string Type { get; }

    public int Size => Data.Length;

    public string TypeText => TypeMapper.Format(Type);
}
