﻿using CommandLine;

namespace StringGenerator.Client {
    public class Options {
        [Option('b', "batch", Required = false, Default = 1, HelpText = "The number of passwords to generate.")]
        public int BatchSize { get; set; }

        [Option('l', "length", Required = false, Default = 64, HelpText = "The length of the passwords to generate.")]
        public int Length { get; set; }

        [Option('s', "symbols", HelpText = "Use symbols in password (or not). If this switch is present then symbols will be used.")]
        public bool UseSymbols { get; set; }
    }
}
