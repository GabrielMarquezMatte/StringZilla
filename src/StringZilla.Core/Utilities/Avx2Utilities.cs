using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace StringZilla.Core.Utilities
{
    public static class Avx2Utilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Equal(ReadOnlySpan<byte> input, ReadOnlySpan<byte> value)
        {
            return input.SequenceEqual(value);
        }
        public static unsafe void FillAvx2(Span<byte> output, byte value)
        {
            Vector256<byte> valueVector = Vector256.Create(value);
            int length = output.Length;
            int count = Vector256<byte>.Count;
            while (length >= count)
            {
                fixed (byte* bytePtr = output)
                {
                    Avx.StoreAligned(bytePtr, valueVector);
                    output = output[count..];
                    length -= count;
                }
            }
            foreach (ref byte outputByte in output)
            {
                outputByte = value;
            }
        }
        public static unsafe int FindByteAvx2(ReadOnlySpan<byte> input, byte value)
        {
            Vector256<byte> valueVector = Vector256.Create(value);
            int count = Vector256<byte>.Count;
            while (input.Length >= count)
            {
                Vector256<byte> inputVector = Vector256.Create(input);
                int mask = Avx2.MoveMask(Avx2.CompareEqual(inputVector, valueVector));
                if (mask != 0)
                {
                    return BitOperations.TrailingZeroCount((uint)mask);
                }
                input = input[count..];
            }
            return input.IndexOf(value);
        }
        private static void LocateNeedleAnomalies(ReadOnlySpan<byte> value, out int offsetFirst, out int offsetMid, out int offsetLast)
        {
            offsetFirst = 0;
#pragma warning disable SS003 // The operands of a divisive expression are both integers and result in an implicit rounding.
            offsetMid = value.Length / 2;
#pragma warning restore SS003 // The operands of a divisive expression are both integers and result in an implicit rounding.
            offsetLast = value.Length - 1;
            bool hasDuplicates = value[offsetFirst] == value[offsetMid] || value[offsetMid] == value[offsetLast] || value[offsetFirst] == value[offsetLast];
            if (value.Length > 3 && hasDuplicates)
            {
                while (value[offsetMid] == value[offsetFirst] && offsetMid + 1 < offsetLast) { ++offsetMid; }
                while ((value[offsetLast] == value[offsetMid] || value[offsetLast] == value[offsetFirst]) && offsetLast > (offsetMid + 1)) { --offsetLast; }
            }
            if (value.Length <= 8)
            {
                return;
            }
            int vibrantFirst = offsetFirst;
            int vibrantMid = offsetMid;
            int vibrantLast = offsetLast;
            while ((value[vibrantMid] > 191 || value[vibrantMid] == value[vibrantLast]) && (vibrantMid + 1 < vibrantLast))
            {
                ++vibrantMid;
            }
            if (value[vibrantMid] < 191)
            {
                offsetMid = vibrantMid;
            }
            else
            {
                vibrantMid = offsetMid;
            }
            while ((value[vibrantFirst] > 191
                    || value[vibrantFirst] == value[vibrantMid]
                    || value[vibrantFirst] == value[vibrantLast]) && (vibrantFirst + 1 < vibrantMid))
            {
                ++vibrantFirst;
            }
            if (value[vibrantFirst] < 191)
            {
                offsetFirst = vibrantFirst;
            }
        }
        public static int FindAvx2(ReadOnlySpan<byte> input, ReadOnlySpan<byte> value)
        {
            if (input.Length < value.Length)
            {
                return -1;
            }
            if (value.Length == 1)
            {
                return FindByteAvx2(input, value[0]);
            }
            LocateNeedleAnomalies(value, out int offsetFirst, out int offsetMid, out int offsetLast);
            Vector256<byte> valueFirstVector = Vector256.Create(value[offsetFirst]);
            Vector256<byte> valueMidVector = Vector256.Create(value[offsetMid]);
            Vector256<byte> valueLastVector = Vector256.Create(value[offsetLast]);
            int count = Vector256<byte>.Count;
            while (input.Length >= count)
            {
                Vector256<byte> inputFirstVector = Vector256.Create(input[offsetFirst..]);
                Vector256<byte> inputMidVector = Vector256.Create(input[offsetMid..]);
                Vector256<byte> inputLastVector = Vector256.Create(input[offsetLast..]);
                int matches = BitOperations.TrailingZeroCount(
                    (uint)(Avx2.MoveMask(Avx2.CompareEqual(inputFirstVector, valueFirstVector)) &
                    Avx2.MoveMask(Avx2.CompareEqual(inputMidVector, valueMidVector)) &
                    Avx2.MoveMask(Avx2.CompareEqual(inputLastVector, valueLastVector))));
                while (matches != 0)
                {
                    int index = BitOperations.TrailingZeroCount((uint)matches);
                    if (Equal(input[index..], value))
                    {
                        return index;
                    }
                    matches &= matches - 1;
                }
                input = input[count..];
            }
            return input.IndexOf(value);
        }
    }
}