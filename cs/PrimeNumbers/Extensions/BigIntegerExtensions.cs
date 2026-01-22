using PrimeNumbers.Extensions;
using PrimeNumbers.Helpers;
using PrimeNumbers.Entities;
using PrimeNumbers.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Fractions;

namespace PrimeNumbers.Extensions {
    public static class BigIntegerExtensions {
        private static Random _random = new Random();

        public static BigDecimal Average(this List<BigDecimal> numbers) {
            var sum = new BigDecimal();
            foreach (var number in numbers) {
                sum += number;
            }

            return numbers.Count / sum;
        }

        public static BigDecimal Average(this List<BigInteger> numbers) {
            var sum = new BigDecimal();
            foreach (var number in numbers) {
                sum += new BigDecimal(number, 0);
            }

            return numbers.Count / sum;
        }

        public static BigInteger AsRandom(this BigInteger number) {
            var bytes = new Byte[number.ToByteArray().LongLength];
            _random.NextBytes(bytes);

            var result = new BigInteger(bytes);

            return result * result.Sign;
        }

        public static BigInteger ToRandom(this BigInteger number, int maxBytes = 4) {
            var bytes = new Byte[_random.Next(1, maxBytes)];
            _random.NextBytes(bytes);

            var result = new BigInteger(bytes);

            return result * result.Sign;
        }

        public static BigInteger UniqueCount(this BigInteger number) {
            var uniques = new List<BigInteger>();

            while (number != 0) {
                var digitValue = number % 10;

                if (!uniques.Contains(digitValue)) {
                    uniques.Add(digitValue);
                }

                number /= 10;
            }

            return uniques.Count;
        }

        public static string ToOddEvenString(this BigInteger number) {
            var result = new StringBuilder();

            while (number != 0) {
                var digitValue = number % 10;

                if (digitValue.IsEven) {
                    result.Append('1');
                }
                else {
                    result.Append('0');
                }

                number /= 10;
            }

            return result.ToString();
        }

        /// <summary>
        /// A number of 4321 returns a List as [4, 3, 2, 1]
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static List<BigInteger> Digits(this BigInteger number) {
            var digits = new List<BigInteger>();

            while (number != 0) {
                var digitValue = number % 10;
                digits.Add(digitValue);

                number /= 10;
            }

            digits.Reverse();

            return digits;
        }

        public static BigFraction Zeta(this BigInteger s, BigInteger iterations) {

            var sum = new BigFraction(1);
            for (int i = 0; i < iterations; i++) {
                var d = BigInteger.Pow(i, (int)s);
                var v = new BigFraction(1, d);

                sum += v;

            }

            return sum;
        }

        public static BigInteger Pow(BigInteger value, BigInteger exponent) {
            BigInteger originalValue = value;
            while (exponent-- > 1)
                value = BigInteger.Multiply(value, originalValue);
            return value;
        }

        public static string ToSuperscript(this BigInteger number) {
            var values = new List<char>();

            while (number != 0) {
                var digit = (int)(number % 10);

                switch (digit) {
                    case 0:
                        values.Add('\u2070');
                        break;
                    case 1:
                        values.Add('\u00B9');
                        break;
                    case 2:
                        values.Add('\u00B2');
                        break;
                    case 3:
                        values.Add('\u00B3');
                        break;
                    case 4:
                        values.Add('\u2074');
                        break;
                    case 5:
                        values.Add('\u2075');
                        break;
                    case 6:
                        values.Add('\u2076');
                        break;
                    case 7:
                        values.Add('\u2077');
                        break;
                    case 8:
                        values.Add('\u2078');
                        break;
                    case 9:
                        values.Add('\u2079');
                        break;
                    default:
                        break;
                }

                number /= 10;
            }

            values.Reverse();

            var value = string.Join("", values);
            if (value.Length >= 32) {
                value = $"{value.Left(7)}····{value.Right(7)}";
            }

            return value;
        }

