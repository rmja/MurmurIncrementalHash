using System;
using System.Text;
using Xunit;

namespace MurmurIncrementalHash.Tests
{
    public class Murmur32Tests
    {
        [Theory]
        [InlineData(new byte[] { 1, 2, 3 }, 293, 1971362553)]
        [InlineData(new byte[] { 1, 2, 3, 4 }, 293, 2911303516)]
        [InlineData(new byte[] { 1, 2, 3, 4, 5 }, 293, 3144920404)]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6 }, 293, 3796703664)]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7 }, 293, 3433363787)]
        [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 293, 2343089733)]
        [InlineData(new byte[0], 0, 0)]
        [InlineData(new byte[0], 1, 0x514E28B7)]
        [InlineData(new byte[0], 0xffffffff, 0x81F16F39)]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 0, 0x76293B50)]
        [InlineData(new byte[] { 0x21, 0x43, 0x65, 0x87 }, 0, 0xF55B516B)]
        [InlineData(new byte[] { 0x21, 0x43, 0x65, 0x87 }, 0x5082EDEE, 0x2362F9DE)]
        [InlineData(new byte[] { 0x21, 0x43, 0x65 }, 0, 0x7E4A8634)]
        [InlineData(new byte[] { 0x21, 0x43 }, 0, 0xA0F7B07A)]
        [InlineData(new byte[] { 0x21 }, 0, 0x72661CF4)]
        [InlineData(new byte[] { 0x0, 0x0, 0x0, 0x0 }, 0, 0x2362F9DE)]
        [InlineData(new byte[] { 0x0, 0x0, 0x0 }, 0, 0x85F0B427)]
        [InlineData(new byte[] { 0x0, 0x0 }, 0, 0x30F4C306)]
        [InlineData(new byte[] { 0x0 }, 0, 0x514E28B7)]
        public void SmokeTest(byte[] data, uint seed, uint expectedHash)
        {
            // Given
            var murmur = Murmur32.Create(seed);

            // When
            murmur.AppendData(data);
            var hash = murmur.GetHashAndReset();

            // Then
            Assert.Equal(expectedHash, hash);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        public void MultiAppendTest(int split)
        {
            // Given
            var murmur = Murmur32.Create(293);
            var data = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

            // When
            murmur.AppendData(data.AsSpan(0, split));
            murmur.AppendData(data.AsSpan(split));
            var hash = murmur.GetHashAndReset();

            // Then
            Assert.Equal(2343089733U, hash);
        }

        // https://asecuritysite.com/hash/mur
        [Theory]
        [InlineData("t", 3397902157)]
        [InlineData("te", 3988319771)]
        [InlineData("tes", 196677210)]
        [InlineData("test", 3127628307)]
        [InlineData("testt", 980066067)]
        [InlineData("testte", 1691472556)]
        [InlineData("testtes", 1062529260)]
        [InlineData("testtest", 723759603)]
        public void Online(string input, uint expectedHash)
        {
            // Given
            var murmur = Murmur32.Create();
            var bytes = Encoding.ASCII.GetBytes(input);

            // When
            murmur.AppendData(bytes);
            var hash = murmur.GetHashAndReset();

            // Then
            Assert.Equal(expectedHash, hash);
        }
    }
}