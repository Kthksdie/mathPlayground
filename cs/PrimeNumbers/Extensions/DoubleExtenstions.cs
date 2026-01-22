using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Extensions {
    public static class DoubleExtenstions {
        public static BigInteger RemoveLeftSide(this float number) {
            var numberString = number.ToString();

            if (numberString.Contains(".")) {
                var logNumberString = numberString.Split('.')[1];
                return BigInteger.Parse(logNumberString);
            }

            return 0;
        }

        public static BigInteger RemoveLeftSide(this double number) {
            var numberString = number.ToString();

            if (numberString.Contains(".")) {
                var logNumberString = numberString.Split('.')[1];
                return BigInteger.Parse(logNumberString);
            }

            return 0;
        }

        public static BigInteger RemoveLeftSide(this decimal number) {
            var numberString = number.ToString();

            if (numberString.Contains(".")) {
                var logNumberString = numberString.Split('.')[1];
                return BigInteger.Parse(logNumberString);
            }

            return 0;
        }

        public static float Truncate(this float value, int precision) {
            var step = (float)Math.Pow(10, precision);
            var tmp = (float)Math.Truncate(step * value);
            return tmp / step;
        }

        public static double Truncate(this double value, int precision) {
            var step = (double)Math.Pow(10, precision);
            var tmp = Math.Truncate(step * value);
            return tmp / step;
        }

        public static decimal Truncate(this decimal value, int precision) {
            var v = (decimal)value;
            var step = (decimal)Math.Pow(10, precision);
            var tmp = Math.Truncate(step * v);
            return tmp / step;
        }
    }
}
