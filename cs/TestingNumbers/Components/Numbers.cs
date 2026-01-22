using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Extensions;

namespace TestingNumbers.Components {
    public static class Numbers {
        public static (BigInteger Min, BigInteger Max) Cross(BigInteger left, BigInteger right) {
            if (left == right) {
                return (left, right);
            }

            if (left < right) {
                return (left, right);
            }
            else {
                return (right, left);
            }
        }

        public static BigInteger Random(BigInteger min, BigInteger max) {
            return BigIntegerExtensions.Next(min, max);
        }
    }
}
