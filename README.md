# MurmurIncrementalHash
Fast, 0 allocation, Murmur32 and Murmur128 incremental hash implementation.

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

