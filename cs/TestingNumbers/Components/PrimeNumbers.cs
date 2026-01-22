using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Extensions;

namespace TestingNumbers.Components {
    public static class PrimeNumbers {
        public static BigInteger Random(int numberOfDigits, bool prime) {
            var min = BigInteger.Pow(10, numberOfDigits - 1);
            var max = (min * 10) - 1;

            var r = BigIntegerExtensions.Next(min, max);
            if (!prime) {
                while (r.IsProbablyPrime()) {
                    r = BigIntegerExtensions.Next(min, max);
                }

                return r;
            }

            while (!r.IsProbablyPrime()) {
                r = BigIntegerExtensions.Next(min, max);
            }

            return r;
        }

        public static IEnumerable<BigInteger> Get() {
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

                n += Constants.Digitas[lastDigit];
                lastDigit = Constants.Digitos[lastDigit];
            }
        }

        public static IEnumerable<BigInteger> Get(BigInteger start) {
            var lastDigit = (int)(start % 10);

            while (!start.IsProbablyPrime()) {
                start += Constants.Digitas[lastDigit];
                lastDigit = Constants.Digitos[lastDigit];
            }

            while (true) {
                if (start.IsProbablyPrime()) {
                    yield return start;
                }

                start += Constants.Digitas[lastDigit];
                lastDigit = Constants.Digitos[lastDigit];
            }
        }

        public static IEnumerable<BigInteger> PrimeSquares() {
            foreach (var p in Get()) {
                yield return p * p;
            }
        }

        public static IEnumerable<BigInteger> PrimeCubes() {
            foreach (var p in Get()) {
                yield return p * p * p;
            }
        }
    }
}
