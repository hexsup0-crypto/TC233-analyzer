using TC233Analyzer;

const string Usage = "Usage: TC233-analyzer.exe <dump1.bin> <dump2.bin> <output.txt> [--mask|-m mask.txt] [--verbose]";

var files = new List<string>();
string? mappingPath = null;
bool verbose = false;
for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--mask" or "--mapping" or "-m":
            if (i + 1 >= args.Length) { Console.Error.WriteLine($"Error: {args[i]} requires a file path\n{Usage}"); return 1; }
            mappingPath = args[++i];
            break;
        case "--verbose" or "-v":
            verbose = true;
            break;
        default:
            files.Add(args[i]);
            break;
    }
}
if (files.Count != 3) { Console.Error.WriteLine(Usage); return 1; }

try
{
    var mapper = mappingPath == null ? TypeMapper.Empty : TypeMapper.FromFile(mappingPath);
    var blocks1 = DumpParser.Parse(files[0]);
    var blocks2 = DumpParser.Parse(files[1]);
    if (blocks1.Count == 0) Console.Error.WriteLine($"Warning: No blocks found in {files[0]}");
    if (blocks2.Count == 0) Console.Error.WriteLine($"Warning: No blocks found in {files[1]}");
    var result = BlockComparer.Compare(blocks1, blocks2, mapper);
    string n1 = Path.GetFileName(files[0]), n2 = Path.GetFileName(files[1]);
    File.WriteAllText(files[2], OutputFormatter.Format(result, n1, n2));
    if (verbose) Console.WriteLine(OutputFormatter.Summary(result, n1, n2, files[2], blocks1.Count, blocks2.Count));
    return 0;
}
catch (Exception ex) when (ex is IOException or FormatException or InvalidDataException or UnauthorizedAccessException)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    return 1;
}
