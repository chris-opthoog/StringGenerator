using CommandLine;
using System;
using System.Linq;

namespace StringGenerator.Client {
    class Program {
        static void Main(string[] args) {

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(opts => {
                       CryptoStringGenerator.GetNextBatch(opts.BatchSize, opts.Length, opts.UseSymbols)
                        .ToList()
                        .ForEach(x => Console.WriteLine(x));
                   });
        }
    }
}
