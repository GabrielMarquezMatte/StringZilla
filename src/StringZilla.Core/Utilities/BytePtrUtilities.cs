using System.Numerics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace StringZilla.Core.Utilities
{
    public static class BytePtrUtilities
    {
        private static unsafe bool Equal(byte* input, byte* value, int valueLength)
        {
            for (int i = 0; i < valueLength; i++)
            {
                if (input[i] != value[i])
                {
                    return false;
                }
            }
            return true;
        }
        public static unsafe int FindSerial(byte* input, int length, byte* value, int valueLength)
        {
            for (int i = 0; i < length; i++, input++)
            {
                if (Equal(input, value, valueLength))
                {
                    return i;
                }
            }
            return -1;
        }
        public static unsafe void FillAvx2(byte* output, int length, byte value)
        {
            Vector256<byte> valueVector = Vector256.Create(value);
            while (length >= 32)
            {
                Avx.Store(output, valueVector);
                output += 32;
                length -= 32;
            }
            for (; length > 0; length--, output++)
            {
                *output = value;
            }
        }
        public static unsafe int FindByteAvx2(byte* input, int length, byte value)
        {
            Vector256<byte> valueVector = Vector256.Create(value);
            while (length >= 32)
            {
                Vector256<byte> inputVector = Avx.LoadVector256(input);
                int mask = Avx2.MoveMask(Avx2.CompareEqual(inputVector, valueVector));
                if (mask != 0)
                {
                    return BitOperations.TrailingZeroCount((uint)mask);
                }
                input += 32;
                length -= 32;
            }
            return FindSerial(input, length, &value, 1);
        }
        private static unsafe void LocateNeedleAnomalies(byte* start, int length, out int first, out int second, out int third)
        {
            first = 0;
#pragma warning disable SS003 // The operands of a divisive expression are both integers and result in an implicit rounding.
            second = length / 2;
#pragma warning restore SS003 // The operands of a divisive expression are both integers and result in an implicit rounding.
            third = length - 1;
            bool hasDuplicates = start[first] == start[second] || start[second] == start[third] || start[first] == start[third];
            if (length > 3 && hasDuplicates)
            {
                while (start[second] == start[first] && second + 1 < third) { ++second; }
                while ((start[third] == start[second] || start[third] == start[first]) && third > (second + 1)) { --third; }
            }
            if (length <= 8)
            {
                return;
            }
            int vibrantFirst = first;
            int vibrantSecond = second;
            int vibrantThird = third;
            byte* start_u8 = start;
            while ((start_u8[vibrantSecond] > 191 || start_u8[vibrantSecond] == start_u8[vibrantThird]) && (vibrantSecond + 1 < vibrantThird))
            {
                ++vibrantSecond;
            }
            if (start_u8[vibrantSecond] < 191)
            {
                second = vibrantSecond;
            }
            else
            {
                vibrantSecond = second;
            }
            while ((start_u8[vibrantFirst] > 191
                    || start_u8[vibrantFirst] == start_u8[vibrantSecond]
                    || start_u8[vibrantFirst] == start_u8[vibrantThird]) && (vibrantFirst + 1 < vibrantSecond))
            {
                ++vibrantFirst;
            }
            if (start_u8[vibrantFirst] < 191)
            {
                first = vibrantFirst;
            }
        }
        public static unsafe int FindAvx2(byte* input, int length, byte* value, int valueLength)
        {
            if (length < valueLength)
            {
                return -1;
            }
            if (valueLength == 1)
            {
                return FindByteAvx2(input, length, *value);
            }
            LocateNeedleAnomalies(value, valueLength, out int offsetFirst, out int offsetMid, out int offsetLast);
            Vector256<byte> valueFirstVector = Vector256.Create(value[offsetFirst]);
            Vector256<byte> valueMidVector = Vector256.Create(value[offsetMid]);
            Vector256<byte> valueLastVector = Vector256.Create(value[offsetLast]);
            while (length >= valueLength + 32)
            {
                Vector256<byte> inputFirstVector = Avx.LoadVector256(input + offsetFirst);
                Vector256<byte> inputMidVector = Avx.LoadVector256(input + offsetMid);
                Vector256<byte> inputLastVector = Avx.LoadVector256(input + offsetLast);
                int matches = BitOperations.TrailingZeroCount(
                    (uint)(Avx2.MoveMask(Avx2.CompareEqual(inputFirstVector, valueFirstVector)) &
                    Avx2.MoveMask(Avx2.CompareEqual(inputMidVector, valueMidVector)) &
                    Avx2.MoveMask(Avx2.CompareEqual(inputLastVector, valueLastVector))));
                while (matches != 0)
                {
                    int potentialOffset = BitOperations.TrailingZeroCount((uint)matches);
                    if (Equal(input + potentialOffset, value, valueLength))
                    {
                        return potentialOffset;
                    }
                    matches &= matches - 1;
                }
                input += 32;
                length -= 32;
            }
            return FindSerial(input, length, value, valueLength);
        }
    }
}