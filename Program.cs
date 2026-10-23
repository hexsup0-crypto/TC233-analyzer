using TC233_analyzer;

if (args.Length != 3)
{
    Console.Error.WriteLine("Usage: TC233-analyzer.exe <dump1.bin> <dump2.bin> <output.txt>");
    return 1;
}

var dump1Path = args[0];
var dump2Path = args[1];
var outputPath = args[2];

foreach (var filePath in new[] { dump1Path, dump2Path })
{
    if (!File.Exists(filePath))
    {
        Console.Error.WriteLine($"Error: File not found - {filePath}");
        return 1;
    }
}

try
{
    var dump1Blocks = DumpParser.Parse(dump1Path);
    var dump2Blocks = DumpParser.Parse(dump2Path);

    if (dump1Blocks.Count == 0)
    {
        Console.WriteLine($"Warning: No blocks found in {dump1Path}");
    }

    if (dump2Blocks.Count == 0)
    {
        Console.WriteLine($"Warning: No blocks found in {dump2Path}");
    }

    var groups = BlockComparer.FindDuplicates(dump1Blocks, dump2Blocks);
    OutputFormatter.Write(outputPath, groups, dump1Path, dump2Path);

    if (groups.Count == 0)
    {
        Console.WriteLine("No duplicate blocks found");
    }

    return 0;
}
catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
{
    Console.Error.WriteLine($"Error: {exception.Message}");
    return 1;
}
