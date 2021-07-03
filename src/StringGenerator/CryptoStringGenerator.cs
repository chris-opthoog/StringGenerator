using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace StringGenerator {

    public class CryptoStringGenerator : StringGeneratorBase, IDisposable {

        private readonly RandomNumberGenerator _rng;
        private bool disposedValue;

        public CryptoStringGenerator() : base() {
            _rng = RandomNumberGenerator.Create();
        }

        public static string GetNext(int length = 32, bool useSymbols = true) {
            using var g = new CryptoStringGenerator();
            var rs = g.Next(length, useSymbols);
            return rs;
        }

        public static IEnumerable<string> GetNextBatch(int batchLength = 1, int length = 32, bool useSymbols = true) {
            using var g = new CryptoStringGenerator();
            var batch = g.NextBatch(batchLength, length, useSymbols);
            return batch;
        }

        public override string Next(int length = 32, bool useSymbols = true) {

            if (length <= 0) {
                throw new ArgumentException("Length must be greater that zero", nameof(length));
            }

            var sb = new StringBuilder(length);
            int charSpace = ALPHA_SIZE + NUM_SIZE + (useSymbols ? SYMBOL_SIZE : 0);

            var rndBytes = new byte[length];
            _rng.GetBytes(rndBytes);

            for (var i = 0u; i < length; i++) {
                int index = rndBytes[i] % charSpace;
                sb.Append(ALPHABET[index]);
            }

            return sb.ToString();
        }

        public override IEnumerable<string> NextBatch(int batchLength = 1, int length = 32, bool useSymbols = true) {
            var rndStrings = new List<string>();

            for (var i = 0; i < batchLength; i++) {
                var s = Next(length, useSymbols);
                rndStrings.Add(s);
            }

            return rndStrings.AsEnumerable();
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: dispose managed state (managed objects)
                    _rng.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~CryptoStringGenerator()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
