using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringGenerator {
    public class PseudoRandomStringGenerator : StringGeneratorBase, IDisposable {
        private bool disposedValue;
        private Random _rng;

        public PseudoRandomStringGenerator() : base() {
            _rng = new Random(DateTime.Now.Millisecond);
        }

        public override string Next(int length = 32, bool useSymbols = false) {
            if (length <= 0) {
                throw new ArgumentException("Length must be greater that zero", nameof(length));
            }

            var sb = new StringBuilder(length);
            int charSpace = ALPHA_SIZE + NUM_SIZE + (useSymbols ? SYMBOL_SIZE : 0);

            

            for (var i = 0u; i < length; i++) {
                var index = _rng.Next(0, charSpace);
                sb.Append(ALPHABET[index]);
            }

            return sb.ToString();
        }

        public override IEnumerable<string> NextBatch(int batchLength = 1, int length = 32, bool useSymbols = false) {
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
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PseudoRandomStringGenerator()
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
