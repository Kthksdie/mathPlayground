using Microsoft.VisualBasic;
using Numberality.Con.Components;
using Numberality.Con.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Extensions {
    public static class BigIntegerExtensions {
        public static double Phi = 1.6180339887498948482045868343656;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberOfDigits(this BigInteger n) {
            return n.Abs().ToString().Length; // Noticably Faster

            //var count = BigInteger.Zero;
            //while (n > 0) {
            //    n /= 10;
            //    count++;
            //}

            //return count;
        }

        public static BigInteger Weight(this BigInteger n) {
            var result = BigInteger.Zero;
            while (n > 0) {
                var digit = n % 10;

                if (digit >= 5) {
                    result++;
                }
                else if (digit < 5) {
                    result--;
                }

                n /= 10;
            }

            return result;
        }

        public static (BigInteger X, BigInteger Y) Pos(this BigInteger n, BigInteger modulus) {
            var x = n % modulus;
            var y = modulus - x.Abs();

            if (n > 0) {
                return (-x, y);
            }
            else {
                return (-y, x); // brain hurty
            }
        }

        public static BigInteger S(this BigInteger n, BigInteger k) {
            var result = BigInteger.Zero;
            while (n > k) {
                n /= k;
                result++;
            }

            return result;
        }

        public static BigInteger ModLeft(this BigInteger n, BigInteger k) {
            return n - (n % k);
        }

        public static BigInteger ModRight(this BigInteger n, BigInteger k) {
            return (n - (n % k)) + k;
        }

        public static double ToDouble(this BigInteger n) {
            return double.Parse(n.ToString());
        }

        public static decimal ToDecimal(this BigInteger n) {
            return decimal.Parse(n.ToString());
        }

        public static IEnumerable<PowerOf> Factorize(this BigInteger n) {
            if (n == 0) {
                yield break;
            }

            var powerOf = new PowerOf(n, 1);

            yield return powerOf;

            if (n % 2 == 0) {
                powerOf = new PowerOf(2, 0);
                while (n % 2 == 0) {
                    n /= 2;
                    powerOf.Exponent++;
                }

                if (powerOf.Exponent > 1) {
                    yield return powerOf;
                }
                else {
                    yield break;
                }

                if (n.IsProbablyPrime()) {
                    //yield return new PowerOf(n, 1);
                    yield break;
                }
            }

            for (BigInteger p = 3; p <= 7; p += 2) {
                if (n % p == 0) {
                    powerOf = new PowerOf(p, 0);
                    while (n % p == 0) {
                        n /= p;
                        powerOf.Exponent++;
                    }

                    if (powerOf.Exponent > 1) {
                        yield return powerOf;
                    }
                    else {
                        yield break;
                    }

                    if (n.IsProbablyPrime()) {
                        //yield return new PowerOf(n, 1);
                        yield break;
                    }
                }
            }

            var k = new BigInteger(11);
            var lastDigit = 1;
            while (n >= k) {
                if (n % k == 0) {
                    powerOf = new PowerOf(k, 0);
                    while (n % k == 0) {
                        n /= k;
                        powerOf.Exponent++;
                    }

                    if (powerOf.Exponent > 1) {
                        yield return powerOf;
                    }
                    else {
                        yield break;
                    }

                    if (n.IsProbablyPrime()) {
                        //yield return new PowerOf(n, 1);
                        yield break;
                    }
                }

                k += Integers.Digitas[lastDigit];
                lastDigit = Integers.Digitos[lastDigit];
            }
        }

        public static BigInteger Divide(this BigInteger n, double k) {
            var s = k.ToString().Replace(".", string.Empty);
            var right = BigInteger.Parse(s);
            var t = BigInteger.Pow(10, s.Length - 1);

            return BigInteger.Divide(n * t, right);
        }

        public static BigInteger Multiply(this BigInteger n, double k) {
            var s = k.ToString().Replace(".", string.Empty);
            var right = BigInteger.Parse(s);
            var t = BigInteger.Pow(10, s.Length - 1);

            return BigInteger.Multiply(n * t, right) / BigInteger.Pow(t, 2);
        }

        public static BigInteger Gcd(this BigInteger a, BigInteger b) {
            if (b > a) {
                (a, b) = (b, a);
            }

            while (true) {
                var c = BigInteger.DivRem(a, b);

                a = b;
                b = c.Remainder;

                if (c.Remainder == 0) {
                    break;
                }
            }

            return a;
        }

        public static BigInteger Gcd2(this BigInteger n, BigInteger k) {
            return BigInteger.GreatestCommonDivisor(n, k);
        }

        public static BigInteger Abs(this BigInteger n) {
            return BigInteger.Abs(n);
        }

        public static BigInteger IntegerCbrt(this BigInteger n) {
            var k = BigInteger.One;

            while (BigInteger.Pow(k, 3) < n) {
                k++;
            }

            return k;
        }

        public static BigInteger IntegerNthrt(this BigInteger n, int root) {
            var k = BigInteger.One;

            while (BigInteger.Pow(k, root) < n) {
                k++;
            }

            return k;
        }

        public static BigInteger IntegerSqrt(this BigInteger n, SqrtMethod method = SqrtMethod.NewtonPlus) {
            if (method == SqrtMethod.NewtonPlus) {
                return NewtonPlusSqrt(n); // Fastest
            }
            else if (method == SqrtMethod.Steiner) {
                return SteinerSqrt(n); // Normal?
            }
            else if (method == SqrtMethod.JeremyKahan) {
                return JeremyKahanSqrt(n); // Slowest?
            }

            throw new NotImplementedException();
        }

        public static BigInteger JeremyKahanSqrt(BigInteger n) {
            var oddNumber = BigInteger.One;
            var result = BigInteger.Zero;

            while (n >= oddNumber) {
                n -= oddNumber;
                oddNumber += 2;
                result++;
            }

            return result;
        }

        public static BigInteger SteinerSqrt(BigInteger n) {
            if (n < 9) {
                if (n == 0) {
                    return 0;
                }

                if (n < 4) {
                    return 1;
                }
                else {
                    return 2;
                }
            }

            var result = BigInteger.Zero;
            var p = BigInteger.Zero;

            var high = n >> 1;
            var low = BigInteger.Zero;

            while (high > low + 1) {
                result = high + low >> 1;
                p = BigInteger.Pow(result, 2);

                if (n < p) {
                    high = result;
                }
                else if (n > p) {
                    low = result;
                }
                else {
                    break;
                }
            }

            return n == p ? result : low;
        }

        public static BigInteger NewtonPlusSqrt(BigInteger n) {
            n = BigInteger.Abs(n);

            if (n < 144838757784765629) { // 1.448e17 = ~1 << 57
                uint vInt = (uint)Math.Sqrt((ulong)n);
                if (n >= 4503599761588224 && (ulong)vInt * vInt > (ulong)n) { // 4.5e15 = ~1 << 52
                    vInt--;
                }
                return vInt;
            }

            double xAsDub = (double)n;
            if (xAsDub < 8.5e37) { // long.max * long.max
                ulong vInt = (ulong)Math.Sqrt(xAsDub);
                BigInteger v = vInt + (ulong)(n / vInt) >> 1;
                return v * v <= n ? v : v - 1;
            }

            if (xAsDub < 4.3322e127) {
                BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
                v = v + n / v >> 1;
                if (xAsDub > 2e63) {
                    v = v + n / v >> 1;
                }
                return v * v <= n ? v : v - 1;
            }

            int xLen = (int)n.GetBitLength();
            int wantedPrecision = (xLen + 1) / 2;
            int xLenMod = xLen + (xLen & 1) + 1;

            // Do the first Sqrt on hardware
            long tempX = (long)(n >> xLenMod - 63);
            double tempSqrt1 = Math.Sqrt(tempX);
            ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
            if (valLong == 0) {
                valLong = 1UL << 53;
            }

            // Classic Newton Iterations
            BigInteger val = ((BigInteger)valLong << 52) + (n >> xLenMod - 3 * 53) / valLong;
            int size = 106;
            for (; size < 256; size <<= 1) {
                val = (val << size - 1) + (n >> xLenMod - 3 * size) / val;
            }

            if (xAsDub > 4e254) { // 4e254 = 1 << 845.76973610139
                int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 2;

                // Apply Starting Size
                int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
                int needToShiftBy = size - wantedSize;
                val >>= needToShiftBy;
                size = wantedSize;
                do {
                    // Newton Plus Iterations
                    int shiftX = xLenMod - 3 * size;
                    BigInteger valSqrd = val * val << size - 1;
                    BigInteger valSU = (n >> shiftX) - valSqrd;
                    val = (val << size) + valSU / val;
                    size *= 2;
                } while (size < wantedPrecision);
            }

            // There are a few extra digits here, lets save them
            int oversidedBy = size - wantedPrecision;
            BigInteger saveDroppedDigitsBI = val & (BigInteger.One << oversidedBy) - 1;
            int downby = oversidedBy < 64 ? (oversidedBy >> 2) + 1 : oversidedBy - 32;
            ulong saveDroppedDigits = (ulong)(saveDroppedDigitsBI >> downby);

            // Shrink result to wanted Precision
            val >>= oversidedBy;

            // Detect a round-ups
            if (saveDroppedDigits == 0 && val * val > n) {
                val--;
            }

            // Error Detection (removed)

            return val;
        }

        public static (bool Primality, BigInteger Divisor) IsPrime(this BigInteger n) {
            if (n % 5 == 0) {
                return (false, 5);
            }

            var divisor = new BigInteger(2);
            var lastDigit = 2;

            for (var boundary = n; boundary >= divisor; boundary -= divisor) {
                if (n % divisor == 0) {
                    return (false, divisor);
                }

                divisor += Integers.Digitas[lastDigit];
                lastDigit = Integers.Digitos[lastDigit];
            }

            return (true, divisor);
        }

        public static bool IsProbablyPrime(this BigInteger n, int certainty = 8) {
            // https://rosettacode.org/wiki/Miller%E2%80%93Rabin_primality_test#C#

            if (n <= 0) {
                return false;
            }

            if (n == 1 || n == 2 || n == 3) { // Meh n == 1
                return true;
            }

            if (n.IsEven) {
                return false;
            }

            var nMinusOne = n - 1;
            var nMinusTwo = n - 2;

            var d = nMinusOne;
            int s = 0;

            while (d % 2 == 0) {
                d /= 2;
                s += 1;
            }

            BigInteger a, x;
            for (int i = 0; i < certainty; i++) {
                a = Integers.Random(2, nMinusTwo);

                x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == nMinusOne) {
                    continue;
                }

                for (int r = 1; r < s; r++) {
                    x = BigInteger.ModPow(x, 2, n);

                    if (x == 1) {
                        return false;
                    }

                    if (x == nMinusOne) {
                        break;
                    }
                }

                if (x != nMinusOne) {
                    return false;
                }
            }

            return true;
        }

        public static int LegendreSymbol(this BigInteger n, BigInteger p) {
            if (p < 2) { throw new ArgumentOutOfRangeException(nameof(p), $"Parameter '{nameof(p)}' must not be < 2, but you have supplied: {p}"); }
            if (n == 0) { return 0; }
            if (n == 1) { return 1; }

            int result;
            if (n % 2 == 0) {
                result = LegendreSymbol(n >> 2, p); // >> right shift == /2
                if (((p * p - 1) & 8) != 0) // instead of dividing by 8, shift the mask bit
                {
                    result = -result;
                }
            }
            else {
                result = LegendreSymbol(p % n, n);
                if (((n - 1) * (p - 1) & 4) != 0) // instead of dividing by 4, shift the mask bit
                {
                    result = -result;
                }
            }
            return result;
        }

        public static bool IsQuadraticResidue(this BigInteger n, BigInteger p) {
            var quotient = BigInteger.Divide(p - 1, 2);
            var modPow = BigInteger.ModPow(n, quotient, p);

            return modPow.IsOne;
        }

        public static bool CheckMersennePrimality(this BigInteger canidate, BigInteger primeExponent) {
            BigInteger d, x;

            if (!primeExponent.IsProbablyPrime()) {
                return false;
            }

            var lastDigit = canidate % 10;
            if (lastDigit == 1) {
                d = (canidate / (primeExponent * 2));
                x = BigInteger.ModPow(5, d, canidate);

                if (x.IsPowerOfTwo) {
                    return true;
                }

                return false;
            }

            d = (canidate / primeExponent);
            x = BigInteger.ModPow(3, d, canidate);

            if (x.IsPowerOfTwo) {
                return true;
            }

            return false;
        }

        public static bool CheckLucasLehmerPrimality(this BigInteger canidate, BigInteger primeExponent) {
            var s = new BigInteger(4);
            for (BigInteger i = 0; i < primeExponent - 2; i++) {
                s = BigInteger.ModPow(s, 2, canidate) - 2;
            }

            if (s == 0) {
                return true; // Prime
            }

            return false; // Composite
        }

        public static BigInteger DerivedPrime(this BigInteger n, BigInteger exponent) {
            var two_e = exponent * 2;
            var k = 1 + two_e;

            //var i3 = 1;
            //var t3 = 1 + two_e;
            //while (t3 % 3 != 0) {
            //    t3 += two_e;
            //    i3++;
            //}

            //var i5 = 1;
            //var t5 = 1 + two_e;
            //while (t5 % 5 != 0) {
            //    t5 += two_e;
            //    i5++;
            //}

            var boundary = n.IntegerSqrt();
            while (k <= boundary) {
                //if (i3 == 3 || i5 == 5) {
                //    i3 = i3 == 3 ? 1 : i3++;
                //    i5 = i5 == 5 ? 1 : i5++;

                //    k += two_e;
                //    continue;
                //}

                if (k % 3 != 0 && k % 5 != 0) {
                    if (n % k == 0) {
                        return k;
                    }
                }

                k += two_e;
            }

            return 0;
        }

        public static BigInteger DerivedPrime_Unbound(this BigInteger n, BigInteger exponent) {
            var two_e = exponent * 2;
            //var six_e = exponent * 6;
            var k = 1 + two_e;
            //var q = 1 + six_e;

            //var boundary = n.IntegerSqrt();
            while (n % k != 0) {
                k += two_e;
            }

            return k;
        }

        // Testing || Random
        public static (bool Primality, BigInteger Iterations, BigInteger Divisor, BigInteger Boundary) TEST_IsPrime(this BigInteger n) {
            if (n % 5 == 0) {
                return (false, 1, 5, n);
            }

            var divisor = new BigInteger(2);
            var lastDigit = 2;

            var boundary = n;
            var i = new BigInteger(2);
            for (; boundary >= divisor; boundary -= divisor, i++) {
                if (n % divisor == 0) {
                    return (false, i, divisor, boundary);
                }

                divisor += Integers.Digitas[lastDigit];
                lastDigit = Integers.Digitos[lastDigit];
            }

            return (true, i, divisor, boundary);
        }

        public static (BigInteger IntegerSqrt, BigInteger Result, BigInteger Number) TEST_JeremyKahanSqrt(this BigInteger n, BigInteger k) {
            var oddNumber = BigInteger.Zero;
            var result = BigInteger.Zero;

            // k =    2, result * 1
            // k =    4, result * √2
            // k =    8, result * 2
            // k =   16, result * √8
            // k =   32, result * 4
            // k =   64, result * √32
            // k =  128, result * 8
            // k =  256, result * √128
            // k =  512, result * 16
            // k = 1024, result * √512
            // k = 2048, result * 32
            // k = 4096, result * √2048
            // k = 8192, result * 64 || oddNumber / 128

            while (n > oddNumber) {
                n -= oddNumber;
                oddNumber += k;
                result++;
            }

            var integerSqrt = result * (k / 2).IntegerSqrt();

            return (integerSqrt, result, oddNumber);
        }

        public static (BigInteger Steps, BigInteger IntegerSqrt) TEST_NewtonPlusSqrt(this BigInteger n) {
            var steps = new BigInteger(1);

            if (n < 144838757784765629) { // 1.448e17 = ~1 << 57
                uint vInt = (uint)Math.Sqrt((ulong)n);
                if (n >= 4503599761588224 && (ulong)vInt * vInt > (ulong)n) { // 4.5e15 = ~1 << 52
                    vInt--;
                }
                return (steps, vInt);
            }

            double xAsDub = (double)n;
            if (xAsDub < 8.5e37) { // long.max * long.max
                ulong vInt = (ulong)Math.Sqrt(xAsDub);
                BigInteger v = vInt + (ulong)(n / vInt) >> 1;
                return (steps, v * v <= n ? v : v - 1);
            }

            if (xAsDub < 4.3322e127) {
                BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
                v = v + n / v >> 1;
                if (xAsDub > 2e63) {
                    v = v + n / v >> 1;
                }
                return (steps, v * v <= n ? v : v - 1);
            }

            int xLen = (int)n.GetBitLength();
            int wantedPrecision = (xLen + 1) / 2;
            int xLenMod = xLen + (xLen & 1) + 1;

            // Do the first Sqrt on hardware
            long tempX = (long)(n >> xLenMod - 63);
            double tempSqrt1 = Math.Sqrt(tempX);
            ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
            if (valLong == 0) {
                valLong = 1UL << 53;
            }

            // Classic Newton Iterations
            BigInteger val = ((BigInteger)valLong << 52) + (n >> xLenMod - 3 * 53) / valLong;
            int size = 106;
            for (; size < 256; size <<= 1) {
                val = (val << size - 1) + (n >> xLenMod - 3 * size) / val;
                steps++;
            }

            if (xAsDub > 4e254) { // 4e254 = 1 << 845.76973610139
                int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 2;

                // Apply Starting Size
                int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
                int needToShiftBy = size - wantedSize;
                val >>= needToShiftBy;
                size = wantedSize;
                do {
                    // Newton Plus Iterations
                    int shiftX = xLenMod - 3 * size;
                    BigInteger valSqrd = val * val << size - 1;
                    BigInteger valSU = (n >> shiftX) - valSqrd;
                    val = (val << size) + valSU / val;
                    size *= 2;
                    steps++;
                } while (size < wantedPrecision);
            }

            // There are a few extra digits here, lets save them
            int oversidedBy = size - wantedPrecision;
            BigInteger saveDroppedDigitsBI = val & (BigInteger.One << oversidedBy) - 1;
            int downby = oversidedBy < 64 ? (oversidedBy >> 2) + 1 : oversidedBy - 32;
            ulong saveDroppedDigits = (ulong)(saveDroppedDigitsBI >> downby);

            // Shrink result to wanted Precision
            val >>= oversidedBy;

            // Detect a round-ups
            if (saveDroppedDigits == 0 && val * val > n) {
                val--;
            }

            // Error Detection (removed)

            return (steps, val);
        }
    }
}
