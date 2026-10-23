namespace TC233_analyzer;

internal static class DumpParser
{
    private const uint StartMarker = 0x65AFF5F5;
    private const int BufferSize = 64 * 1024;

    public static List<DumpBlock> Parse(string filePath)
    {
        var blocks = new List<DumpBlock>();
        using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            BufferSize,
            FileOptions.SequentialScan);
        var buffer = new byte[BufferSize];
        List<byte>? currentBlock = null;
        long blockAddress = 0;
        long offset = 0;
        uint startWindow = 0;
        var startWindowLength = 0;

        int bytesRead;
        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (var i = 0; i < bytesRead; i++)
            {
                var value = buffer[i];
                startWindow = (startWindow << 8) | value;
                startWindowLength = Math.Min(startWindowLength + 1, 4);

                if (currentBlock is null)
                {
                    if (startWindowLength == 4 && startWindow == StartMarker)
                    {
                        blockAddress = offset - 3;
                        currentBlock = [0x65, 0xaf, 0xf5, 0xf5];
                    }
                }
                else
                {
                    currentBlock.Add(value);
                    if (currentBlock.Count >= 11 &&
                        currentBlock[^5] == 0x50 &&
                        currentBlock[^4] == 0x0a &&
                        currentBlock[^3] == currentBlock[8] &&
                        currentBlock[^2] == currentBlock[9] &&
                        currentBlock[^1] == currentBlock[10])
                    {
                        var content = currentBlock.ToArray();
                        var type = content[^3..];
                        blocks.Add(new DumpBlock(blockAddress, type, content));
                        currentBlock = null;
                    }
                }

                offset++;
            }
        }

        return blocks;
    }
}
