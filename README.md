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
|     Murmur32 |  60.26 ns | 0.322 ns | 0.286 ns |         - |
| Murmur128x64 |  56.84 ns | 0.507 ns | 0.474 ns |         - |
| Murmur128x86 |  58.29 ns | 0.166 ns | 0.130 ns |         - |
|          Md5 | 275.34 ns | 0.677 ns | 0.634 ns |         - |
