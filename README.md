# TC233-analyzer

Groups blocks from two TC233 binary dumps by their three-byte type. Blocks start with `65 af f5 f5` and end with `50 0a` + 3-byte type + 3 service bytes.

```
TC233-analyzer.exe dump1.bin dump2.bin output.txt [--mask mask.txt] [--verbose|-v]
```

The report lists every occurrence of each block type present in both dumps under `BLOCKS BY TYPE`. Types found in only one dump are listed under `UNPAIRED BLOCKS`. Each occurrence includes its source dump, address, size, and content; content is wrapped at 50 bytes per line.

## Type mask
`mask.txt` lines have the form `type_dump1 = type_dump2` (three hexadecimal bytes on each side, no `0x` prefix); `#` starts a comment. Mapped types are grouped together, regardless of block contents. Blank lines and comments are ignored. Use `-m` as a shorthand for `--mask`. The older `--mapping` option is also accepted. See `examples/mask_example.txt`.
