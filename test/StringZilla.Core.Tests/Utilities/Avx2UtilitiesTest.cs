using StringZilla.Core.Static;
using StringZilla.Core.Utilities;

namespace StringZilla.Core.Tests.Utilities
{
    public sealed class Avx2UtilitiesTest
    {
        private static Span<byte> CreateSpan(Span<byte> output)
        {
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = (byte)(i % 256);
            }
            return output;
        }
        [Fact]
        public void FillAvx2()
        {
            Span<byte> output = stackalloc byte[32];
            Avx2Utilities.FillAvx2(output, 0x12);
            foreach (ref readonly var item in output)
            {
                Assert.Equal(0x12, item);
            }
        }

        [Fact]
        public void FindByteAvx2_Found()
        {
            Span<byte> input = stackalloc byte[32];
            input = CreateSpan(input);
            int index = Avx2Utilities.FindByteAvx2(input, 0x12);
            Assert.Equal(0x12, index);
        }

        [Fact]
        public void FindByteAvx2_NotFound()
        {
            Span<byte> input = stackalloc byte[32];
            input = CreateSpan(input);
            int index = Avx2Utilities.FindByteAvx2(input, 0x33);
            Assert.Equal(-1, index);
        }

        [Fact]
        public void FindAvx2_Found()
        {
            Span<byte> input = stackalloc byte[32];
            Span<byte> compare = stackalloc byte[3];
            input = CreateSpan(input);
            compare[0] = 0x12;
            compare[1] = 0x13;
            compare[2] = 0x14;
            int index = Avx2Utilities.FindAvx2(input, compare);
            Assert.Equal(0x12, index);
        }

        [Fact]
        public void FindAvx2_NotFound()
        {
            Span<byte> input = stackalloc byte[32];
            Span<byte> compare = stackalloc byte[3];
            input = CreateSpan(input);
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
        public void FindDigitsAvx2_Found()
        {
            TestFindAvx2(CharacterSets.AsciiHexDigits, CharacterSets.AsciiDigits);
        }

        [Fact]
        public void FindDigitsAvx2_NotFound()
        {
            TestFindAvx2(CharacterSets.AsciiHexDigits, CharacterSets.AsciiLetters);
        }

        [Fact]
        public void FindPunctuationAvx2_Found()
        {
            TestFindAvx2(CharacterSets.PrintableAscii, CharacterSets.Punctuation);
        }

        [Fact]
        public void FindPunctuationAvx2_NotFound()
        {
            TestFindAvx2(CharacterSets.PrintableAscii, CharacterSets.AsciiDigits);
        }

        [Fact]
        public void FindWhitespaceAvx2_Found()
        {
            TestFindAvx2(CharacterSets.PrintableAscii, CharacterSets.Whitespaces);
        }

        [Fact]
        public void FindWhitespaceAvx2_NotFound()
        {
            TestFindAvx2(CharacterSets.PrintableAscii, CharacterSets.AsciiDigits);
        }

        [Fact]
        public void FindControlCharactersAvx2_Found()
        {
            TestFindAvx2(CharacterSets.AsciiControlCharacters, CharacterSets.AsciiControlCharacters);
        }

        [Fact]
        public void FindControlCharactersAvx2_NotFound()
        {
            TestFindAvx2(CharacterSets.PrintableAscii, CharacterSets.AsciiControlCharacters);
        }
    }
}