using System;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace MurmurIncrementalHash.Tests
{
    public class Murmur128Tests
    {
        public Murmur128Tests(ITestOutputHelper testOutputHelper)
        {
            //Console.SetOut(new TestOutputTextWriter(testOutputHelper));
        }

        [Theory]
        [InlineData(new byte[] { 0x13, 0x37 }, 0, "cfa4acc1956d584456979e933134d835")]
        public void SmokeTest(byte[] data, uint seed, string expectedHashHex)
        {
            // Given
            var hash = new byte[16];
            var murmur = Murmur128.Create(seed, Murmur128Algorithm.X64);

            // When
            murmur.AppendData(data);
            murmur.GetHashAndReset(hash);

            // Then
            Assert.IsType<Murmur128x64>(murmur);
            Assert.Equal(expectedHashHex, ToHex(hash));
        }


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