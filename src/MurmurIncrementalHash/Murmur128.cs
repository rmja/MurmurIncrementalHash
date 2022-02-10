namespace MurmurIncrementalHash
{
    public static class Murmur128
    {
        /// <summary>
        /// Create a new 128 bit Murmur hash.
        /// </summary>
        /// <param name="seed">The algorithm seed.</param>
        /// <param name="algorithm">The type of algorithm to return. By default, the fastest for the arhitecture is returned.</param>
        /// <returns>The incremental hash instance.</returns>
        public static MurmurIncrementalHash Create(uint seed = 0, Murmur128Algorithm algorithm = Murmur128Algorithm.Default) => algorithm switch
        {
            Murmur128Algorithm.X64 => new Murmur128x64(seed),
            Murmur128Algorithm.X86 => new Murmur128x86(seed),
            _ => Environment.Is64BitProcess
                ? new Murmur128x64(seed)
                : new Murmur128x86(seed)
        };
    }
}
