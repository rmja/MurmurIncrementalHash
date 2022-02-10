using System;
using System.Text;
using Xunit;

namespace MurmurIncrementalHash.Tests
{
    public class Murmur128Tests
    {
        // https://asecuritysite.com/hash/mur
        [Theory]
        [InlineData("", "00000000000000000000000000000000")]
        [InlineData("t", "16281b5522c18c1322c18c1322c18c13")]
        [InlineData("te", "0f623fd440dd081940dd081940dd0819")]
        [InlineData("tes", "c701bcd2562e9fd5562e9fd5562e9fd5")]
        [InlineData("test1test2test3test4test5test6test7test8", "0671d271b0586fd5a3703943516972fd")]
        public void OnlineX86(string input, string expectedHashHex)
        {
            // Given
            var murmur = Murmur128.Create(0, Murmur128Algorithm.X86);
            var bytes = Encoding.ASCII.GetBytes(input);

            // When
            murmur.AppendData(bytes);
            var hash = murmur.GetHashAndReset();

            // Then
            Assert.Equal(expectedHashHex, ToHex(hash));
        }

        // https://asecuritysite.com/hash/mur
        [Theory]
        [InlineData("", "00000000000000000000000000000000")]
        [InlineData("t", "e64c0a6370bc7d684cd8d67043ac6518")]
        [InlineData("te", "73dfaf41c32f64598050c25f60d6902b")]
        [InlineData("tes", "f821aead074da1a508e371cc71da7ec7")]
        [InlineData("test1test2test3test4test5test6test7test8", "adb15144771d87563783a24e4ffc52b8")]
        public void OnlineX64(string input, string expectedHashHex)
        {
            // Given
            var murmur = Murmur128.Create(0, Murmur128Algorithm.X64);
            var bytes = Encoding.ASCII.GetBytes(input);

            // When
            murmur.AppendData(bytes);
            var hash = murmur.GetHashAndReset();

            // Then
            Assert.Equal(expectedHashHex, ToHex(hash));
        }

        private static string ToHex(byte[] value) => BitConverter.ToString(value).Replace("-", string.Empty).ToLowerInvariant();
    }
}