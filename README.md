# StringGenerator

Use StringGenerator to generate random strings of specified length. The library uses MS cryptographic providers to produce true random selection of characters over the defined alphabet.

## Usage

### Single Random String

Generate a single random string using the default for length and inclusion of symbols.

```
var randomString = new CryptoStringGenerator().Next();
```
