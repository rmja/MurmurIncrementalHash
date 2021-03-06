using BenchmarkDotNet.Attributes;
using System.Security.Cryptography;

namespace MurmurIncrementalHash.Benchmarks;

[MemoryDiagnoser]
public class MurmurBenchmarks
{
    private readonly Murmur32 _murmur32 = MurmurIncrementalHash.Murmur32.Create(0);
    private readonly Murmur _murmur128x64 = Murmur128.Create(0, Murmur128Algorithm.X64);
    private readonly Murmur _murmur128x86 = Murmur128.Create(0, Murmur128Algorithm.X86);
    private readonly IncrementalHash _md5 = IncrementalHash.CreateHash(HashAlgorithmName.MD5);
    private readonly IncrementalHash _sha1 = IncrementalHash.CreateHash(HashAlgorithmName.SHA1);
    private readonly IncrementalHash _sha256 = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
    private readonly byte[] _data = new byte[] { 1, 2, 3, 4, 5, 6 };

    [Benchmark]
    public void Murmur32()
    {
        _murmur32.AppendData(_data);
        _murmur32.AppendData(_data);
        _murmur32.AppendData(_data);
        _murmur32.AppendData(_data);

        _murmur32.GetHashAndReset();
    }

    [Benchmark]
    public void Murmur128x64()
    {
        _murmur128x64.AppendData(_data);
        _murmur128x64.AppendData(_data);
        _murmur128x64.AppendData(_data);
        _murmur128x64.AppendData(_data);

        Span<byte> hash = stackalloc byte[16];
        _murmur128x64.GetHashAndReset(hash);
    }

    [Benchmark]
    public void Murmur128x86()
    {
        _murmur128x86.AppendData(_data);
        _murmur128x86.AppendData(_data);
        _murmur128x86.AppendData(_data);
        _murmur128x86.AppendData(_data);

        Span<byte> hash = stackalloc byte[16];
        _murmur128x86.GetHashAndReset(hash);
    }

    [Benchmark]
    public void Md5()
    {
        _md5.AppendData(_data);
        _md5.AppendData(_data);
        _md5.AppendData(_data);
        _md5.AppendData(_data);

        Span<byte> hash = stackalloc byte[_md5.HashLengthInBytes];
        _md5.GetHashAndReset(hash);
    }

    [Benchmark]
    public void Sha1()
    {
        _sha1.AppendData(_data);
        _sha1.AppendData(_data);
        _sha1.AppendData(_data);
        _sha1.AppendData(_data);

        Span<byte> hash = stackalloc byte[_sha1.HashLengthInBytes];
        _sha1.GetHashAndReset(hash);
    }

    [Benchmark]
    public void Sha256()
    {
        _sha256.AppendData(_data);
        _sha256.AppendData(_data);
        _sha256.AppendData(_data);
        _sha256.AppendData(_data);

        Span<byte> hash = stackalloc byte[_sha256.HashLengthInBytes];
        _sha256.GetHashAndReset(hash);
    }
}