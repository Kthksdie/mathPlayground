using PrimeNumbers.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Helpers {
    public static class BigIntegerManipulations {
        private static List<BigInteger> _specialPrimeDigits = new List<BigInteger>() {
            1, 3, 7, 9
        };

        public static double AsHarmonic(this BigInteger number) {
            var sum = 0d;

            while (number != 0) {
                var digitValue = number % 10;
                sum += 1 / (double)digitValue;
                number /= 10;
            }

            return sum;
        }

        public static BigInteger AsFactorial(this BigInteger number) {
            var result = new BigInteger(1);

            var digits = number.Digits();

            foreach (var digit in digits) {
                result *= digit;
            }

            return result;
        }

        public static BigInteger AsSumplier(this BigInteger number, int multiplier = 2) {
            var result = new BigInteger(1);

            var digits = number.Digits();

            foreach (var digit in digits) {
                result += digit * multiplier;
            }

            return result;
        }

        public static BigInteger AsSumponent(this BigInteger number, int exponent = 2) {
            var result = new BigInteger(1);

            var digits = number.Digits();

            foreach (var digit in digits) {
                result += BigInteger.Pow(digit, exponent);
            }

            return result;
        }

        public static BigInteger AsSumponplier(this BigInteger number, int exponent = 2, int multiplier = 2) {
            var result = new BigInteger(1);

            var digits = number.Digits();

            foreach (var digit in digits) {
                result += BigInteger.Pow(digit, exponent) * multiplier;
            }

            return result;
        }

        public static BigInteger AsSpecial(this BigInteger number) {
            var flipFlop = new FlipFlop();

            if (number.IsEven) {
                number = number - 1;

                if (number == -1) {
                    return 0;
                }
            }
            else if (number.LastDigit() == 0) {
                number = number + 1;
            }
            else if (number.LastDigit() == 5) {
                number = number + 2;
                //if (flipFlop.Value()) {
                //    number = number + 2;
                //}
                //else {
                //    number = number - 2;
                //}
            }

            return number;
        }

        public static BigInteger AsNextSpecial(this BigInteger number) {
            while (!_specialPrimeDigits.Contains(number.LastDigit())) {
                number++;
            }

            return number;
        }

        public static BigInteger Flip(this BigInteger number) {
            var values = new List<string>();

            if (number == 0) {
                values.Add($"0");
            }
            else {
                while (number != 0) {
                    var digit = (int)(number % 10);

                    values.Add($"{digit}");

                    number /= 10;
                }
            }

            //values.Reverse();

            return BigInteger.Parse(string.Join("", values));
        }

        public static BigInteger LogRemoveLeftSide(this BigInteger number) {
            var log = BigInteger.Log(number);
            var logString = log.ToString();

            if (logString.Contains(".")) {
                var logNumberString = logString.Split('.')[1];
                return BigInteger.Parse(logNumberString);
            }

            return 0;
        }

        public static BigInteger Log10RightSideOnly(this BigInteger number) {
            var log = BigInteger.Log10(number);
            var logString = log.ToString();

            if (logString.Contains(".")) {
                var logNumberString = logString.Split('.')[1];
                return BigInteger.Parse(logNumberString);
            }

            return 0;
        }

        public static BigInteger ToImperfect(this BigInteger number) {
            while (number.IsEven) {
                number++;
            }

            return number;
        }

        public static bool IsSpecial(this BigInteger number) {
            return _specialPrimeDigits.Contains(number % 10);
        }

        public static BigInteger RemoveLeadingZeros(this BigInteger number) {
            var numberOfDigits = number.NumberOfDigits();
            if (numberOfDigits == 1) {
                return number;
            }

            var numberString = number.ToString().Substring(1);
            while (!string.IsNullOrEmpty(numberString) && numberString[0] == '0') {
                numberString = numberString.Substring(1);
            }

            if (string.IsNullOrEmpty(numberString)) {
                return 0;
            }

            return BigInteger.Parse(numberString);
        }

        public static BigInteger RemoveTrailingZeros(this BigInteger number) {
            var numberOfDigits = number.NumberOfDigits();
            if (numberOfDigits == 1) {
                return number;
            }

            var numberString = number.ToString().Substring(1);
            while (!string.IsNullOrEmpty(numberString) && numberString[numberString.Length - 1] == '0') {
                numberString = numberString.Substring(0, numberString.Length - 1);
            }

            if (string.IsNullOrEmpty(numberString)) {
                return 0;
            }

            return BigInteger.Parse(numberString);
        }

        public static BigInteger SwitchLast(this BigInteger number) {
            var numberOfDigits = number.NumberOfDigits();
            if (numberOfDigits == 1) {
                return 0;
            }

            var numberString = number.ToString();
            var lastDigit = numberString[numberString.Length - 1];

            numberString = numberString.Remove(numberString.Length - 1, 1);
            numberString = $"{lastDigit}{numberString}";

            if (string.IsNullOrEmpty(numberString)) {
                return 0;
            }

            return BigInteger.Parse(numberString);
        }

        public static BigInteger SwitchUntilSpecial(this BigInteger number) {
            var numberOfDigits = number.NumberOfDigits();
            if (numberOfDigits == 1) {
                return number;
            }

            var numberString = number.ToString();

            var count = 0;
            while (true) {
                count++;

                numberString = number.SwitchLast().ToString();

                var lastDigitIndex = numberString.Length - 1;
                var lastDigit = BigInteger.Parse($"{numberString[numberString.Length - 1]}");

                numberString = numberString.Remove(lastDigitIndex, 1);
                numberString = $"{lastDigit}{numberString}";
                numberString = numberString.Remove(numberString.Length - 1, 1);

                if (string.IsNullOrEmpty(numberString)) {
                    break;
                }


                number = BigInteger.Parse(numberString);

                lastDigit = BigInteger.Parse($"{numberString[numberString.Length - 1]}");
                if (_specialPrimeDigits.Contains(lastDigit)) {
                    break;
                }

                //if (count >= numberOfDigits) {
                //    break;
                //}
            }

            if (string.IsNullOrEmpty(numberString)) {
                return 0;
            }

            return BigInteger.Parse(numberString);
        }
    }
}
