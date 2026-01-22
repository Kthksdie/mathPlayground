using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Components;
using static System.Net.Mime.MediaTypeNames;

namespace TestingNumbers.Extensions {

    public static class BigIntegerExtensions {
        public static BigInteger[] MersennePrimes = new BigInteger[] {
            1, 2, 3, 5, 7,
            13, 17, 19, 31, 61, 89,
            107, 127, 521, 607,
            1279, 2203, 2281, 3217, 4253, 4423, 9689, 9941,
            11213, 19937, 21701, 23209, 44497, 86243,
            110503, 132049, 216091, 756839, 859433,
            1257787, 1398269, 2976221, 3021377, 6972593,
            13466917, 20996011, 24036583, 25964951, 30402457, 32582657, 37156667, 42643801, 43112609, 57885161, 74207281, 77232917, 82589933
        };

        public static ThreadLocal<Random> ThreadedRandom = new ThreadLocal<Random>(() => {
            return new Random();
        });

        public static BigInteger Next(BigInteger minValue, BigInteger maxValue) {
            // Returns a random BigInteger that is within a specified range.
            // The lower bound is inclusive, and the upper bound is exclusive.
            // https://stackoverflow.com/a/68593532 - Theodor Zoulias

            if (minValue > maxValue) {
                throw new ArgumentException();
            }
            else if (minValue == maxValue) {
                return minValue;
            }

            var zeroBasedUpperBound = maxValue - 1 - minValue; // Inclusive
            var bytes = zeroBasedUpperBound.ToByteArray();
            var lastByteMask = (byte)0b11111111; // Search for the most significant non-zero bit
            for (var mask = (byte)0b10000000; mask > 0; mask >>= 1, lastByteMask >>= 1) {
                if ((bytes[bytes.Length - 1] & mask) == mask) {
                    break; // We found it
                }
            }

            var result = BigInteger.Zero;
            while (true) {
                ThreadedRandom.Value.NextBytes(bytes);

                bytes[bytes.Length - 1] &= lastByteMask;

                result = new BigInteger(bytes);

                if (result <= zeroBasedUpperBound) {
                    return result + minValue;
                }
            }
        }

        public static BigInteger Next(this Random random, BigInteger minValue, BigInteger maxValue) {
            // Returns a random BigInteger that is within a specified range.
            // The lower bound is inclusive, and the upper bound is exclusive.
            // https://stackoverflow.com/a/68593532 - Theodor Zoulias

            if (minValue > maxValue) {
                throw new ArgumentException();
            }
            else if (minValue == maxValue) {
                return minValue;
            }

            var zeroBasedUpperBound = maxValue - 1 - minValue; // Inclusive
            var bytes = zeroBasedUpperBound.ToByteArray();
            var lastByteMask = (byte)0b11111111; // Search for the most significant non-zero bit
            for (var mask = (byte)0b10000000; mask > 0; mask >>= 1, lastByteMask >>= 1) {
                if ((bytes[bytes.Length - 1] & mask) == mask) {
                    break; // We found it
                }
            }

            var result = BigInteger.Zero;
            while (true) {
                random.NextBytes(bytes);

                bytes[bytes.Length - 1] &= lastByteMask;

                result = new BigInteger(bytes);

                if (result <= zeroBasedUpperBound) {
                    return result + minValue;
                }
            }
        }

        public static BigInteger AbsMod(this BigInteger n, BigInteger m) {
            var r = n % m;
            if (m - r > r) {
                return -r;
            }

            return m - r;
        }

        public static (BigInteger X, BigInteger Y, BigInteger Z) Pos(this BigInteger n, BigInteger m) {
            var x = -(n % m);
            var y = m + x;
            var z = y + x;

            return (x.Abs(), y, z.Abs());
        }

        public static PowerOf GetPowerOf(this BigInteger n, BigInteger @base) {
            var powerOf = new PowerOf(@base, 0);

            while (n % powerOf.Base == 0) {
                n /= powerOf.Base;
                powerOf.Exponent++;
            }

            return powerOf;
        }

        public static double ToDouble(this BigInteger n) {
            return double.Parse(n.ToString());
        }

