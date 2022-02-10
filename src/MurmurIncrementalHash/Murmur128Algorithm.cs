namespace MurmurIncrementalHash
{
    public enum Murmur128Algorithm
    {
        /// <summary>
        /// The default algorithm, which is the fastest for the current architecture.
        /// Do not use this if repeatability is needed across platforms.
        /// </summary>
        Default,

        /// <summary>
        /// The 64 bit algorithm version.
        /// </summary>
        X64,

        /// <summary>
        /// The 32 bit algorithm version.
        /// </summary>
        X86,
    }
}
