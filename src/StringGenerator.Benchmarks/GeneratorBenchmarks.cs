using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringGenerator.Benchmarks {
    public class GeneratorBenchmarks {

        private PseudoRandomStringGenerator _pg;
        private CryptoStringGenerator _cg;

        [GlobalSetup]
        public void GlobalSetup() {
            _pg = new PseudoRandomStringGenerator();
            _cg = new CryptoStringGenerator();
        }

        [Benchmark(Baseline = true)]
        public string PseudoRnd() {

            var s = _pg.Next();
            return s;

        }

        [Benchmark]
        public string CryptoRnd() {

            var s = _cg.Next();
            return s;

        }

        [GlobalCleanup]
        public void GlobalCleanup() {
            _pg.Dispose();
            _cg.Dispose();
        }
    }
}
