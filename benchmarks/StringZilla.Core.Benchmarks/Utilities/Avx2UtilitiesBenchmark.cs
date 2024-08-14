using BenchmarkDotNet.Attributes;
using StringZilla.Core.Utilities;

namespace StringZilla.Core.Benchmarks.Utilities
{
    public class Avx2UtilitiesBenchmark
    {
        [Benchmark]
        public unsafe void FillAvx2Benchmark()
        {
            Span<byte> output = stackalloc byte[1024];
            Avx2Utilities.FillAvx2(output, 0x42);
        }
        [Benchmark]
        public unsafe void FillSerialBenchmark()
        {
            foreach (ref var item in (stackalloc byte[1024]))
            {
                item = 0x42;
            }
        }
        [Benchmark]
        public int FindByteAvx2Benchmark()
        {
            Span<byte> input = stackalloc byte[1024];
            for (int i = 0; i < 1024; i++)
            {
                input[i] = (byte)i;
            }
            return Avx2Utilities.FindByteAvx2(input, input[1023]);
        }
        [Benchmark]
        public int FindByteSerialBenchmark()
        {
            Span<byte> input = stackalloc byte[1024];
            for (int i = 0; i < 1024; i++)
            {
                input[i] = (byte)i;
            }
            for (int i = 0; i < 1024; i++)
            {
                if (input[i] == input[1023])
                {
                    return i;
                }
            }
            return -1;
        }
        [Benchmark]
        public int FindAvx2Benchmark()
        {
            Span<byte> input = stackalloc byte[1024];
            for (int i = 0; i < 1024; i++)
            {
                input[i] = (byte)i;
            }
            ReadOnlySpan<byte> value = input[^16..];
            return Avx2Utilities.FindAvx2(input, value);
        }
        [Benchmark]
        public int FindSerialBenchmark()
        {
            Span<byte> input = stackalloc byte[1024];
            for (int i = 0; i < 1024; i++)
            {
                input[i] = (byte)i;
            }
            ReadOnlySpan<byte> value = input[^16..];
            return input.IndexOf(value);
        }
    }
}