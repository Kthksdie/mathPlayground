using ExtendedNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Extensions {
    public static class BigDecimalExtensions {
        public static BigDecimal Divide(BigDecimal dividend, BigDecimal divisor) {
            if (divisor == 0) {
                throw new DivideByZeroException("divisor");
            }

            int num = dividend.Exponent - divisor.Exponent;
            int num2 = 0;
            BigInteger remainder;
            BigInteger bigInteger = BigInteger.DivRem(dividend.Mantissa, divisor.Mantissa, out remainder);
            bool flag = true;
            BigInteger bigInteger2 = 0;

            while (remainder != 0L) {
                if (flag) {
                    flag = false;
                }
                else if (remainder == bigInteger2) {
                    if (bigInteger.GetNumberOfSignifigantDigits() >= divisor.SignifigantDigits) {
                        break;
                    }
                }
                else if (bigInteger.GetNumberOfSignifigantDigits() >= BigDecimal.Precision) {
                    break;
                }

                while (BigInteger.Abs(remainder) < BigInteger.Abs(divisor.Mantissa)) {
                    remainder *= (BigInteger)10;
                    bigInteger *= (BigInteger)10;
                    num2++;
                }

                bigInteger2 = remainder;
                bigInteger += BigInteger.DivRem(remainder, divisor.Mantissa, out remainder);
            }

            return new BigDecimal(bigInteger, num - num2);
        }
    }
}
