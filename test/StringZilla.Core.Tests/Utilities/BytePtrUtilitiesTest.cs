using StringZilla.Core.Static;
using StringZilla.Core.Utilities;

namespace StringZilla.Core.Tests.Utilities
{
    public sealed class BytePtrUtilitiesTest
    {
        [Fact]
        public unsafe void FillAvx2()
        {
            byte* output = stackalloc byte[32];
            BytePtrUtilities.FillAvx2(output, 32, 0x12);
            for (int i = 0; i < 32; i++)
            {
                Assert.Equal(0x12, output[i]);
            }
        }

        [Fact]
        public unsafe void FindByteAvx2_Found()
        {
            byte* input = stackalloc byte[32];
            for (int i = 0; i < 32; i++)
            {
                input[i] = (byte)i;
            }
            int index = BytePtrUtilities.FindByteAvx2(input, 32, 0x12);
            Assert.Equal(0x12, index);
        }

        [Fact]
        public unsafe void FindByteAvx2_NotFound()
        {
            byte* input = stackalloc byte[32];
            for (int i = 0; i < 32; i++)
            {
                input[i] = (byte)i;
            }
            int index = BytePtrUtilities.FindByteAvx2(input, 32, 0x33);
            Assert.Equal(-1, index);
        }

        [Fact]
        public unsafe void FindAvx2_Found()
        {
            byte* input = stackalloc byte[32];
            byte* compare = stackalloc byte[3];
            for (int i = 0; i < 32; i++)
            {
                input[i] = (byte)i;
            }
            compare[0] = 0x12;
            compare[1] = 0x13;
            compare[2] = 0x14;
            int index = BytePtrUtilities.FindAvx2(input, 32, compare, 3);
            Assert.Equal(0x12, index);
        }

        [Fact]
        public unsafe void FindAvx2_NotFound()
        {
            byte* input = stackalloc byte[32];
            byte* compare = stackalloc byte[3];
            for (int i = 0; i < 32; i++)
            {
                input[i] = (byte)i;
            }
            compare[0] = 0x12;
            compare[1] = 0x34;
            compare[2] = 0x56;
            int index = BytePtrUtilities.FindAvx2(input, 32, compare, 3);
            Assert.Equal(-1, index);
        }

        private static unsafe void TestFindAvx2(ReadOnlySpan<byte> input, ReadOnlySpan<byte> compare)
        {
            fixed (byte* inputPtr = input)
            {
                fixed (byte* comparePtr = compare)
                {
                    int index = BytePtrUtilities.FindAvx2(inputPtr, input.Length, comparePtr, compare.Length);
                    int index1 = BytePtrUtilities.FindSerial(inputPtr, input.Length, comparePtr, compare.Length);
                    Assert.Equal(index1, index);
                }
            }
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