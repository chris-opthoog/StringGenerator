using System.Collections.Generic;

namespace StringGenerator {
    public abstract class StringGeneratorBase
    {
        public const string ALPHABET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+-=:;\"\\<>,?[]{}|~";

        /// <summary>
        /// Generate a random string to specified length using lower case letters, upper case letters, numbers 0-9 and optionally symbols.
        /// </summary>
        /// <param name="length">Default is 32.</param>
        /// <param name="useSymbols">Default is true.</param>
        /// <returns></returns>
        public abstract string Next(int length = 32, bool useSymbols = true);

        /// <summary>
        /// Generate batches of strings.
        /// </summary>
        /// <param name="batchLength">Default is 1.</param>
        /// <param name="length">Default is 32.</param>
        /// <param name="useSymbols">Default is true.</param>
        /// <returns></returns>
        public abstract IEnumerable<string> NextBatch(int batchLength = 1, int length = 32, bool useSymbols = true);

        
    }
}