        public static decimal ToDecimal(this BigInteger n) {
            return decimal.Parse(n.ToString());
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


        public static (BigInteger X, BigInteger Y, BigInteger Z) AbsPos(this BigInteger n, BigInteger m) {
            var x = -(n % m);
            var y = m + x;
            var z = y + x;

            return (x, y, z);
        }

        public static (BigInteger Min, BigInteger Max, BigInteger Cen) SqrtLimits(this BigInteger n) {
            var n_sqrt = n.Sqrt();

            var min = n_sqrt / Constants.SqrtOf10;
            var max = n_sqrt * Constants.SqrtOf10;
            var center = (max + min) / 2;

            return (min.WholeValue, max.WholeValue, center.WholeValue);
        }

        public static bool IsMerseneePrime(this BigInteger n, BigInteger primeExponent) {
            //var x = (n - 1) / primeExponent;
            //var pe_pos = x.Pos(primeExponent);

            //if (pe_pos.X.IsPowerOfTwo || pe_pos.Y.IsPowerOfTwo) {
            //    return true;
            //}

            return false;

            //if ((pe_pos.X.IsPowerOfTwo || pe_pos.X % 6 == 0) && (pe_pos.Y.IsPowerOfTwo || pe_pos.Y % 6 == 0)) {
            //    return true;
            //}
            //else {
            //    return false;
            //}
        }

        public static BigInteger SqrtPow(this BigInteger n, BigInteger k) {
            var k_sqrt = k.Sqrt();

            var pow = k_sqrt;
            //var n_dbl = n.ToDouble();

            for (int e = 1; e <= n; e++) {
                pow *= k_sqrt;
            }

            return pow.WholeValue;

        }

        public static BigInteger SqrtPow2(this BigInteger n, BigInteger k) {
            var k_sqrt = k.Sqrt();

            var pow = (n / k_sqrt) * k_sqrt;

            if (pow == 0) {
                return 1;
            }

            return pow.WholeValue;

        }

        public static BigInteger Abs(this BigInteger n) {
            return BigInteger.Abs(n);

        }

        public static (BigInteger Q, BigInteger R) Divide(this BigInteger n, BigDecimal k) {
            if (k == 0) {
                return (0, 0);
            }

            var kString = k.ToString();
            var t = BigInteger.Pow(10, kString.Replace(".", string.Empty).Length - 1);
            var qr = BigInteger.DivRem(n * t, k.Mantissa);

            return qr;
        }

        public static (BigInteger Q, BigInteger R) Reciprocal(this BigInteger n, int exponent) {
            var t = BigInteger.Pow(10, exponent);
            var qr = BigInteger.DivRem(t, n);

            return qr;
        }

        public static (BigInteger Q, BigInteger R) DivideRec(this BigInteger n, BigInteger k, int exponent) {
            if (k == 0) {
                return (0, 0);
            }

            var kString = k.ToString();
            var t = BigInteger.Pow(10, exponent);
            var qr = BigInteger.DivRem(n * t, k);

            return qr;
        }

        public static (double X, double Y, double Z) Rotate(this BigInteger n, string axis, double angle) {
            var n_dbl = n.ToDouble();
            var n_sqrt = Math.Sqrt(n_dbl);

            var v = new {
                X = n_dbl,
                Y = n_sqrt,
                Z = 0d
            };

            var radians = (angle * Math.PI) / 180;
            var matrix = new double[][] {
                new double[] { 0, 0, 0 },
                new double[] { 0, 0, 0 },
                new double[] { 0, 0, 0 }
            };

            if (axis == "x") {
                matrix = new double[][] {
                    new double[] { 1, 0, 0 },
                    new double[] { 0, Math.Cos(radians), -Math.Sin(radians) },
                    new double[] { 0, Math.Sin(radians), Math.Cos(radians) }
                };
            }
            else if (axis == "y") {
                matrix = new double[][] {
                    new double[] { Math.Cos(radians), 0, Math.Sin(radians) },
                    new double[] { 0, 1, 0 },
                    new double[] { -Math.Sin(radians), 0, Math.Cos(radians) }
                };
            }
            else if (axis == "z") {
                matrix = new double[][] {
                    new double[] { Math.Cos(radians), -Math.Sin(radians), 0 },
                    new double[] { Math.Sin(radians), Math.Cos(radians), 0 },
                    new double[] { 0, 0, 1 }
                };
            }

            v = new {
                X = matrix[0][0] * v.X + matrix[0][1] * v.Y + matrix[0][2] * v.Z,
                Y = matrix[1][0] * v.X + matrix[1][1] * v.Y + matrix[1][2] * v.Z,
                Z = matrix[2][0] * v.X + matrix[2][1] * v.Y + matrix[2][2] * v.Z
            };

            //Program.Print($" v.{axis} | {v}");

            return ( v.X, v.Y, v.Z );
        }

        public static double Arc(this BigInteger n, double scale) {
            var n_dbl = n.ToDouble();
            var n_sqrt = Math.Sqrt(n_dbl);
            var sqr = Math.Truncate(n_sqrt);
            var sqrf = 1d / sqr;
            var sqrv = sqr - (n_dbl % sqr);
            var sqra = sqrv * sqrf;

            //var sc = Math.SinCos(n_dbl);
            //var Log = Math.Log(n_dbl);
            //var Log2 = Math.Log2(n_dbl);
            //var Log10 = Math.Log10(n_dbl);
            //var RecE = Math.ReciprocalEstimate(n_dbl);
            //var ILogB = Math.ILogB(n_dbl);
            //var Cos = Math.Cos(n_dbl);
            //var Acos = Math.Acos(n_dbl);
            //var Acosh = Math.Acosh(n_dbl);
            //var Sin = Math.Sin(n_dbl);
            //var Asin = Math.Asin(n_dbl);
            //var Asinh = Math.Asinh(n_dbl);
            //var Tan = Math.Tan(n_dbl);
            //var Atan = Math.Atan(n_dbl);
            //var Atanh = Math.Atanh(n_dbl);
            //var Atan2 = Math.Atan2(n_dbl, n_sqrt);

            //Program.Print($" sc    | {sc}");
            //Program.Print($" Log   | {Log}");
            //Program.Print($" Log2  | {Log2}");
            //Program.Print($" Log10 | {Log10}");
            //Program.Print($" ILogB | {ILogB}");
            //Program.Print($" RecE  | {RecE}");
            //Program.Print($" Cos   | {Cos}");
            //Program.Print($" Acos  | {Acos}");
            //Program.Print($" Acosh | {Acosh}");
            //Program.Print($" Sin   | {Sin}");
            //Program.Print($" Asin  | {Asin}");
            //Program.Print($" Asinh | {Asinh}");
            //Program.Print($" Tan   | {Tan}");
            //Program.Print($" Atan  | {Atan}");
            //Program.Print($" Atanh | {Atanh}");
            //Program.Print($" Atan2 | {Atan2}");

            Program.Print();

            Program.Print($" sqrt | {n_sqrt}");
            Program.Print($" sqr  | {sqr} | r {n_dbl % sqr}");
            Program.Print($" sqrf | {sqrf}");
            Program.Print($" sqrv | {sqrv}");
            Program.Print($" sqra | {sqra}");

            return (1 + sqra) * scale;
        }

        public static IEnumerable<BigInteger> PopPush(this BigInteger n) {
            var s = n;
            var len = s.NumberOfDigits();
            var t = BigInteger.Pow(10, len - 1);

            for (int i = 0; i < len; i++) {
                var digit = s % 10;
                s /= 10;

                s = t * digit + s;

                yield return s;
            }

        }

        public static uint[] GetBits(this BigInteger n) {
            var bits = n.GetPrivateField<uint[]>("_bits");
            if (bits == null) {
                // They store small integers in _sign
                return new uint[1] { unchecked((uint)n) };
            }

            return bits;
        }

        public static int JoinDigits(this int[] setOfDigits) {
            var result = 0;
            var i = setOfDigits.Length - 1;
            var t = 1;
            while (i >= 0) {
                result += setOfDigits[i] * t;
                t *= 10;
                i--;
            }

            return result;
        }

        public static BigInteger JoinDigits(this BigInteger[] setOfDigits) {
            var result = BigInteger.Zero;
            var i = setOfDigits.Length - 1;
            var t = 1;
            while (i >= 0) {
                result += setOfDigits[i] * t;
                t *= 10;
                i--;
            }

            return result;
        }

        public static List<BigInteger> LongMultiply(this BigInteger a, BigInteger b, bool padded) {
            var results = new List<BigInteger>();

            var b_digits = b.Digits(littleEndian: true);

            var t = 1;
            foreach (var b_digit in b_digits) {
                var result = (a * b_digit);

                if (padded) {
                    result *= t;
                }

                results.Add(result);

                t *= 10;
            }

            return results;
        }

        public static (BigInteger Minus, BigInteger Plus) PmOf(this BigInteger n, BigInteger k) {
            return ((n - 1).GmOf(k), (n + 1).GmOf(k));
        }

        public static BigInteger GmOf(this BigInteger n, BigInteger k) {
            if (n % k != 0) {
                return 0;
            }

            var m = k;
            while (n % m == 0) {
                m *= k;
            }

            return m / k;
        }

        public static (BigInteger Exponent, BigInteger Value) GeOf(this BigInteger n, BigInteger k) {
            var e = 0;
            var m = k;
            while (m < n) {
                m *= k;
                e++;
            }

            return (e, m / k);
        }

        public static bool IsProbablyPrime(this BigInteger n, int certainty = 8) {
            // https://rosettacode.org/wiki/Miller%E2%80%93Rabin_primality_test#C#

            if (n <= 1) {
                return false;
            }

            if (n == 2 || n == 3 || n == 5 || n == 7) {
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
                a = Next(2, nMinusTwo);

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

        public static BigInteger NextPrimeNumber(this BigInteger n) {
            n += 1;

            var lastDigit = (int)(n % 10);

            while (!n.IsProbablyPrime()) {
                n += Constants.Digitas[lastDigit];
                lastDigit = Constants.Digitos[lastDigit];
            }

            return n;
        }

        public static BigInteger RandomPrimeNumber(int digits) {
            var lmin = BigInteger.Pow(10, digits - 1);
            var lmax = (lmin * 10) - 1;

            var l = Next(lmin, lmax);
            while (!l.IsProbablyPrime()) {
                l = Next(lmin, lmax);
            }

            return l;
        }

        public static BigInteger Shave(this BigInteger n, BigInteger k) {
            var nDigits = n.Digits();
            var digits = new List<BigInteger>();

            foreach (var digit in nDigits) {
                var d = digit - k;
                if (d >= 0) {
                    digits.Add(d);
                }
            }

            return BigInteger.Parse(string.Join("", digits));
        }

        public static int SignifigantDigits(this BigInteger n) {
            var text = BigInteger.Abs(n).ToString().TrimEnd('0');
            if (string.IsNullOrEmpty(text)) {
                return 0;
            }

            return text.Length;
        }

        public static BigInteger Jacobi(this BigInteger k, BigInteger n) {
            var t = BigInteger.One;
            var r = BigInteger.Zero;

            n %= k;

            while (n != 0) {
                while (n % 2 == 0) {
                    n /= 2;
                    r = k % 8;

                    if (r == 3 || r == 5) {
                        t = -t;
                    }
                }

                r = k;
                k = n;
                n = r;

                if (n % 4 == 3 && k % 4 == 3) {
                    t = -t;
                }

                n %= k;
            }

            if (k == 1) {
                return t;
            }
            else {
                return BigInteger.Zero;
            }
        }

        // a^(p-1)/2 ≡ 1 (mod p)
        public static bool IsQuadraticResidue(this BigInteger a, BigInteger p) {
            if (a == 0 || p == 0) {
                return false;
            }

            BigInteger quotient = BigInteger.Divide(p - 1, 2);
            BigInteger modPow = BigInteger.ModPow(a, quotient, p);

            return modPow.IsOne;
        }

        public static BigInteger Gcd(this BigInteger n, BigInteger k) {
            return BigInteger.GreatestCommonDivisor(n, k);
        }

        public static BigInteger Lcm(this BigInteger n, BigInteger k) {
            return n * k / n.Gcd(k);
        }

        public static (BigInteger X, BigInteger Y, BigInteger Z) ModGcd(this BigInteger n, BigInteger k, BigInteger m) {
            var pos = k.Pos(m);

            var x = n.Gcd(k - pos.X);
            var y = n.Gcd(k + pos.Y);
            var z = n.Gcd(k);

            return (x, y, z);
        }

        private static BigDecimal _bigDecimalTwo = new BigDecimal(2);

        public static BigDecimal GoldenRatio(this BigInteger n) {
            var sqrt = n.Sqrt();

            var rn = new BigDecimal(n);

            var d = new BigDecimal(0);
            if (n <= 2) {
                //d = (n - new BigDecimal(1, -precision)) / 2; maybe?
                d = rn / 2;
            }
            else {
                d = (rn - 1) / 2;
            }

            return (1 + sqrt) / d;
        }

        public static BigDecimal SquareRatio(this BigInteger n) {
            var sqrt = n.Sqrt();

            return sqrt / n;
        }

        public static BigDecimal Sqrt(this BigInteger n) {
            if (BigDecimal.Precision == 0) {
                return n.IntegerSqrt();
            }

            BigInteger mantissa = n;

            int root = 2;
            int currentExponent = 0;
            int precisionExponent = BigDecimal.Precision * root;

            mantissa *= BigInteger.Pow(10, precisionExponent);
            currentExponent -= precisionExponent;

            BigInteger mantissa2 = mantissa.IntegerSqrt();

            currentExponent /= root;

            //Program.Print($" Exponent {currentExponent}");

            return new BigDecimal(mantissa2, currentExponent);
        }

        public static BigInteger ModLeft(this BigInteger n, BigInteger k) {
            return n - (n % k);
        }

        public static BigInteger ModRight(this BigInteger n, BigInteger k) {
            return n.ModLeft(k) + k;
        }

        public static int Matches(this BigInteger n, BigInteger z) {
            var count = 0;

            while (true) {
                if (n % 10 == z % 10) {
                    count++;
                }
                else {
                    break;
                }

                n /= 10;
                z /= 10;

                if (n < 1 || z < 1) {
                    break;
                }
            }

            return count;
        }

        public static BigInteger Complement(this BigInteger n, BigInteger by) {
            var digits = n.Digits();
            var f = BigInteger.Pow(10, digits.Count) - 1;
            // TODO: support 'by' | currently only 9's
            return f - n;
        }

        public static BigInteger Thingy(this BigInteger n, BigInteger by) {
            var digits = n.Digits();
            var f = BigInteger.Pow(10, digits.Count);
            // TODO: support 'by' | currently only 9's

            return f - n;
        }

        public static BigInteger ToBigInteger(this byte[] bytes) {
            var pos = 0;
            var n = BigInteger.Zero;

            foreach (var bit in bytes) {
                n |= (BigInteger)bit << pos;

                pos += 8;
            }

            return n;
        }

        public static BigInteger ToPentagonal(this BigInteger n) {
            //return n * (4 * n - 1) / 2;
            return n * ((3 * n) - 1) / 2;
        }

        public static BigInteger IsPowerOf(this BigInteger n) {
            if (n.IsProbablyPrime()) {
                return 1;
            }

            var n_integerSqrt = n.IntegerSqrt();

            foreach (var p in PrimeNumbers.Get()) {
                if (p > n_integerSqrt) {
                    break;
                }

                if (n.IsPowerOf(p)) {
                    return p;
                }
            }

            return BigInteger.Zero;
        }

        public static bool IsPowerOf(this BigInteger n, BigInteger baseOf) {
            if (n % baseOf != 0) {
                return false;
            }

            while (n % baseOf == 0) {
                n /= baseOf;
            }

            return n == 1;
        }

        public static (BigInteger I, BigInteger P) DerivedBy(this BigInteger n) {
            if (n == 1) {
                return (0, 1);
            }
            else if (n == 2) {
                return (0, 1);
            }
            else if (n == 3) {
                return (0, 2);
            }

            if (n.IsProbablyPrime()) {
                return Reduce(n - 1);
            }

            return Reduce(n);

            static (BigInteger I, BigInteger P) Reduce(BigInteger n) {
                var i = BigInteger.Zero;

                while (n % 2 == 0) {
                    n /= 2;
                    i++;

                    if (n.IsProbablyPrime()) {
                        return (i, n);
                    }
                }

                while (n % 5 == 0) {
                    n /= 5;
                    i++;

                    if (n.IsProbablyPrime()) {
                        return (i, n);
                    }
                }

                var k = new BigInteger(3);
                var lastDigit = 3;

                while (true) {
                    while (n % k == 0) {
                        n /= k;
                        i++;

                        if (n.IsProbablyPrime()) {
                            return (i, n);
                        }
                    }

                    k += Constants.Digitas[lastDigit];
                    lastDigit = Constants.Digitos[lastDigit];
                    i++;
                }
            }
        }

        public static BigInteger DerivedPrime(this BigInteger n, BigInteger p) {
            if (n.IsProbablyPrime()) {
                return 1;
            }

            var boundary = n.IntegerSqrt();
            for (BigInteger d = p; d < boundary; d += p) {
                if (n % (d + 1) == 0) {
                    return (d + 1);
                }
            }

            return BigInteger.One;
        }

        public static BigInteger DerivedPrimeAlt(this BigInteger n, BigInteger p) {
            if (n.IsProbablyPrime()) {
                return 1;
            }

            var boundary = n.IntegerSqrt();
            for (BigInteger d = p; d < boundary; d += p) {
                if (n % (d + 1) == 0) {
                    return (d + 1);
                }
                else if (n % (d - 1) == 0) {
                    return (d - 1);
                }
            }

            return BigInteger.One;
        }

        public static IEnumerable<PowerOf> Factorize(this BigInteger n) {
            if (n == 0) {
                yield break;
            }

            if (n.IsProbablyPrime()) {
                n--;
            }

            var powerOf = new PowerOf(1, 0);

            if (n % 2 == 0) {
                powerOf = new PowerOf(2, 0);
                while (n % 2 == 0) {
                    n /= 2;
                    powerOf.Exponent++;
                }

                yield return powerOf;
            }

            for (BigInteger p = 3; p <= 7; p += 2) {
                if (n % p == 0) {
                    powerOf = new PowerOf(p, 0);
                    while (n % p == 0) {
                        n /= p;
                        powerOf.Exponent++;
                    }

                    yield return powerOf;
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

                    yield return powerOf;
                }

                k += Constants.Digitas[lastDigit];
                lastDigit = Constants.Digitos[lastDigit];
            }
        }

        public static BigInteger MaxPowerOf(this BigInteger n, BigInteger baseOf, out BigInteger exponent) {
            exponent = BigInteger.Zero;

            var result = BigInteger.One;

            var t = baseOf;

            while (true) {
                if (t <= n) {
                    result = t;
                }
                else {
                    break;
                }

                t *= baseOf;
                exponent++;
            }

            return result;
        }

        public static BigInteger MaxPowerOfAlt(this BigInteger n, BigInteger baseOf, out BigInteger exponent) {
            exponent = BigInteger.Zero;

            var t = baseOf;

            while (n % t == 0) {
                t *= baseOf;
                exponent++;
            }

            return t;
        }

        public static BigInteger Scan(this BigInteger n, BigInteger previous) {
            var boundary = n.IntegerSqrt();

            for (BigInteger d = previous + 1; d < boundary; d++) {
                if (n % d == 0) {
                    return d;
                }
            }

            return BigInteger.One;
        }

        public static List<BigInteger> Divisors(this BigInteger n, BigInteger boundary) {
            var divisors = new List<BigInteger>() {
                //1,
                //n
            };

            if (boundary == -1) {
                boundary = n.IntegerSqrt(SqrtMethod.NewtonPlus);
            }

            for (BigInteger d = 2; d <= boundary; d++) {
                if (n % d == 0) {
                    divisors.Add(d);

                    var high = n / d;
                    if (high > boundary) {
                        divisors.Add(high);
                    }
                }
            }

            return divisors;
        }

        public static IEnumerable<BigInteger> Factors(this BigInteger n) {
            var boundary = n.IntegerSqrt(SqrtMethod.NewtonPlus);
            for (BigInteger d = 2; d <= boundary; d++) {
                if (n % d == 0) {
                    yield return d;

                    var high = n / d;
                    if (high > boundary) {
                        yield return high;
                    }
                }
            }
        }

        public static IEnumerable<BigInteger> Shifter(this BigInteger n) {
            var s = n;
            var boundary = n.IntegerSqrt(SqrtMethod.NewtonPlus);
            for (BigInteger d = 2; d <= boundary; d++) {

                var r = s % d;
                if (r == 0) {
                    s -= d;
                }
                else {
                    s -= r;
                }

                s /= d;

                yield return s;
            }
        }

        public static List<BigInteger> Primisors(this BigInteger n) {
            var primisors = new List<BigInteger>();

            var divisors = n.Divisors(-1);

            foreach (var pairOfDivisors in divisors.Chunk(2)) {
                if (pairOfDivisors.Any(x => x.IsPrime())) {
                    primisors.AddRange(pairOfDivisors);
                }
            }

            return primisors;
        }

        public static List<BigInteger> SquaredDivisors(this BigInteger n) {
            var divisors = new List<BigInteger>();

            var integerSqrt = n.IntegerSqrt(SqrtMethod.NewtonPlus);

            if (n.IsPowerOfTwo) {
                divisors.Add(2);

                return divisors;
            }
            else if (n.IsPrime()) {
                divisors.Add(1);
                divisors.Add(n);

                return divisors;
            }

            var d = integerSqrt;

            if (d == 1) {
                divisors.Add(1);

                return divisors;
            }

            while (true) {
                if (d == 1) {
                    break;
                }

                if (n % d != 0) {
                    d--;
                }
                else {
                    break;
                }
            }

            if (d > 1 && n % d == 0) {
                divisors.Add(d);

                var high = n / d;
                if (high >= integerSqrt) {
                    divisors.Add(high);
                }
            }

            return divisors;
        }

        public static int NumberOfDigits_Fast(this BigInteger n) {
            if (n == BigInteger.Zero) {
                return 1;
            }

            return (int)Math.Floor(BigInteger.Log10(n) + 1);
        }

        private static int _bufferSize = 128;

        public static int NumberOfDigits(this BigInteger n) {
            if (n == BigInteger.Zero) {
                return 1;
            }

            Span<char> characters = new char[_bufferSize];
            if (n.TryFormat(characters, out int charsWritten)) {
                return charsWritten;
            }
            else {
                _bufferSize *= 2;
                return n.NumberOfDigits();
            }
        }

        public static int GetNumberOfSignifigantDigits(this BigInteger n) {
            if (n == BigInteger.Zero) {
                return 0;
            }

            var trailingZeros = (int)BigInteger.TrailingZeroCount(n);

            return n.NumberOfDigits() - trailingZeros;
        }

        public static BigInteger Flip(this BigInteger n) {
            var digits = n.Digits(littleEndian: true);

            return BigInteger.Parse(string.Join("", digits));
        }

        public static List<BigInteger> Digits(this BigInteger n, bool littleEndian = false) {
            var digits = new List<BigInteger>();

            while (n != 0) {
                digits.Add(n % 10);

                n /= 10;
            }

            if (!littleEndian) {
                digits.Reverse(); // last digit first
            }

            return digits; // last digit last
        }

        public static List<BigInteger> Digits(this BigInteger n, BigInteger baseOf, bool inverse = false, bool reversed = false) {
            var digits = new List<BigInteger>();

            var digit = BigInteger.Zero;
            while (n != 0) {
                digit = n % baseOf;
                if (inverse) {
                    digit = (baseOf - digit) % baseOf;

                    digits.Add(digit);
                }
                else {
                    digits.Add(digit);
                }

                n /= baseOf;
            }

            if (!reversed) {
                digits.Reverse();
            }

            return digits;
        }

        public static BigInteger DigitalRoot(this BigInteger n) {
            var digitalRoot = n;

            while (true) {
                var digits = digitalRoot.Digits(littleEndian: true);
                if (digits.Count == 1) {
                    break;
                }

                digitalRoot = digits.Sum();
            }

            return digitalRoot;
        }

        public static List<BigInteger> DigitalRoots(this BigInteger n) {
            var digitalRoot = n;
            var digitalRoots = new List<BigInteger>();

            while (true) {
                var digits = digitalRoot.Digits();
                if (digits.Count == 1) {
                    break;
                }

                digitalRoot = digits.Sum();
                digitalRoots.Add(digitalRoot);
            }

            return digitalRoots;
        }

        public static (BigInteger Sum, BigInteger Product) DigitalProduct(this BigInteger n) {
            var digits = n.Digits();

            var result = BigInteger.Zero;
            var sum = BigInteger.Zero;
            var product = BigInteger.One;
            var results = new List<BigInteger>();
            foreach (var pairOfDigits in digits.Chunk(2)) {

                if (pairOfDigits.Length == 2) {
                    result = pairOfDigits[0] * pairOfDigits[1];

                    //var temp = result == 0 ? 1 : result;
                    //Program.Print($" {pairOfDigits[0]} * {pairOfDigits[1]} = {result,3} | r {n % temp,3}");

                }
                else {
                    result = pairOfDigits[0];

                    //var temp = result == 0 ? 1 : result;
                    //Program.Print($" {pairOfDigits[0]} * 1 = {result,3} | r {n % temp,3}");
                }

                while (result / 10 > 0) {
                    var a = result / 10;
                    var b = result % 10;
                    result = a * b;

                    //var temp = result == 0 ? 1 : result;
                    //Program.Print($" {a} * {b} = {result,3} | r {n % temp,3}");
                }

                sum += result;
                product *= result == 0 ? 1 : result;
            }

            //Program.Print($"s= {sum,3} | r {n % sum,3}");
            //Program.Print($"p= {product,3} | r {n % product,3}");

            return (sum, product);
        }

        public static BigInteger Sum(this List<BigInteger> numbers) {
            var sum = BigInteger.Zero;
            foreach (var number in numbers) {
                sum += number;
            }

            return sum;
        }

        public static BigInteger Average(this List<BigInteger> numbers) {
            return numbers.Sum() / numbers.Count;
        }

        public static BigDecimal Sum(this List<BigDecimal> numbers) {
            var sum = BigDecimal.Zero;
            foreach (var number in numbers) {
                sum += number;
            }

            return sum;
        }

        public static BigDecimal Average(this List<BigDecimal> numbers) {
            return numbers.Sum() / numbers.Count;
        }

        public static BigInteger AlternatingSum(this List<BigInteger> numbers) {

            var sum = BigInteger.Zero;
            var direction = true;
            foreach (var n in numbers) {
                if (direction) {
                    sum += n;
                }
                else {
                    sum -= n;
                }

                direction = !direction;
            }

            return sum;
        }

        public static BigInteger Product(this List<BigInteger> numbers) {
            var sum = BigInteger.One;
            foreach (var number in numbers) {
                if (number == 0) {
                    continue;
                }
                else {
                    sum *= number;
                }
            }

            return sum;
        }

        public static BigInteger ModProduct(this IEnumerable<BigInteger> numbers, BigInteger modulus) {
            numbers = numbers.OrderByDescending(x => x);

            var product = BigInteger.One;

            foreach (BigInteger n in numbers) {
                product %= n;

                if (product >= modulus || product <= -modulus) {
                    product %= modulus;
                }
            }

            return product;
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

        public enum SqrtMethod {
            Test,
            NewtonPlus,
            Steiner,
            JeremyKahan
        }

        public static bool IsPrimeAlt(this BigInteger n) {
            if (n <= 1) {
                return false;
            }
            else if (n <= 10) {
                if (n == 2 || n == 3 || n == 5 || n == 7) {
                    return true;
                }
                else {
                    return false;
                }
            }

            // Works as intended, but slower :(

            var integerSqrt = n.IntegerSqrt(SqrtMethod.NewtonPlus);
            var k = integerSqrt; // can be any number <= integerSqrt;
            for (BigInteger d = 2; d < integerSqrt; d++) {
                if ((n - d * k) % d == 0) {
                    return false;
                }
            }

            return true;
        }

        public static bool IsPrime(this BigInteger n) {
            if (n <= 1) {
                return false;
            }
            else if (n <= 10) {
                if (n == 2 || n == 3 || n == 5 || n == 7) {
                    return true;
                }
                else {
                    return false;
                }
            }

            var integerSqrt = n.IntegerSqrt(SqrtMethod.NewtonPlus);

            return n.IsPrime(integerSqrt);
        }

        public static bool IsPrime(this BigInteger n, BigInteger integerSqrt) {
            if (n <= 1) {
                return false;
            }
            else if (n <= 10) {
                if (n == 2 || n == 3 || n == 5 || n == 7) {
                    return true;
                }
                else {
                    return false;
                }
            }

            for (BigInteger d = 2; d <= integerSqrt; d++) {
                if (n % d == 0) {
                    return false;
                }
            }

            return true;
        }

        public static bool IsSquare(this BigInteger n) {
            var integerSqrt = n.IntegerSqrt();
            if (integerSqrt * integerSqrt == n) {
                return true;
            }

            integerSqrt++;
            if (integerSqrt * integerSqrt == n) {
                return true;
            }

            return false;
        }

        public static bool IsCube(this BigInteger n) {
            var k = BigInteger.One;
            var r = BigInteger.Zero;

            while (true) {
                r = k * k * k;

                if (r == n) {
                    return true;
                }
                else if (r > n) {
                    break;
                }

                k++;
            }

            return false;
        }

        public static BigInteger IntegerCbrt(this BigInteger n) {
            var result = BigInteger.One;

            while (BigInteger.Pow(result, 3) < n) {
                result++;
            }

            return result;
        }

        public static BigInteger IntegerSqrt(this BigInteger n, SqrtMethod method = SqrtMethod.NewtonPlus) {
            if (method == SqrtMethod.NewtonPlus) {
                return NewtonPlusSqrt(n); // Fastest
            }
            else if (method == SqrtMethod.Steiner) {
                return SteinerSqrt(n); // Normal
            }
            else if (method == SqrtMethod.JeremyKahan) {
                return JeremyKahanSqrt(n); // Slowest?
            }

            throw new NotImplementedException();
        }

        private static readonly Dictionary<int, int> _digitas = new Dictionary<int, int>() {
            { 1, 2 },
            { 3, 2 },
            { 5, 2 },
            { 7, 4 },
            { 9, 2 },
        };

        private static readonly Dictionary<int, int> _digitos = new Dictionary<int, int>() {
            { 1, 9 },
            { 3, 1 },
            { 5, 3 },
            { 7, 3 },
            { 9, 7 },
        };

        public static BigInteger SqrtBoundary(this BigInteger n, BigInteger integerSqrt, ref BigInteger iterations) {
            //iterations = BigInteger.Zero;

            var k = integerSqrt;
            if (k % 2 == 0) {
                k += 1;
            }

            var lastDigit = (int)(k % 10);
            while (n % k != 0) {
                k -= _digitas[lastDigit];
                lastDigit = _digitos[lastDigit];

                iterations++;
            }

            return k;
        }

        private static Stopwatch _globalWatch = new Stopwatch();

        public static BigInteger SixBoundaryv1(this BigInteger n, BigInteger integerSqrt, ref BigInteger iterations) {
            _globalWatch.Restart();

            var k = integerSqrt - (integerSqrt % 6);
            while (true) {
                if (n % (k + 1) == 0) {
                    k += 1;
                    break;
                }
                else if (n % (k - 1) == 0) {
                    k -= 1;
                    break;
                }

                k -= 6;
                //iterations++;
                //iterations += 2;
            }

            _globalWatch.Stop();

            Program.Print($" k {k} | i {iterations} | v1 {_globalWatch.Elapsed}");

            return k;
        }

        private static readonly Dictionary<int, int> _digites = new Dictionary<int, int>() {
            { 6, 0 },
            { 0, 4 },
            { 4, 8 },
            { 8, 2 },
            { 2, 6 },
        };

        public static BigInteger SixBoundaryv2(this BigInteger n, BigInteger integerSqrt, ref BigInteger iterations) {
            _globalWatch.Restart();

            var k = integerSqrt - (integerSqrt % 6);
            var lastDigit = (int)(k % 10);

            while (true) {
                if (lastDigit != 6) {
                    if (n % (k + 1) == 0) {
                        k += 1;
                        break;
                    }
                    else if (n % (k - 1) == 0) {
                        k -= 1;
                        break;
                    }
                }
                else if (n % (k + 1) == 0) {
                    k += 1;
                    break;
                }

                k -= 6;
                lastDigit = _digites[lastDigit];

                //iterations++;
            }

            _globalWatch.Stop();

            Program.Print($" k {k} | i {iterations} | v2 {_globalWatch.Elapsed}");

            return k;
        }

        public static BigInteger MpBoundary(this BigInteger n, BigInteger p, BigInteger integerSqrt, ref BigInteger iterations) {
            //iterations = BigInteger.Zero;

            var mp = p * 2;
            var k = integerSqrt - (integerSqrt % mp);

            for (BigInteger m = mp; m < k; m += mp) {
                var tp = m + 1;
                if (n % tp == 0) {
                    return tp;
                }

                k -= mp; // Needs to be corrected?
                if (n % (k + 1) == 0) {
                    return k;
                }

                iterations++;
            }

            return 1;
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

        public static BigInteger NewtonPlusSqrt(BigInteger x) {
            if (x < 144838757784765629)    // 1.448e17 = ~1<<57
            {
                uint vInt = (uint)Math.Sqrt((ulong)x);
                if (x >= 4503599761588224 && (ulong)vInt * vInt > (ulong)x)  // 4.5e15 =  ~1<<52
                {
                    vInt--;
                }
                return vInt;
            }

            double xAsDub = (double)x;
            if (xAsDub < 8.5e37)   //  long.max*long.max
            {
                ulong vInt = (ulong)Math.Sqrt(xAsDub);
                BigInteger v = vInt + (ulong)(x / vInt) >> 1;
                return v * v <= x ? v : v - 1;
            }

            if (xAsDub < 4.3322e127) {
                BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
                v = v + x / v >> 1;
                if (xAsDub > 2e63) {
                    v = v + x / v >> 1;
                }
                return v * v <= x ? v : v - 1;
            }

            int xLen = (int)x.GetBitLength();
            int wantedPrecision = (xLen + 1) / 2;
            int xLenMod = xLen + (xLen & 1) + 1;

            //////// Do the first Sqrt on hardware ////////
            long tempX = (long)(x >> xLenMod - 63);
            double tempSqrt1 = Math.Sqrt(tempX);
            ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
            if (valLong == 0) {
                valLong = 1UL << 53;
            }

            ////////  Classic Newton Iterations ////////
            BigInteger val = ((BigInteger)valLong << 52) + (x >> xLenMod - 3 * 53) / valLong;
            int size = 106;
            for (; size < 256; size <<= 1) {
                val = (val << size - 1) + (x >> xLenMod - 3 * size) / val;
            }

            if (xAsDub > 4e254) // 4e254 = 1<<845.76973610139
            {
                int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 2;

                //////  Apply Starting Size  ////////
                int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
                int needToShiftBy = size - wantedSize;
                val >>= needToShiftBy;
                size = wantedSize;
                do {
                    ////////  Newton Plus Iterations  ////////
                    int shiftX = xLenMod - 3 * size;
                    BigInteger valSqrd = val * val << size - 1;
                    BigInteger valSU = (x >> shiftX) - valSqrd;
                    val = (val << size) + valSU / val;
                    size *= 2;
                } while (size < wantedPrecision);
            }

            /////// There are a few extra digits here, lets save them ///////
            int oversidedBy = size - wantedPrecision;
            BigInteger saveDroppedDigitsBI = val & (BigInteger.One << oversidedBy) - 1;
            int downby = oversidedBy < 64 ? (oversidedBy >> 2) + 1 : oversidedBy - 32;
            ulong saveDroppedDigits = (ulong)(saveDroppedDigitsBI >> downby);

            ////////  Shrink result to wanted Precision  ////////
            val >>= oversidedBy;

            ////////  Detect a round-ups  ////////
            if (saveDroppedDigits == 0 && val * val > x) {
                val--;
            }

            ////////// Error Detection ////////
            //// I believe the above has no errors but to guarantee the following can be added.
            //// If an error is found, please report it.
            //BigInteger tmp = val * val;
            //if (tmp > x)
            //{
            //    Console.WriteLine($"Missed  , {ToolsForOther.ToBinaryString(saveDroppedDigitsBI, oversidedBy)}, {oversidedBy}, {size}, {wantedPrecision}, {saveDroppedDigitsBI.GetBitLength()}");
            //    if (saveDroppedDigitsBI.GetBitLength() >= 6)
            //        Console.WriteLine($"val^2 ({tmp}) < x({x})  off%:{((double)(tmp)) / (double)x}");
            //    //throw new Exception("Sqrt function had internal error - value too high");
            //}
            //if ((tmp + 2 * val + 1) <= x)
            //{
            //    Console.WriteLine($"(val+1)^2({((val + 1) * (val + 1))}) >= x({x})");
            //    //throw new Exception("Sqrt function had internal error - value too low");
            //}

            return val;
        }

        public static BigDecimal ExtNumSqrt(this BigInteger n) {
            return BigDecimal.SquareRoot(n, BigDecimal.Precision);
        }

        public static string ToBaseString(this BigInteger value, int baseOf) {
            const string digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (baseOf > digits.Length || baseOf < 2) {
                throw new ArgumentOutOfRangeException("radix", baseOf, $"Radix has to be within range <2, {digits.Length}>;");
            }

            var result = new StringBuilder();
            do {
                value = BigInteger.DivRem(value, baseOf, out BigInteger remainder);

                result.Insert(0, digits[(int)remainder]);

            } while (value > 0);

            return result.ToString();
        }

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a binary string.
        /// </summary>
        /// <param name="bigint">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="string"/> containing a binary
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
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

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a hexadecimal string.
        /// </summary>
        /// <param name="bigint">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="string"/> containing a hexadecimal
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToHexadecimalString(this BigInteger bigint) {
            return bigint.ToString("X");
        }

        /// <summary>
        /// Converts a <see cref="BigInteger"/> to a octal string.
        /// </summary>
        /// <param name="bigint">A <see cref="BigInteger"/>.</param>
        /// <returns>
        /// A <see cref="string"/> containing an octal
        /// representation of the supplied <see cref="BigInteger"/>.
        /// </returns>
        public static string ToOctalString(this BigInteger bigint) {
            var bytes = bigint.ToByteArray();
            var idx = bytes.Length - 1;

            // Create a StringBuilder having appropriate capacity.
            var base8 = new StringBuilder((bytes.Length / 3 + 1) * 8);

            // Calculate how many bytes are extra when byte array is split
            // into three-byte (24-bit) chunks.
            var extra = bytes.Length % 3;

            // If no bytes are extra, use three bytes for first chunk.
            if (extra == 0) {
                extra = 3;
            }

            // Convert first chunk (24-bits) to integer value.
            int int24 = 0;
            for (; extra != 0; extra--) {
                int24 <<= 8;
                int24 += bytes[idx--];
            }

            // Convert 24-bit integer to octal without adding leading zeros.
            var octal = Convert.ToString(int24, 8);

            // Ensure leading zero exists if value is positive.
            //if (octal[0] != '0' && bigint.Sign == 1) {
            //    base8.Append('0');
            //}

            // Append first converted chunk to StringBuilder.
            base8.Append(octal);

            // Convert remaining 24-bit chunks, adding leading zeros.
            for (; idx >= 0; idx -= 3) {
                int24 = (bytes[idx] << 16) + (bytes[idx - 1] << 8) + bytes[idx - 2];
                base8.Append(Convert.ToString(int24, 8).PadLeft(8, '0'));
            }

            return base8.ToString();
        }
    }
}
