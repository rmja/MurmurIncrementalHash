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

An instance can be safely reused after the call to `GetHashAndReset()`.
The `Create()` method is fast but it allocates an internal buffer.


# Performance
[See the test suite here](src/MurmurIncrementalHash.Benchmarks/MurmurBenchmarks.cs). These numbers are from Ryzen 4000, 64 bit laptop.

|       Method |      Mean |    Error |   StdDev | Allocated |
|------------- |----------:|---------:|---------:|----------:|
|     Murmur32 |  62.75 ns | 1.266 ns | 1.601 ns |         - |
| Murmur128x64 |  56.03 ns | 0.470 ns | 0.392 ns |         - |
| Murmur128x86 |  60.51 ns | 0.185 ns | 0.173 ns |         - |
|          Md5 | 269.07 ns | 1.713 ns | 1.603 ns |         - |
|         Sha1 | 271.19 ns | 1.680 ns | 1.403 ns |         - |
|       Sha256 | 195.18 ns | 1.092 ns | 0.912 ns |         - |
