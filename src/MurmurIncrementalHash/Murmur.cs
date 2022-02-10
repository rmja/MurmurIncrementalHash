using System.Numerics;
using System.Runtime.CompilerServices;

namespace MurmurIncrementalHash
{
    // https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp
    public abstract class Murmur
    {
        protected int _len;

        /// <summary>
        /// The algorithm seed.
        /// </summary>
        public uint Seed { get; }

        /// <summary>
        /// The hash size.
        /// This is 4 for Murmur32 and 16 for Murmur128.
        /// </summary>
        public int HashSize { get; }

        internal Murmur(uint seed, int hashSize)
        {
            Seed = seed;
            HashSize = hashSize;
        }

        /// <summary>
        /// Append data to the hash computation. This can be called multiple times befere the final hash is computed.
        /// </summary>
        /// <param name="data">The data include in the hash.</param>
        public abstract void AppendData(ReadOnlySpan<byte> data);

        /// <summary>
        /// Get the hash value and reset the internal state to allow for new continous use.
        /// </summary>
        /// <param name="destination">The buffer to where the hash is written. <see cref="HashSize"/> bytes are written.</param>
        public abstract void GetHashAndReset(Span<byte> destination);

        /// <summary>
        /// Get the hash value and reset the internal state to allow for new continous use.
        /// Prefer <see cref="GetHashAndReset(Span{byte})"/> to avoid allocation.
        /// </summary>
        /// <returns>The hash</returns>
        public byte[] GetHashAndReset()
        {
            var hash = new byte[HashSize];
            GetHashAndReset(hash);
            return hash;
        }
    }

    public abstract class Murmur<T> : Murmur where T: struct
    {
        protected T _h;
        private readonly byte[] _carryBuffer;
        private int _carryLength;

        internal Murmur(uint seed, int hashSize) : base(seed, hashSize)
        {
            _carryBuffer = new byte[HashSize];
            Reset();
        }

        protected abstract void Clear();

        /// <summary>
        /// Reset the internal state.
        /// </summary>
        public void Reset()
        {
            Clear();
            _len = 0;
            _carryLength = 0;
        }

        public override void AppendData(ReadOnlySpan<byte> data)
        {
            var h = _h;

            var offset = 0;
            var blockSize = HashSize;
            var carryLength = _carryLength;
            if (carryLength > 0)
            {
                // Try and fill carry
                offset = Math.Min(data.Length, blockSize - carryLength);
                data.Slice(0, offset).CopyTo(_carryBuffer.AsSpan(carryLength));
                carryLength += offset;

                if (carryLength == blockSize)
                {
                    // Consume carry
                    var k = ReadBlock(_carryBuffer);
                    Mix(ref h, k);
                    carryLength = 0;
                }
            }

            var length = data.Length;
            var remaining = length - offset;
            var remainder = remaining & (blockSize - 1);
            var blocksLength = remaining - remainder;
            for (var start = offset; start < blocksLength; start += blockSize)
            {
                var k = ReadBlock(data.Slice(start, blockSize));
                Mix(ref h, k);
            }

            if (remainder > 0)
            {
                data.Slice(blocksLength).CopyTo(_carryBuffer.AsSpan(carryLength));
                carryLength += remainder;
            }

            _h = h;
            _len += length;
            _carryLength = carryLength;
        }

        protected ReadOnlySpan<byte> GetTail() => _carryBuffer.AsSpan(0, _carryLength);

        public override void GetHashAndReset(Span<byte> destination)
        {
            var hash = GetHash();
            WriteBlock(destination, hash);
            Reset();
        }

        protected abstract T ReadBlock(ReadOnlySpan<byte> data);
        protected abstract void WriteBlock(Span<byte> destination, T h);
        protected abstract void Mix(ref T h, T k);
        protected abstract T GetHash();

        // https://github.com/dotnet/corert/blob/master/src/System.Private.CoreLib/shared/System/Numerics/BitOperations.cs#L365-L366
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static uint Rotl(uint value, int offset) => BitOperations.RotateLeft(value, offset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static ulong Rotl(ulong value, int offset) => BitOperations.RotateLeft(value, offset);

        // See https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L68-L77
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static uint FMix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }

        // See https://github.com/aappleby/smhasher/blob/master/src/MurmurHash3.cpp#L81-L90
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static ulong FMix(ulong k)
        {
            k ^= k >> 33;
            k *= 0xff51afd7ed558ccd;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53;
            k ^= k >> 33;
            return k;
        }
    }
}