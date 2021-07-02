using BenchmarkDotNet.Running;
using System;

namespace StringGenerator.Benchmarks {
    class Program {
        static void Main(string[] args) {
            _ = BenchmarkRunner.Run<GeneratorBenchmarks>();
        }
    }
}
