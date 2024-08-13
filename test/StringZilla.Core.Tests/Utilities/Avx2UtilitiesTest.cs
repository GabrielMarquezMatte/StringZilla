using StringZilla.Core.Static;
using StringZilla.Core.Utilities;

namespace StringZilla.Core.Tests.Utilities
{
    public sealed class Avx2UtilitiesTest
    {
        [Fact]
        public unsafe void FillAvx2()
        {
            Span<byte> output = stackalloc byte[32];
            Avx2Utilities.FillAvx2(output, 0x12);
            foreach (ref readonly var item in output)
            {
                Assert.Equal(0x12, item);
            }
        }

        [Fact]
        public unsafe void FindByteAvx2_Found()
        {
            Span<byte> input = stackalloc byte[32];
            for (int i = 0; i < 32; i++)
            {
                input[i] = (byte)i;
            }
            int index = Avx2Utilities.FindByteAvx2(input, 0x12);
            Assert.Equal(0x12, index);
        }

        [Fact]
        public unsafe void FindByteAvx2_NotFound()
        {
            Span<byte> input = stackalloc byte[32];
            for (int i = 0; i < 32; i++)
            {
                input[i] = (byte)i;
            }
            int index = Avx2Utilities.FindByteAvx2(input, 0x33);
            Assert.Equal(-1, index);
        }

        [Fact]
        public unsafe void FindAvx2_Found()
        {
            Span<byte> input = stackalloc byte[32];
            Span<byte> compare = stackalloc byte[3];
            for (int i = 0; i < 32; i++)
            {
                input[i] = (byte)i;
            }
            compare[0] = 0x12;
            compare[1] = 0x13;
            compare[2] = 0x14;
            int index = Avx2Utilities.FindAvx2(input, compare);
            Assert.Equal(0x12, index);
        }

        [Fact]
        public unsafe void FindAvx2_NotFound()
        {
            Span<byte> input = stackalloc byte[32];
            Span<byte> compare = stackalloc byte[3];
            for (int i = 0; i < 32; i++)
            {
                input[i] = (byte)i;
            }
            compare[0] = 0x12;
            compare[1] = 0x34;
            compare[2] = 0x56;
            int index = Avx2Utilities.FindAvx2(input, compare);
            Assert.Equal(-1, index);
        }

        private static unsafe void TestFindAvx2(ReadOnlySpan<byte> input, ReadOnlySpan<byte> compare)
        {
            int index = Avx2Utilities.FindAvx2(input, compare);
            int index1 = input.IndexOf(compare);
            Assert.Equal(index1, index);
        }

        [Fact]
        public unsafe void FindDigitsAvx2_Found()
        {
            TestFindAvx2(CharacterSets.AsciiHexDigits, CharacterSets.AsciiDigits);
        }

        [Fact]
        public unsafe void FindDigitsAvx2_NotFound()
        {
            TestFindAvx2(CharacterSets.AsciiHexDigits, CharacterSets.AsciiLetters);
        }

        [Fact]
        public unsafe void FindPunctuationAvx2_Found()
        {
            TestFindAvx2(CharacterSets.PrintableAscii, CharacterSets.Punctuation);
        }
    }
}