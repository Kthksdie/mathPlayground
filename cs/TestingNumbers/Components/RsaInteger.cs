using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Extensions;

namespace TestingNumbers.Components
{
    public class RsaInteger {
        public BigInteger N { get; set; }

        public BigInteger L { get; set; }

        public BigInteger R { get; set; }

        public RsaInteger() {

        }

        public RsaInteger(int leftLength, int rightLength) {
            var l = RsaInteger.Single(leftLength, prime: true);
            var r = RsaInteger.Single(rightLength, prime: true);

            if (l > r) {
                R = l;
                L = r;
            }
            else {
                R = r;
                L = l;
            }

            N = L * R;
        }

        public static BigInteger Single(int numberOfDigits, bool prime) {
            var min = BigInteger.Pow(10, numberOfDigits - 1);
            var max = (min * 10) - 1;

            var r = BigIntegerExtensions.Next(min, max);
            if (!prime) {
                return r;
            }

            while (!r.IsProbablyPrime()) {
                r = BigIntegerExtensions.Next(min, max);
            }

            return r;
        }

        public static IEnumerable<RsaInteger> Generate(int leftLength, int rightLength) {
            while (true) {
                yield return new RsaInteger(leftLength: leftLength, rightLength: rightLength);
            }
        }
    }
}
