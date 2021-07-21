# StringGenerator

Use StringGenerator to generate random strings of specified length. The library uses MS cryptographic providers to produce true random selection of characters over the defined alphabet.

## Status

![StringGenerator](https://github.com/chris-opthoog/StringGenerator/actions/workflows/dotnet.yml/badge.svg)

## Usage

### Single Random String

Generate a single random string using the default for length (32) and inclusion of symbols (true).

```
var randomString = new CryptoStringGenerator().Next();
```

## NuGet

NuGet package; https://www.nuget.org/packages/StringGenerator/
