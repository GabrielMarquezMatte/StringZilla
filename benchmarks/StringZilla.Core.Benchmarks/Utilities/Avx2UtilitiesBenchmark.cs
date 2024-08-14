using BenchmarkDotNet.Attributes;
using StringZilla.Core.Utilities;

namespace StringZilla.Core.Benchmarks.Utilities
{
    public class Avx2UtilitiesBenchmark
    {
        [Benchmark]
        public unsafe void FillAvx2Benchmark()
        {
            Span<byte> output = stackalloc byte[256];
            Avx2Utilities.FillAvx2(output, 0x42);
        }
        [Benchmark]
        public unsafe void FillSerialBenchmark()
        {
            foreach (ref var item in (stackalloc byte[256]))
            {
                item = 0x42;
            }
        }
        private static Span<byte> CreateSpan(Span<byte> output)
        {
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = (byte)(i % 256);
            }
            return output;
        }
        [Benchmark]
        public int FindByteAvx2Benchmark()
        {
            Span<byte> input = stackalloc byte[256];
            input = CreateSpan(input);
            return Avx2Utilities.FindByteAvx2(input, 255);
        }
        [Benchmark]
        public int FindByteSerialBenchmark()
        {
            Span<byte> input = stackalloc byte[256];
            input = CreateSpan(input);
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == 255)
                {
                    return i;
                }
            }
            return -1;
        }
        [Benchmark]
        public int FindAvx2Benchmark()
        {
            Span<byte> input = stackalloc byte[256];
            input = CreateSpan(input);
            ReadOnlySpan<byte> value = input[^16..];
            return Avx2Utilities.FindAvx2(input, value);
        }
        [Benchmark]
        public int FindSerialBenchmark()
        {
            Span<byte> input = stackalloc byte[256];
            input = CreateSpan(input);
            ReadOnlySpan<byte> value = input[^16..];
            return input.IndexOf(value);
        }
    }
}