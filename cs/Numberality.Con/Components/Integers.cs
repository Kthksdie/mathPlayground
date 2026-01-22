using Microsoft.VisualBasic;
using Numberality.Con.Extensions;
using Numberality.Con.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Components {
    public enum Primality {
        Invalidate,
        Other,
        Prime
    }

    public struct Number {

        private BigInteger _value;

        public BigInteger Value { get { return _value; } }

        private Primality _primality = Primality.Invalidate;

        public Primality Primality { get { return _primality; } }

        public Number(BigInteger value) {
            _value = value;
            
            if (_value.IsProbablyPrime()) {
                _primality = Primality.Prime;
            }
            else {
                _primality = Primality.Other;
            }
        }

        public override string ToString() {
            return $"{Value}";
        }
    }

    public static class Integers {
        public static readonly char SqrtSymbol = '√';

        public static readonly int[] Digitis = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

        public static readonly int[] Digitas = [1, 2, 1, 4, 3, 2, 1, 2, 1, 2];

        public static readonly int[] Digitos = [1, 3, 3, 7, 7, 7, 7, 9, 9, 1];

        public static readonly List<BigInteger> MersennePrimeExponents = new List<BigInteger>() {
            2, 3, 5, 7, 
            13, 17, 19, 31, 61, 89, 
            107, 127, 521, 607, 
            1279, 2203, 2281, 3217, 4253, 4423, 9689, 9941, 
            11213, 19937, 21701, 23209, 44497, 86243, 
            110503, 132049, 216091, 756839, 859433, 
            1257787, 1398269, 2976221, 3021377, 6972593, 
            13466917, 20996011, 24036583, 25964951, 30402457, 32582657, 37156667, 42643801, 43112609, 57885161
        };

        public static BigInteger Random(BigInteger min, BigInteger max) {
            // Returns a random BigInteger that is within a specified range.
            // The lower bound is inclusive, and the upper bound is exclusive.
            // https://stackoverflow.com/a/68593532 - Theodor Zoulias

            if (min > max) {
                throw new ArgumentException();
            }
            else if (min == max) {
                return min;
            }

            var zeroBasedUpperBound = max - 1 - min; // Inclusive
            var bytes = zeroBasedUpperBound.ToByteArray();
            var lastByteMask = (byte)0b11111111; // Search for the most significant non-zero bit
            for (var mask = (byte)0b10000000; mask > 0; mask >>= 1, lastByteMask >>= 1) {
                if ((bytes[bytes.Length - 1] & mask) == mask) {
                    break; // We found it
                }
            }

            var result = BigInteger.Zero;
            while (true) {
                ThreadUtility.Random.Value.NextBytes(bytes);

                bytes[bytes.Length - 1] &= lastByteMask;

                result = new BigInteger(bytes);

                if (result <= zeroBasedUpperBound) {
                    return result + min;
                }
            }
        }

        public static BigInteger Random(int numberOfDigits, bool prime = false) {
            var min = BigInteger.Pow(10, numberOfDigits - 1);
            var max = (min * 10) - 1;

            var n = Random(min, max);
            if (prime) {
                while (!n.IsProbablyPrime()) {
                    n = Random(min, max);
                }
            }

            return n;
        }

        public static IEnumerable<Number> Numbers() {
            var n = BigInteger.One;
            while (true) {
                yield return new Number(n);
                n++;
            }
        }

        public static IEnumerable<BigInteger> PrimeNumbers() {
            yield return 1; // Meh...
            yield return 2;
            yield return 3;
            yield return 5;
            yield return 7;

            var n = new BigInteger(11);
            var lastDigit = 1;
            
            while (true) {
                if (n.IsProbablyPrime()) {
                    yield return n;
                }

                n += Digitas[lastDigit];
                lastDigit = Digitos[lastDigit];
            }
        }

        public static List<Product> Products(BigInteger max) {
            var products = new List<Product>();

            for (BigInteger a = 1; a <= max; a++) {
                for (BigInteger b = a; b <= max; b++) {
                    products.Add(new Product(a, b));
                }
            }

            return products;
        }
    }
}