        public static string ToSubscript(this BigInteger number) {
            var values = new List<char>();

            while (number != 0) {
                var digit = (int)(number % 10);

                switch (digit) {
                    case 0:
                        values.Add('\u2080');
                        break;
                    case 1:
                        values.Add('\u2081');
                        break;
                    case 2:
                        values.Add('\u2082');
                        break;
                    case 3:
                        values.Add('\u2083');
                        break;
                    case 4:
                        values.Add('\u2084');
                        break;
                    case 5:
                        values.Add('\u2085');
                        break;
                    case 6:
                        values.Add('\u2086');
                        break;
                    case 7:
                        values.Add('\u2087');
                        break;
                    case 8:
                        values.Add('\u2088');
                        break;
                    case 9:
                        values.Add('\u2089');
                        break;
                    default:
                        break;
                }

                number /= 10;
            }

            values.Reverse();

            var value = string.Join("", values);
            if (value.Length >= 32) {
                value = $"{value.Left(7)}····{value.Right(7)}";
            }

            return value;
        }

        public static ICollection<ICollection<T>> Variations<T>(this IEnumerable<T> source) {
            if (null == source)
                throw new ArgumentNullException(nameof(source));

            T[] data = source.ToArray();

            var combinations = Enumerable
              .Range(0, 1 << (data.Length))
              .Select(index => data
                 .Where((v, i) => (index & (1 << i)) != 0)
                 .ToArray()).ToList();

            return (ICollection<ICollection<T>>)combinations.Permutations();
        }

        public static IEnumerable<T[]> Combinations<T>(this IEnumerable<T> source) {
            if (null == source)
                throw new ArgumentNullException(nameof(source));

            T[] data = source.ToArray();

            return Enumerable
              .Range(0, 1 << (data.Length))
              .Select(index => data
                 .Where((v, i) => (index & (1 << i)) != 0)
                 .ToArray());
        }

        public static ICollection<ICollection<T>> Permutations<T>(this ICollection<T> list) {
            var result = new List<ICollection<T>>();
            if (list.Count == 1) { // If only one possible permutation
                result.Add(list); // Add it and return it
                return result;
            }
            foreach (var element in list) { // For each element in that list
                var remainingList = new List<T>(list);
                remainingList.Remove(element); // Get a list containing everything except of chosen element
                foreach (var permutation in Permutations<T>(remainingList)) { // Get all possible sub-permutations
                    permutation.Add(element); // Add that element
                    result.Add(permutation);
                }
            }
            return result;
        }

        public static BigInteger FirstHalf(this BigInteger value) {
            if (value < 10000000) {
                return value;
            }

            var valueString = value.ToString();

            var firstHalfLength = (int)(valueString.Length / 2);
            var firstHalf = valueString.Substring(0, firstHalfLength);

            return BigInteger.Parse(firstHalf);
        }

        public static BigInteger LastHalf(this BigInteger value) {
            if (value < 10000000) {
                return value;
            }

            var valueString = value.ToString();

            var firstHalfLength = (int)(valueString.Length / 2);
            var secondHalfLength = valueString.Length - firstHalfLength;
            var lastHalf = valueString.Substring(firstHalfLength, secondHalfLength);

            return BigInteger.Parse(lastHalf);
        }

        public static BigInteger FirstDigit(this BigInteger number) {
            while (number >= 10) {
                number = (number - (number % 10)) / 10;
            }

            return number;
        }

        public static BigInteger LastDigit(this BigInteger number) {
            return number % 10;
        }

        public static BigInteger ReduceSize(this BigInteger value, int maxNumberOfDigits) {
            var numberOfDigits = value.NumberOfDigits();
            if (numberOfDigits <= maxNumberOfDigits) {
                return value;
            }

            var valueString = value.ToString();
            var valueLength = valueString.Length;
            var startIndex = valueLength - maxNumberOfDigits;
            var endIndex = valueLength - startIndex;

            var valueResized = valueString.Substring(startIndex, endIndex);

            return BigInteger.Parse(valueResized);
        }

