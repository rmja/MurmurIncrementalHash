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
        [InlineData("abcdefghijklmnopqrstuvwxyz", "1306343e662f6f666e56f6172c3de344")]
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
        [InlineData("abcdefghijklmnopqrstuvwxyz", "a94a6f517e9d9c7429d5a7b6899cade9")]
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

        [Theory]
        [InlineData(Murmur128Algorithm.X64)]
        [InlineData(Murmur128Algorithm.X86)]
        public void VariousLengths(Murmur128Algorithm algorithm)
        {
            // Given
            var murmur = Murmur128.Create(0, algorithm);
            var data = new byte[2048];

            // When
            for (var i = 0; i <= 2048; i++)
            {
                murmur.AppendData(data.AsSpan(0, i));
                murmur.AppendData(data.AsSpan(i));
            }

            // Then
            murmur.GetHashAndReset();
        }

        [Fact]
        public void SomeIssue()
        {
            // Given
            var murmur = Murmur128.Create();

            // When
            murmur.AppendData(new byte[249]);
            murmur.AppendData(new byte[4062]);
            murmur.AppendData(new byte[2282]);
            murmur.AppendData(new byte[631]);

            // Then
            murmur.GetHashAndReset();
        }

        private static string ToHex(byte[] value) => BitConverter.ToString(value).Replace("-", string.Empty).ToLowerInvariant();
    }
}