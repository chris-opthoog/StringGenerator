using System;
using System.Collections.Generic;

namespace StringGenerator {
    public abstract class StringGeneratorBase
    {
        public const string ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            "0123456789" +
            "!@#$%^&*()_+-=:;\"\\<>,?[]{}|~";

        protected const int ALPHA_SIZE = 26;
        protected const int NUM_SIZE = 10;
        protected const int SYMBOL_SIZE = 28;


        public int CharSpace => ALPHA_SIZE + ALPHA_SIZE + NUM_SIZE + SYMBOL_SIZE; 

        public StringGeneratorBase() {
            if (CharSpace != ALPHABET.Length) {
                throw new ApplicationException("Internal length representation does not match alphabet!");
            }
        }

        /// <summary>
        /// Generate a random string to specified length using lower case letters, upper case letters, numbers 0-9 and optionally symbols.
        /// </summary>
        /// <param name="length">Default is 32.</param>
        /// <param name="useSymbols">Default is False.</param>
        /// <returns></returns>
        public abstract string Next(int length = 32, bool useSymbols = false);

        /// <summary>
        /// Generate batches of strings.
        /// </summary>
        /// <param name="batchLength">Default is 1.</param>
        /// <param name="length">Default is 32.</param>
        /// <param name="useSymbols">Default is False.</param>
        /// <returns></returns>
        public abstract IEnumerable<string> NextBatch(int batchLength = 1, int length = 32, bool useSymbols = false);

        
    }
}
