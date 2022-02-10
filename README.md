# MurmurIncrementalHash
Fast, 0-allocation, Murmur32 and Murmur128 incremental hash implementation.

# Usage
```C#
var murmur = Murmur32.Create();
murmur.AppendData(data1);
murmur.AppendData(data2);
uint hash = murmur.GetHashAndReset();
```

```C#
var murmur = Murmur128.Create();
murmur.AppendData(data1);
murmur.AppendData(data2);
byte[] hash = murmur.GetHashAndReset();
// Span<byte> hash = stackalloc byte[16];
// murmur.GetHashAndReset(hash);
```

# Performance
[See the test suite here](src/MurmurIncrementalHash.Benchmarks/MurmurBenchmarks.cs). These numbers are from Ryzen 4000, 64 bit laptop.

|       Method |     Mean |    Error |   StdDev | Allocated |
|------------- |---------:|---------:|---------:|----------:|
|     Murmur32 | 56.46 ns | 1.088 ns | 1.118 ns |         - |
| Murmur128x64 | 56.16 ns | 0.432 ns | 0.361 ns |         - |
| Murmur128x86 | 62.94 ns | 0.815 ns | 0.681 ns |         - |
