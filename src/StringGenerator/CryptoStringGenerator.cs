using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace StringGenerator {

    public class CryptoStringGenerator : StringGeneratorBase {


        public CryptoStringGenerator() : base() {

        }

        public override string Next(int len = 32, bool useSymbols = true) {

            if (len <= 0) {
                throw new ArgumentException("Length must be greater that zero", nameof(len));
            }

            var sb = new StringBuilder(len);
            int charSpace = ALPHA_SIZE + NUM_SIZE + (useSymbols ? SYMBOL_SIZE : 0);

            using (var rng = RandomNumberGenerator.Create()) {

                var rndBytes = new byte[len];
                rng.GetBytes(rndBytes);

                for (var i = 0u; i < len; i++) {
                    int index = rndBytes[i] % charSpace;
                    sb.Append(ALPHABET[index]);
                }
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
    }
}
