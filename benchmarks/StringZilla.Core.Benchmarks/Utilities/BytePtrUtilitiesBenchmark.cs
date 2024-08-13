using BenchmarkDotNet.Attributes;
using StringZilla.Core.Utilities;

namespace StringZilla.Core.Benchmark.Utilities
{
    public class BytePtrUtilitiesBenchmark
    {
        [Benchmark]
        public unsafe void FillAvx2Benchmark()
        {
            byte* output = stackalloc byte[1024];
            BytePtrUtilities.FillAvx2(output, 1024, 0x42);
        }
        [Benchmark]
        public unsafe void FillSerialBenchmark()
        {
            byte* output = stackalloc byte[1024];
            for(int i = 0; i < 1024; i++)
            {
                output[i] = 0x42;
            }
        }
        [Benchmark]
        public unsafe int FindByteAvx2Benchmark()
        {
            byte* input = stackalloc byte[1024];
            for(int i = 0; i < 1024; i++)
            {
                input[i] = (byte)i;
            }
            return BytePtrUtilities.FindByteAvx2(input, 1024, input[1023]);
        }
        [Benchmark]
        public unsafe int FindByteSerialBenchmark()
        {
            byte* input = stackalloc byte[1024];
            for(int i = 0; i < 1024; i++)
            {
                input[i] = (byte)i;
            }
            for(int i = 0; i < 1024; i++)
            {
                if(input[i] == input[1023])
                {
                    return i;
                }
            }
            return -1;
        }
    }
}