        public static List<DigitCount> CountOfDigits(this BigInteger value) {
            var valueString = value.ToString();
            var valueStringLength = valueString.Length;

            var digitCounts = new List<DigitCount>();
            foreach (var c in valueString) {
                var v = Convert.ToInt32($"{c}");

                var digitCount = digitCounts.FirstOrDefault(x => x.Digit == v);
                if (digitCount == null) {
                    digitCount = new DigitCount();
                    digitCounts.Add(digitCount);
                }

                digitCount.Digit = v;
                digitCount.Count++;

                digitCount.Percent = valueStringLength / (double)digitCount.Count;
            }

            for (int i = 0; i < 10; i++) {
                if (digitCounts.Any(x => x.Digit == i)) {
                    continue;
                }

                digitCounts.Add(new DigitCount() {
                    Digit = i,
                    Count = 0,
                    Percent = 0
                });
            }

            digitCounts = digitCounts.OrderBy(x => x.Digit).ToList();

            return digitCounts;
        }

        public static string ToPrettyString(this BigInteger value) {
            var displayValue = $"{value}";

            if (displayValue.Length >= 32) {
                displayValue = $"{displayValue.Left(14)}....{displayValue.Right(14)}";
            }

            return displayValue;
        }

        public static bool IsPrime(this BigInteger number) {

            var divisor = number.Sqrt();
            while (divisor > 2) {
                divisor -= 2;

                if (BigInteger.Remainder(number, divisor) == 0) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// All non-standard checks.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PrimeResult IsLikelyPrime(this BigInteger value) {
            if (value < 1 || value == 2) {
                return new PrimeResult() {
                    IsPrime = false,
                    FailedAt = "X01"
                };
            }
            else if (value == 1 || value == 3 || value == 7) {
                return new PrimeResult() {
                    IsPrime = true,
                    FailedAt = "X02"
                };
            }

            if (value.IsEven) {
                return new PrimeResult() {
                    IsPrime = false,
                    FailedAt = "X03"
                };
            }

            return null;
        }

        /// <summary>
        /// Miller-Rabin
        /// </summary>
        /// <param name="value"></param>
        /// <param name="witnesses"></param>
        /// <returns></returns>
        public static PrimeResult IsProbablyPrime(this BigInteger value, int witnesses = 10) {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            if (witnesses <= 0) {
                witnesses = 10;
            }

            var isLikelyPrime = value.IsLikelyPrime();
            if (isLikelyPrime != null) {
                isLikelyPrime.Duration = stopWatch.Elapsed;
                return isLikelyPrime;
            }

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0) {
                d /= 2;
                s += 1;
            }

            Byte[] bytes = new Byte[value.ToByteArray().LongLength];
            BigInteger a;

            for (int i = 0; i < witnesses; i++) {
                do {
                    InfiniteParallel.Gen.NextBytes(bytes);

                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= (value - 2));

                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == (value - 1)) {
                    continue;
                }

                for (int r = 1; r < s; r++) {
                    x = BigInteger.ModPow(x, 2, value);

                    if (x == 1) {
                        return new PrimeResult() {
                            IsPrime = false,
                            Duration = stopWatch.Elapsed,
                            FailedAt = "X07"
                        };
                    }

                    if (x == (value - 1)) {
                        break;
                    }
                }

                if (x != (value - 1)) {
                    return new PrimeResult() {
                        IsPrime = false,
                        Duration = stopWatch.Elapsed,
                        FailedAt = "X08"
                    };
                }
            }

            return new PrimeResult() {
                IsPrime = true,
                Duration = stopWatch.Elapsed,
                FailedAt = ""
            };
        }

        public static PrimeResult IsProbablyPrime_Parallel(this BigInteger value, int witnesses = 10, int maxDegreeOfParallelism = 1) {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            if (witnesses <= 0) {
                witnesses = 10;
            }

            var isLikelyPrime = value.IsLikelyPrime();
            if (isLikelyPrime != null) {
                isLikelyPrime.Duration = stopWatch.Elapsed;
                return isLikelyPrime;
            }

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0) {
                d /= 2;
                s += 1;
            }

