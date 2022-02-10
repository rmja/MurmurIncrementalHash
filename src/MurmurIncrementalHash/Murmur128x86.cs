using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MurmurIncrementalHash
{
    internal class Murmur128x86 : Murmur<Murmur128x86.Block>
    {
        private const uint C1 = 0x239b961b;
        private const uint C2 = 0xab0e9789;
        private const uint C3 = 0x38b34ae5;
        private const uint C4 = 0xa1e38b93;

        public record struct Block(uint Item1, uint Item2, uint Item3, uint Item4);

        public Murmur128x86(uint seed) : base(seed, 16)
        {
        }

        protected override void Clear()
        {
            _h = new(Seed, Seed, Seed, Seed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Block ReadBlock(ReadOnlySpan<byte> data) => new
            (
                BitConverter.ToUInt32(data.Slice(0)),
                BitConverter.ToUInt32(data.Slice(4)),
                BitConverter.ToUInt32(data.Slice(8)),
                BitConverter.ToUInt32(data.Slice(12))
            );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void WriteBlock(Span<byte> destination, Block h)
        {
            Debug.Assert(BitConverter.TryWriteBytes(destination.Slice(0), h.Item1));
            Debug.Assert(BitConverter.TryWriteBytes(destination.Slice(4), h.Item2));
            Debug.Assert(BitConverter.TryWriteBytes(destination.Slice(8), h.Item3));
            Debug.Assert(BitConverter.TryWriteBytes(destination.Slice(12), h.Item4));
        }

        // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L277-L283
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Mix(ref Block h, Block k)
        {
            (uint k1, uint k2, uint k3, uint k4) = k;
            (uint h1, uint h2, uint h3, uint h4) = h;

            k1 *= C1; k1 = Rotl(k1, 15); k1 *= C2; h1 ^= k1;
            h1 = Rotl(h1, 19); h1 += h2; h1 = h1 * 5 + 0x561ccd1b;
            k2 *= C2; k2 = Rotl(k2, 16); k2 *= C3; h2 ^= k2;
            h2 = Rotl(h2, 17); h2 += h3; h2 = h2 * 5 + 0x0bcaa747;
            k3 *= C3; k3 = Rotl(k3, 17); k3 *= C4; h3 ^= k3;
            h3 = Rotl(h3, 15); h3 += h4; h3 = h3 * 5 + 0x96cd1c35;
            k4 *= C4; k4 = Rotl(k4, 18); k4 *= C1; h4 ^= k4;
            h4 = Rotl(h4, 13); h4 += h1; h4 = h4 * 5 + 0x32ac3b17;

            h = new(h1, h2, h3, h4);
        }

        protected override Block GetHash()
        {
            (uint h1, uint h2, uint h3, uint h4) = _h;

            // Tail
            // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L286-L314
            (uint k1, uint k2, uint k3, uint k4) = (0, 0, 0, 0);
            var tail = GetTail();
            switch (tail.Length)
            {
                case 15: k4 ^= (uint)tail[14] << 16; goto case 14;
                case 14: k4 ^= (uint)tail[13] << 8; goto case 13;
                case 13: k4 ^= (uint)tail[12] << 0;
                         k4 *= C4; k4 = Rotl(k4, 18); k4 *= C1; h4 ^= k4;
                         goto case 12;
                case 12: k3 ^= (uint)tail[11] << 24; goto case 11;
                case 11: k3 ^= (uint)tail[10] << 16; goto case 10;
                case 10: k3 ^= (uint)tail[9] << 8; goto case 9;
                case 9: k3 ^= (uint)tail[8] << 0;
                        k3 *= C3; k3 = Rotl(k3, 17); k3 *= C4; h3 ^= k3;
                        goto case 8;
                case 8: k2 ^= (uint)tail[7] << 24; goto case 7;
                case 7: k2 ^= (uint)tail[6] << 16; goto case 6;
                case 6: k2 ^= (uint)tail[5] << 8; goto case 5;
                case 5: k2 ^= (uint)tail[4] << 0;
                        k2 *= C2; k2 = Rotl(k2, 16); k2 *= C3; h2 ^= k2;
                        goto case 4;
                case 4: k1 ^= (uint)tail[3] << 24; goto case 3;
                case 3: k1 ^= (uint)tail[2] << 16; goto case 2;
                case 2: k1 ^= (uint)tail[1] << 8; goto case 1;
                case 1: k1 ^= (uint)tail[0] << 0;
                        k1 *= C1; k1 = Rotl(k1, 15); k1 *= C2; h1 ^= k1;
                        break;
            }

            // Finalization
            // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L316-L331
            var len = (uint)_len;
            h1 ^= len; h2 ^= len; h3 ^= len; h4 ^= len;

            h1 += h2; h1 += h3; h1 += h4;
            h2 += h1; h3 += h1; h4 += h1;

            h1 = FMix(h1);
            h2 = FMix(h2);
            h3 = FMix(h3);
            h4 = FMix(h4);

            h1 += h2; h1 += h3; h1 += h4;
            h2 += h1; h3 += h1; h4 += h1;

            return new(h1, h2, h3, h4);
        }
    }
}
