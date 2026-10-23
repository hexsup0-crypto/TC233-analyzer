# TC233-analyzer

Compares two TC233 binary dumps. Blocks start with `65 af f5 f5` and end with `50 0a` + 3-byte type + 3 service bytes.

```
TC233-analyzer dump1.bin dump2.bin output.txt [--mapping|-m mapping.txt] [--verbose|-v]
```

The report has a `DUPLICATE BLOCKS` section (identical blocks from both dumps, with addresses) and an `UNPAIRED BLOCKS` section.

## Type mapping
`mapping.txt` lines have the form `type_dump1 = type_dump2` (hex bytes, no prefix); `#` starts a comment.
Blocks whose types are mapped are compared by content ignoring the type bytes. Missing file, bad format,
duplicate rules and cyclic rules are reported as errors. See `examples/mapping_example.txt`.