            Byte[] bytes = new Byte[value.ToByteArray().LongLength];

            var primeResult = new PrimeResult() {
                IsPrime = true
            };

            var parallelOptions = new ParallelOptions() {
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };

            Parallel.For(0, witnesses, parallelOptions, (i, state) => {
                BigInteger a;

                do {
                    InfiniteParallel.Gen.NextBytes(bytes);

                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= (value - 2));

                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == (value - 1)) {
                    return;
                }

                for (int r = 1; r < s; r++) {
                    x = BigInteger.ModPow(x, 2, value);

                    if (x == 1) {
                        primeResult = new PrimeResult() {
                            IsPrime = false,
                            Duration = stopWatch.Elapsed,
                            FailedAt = "X07"
                        };

                        state.Break();
                        return;
                    }

                    if (x == (value - 1)) {
                        break;
                    }
                }

                if (x != (value - 1)) {
                    primeResult = new PrimeResult() {
                        IsPrime = false,
                        Duration = stopWatch.Elapsed,
                        FailedAt = $"X08: {x}"
                    };

                    state.Break();
                    return;
                }
            });

            primeResult.Duration = stopWatch.Elapsed;
            return primeResult;
        }

        public static int NumberOfDigits(this BigInteger number) {
            if (number == 0) {
                return 1;
            }

            int numberOfDigits = 0;
            while (number != 0) {
                number = number / 10;
                numberOfDigits++;
            }

            return numberOfDigits;
        }

        public static BigInteger SumOfDigits(this BigInteger value) {
            var sum = new BigInteger(0);
            while (value != 0) {
                var digitValue = value % 10;
                sum += digitValue;
                value /= 10;
            }

            return sum;
        }

        public static BigInteger AlternateSumOfDigits(this BigInteger value) {
            var sum = new BigInteger();

            var flipFlop = new FlipFlop();
            while (value != 0) {
                var digitValue = value % 10;

                if (flipFlop.Value()) {
                    sum += digitValue;
                }
                else {
                    sum -= digitValue;
                }

                value /= 10;
            }

            return sum;
        }

        public static string ToBaseString(this BigInteger value, int b) {
            var baseChars = new List<char>();

            switch (b) {
                case -10:
                    baseChars = new List<char>() {
                        //'9', '8', '7', '6', '5', '4', '3', '2', '1', '0'
                        '9', '8', '7', '6', '5', '4', '3', '2', '1', '0'
                    };
                    break;
                case 2:
                    baseChars.AddRange(Enumerable.Range('0', 2).Select(x => (char)x).ToList());
                    break;
                case 12:
                    baseChars.AddRange(Enumerable.Range('0', 10).Select(x => (char)x).ToList());
                    baseChars.AddRange(Enumerable.Range('A', 2).Select(x => (char)x).ToList());
                    break;
                case 16:
                    baseChars.AddRange(Enumerable.Range('0', 10).Select(x => (char)x).ToList());
                    baseChars.AddRange(Enumerable.Range('A', 6).Select(x => (char)x).ToList());
                    break;
                case 26:
                    baseChars.AddRange(Enumerable.Range('A', 26).Select(x => (char)x).ToList());
                    break;
                case 36:
                    baseChars.AddRange(Enumerable.Range('0', 10).Select(x => (char)x).ToList());
                    baseChars.AddRange(Enumerable.Range('A', 26).Select(x => (char)x).ToList());
                    break;
                case 52:
                    baseChars.AddRange(Enumerable.Range('A', 26).Select(x => (char)x).ToList());
                    baseChars.AddRange(Enumerable.Range('a', 26).Select(x => (char)x).ToList());
                    break;
                case 62:
                    baseChars.AddRange(Enumerable.Range('0', 10).Select(x => (char)x).ToList());
                    baseChars.AddRange(Enumerable.Range('A', 26).Select(x => (char)x).ToList());
                    baseChars.AddRange(Enumerable.Range('a', 26).Select(x => (char)x).ToList());
                    break;
                case 79:
                    baseChars.AddRange(Enumerable.Range('0', 79).Select(x => (char)x).ToList());
                    break;
                case 100:
                    baseChars.AddRange(Enumerable.Range('1', 9).Select(x => (char)x).ToList());
                    break;
                case 101:
                    baseChars.AddRange(Enumerable.Range('A', 10).Select(x => (char)x).ToList());
                    break;
                default:
                    baseChars.AddRange(Enumerable.Range('0', b).Select(x => (char)x).ToList());
                    break;
            }

            string result = string.Empty;
            int targetBase = baseChars.Count;

            do {
                var index = value % targetBase;

                result = baseChars.FirstOrDefault(x => baseChars.IndexOf(x) == index) + result;

                value = value / targetBase;
            }
            while (value > 0);

            return result;
        }

        public static string ToBinaryString(this BigInteger bigint) {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base2 = new StringBuilder(bytes.Length * 8);

            // Convert first byte to binary.
            var binary = Convert.ToString(bytes[idx], 2);

            // Ensure leading zero exists if value is positive.
            //if (binary[0] != '0' && bigint.Sign == 1) {
            //    base2.Append('0');
            //}

            // Append binary string to StringBuilder.
            base2.Append(binary);

            // Convert remaining bytes adding leading zeros.
            for (idx--; idx >= 0; idx--) {
                base2.Append(Convert.ToString(bytes[idx], 2).PadLeft(8, '0'));
            }

            return base2.ToString();
        }

        public static BigInteger Sqrt(this BigInteger n) {
            if (n == 0) return BigInteger.Parse("0");
            if (n > 0) {
                int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(n, 2)));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!n.IsSqrt(root)) {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }

        public static bool IsSqrt(this BigInteger n, BigInteger squareRoot) {
            BigInteger lowerBound = squareRoot * squareRoot;
            BigInteger upperBound = (squareRoot + 1) * (squareRoot + 1);

            return (n >= lowerBound && n < upperBound);
        }

        public static BigInteger ParseBinary(this string valueString) {
            byte[] raw;

            int rawLength;
            int rawPosition;
            int bitStart = 0;

            // Calculate the total number of bytes we'll need to store the 
            // result. Remember that 10 bits / 8 = 1.25 bytes --> 2 bytes. 
            rawLength = (int)Math.Ceiling(valueString.Length / 8.0);

            // Force BigInteger to interpret our array as an unsigned value. Leave
            // an unused byte at the end of our array.
            raw = new byte[rawLength + 1];

            rawPosition = rawLength - 1;

            // Lets assume we have the string 10 1111 0101
            // Lets parse that odd chunk '10' first, and then we can parse the rest on nice
            // and simple 8-bit bounderies. Keep in mind that the '10' chunk occurs at indicies 
            // 0 and 1, but represent our highest order bits.
            if (rawLength * 8 != valueString.Length) {
                int leftoverBits = valueString.Length % 8;

                raw[rawPosition] = ParseChunk(valueString, 0, leftoverBits);
                rawPosition--;
                bitStart += leftoverBits;
            }

            // Parse all of the 8-bit chunks that we can.
            for (int i = bitStart; i < valueString.Length; i += 8) {
                raw[rawPosition] = ParseChunk(valueString, i, 8);
                rawPosition--;
            }

            return new BigInteger(raw);
        }

        private static byte ParseChunk(string valueString, int startPosition, int run) {
            byte result = 0;
            byte temp;

            for (int i = 0; i < run; i++) {
                // Abuse the unicode ordering of characters.
                temp = (byte)(valueString[startPosition + i] - '0');
                result |= (byte)(temp << run - i - 1);
            }

            return result;
        }
    }
}
