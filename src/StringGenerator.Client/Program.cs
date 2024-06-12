using CommandLine;
using System;

namespace StringGenerator.Client
{
    class Program {
        static void Main(string[] args) {

            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed(opts => {
                        var gen = new CryptoStringGenerator();
                        foreach (var s in gen.NextBatch(opts.BatchSize, opts.Length, opts.UseSymbols))
                        {
                            Console.WriteLine(s);
                        }
                   });
        }
    }
}
