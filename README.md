# TC233-analyzer

A .NET 8 console application that finds identical TC233 data blocks in two binary dumps.

## Usage

```bash
dotnet run -- dump1.bin dump2.bin output.txt
```

The parser looks for blocks beginning with `65 af f5 f5` and ending at the next
`50 0a XX XX XX` marker. The three bytes after `50 0a` are reported as the block
type; bytes following that marker are not part of the block. Duplicate groups
contain every occurrence from each dump whose complete block contents match
byte-for-byte.
