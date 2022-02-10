using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace MurmurIncrementalHash
{
    public class Murmur32 : MurmurIncrementalHash<uint>
    {
        private const uint C1 = 0xcc9e2d51;
        private const uint C2 = 0x1b873593;

        private Murmur32(uint seed) : base(seed, 4)
        {
        }

        /// <summary>
        /// Create a new 32 bit murmur hash.
        /// </summary>
        /// <param name="seed">The algorithm seed.</param>
        /// <returns>The incremental hash instance.</returns>
        public static Murmur32 Create(uint seed = 0) => new(seed);

        protected override void Clear()
        {
            _h = Seed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override uint ReadBlock(ReadOnlySpan<byte> data) => BitConverter.ToUInt32(data);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void WriteBlock(Span<byte> destination, uint h) => Debug.Assert(BitConverter.TryWriteBytes(destination, h));

        // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L112-L120
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Mix(ref uint h1, uint k1)
        {
            k1 *= C1;
            k1 = Rotl(k1, 15);
            k1 *= C2;
            h1 ^= k1;
            h1 = Rotl(h1, 13);
            h1 = h1 * 5 + 0xe6546b64;
        }

        protected override uint GetHash()
        {
            uint h1 = _h;

            // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L124-L136
            uint k1 = 0;
            var tail = GetTail();
            switch (tail.Length)
            {
                case 3: k1 ^= (uint)tail[2] << 16; goto case 2;
                case 2: k1 ^= (uint)tail[1] << 8; goto case 1;
                case 1:
                    k1 ^= tail[0];
                    k1 *= C1;
                    k1 = Rotl(k1, 15);
                    k1 *= C2;
                    h1 ^= k1;
                    break;
            }

            // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L139-L145
            h1 ^= (uint)_len;
            h1 = FMix(h1);

            return h1;
        }

        /// <summary>
        /// Get the hash value and reset the internal state to allow for new continous use.
        /// </summary>
        /// <returns>The hash</returns>
        public new uint GetHashAndReset()
        {
            uint h1 = GetHash();
            Reset();
            return h1;
        }
    }
}
