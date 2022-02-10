using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MurmurIncrementalHash
{
    internal class Murmur128x64 : MurmurIncrementalHash<Murmur128x64.Block>
    {
        private const ulong C1 = 0x87c37b91114253d5;
        private const ulong C2 = 0x4cf5ad432745937f;

        public record struct Block(ulong Item1, ulong Item2);

        public Murmur128x64(uint seed) : base(seed, 16)
        {
        }

        protected override void Clear()
        {
            _h = new(Seed, Seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Block ReadBlock(ReadOnlySpan<byte> data) => new
            (
                BitConverter.ToUInt64(data.Slice(0)),
                BitConverter.ToUInt64(data.Slice(8))
            );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void WriteBlock(Span<byte> destination, Block h)
        {
            Debug.Assert(BitConverter.TryWriteBytes(destination.Slice(0), h.Item1));
            Debug.Assert(BitConverter.TryWriteBytes(destination.Slice(8), h.Item2));
        }

        // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L277-L283
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Mix(ref Block h, Block k)
        {
            (ulong k1, ulong k2) = k;
            (ulong h1, ulong h2) = h;

            k1 *= C1; k1 = Rotl(k1, 31); k1 *= C2; h1 ^= k1;
            h1 = Rotl(h1, 27); h1 += h2; h1 = h1 * 5 + 0x52dce729;
            k2 *= C2; k2 = Rotl(k2, 33); k2 *= C1; h2 ^= k2;
            h2 = Rotl(h2, 31); h2 += h1; h2 = h2 * 5 + 0x38495ab5;

            h = new(h1, h2);
        }

        protected override Block GetHash()
        {
            (ulong h1, ulong h2) = _h;

            // Tail
            // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L286-L314
            (ulong k1, ulong k2) = (0, 0);
            var tail = GetTail();
            switch (tail.Length)
            {
                case 15: k2 ^= (ulong)tail[14] << 48; goto case 14;
                case 14: k2 ^= (ulong)tail[13] << 40; goto case 13;
                case 13: k2 ^= (ulong)tail[12] << 32; goto case 12;
                case 12: k2 ^= (ulong)tail[11] << 24; goto case 11;
                case 11: k2 ^= (ulong)tail[10] << 16; goto case 10;
                case 10: k2 ^= (ulong)tail[9] << 8; goto case 9;
                case 9: k2 ^= (ulong)tail[8] << 0;
                        k2 *= C2; k2 = Rotl(k2, 33); k2 *= C1; h2 ^= k2;
                        goto case 8;
                case 8: k1 ^= (ulong)tail[7] << 56; goto case 7;
                case 7: k1 ^= (ulong)tail[6] << 48; goto case 6;
                case 6: k1 ^= (ulong)tail[5] << 40; goto case 5;
                case 5: k1 ^= (ulong)tail[4] << 32; goto case 4;
                case 4: k1 ^= (ulong)tail[3] << 24; goto case 3;
                case 3: k1 ^= (ulong)tail[2] << 16; goto case 2;
                case 2: k1 ^= (ulong)tail[1] << 8; goto case 1;
                case 1: k1 ^= (ulong)tail[0] << 0;
                        k1 *= C1; k1 = Rotl(k1, 31); k1 *= C2; h1 ^= k1;
                        break;
            }

            // Finalization
            // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L316-L331
            var len = (ulong)_len;
            h1 ^= len; h2 ^= len;

            h1 += h2;
            h2 += h1;

            h1 = FMix(h1);
            h2 = FMix(h2);

            h1 += h2;
            h2 += h1;

            return new(h1, h2);
        }
    }
}
