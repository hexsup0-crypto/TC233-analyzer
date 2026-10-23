namespace TC233_analyzer;

internal sealed record DuplicateGroup(byte[] Content, IReadOnlyList<DumpBlock> Dump1Blocks, List<DumpBlock> Dump2Blocks);

internal static class BlockComparer
{
    public static List<DuplicateGroup> FindDuplicates(
        IReadOnlyList<DumpBlock> dump1Blocks,
        IReadOnlyList<DumpBlock> dump2Blocks)
    {
        var blocksByContent = new Dictionary<byte[], List<DumpBlock>>(ByteArrayComparer.Instance);
        foreach (var block in dump1Blocks)
        {
            if (!blocksByContent.TryGetValue(block.Content, out var matchingBlocks))
            {
                matchingBlocks = [];
                blocksByContent.Add(block.Content, matchingBlocks);
            }

            matchingBlocks.Add(block);
        }

        var groups = new Dictionary<byte[], DuplicateGroup>(ByteArrayComparer.Instance);
        foreach (var block in dump2Blocks)
        {
            if (!blocksByContent.TryGetValue(block.Content, out var matchingBlocks))
            {
                continue;
            }

            if (!groups.TryGetValue(block.Content, out var group))
            {
                group = new DuplicateGroup(block.Content, matchingBlocks, new List<DumpBlock>());
                groups.Add(block.Content, group);
            }

            group.Dump2Blocks.Add(block);
        }

        return groups.Values.ToList();
    }

    private sealed class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public static ByteArrayComparer Instance { get; } = new();

        public bool Equals(byte[]? left, byte[]? right) =>
            ReferenceEquals(left, right) ||
            (left is not null && right is not null && left.AsSpan().SequenceEqual(right));

        public int GetHashCode(byte[] bytes)
        {
            var hash = new HashCode();
            foreach (var value in bytes)
            {
                hash.Add(value);
            }

            return hash.ToHashCode();
        }
    }
}
