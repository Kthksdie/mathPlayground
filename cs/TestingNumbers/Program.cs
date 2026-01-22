using ExtendedNumerics;
using ExtendedNumerics.Helpers;
using ExtendedNumerics.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using TestingNumbers.Components;
using TestingNumbers.Extensions;
using TestingNumbers.Helpers;
using TestingNumbers.Sequences;
using static TestingNumbers.Extensions.BigIntegerExtensions;

namespace TestingNumbers
{
    internal class Program {
        // √

        private static ConsoleColor _defaultFontColor = ConsoleColor.White;

        static void Main(string[] args) {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = _defaultFontColor;
            //Console.BufferHeight = Console.BufferHeight + 3;

            WindowUtility.MoveWindowToCenter();

            //Print("Started...");

            BigDecimal.Precision = 32;

            //Gap();
            //Shapes();
            //Circles();
            //PrimalityCheck();
            //PieStuff();
            //CollatzStuff();
            //PerfectNumberStuff();
            //DigitStuff();

            //A193651();
            //ASomething2();

            //ExpandingCircle();

            //SqrtIntegers();

            //RSA130();
            //RSA100();

            //Rng();

            //UnmultiplyTesting();
            //UnmultiplyTesting2();

            //UnmultiplyBackward();

            //CollatzStuff();

            //UnmultiplySqrt();

            //MinMaxSqrts();

            //TestMinMaxSqrts();
            //TestMinMaxSqrts2();

            //SquaredRoot();
            //SquareAndGoldenRatios();

            //PerfectNumbers();
            //BigVectors();

            //BaseIntegerTesting();
            //ReducingTests();
            //DivisionByDigits();
            //DivisionChecking();
            //BaseTesting();

            //PowersOfTwo();
            //PowersOfTwo_v2();
            PowersOfTwo_v3();
            //PowersOf3_v1();

            //PowersOfProbablyNot();

            //Measurements();

            Print("Finished.");
            Prompt();
        }

        public static void Print(string message) {
            Console.WriteLine(message);
        }

        public static void Print(string message, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = _defaultFontColor;
        }

        public static void Print() {
            Console.WriteLine("");
        }

        public static void Clear() {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
        }

        public static ConsoleKeyInfo Prompt() {
            return Console.ReadKey();
        }

        private static Task Execute(Action action) {
            return Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
        }

        private static void Measurements() {
            BigDecimal.Precision = 32;

            var gc_kg = BigDecimal.Parse("0.000000000066743");
            var seconds_yr = BigInteger.Parse("31536000");

            var earth_kg = BigInteger.Parse("5970000000000000000000000");

            Print($"gc {gc_kg,16}");

            var earth_age_yr = earth_kg / gc_kg / seconds_yr;

            Print($" - {earth_age_yr,16} | earth_age_yr");
        }

        private static IEnumerable<BigDecimal> PrimeNumberGoldenRatios() {
            foreach (var p in PrimeNumbers.Get()) {
                var g = p.GoldenRatio();

                yield return p.GoldenRatio();
            }
        }

        private static void Rng(int length) {

            while (true) {
                var rsaInt = new RsaInteger(leftLength: length, rightLength: length);
                var integerSqrt = rsaInt.N.IntegerSqrt();

                Print($" n {rsaInt.N} | l {rsaInt.N.NumberOfDigits()}");
                Print($" √ {integerSqrt} | l {integerSqrt.NumberOfDigits()}");

                //var xdr = x.DigitalRoot();
                //var ydr = y.DigitalRoot();

                Print($" r {rsaInt.R} | l {rsaInt.R.NumberOfDigits()}");
                Print($" l {rsaInt.L} | l {rsaInt.L.NumberOfDigits()}");

                Print($" {rsaInt.L} * {rsaInt.R} = {rsaInt.N}");
                Prompt();
            }

        }

        private static void ReducingTests() {
            var k = BigInteger.Parse("10");

            while (true) {
                //var n = PrimeNumbers.Random(numberOfDigits: 6, prime: true);
                //var n = BigInteger.Parse("200000000116");
                //var nDrt = n.DigitalRoot();

                //var k = PrimeNumbers.Random(numberOfDigits: 2, prime: false);
                //var k = BigInteger.Parse("468"); // 13 * 6 = 78
                //var kDrt = k.DigitalRoot();

                //var n_digits = n.Digits();
                //n_digits.Reverse();

                //var k_digits = k.Digits();

                //Clear();
                //Print($" n {n,12} | drt {nDrt}");
                //Print($" k {k,12} | drt {kDrt}");
                //Print($"cq {cq,12}");
                //Print();

                //var rsa = RsaInteger.Generate(leftLength: 4, rightLength: 4).Take(1).First();
                //var size = k.NumberOfDigits() * 3;

                var n = PrimeNumbers.Random(numberOfDigits: 36, prime: false);
                //var n = rsa.N;
                var nDrt = n.DigitalRoot();
                //var numOfDigits = n.NumberOfDigits();
                //var integerSqrt = n.IntegerSqrt();

                //var k = PrimeNumbers.Random(numberOfDigits: 2, prime: false);
                //var k = BigInteger.Parse("36");
                var k_digits = k.Digits();
                var kq = n / k;
                ////InvestigateMore(n, chunkSize: 2);

                // 36 | drt: 9
                // 3 * 6 = 18 | drt: 9
                // 6 / 3 = 2

                var qn = n;
                var ki = 0;
                foreach (var k_digit in k_digits) {
                    if (k_digit == 0 || k_digit == 1) {
                        var q5 = Divitize(qn, 5);
                        //Print($" q {q5,12} | drt {q5.Quotient.DigitalRoot()} | d {5,2}");

                        var q2 = Divitize(q5.Quotient, 2);
                        //Print($" q {q2,12} | drt {q2.Quotient.DigitalRoot()} | d {2,2}");

                        qn = q2.Quotient;
                    }
                    else {
                        var q = Divitize(qn, (int)k_digit);
                        //Print($" q {q,12} | drt {q.Quotient.DigitalRoot()} | d {k_digit,2}");

                        qn = q.Quotient;
                    }

                    ki++;
                }

                //(BigInteger qnq, BigInteger qnr) = BigInteger.DivRem(qn, kq);

                qn /= ki;

                if (qn == kq) {
                    //Print($" n {n,12} | drt {nDrt}");
                    //Print($" k {k,12} | krt {k.DigitalRoot()}");
                    //Print($" q {kq}");
                    //Print();
                    //Print($"qf {qn,8} | d {ki,2}");
                    var kPrime = k.IsProbablyPrime();
                    //if (kPrime) {
                    //    Print($" k {k,12} | {(kPrime ? "Prime" : "")}");
                    //    Prompt();
                    //}

                    Print($" k {k,12} | {(kPrime ? "Prime" : "")}");
                    Prompt();
                }

                k += 1;
                //Task.Delay(1000 / 30).Wait();

                //Print($"qf {qn,8} | d {ki,2}");
                Prompt();
            }

            static (BigInteger Quotient, BigInteger Remainder) Divitize(BigInteger n, int digit) {
                var qs = new StringBuilder();

                var n_digits = n.Digits();
                var carry = 0;
                var d = BigInteger.Zero;
                foreach (var pairOfDigits in n_digits.Chunk(2)) {
                    d = BigInteger.Zero;

                    if (pairOfDigits.Length == 2) {
                        d = ((pairOfDigits[0] * 10) + pairOfDigits[1]);
                    }
                    else {
                        d = pairOfDigits[0];
                    }

                    if (carry > 0) {
                        d += carry * (int)Math.Pow(10, pairOfDigits.Length);
                    }

                    (int dq, int dr) = Math.DivRem((int)d, digit);

                    var qsp = dq.ToString().PadLeft(pairOfDigits.Length, '0');
                    qs.Append(qsp);

                    //Print($" d {d,3} / {digit,1}  | c {carry,2} | q {qsp,2} | r {dr,2}");

                    carry = dr;
                }

                return (BigInteger.Parse(qs.ToString()), carry);
            }

            static (int nq, int nr, int ss) Digitize(BigInteger n, BigInteger baseOf) {
                var digits = n.Digits().ToArray();

                int d = 0;
                int rs = 0;
                int m = digits.Length;
                for (int i = 0; i < digits.Length; i++) {
                    var digit = digits[i] == 0 ? baseOf : digits[i];

                    (BigInteger q, BigInteger r) = BigInteger.DivRem(baseOf, digit);
                    d += (int)q;
                    rs += (int)r;

                    //Print($" {baseOf,3} / {digits[i]} = {q,6} r {r,6} | {d,6}  rs {rs} = {d * rs}");
                }

                Print($"      ~ {d,6} rs {rs} = {d * rs}");

                var ss = d * (rs == 0 ? 1 : rs);

                return (d, rs, ss);
            }

            static BigInteger Reduce(BigInteger n) {
                var digits = n.Digits().ToArray();

                BigInteger d = 0;
                int m = digits.Length;
                for (int i = 0; i < digits.Length; i++) {
                    BigInteger v = (BigInteger)digits[i] >> i;
                    d += v;

                    Print($" {digits[i],3} << {i} | {v,6} | {d,6}");
                }

                return d;
            }
        }

        private static void DivisionByDigits() {
            BigDecimal.Precision = 6;
            //var k = BigInteger.Parse("1000");

            while (true) {
                var n = PrimeNumbers.Random(numberOfDigits: 4, prime: true);
                //var n = BigInteger.Parse("387");
                var nDrt = n.DigitalRoot();

                //var k = PrimeNumbers.Random(numberOfDigits: 2, prime: false);
                var k = BigInteger.Parse("15");
                var kDrt = k.DigitalRoot();

                var kResult = BigInteger.DivRem(n, k);

                Clear();
                Print($" n {n,12}");
                Print($" k {k,12}");
                Print();
                Print($" q {kResult.Quotient,12} | r {kResult.Remainder}");
                Print();

                Print($" k {k,12} | {n - kResult.Remainder,3} ~ {kResult.Remainder,3}");

                var kpart = k;
                while (kpart > 10) {
                    kpart /= 10;

                    var kval = (kpart * 10);
                    var kres = kResult.Quotient * kval;
                    Print($" k {kval,12} | {kResult.Quotient * kval,3} ~ {n - kres,3}");
                }

                var kvalb = (k % 10);
                var kresb = kResult.Quotient * kvalb;
                Print($" k {kvalb,12} | {kresb,3} ~ {n - kresb,3}");

                Print();

                //var tq = Divide_1(n, (int)k);
                //var tq = Divide_2(n, k);
                var tq = Divide_3(n, k);

                Print($"tq {tq}");

                //for (int t = 0; t <= 10; t++) {
                //    var testResult = t >> 1;

                //    Print($" t {t} | testResult {testResult}");
                //}



                Prompt();
            }

            // Divides a number of any length, by a one digit number.
            static (BigInteger Quotient, BigInteger Remainder) Divide_1(BigInteger n, int digit) {
                var qs = new StringBuilder();

                var n_digits = n.Digits();
                var carry = 0;
                var d = BigInteger.Zero;
                foreach (var pairOfDigits in n_digits.Chunk(2)) {
                    d = BigInteger.Zero;

                    if (pairOfDigits.Length == 2) {
                        d = ((pairOfDigits[0] * 10) + pairOfDigits[1]);
                    }
                    else {
                        d = pairOfDigits[0];
                    }

                    if (carry > 0) {
                        d += carry * (int)Math.Pow(10, pairOfDigits.Length);
                    }

                    //Print($" d {d,3} % {digit,1}  | r {d % digit,2}");

                    (int dq, int dr) = Math.DivRem((int)d, digit);

                    var qsp = dq.ToString().PadLeft(pairOfDigits.Length, '0');
                    qs.Append(qsp);

                    //Print($" d {d,3} / {digit,1}  | c {carry,2} | q {qsp,2} | r {dr,2}");

                    carry = dr;
                }

                return (BigInteger.Parse(qs.ToString()), carry);
            }

            // Divides a number of even length > 2, by a two digit number.
            // Kinda works with odd length..
            static (BigInteger Quotient, BigInteger Remainder) Divide_2(BigInteger n, BigInteger k) {
                
                var n_digits = n.Digits();
                var k_digits = k.Digits();

                var n_sets = new List<int[]>();
                var k_sets = new List<int[]>();

                // TODO: Carry Zero's?

                // Fixed values for this method.
                var nSetLength = 4;
                var kSetLength = 2;

                var offset = 0;
                while (true) {
                    var set = n_digits.Skip(offset).Take(nSetLength);
                    if (set.Count() == 0 || set.Count() != nSetLength) {
                        break;
                    }

                    n_sets.Add(set.Select(x => (int)x).ToArray());
                    offset += kSetLength;
                }

                offset = 0;
                while (true) {
                    var set = k_digits.Skip(offset).Take(kSetLength);
                    if (set.Count() == 0) {
                        break;
                    }

                    k_sets.Add(set.Select(x => (int)x).ToArray());
                    offset += kSetLength;
                }

                var quotientString = new StringBuilder();

                var remainder = 0;
                var carry = -1;
                var divisor = 0;
                var dividend = 0;
                foreach (var k_set in k_sets) {
                    divisor = JoinDigits(k_set);

                    foreach (var n_set in n_sets) {
                        var set_n = string.Join("", n_set);
                        if (carry >= 0) {
                            n_set[0] = carry / 10;
                            n_set[1] = carry % 10;
                        }

                        dividend = JoinDigits(n_set);

                        var result = Math.DivRem(dividend, divisor);
                        carry = result.Remainder;

                        quotientString.Append(result.Quotient.ToString().PadLeft(kSetLength, '0'));

                        Print($" {set_n,4}: {dividend,4} / {divisor,2} = {result,3} | carry {carry,2}");
                    }

                    remainder = carry;
                }

                return (BigInteger.Parse(quotientString.ToString()), remainder);

                // Converts an array of big-endian base10 digits into a number.
                static int JoinDigits(int[] setOfDigits) {
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
            }

            static (BigInteger Quotient, BigInteger Remainder) Divide_3(BigInteger n, BigInteger k) {
                var quotientString = new StringBuilder();
                var quotients = new List<string>();

                var n_digits = n.Digits();
                var n_sets = new List<int[]>();

                var offset = 0;
                while (true) {
                    var set = n_digits.Skip(offset).Take(2);
                    if (set.Count() == 0 || set.Count() != 2) {
                        break;
                    }

                    n_sets.Add(set.Select(x => (int)x).ToArray());
                    offset += 1;
                }

                var k_digits = k.Digits();

                //n_digits.Reverse();
                k_digits.Reverse();

                var pairsOfDigits = n_digits.Chunk(1).ToList();
                //var pairsOfDigits = n_sets;
                //pairsOfDigits.Reverse();

                var dividend = 0;
                var divisor = 0;
                var carry = 0;
                var remainder = 0;

                var k_i = 0;
                var k_skip = k_digits.Count - 1;
                foreach (var k_digit in k_digits) {
                    divisor = (int)k_digit;

                    if (divisor == 0) {
                        divisor = 10;
                        //continue;
                    }
                    else {
                        //divisor *= 10;
                    }

                    var digitQuotientString = new StringBuilder();
                    foreach (var pairOfDigits in pairsOfDigits) {
                        var set_n = string.Join("", pairOfDigits);
                        dividend = 0;

                        //var tens = carry > 0 ? carry * 10 : pairOfDigits[0] * 10;

                        if (pairOfDigits.Length == 2) {
                            dividend = (int)((pairOfDigits[0] * 10) + pairOfDigits[1]);
                        }
                        else {
                            dividend = (int)pairOfDigits[0];
                        }

                        //dividend -= 10 - (int)k_digit;

                        if (carry > 0) {
                            //dividend += carry * (int)Math.Pow(10, pairOfDigits.Length);
                            dividend += carry * 10;
                        }

                        var result = Math.DivRem(dividend, divisor);
                        carry = result.Remainder;

                        Print($" {set_n,4}: {dividend,4} / {divisor,2} = {result,3} | carry {carry,2}");

                        var qString = result.Quotient.ToString(); //.PadLeft(pairOfDigits.Length, '0');
                        digitQuotientString.Append(qString);
                        quotientString.Append(qString);
                    }

                    Print($"pq {digitQuotientString}");

                    remainder = carry;
                    carry = 0;
                    k_i++;
                    k_skip--;
                }

                return (BigInteger.Parse(quotientString.ToString()), remainder);
            }

            static void DoesntWork(BigInteger n, int k, int chunk) {
                var n_digits = n.Digits();
                var carry = BigInteger.Zero;
                var barrow = BigInteger.Zero;

                var chunks = n_digits.Chunk(chunk * 2).ToList();

                chunks.Reverse();

                var qsum = BigInteger.Zero;
                var csum = BigInteger.Zero;
                foreach (var groupOfDigits in chunks) {
                    var d = BigInteger.Zero;

                    var l = groupOfDigits.Length - 1;
                    var t = 1;
                    while (l >= 0) {
                        d += groupOfDigits[l] * t;
                        t *= 10;
                        l--;
                    }

                    d -= barrow;

                    if (d < k) {
                        d += BigInteger.Pow(10, groupOfDigits.Length);
                        barrow = 1;
                    }
                    else {
                        barrow = 0;
                    }

                    //d += carry;

                    var result = (d / k);
                    carry = (d * k) - result;

                    qsum += result;
                    csum += carry;

                    Print($" k {k} | d {d} | q {result} | barrow {barrow} | carry {carry}");
                }

                Print($"qs {qsum}");
                Print($"cs {csum}");
            }

            static (int nq, int nr, int ss) Digitize(BigInteger n, BigInteger baseOf) {
                var digits = n.Digits().ToArray();

                int d = 0;
                int rs = 0;
                int m = digits.Length;
                for (int i = 0; i < digits.Length; i++) {
                    var digit = digits[i] == 0 ? baseOf : digits[i];

                    (BigInteger q, BigInteger r) = BigInteger.DivRem(baseOf, digit);
                    d += (int)q;
                    rs += (int)r;

                    //Print($" {baseOf,3} / {digits[i]} = {q,6} r {r,6} | {d,6}  rs {rs} = {d * rs}");
                }

                Print($"      ~ {d,6} rs {rs} = {d * rs}");

                var ss = d * (rs == 0 ? 1 : rs);

                return (d, rs, ss);
            }

            static BigInteger Reduce(BigInteger n) {
                var digits = n.Digits().ToArray();

                BigInteger d = 0;
                int m = digits.Length;
                for (int i = 0; i < digits.Length; i++) {
                    BigInteger v = (BigInteger)digits[i] >> i;
                    d += v;

                    Print($" {digits[i],3} << {i} | {v,6} | {d,6}");
                }

                return d;
            }
        }

        private static List<(BigInteger X, BigInteger Y, BigInteger Z)> _timesTable = TimesTable(from: 1, to: 9);

        private static void DivisionChecking() {
            //var t = 0;
            //foreach (var result in timesTable) {
            //    Print($" i {t} | {result.X} * {result.Y} = {result.Z}");
            //    t++;
            //}
            //List<(BigInteger X, BigInteger Y, BigInteger Z)> timesTable = TimesTable(from: 1, to: 9);

            //Prompt();

            _timesTable = TimesTable(from: 1, to: 9);

            var numberOfDigits = 2;
            while (true) {
                var a = PrimeNumbers.Random(numberOfDigits, prime: true);
                var b = PrimeNumbers.Random(numberOfDigits, prime: true);

                //var a = BigInteger.Parse("43");
                //var b = BigInteger.Parse("17");

                while (a == b) {
                    b = PrimeNumbers.Random(numberOfDigits, prime: true);
                }

                if (b >= a) {
                    (a, b) = (b, a);
                }

                var n = a * b;
                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.Digits().Count;
                BigDecimal.Precision = n_numberOfDigits;

                var a_sqrt = a.IntegerSqrt();
                var b_sqrt = b.IntegerSqrt();

                var m = (a + b) / 2;
                var m_sqrt = m.IntegerSqrt();

                Clear();
                //Print($"tc {timesTable.Count}");
                //Print();
                Print($" n {n,13} | √ {n_integerSqrt,7} | len {n_numberOfDigits}");
                Print($" a {a,13}");
                Print($" b {b,13}");
                Print($" ~ {m,13}");
                Print();

                //Trail18(n, a, b);
                //Trail19(n, a, b);

                //Trail20(n, a, b);
                //Trail21(n, a, b);
                //Trail22(n, a, b);
                //Trail23(n, a, b);
                //Trail24(n, a, b);
                //Trail25(n, a, b);
                //Trail26(n, a, b);
                //Trail27(n, a, b);
                //Trail28(n, a, b);
                //Trail29(n, a, b);
                //Trail30(n, a, b);
                Trail31(n, a, b);

                //Trial(n);
                //Trial2(n);
                //Trail3(n, a, b);
                //Trail4(n, a, b);
                //Trail6(n, a, b);
                //Trail5(n, a, b);
                //Trail7(n, a, b);
                //Trail8(n, a, b);
                //Trail9(n, a, b);
                //Trail10(n, a, b);
                //Trail11(n, a, b);
                //Trail12(n, a, b);
                //Trail13(n, a, b);
                //Trail14(n, a, b);
                //Trail15(n, a, b);

                Prompt();
            }

            static void Trail16(BigInteger n, BigInteger a, BigInteger b) {
                var d_h = n;
                var d_l = (n / Constants.SqrtOf2).WholeValue;

                while (true) {
                    var d_m = d_l + (BigInteger.Abs(d_h - d_l) / 2);
                    var d_n = d_m + (BigInteger.Abs(d_h - d_m) / 2);

                    //var r = (n % d_h, n % d_l, n % d_m);

                    Print($" d {d_h,8} | {d_l,8} = {d_m,8} = {d_n,8}| {n.Gcd(d_m)} | {n.Gcd(d_n)}");

                    d_h = (d_h / Constants.SqrtOf2).WholeValue;
                    d_l = (d_l / Constants.SqrtOf2).WholeValue;
                    if (d_l <= 1) {
                        break;
                    }
                }
            }

            static void Trail17(BigInteger n, BigInteger a, BigInteger b) {
                var d_h = (n / Constants.SqrtOf2).WholeValue;

                var d_i = BigInteger.One;
                while (true) {
                    var d_r = n % d_h;
                    var r_r = d_r > 0 ? (d_h % a, d_h % b) : (0, 0);

                    Print($" i {d_i,8} | d {d_h,8} | d_r {d_r,8} | r_r {r_r,8}");

                    d_h = (d_h / Constants.SqrtOf2).WholeValue;
                    d_i++;
                    if (d_h <= 0) {
                        break;
                    }
                }
            }

            static void Trail18(BigInteger n, BigInteger a, BigInteger b) {
                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();
                // 1.4142135623730950488016887242097

                // 3.1415926535897932384626433832795
                //  31415926535897932384626433832795
                //                  5604991216397928.6993112824338688

                //  9.9999999999999999999999999999999
                // √3.1622776601683793319988935444327

                //var sqrt_c = new BigDecimal(3.1622776601683793319988935444327);
                //var sqrt_2 = new BigDecimal(1.4142135623730950488016887242097);
                //var pi = new BigDecimal(3.1415926535897932384626433832795);

                var c = Center(n);
                var lc = BigInteger.Zero;
                while (true) {
                    var q = new BigDecimal(n, c);

                    Print($" c {c,13} | {q} | ar {c % a} | br {c % b}"); //  | {q} | ar {c % a} | br {c % b}

                    c = Center(c);

                    if (c == lc) {
                        break;
                    }
                    else {
                        lc = c;
                    }
                }

                static BigInteger Center(BigInteger n) {
                    //var sqrt_2 = new BigDecimal(1.4142135623730950488016887242097);
                    //var pi = new BigDecimal(3.1415926535897932384626433832795);

                    var f = new BigDecimal(1.4142135623730950488016887242097);
                    var n_h = n;
                    var n_l = (n / f).WholeValue;// (n / f);// BigInteger.One;

                    while (true) {
                        n_h = n_h - (BigInteger.Abs(n_h - n_l) / f).WholeValue;
                        n_l = n_h - (BigInteger.Abs(n_h - n_l) / f).WholeValue;

                        if (n_h == n_l) {
                            return n_h;
                        }
                    }
                }
            }

            static void Trail19(BigInteger n, BigInteger a, BigInteger b) {
                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();
                var s_numberOfDigits = n_integerSqrt.NumberOfDigits();

                var s_sqrt = n.IntegerSqrt().IntegerSqrt();

                var a_sqrt = a.IntegerSqrt();
                var b_sqrt = b.IntegerSqrt();
                var c = a_sqrt + b_sqrt;

                Print($" ss {s_sqrt,13} | r {rmod(n, s_sqrt)}");

                Print($" as {a_sqrt,13} | r {rmod(a, s_sqrt)}");
                Print($" bs {b_sqrt,13} | r {rmod(b, s_sqrt)}");

                //Print($" cs {c,13} | r {n % c}");
                Print();

                return;

                var nt = n % 100;
                var nd = n % 10000;
                foreach (var product in Product.Table(1, 99)) {
                    if (product.Z % 100 != nt) {
                        continue;
                    }

                    var zd = BigInteger.Abs(product.X - product.Y);
                    var zs = product.Z.IntegerSqrt();

                    Print($" {product.X,4} * {product.Y,4} = {product.Z,4} | √ {zs,4} | sm {stan(n, zs),4}");
                }

                static BigInteger stan(BigInteger n, BigInteger d) {
                    var sqrt = n.IntegerSqrt();

                    while (sqrt.IntegerSqrt() > d) {
                        sqrt = sqrt.IntegerSqrt();
                    }

                    var x = sqrt - d;
                    var y = d - sqrt.IntegerSqrt();
                    if (x < y) {
                        return x;
                    }
                    else {
                        return y;
                    }
                }

                static BigInteger smod(BigInteger n, BigInteger d) {
                    var sqrt_2 = new BigDecimal(1.4142135623730950488016887242097);

                    var t = new BigDecimal(n);
                    while (t / sqrt_2 > d) {
                        t /= sqrt_2;
                    }

                    var x = t.WholeValue - d;
                    var y = d - (t / sqrt_2).WholeValue;
                    if (x < y) {
                        return x;
                    }
                    else {
                        return y;
                    }
                }

                static BigInteger rmod(BigInteger n, BigInteger m) {
                    var r = n % m;
                    if (m - r > r) {
                        return r;
                    }

                    return m - r;
                }
            }

            static void Trail20(BigInteger n, BigInteger left, BigInteger right) {
                var n_dbl = double.Parse((n / 2).ToString());
                var n_sqrt = Math.Sqrt(n_dbl);

                var middle = double.Parse((left + right).ToString()) / 2d;
                var m_sqrd = Math.Pow(middle, 2);

                Print($" {middle,21} | m");
                Print($" {m_sqrd,21} | m_sqrd");

                var ffd = 0.4254517622670592; // Math.Sin(45d) / 2d
                var otfd = 0.04418434305200072; // Math.Sin(135d) / 2d

                var a = n_dbl - n_sqrt;
                var c = n_dbl + n_sqrt;
                var b = n_dbl;
                //var b = a + c;

                var area_a = (a * b) * (Math.Sin(45d) / 2d);
                var height_a = 2d * (area_a / b);

                var area_c = (a * b) * (Math.Sin(135) / 2d);
                var height_c = 2d * (area_c / b);

                var perimeter = a + c + b;

                var area_m = (area_a + area_c) / 2;
                var height_m = (height_a + height_c) / 2;


                Print($" {a,21} | a");
                Print($" {c,21} | c");
                Print($" {b,21} | b");
                Print($" {perimeter,21} | perimeter");
                Print($" {area_a,21} | area_a");
                Print($" {height_a,21} | height_a");
                Print($" {area_c,21} | area_c");
                Print($" {height_c,21} | height_c");
                Print($" {area_m,21} | area_m");
                Print($" {height_m,21} | height_m");
                Print();

                var sqrt_2 = Math.Sqrt(2);
                var sqrt_10 = Math.Sqrt(10);
                var sqrt_h = (n * 10).IntegerSqrt();
                var sqrt_l = n.IntegerSqrt();

                var sqrt_m = (sqrt_h + sqrt_l) / 2;

                Print($" {sqrt_h,21} | sqrt_h");
                Print($" {sqrt_l,21} | sqrt_l");
                Print($" {sqrt_m,21} | sqrt_m");
            }

            static void Trail21(BigInteger n, BigInteger a, BigInteger b) {
                var m = (a + b) / 2;
                var m_dbl = m.ToDouble();

                Print($" m   | {m}");

                for (int angle = 0; angle <= 360; angle += 6) {
                    var vx = n.Rotate("x", angle);
                    //Print($" v.x | {vx}");

                    var vy = Math.Truncate(Math.Abs(vx.Y) + Math.Abs(vx.Z));

                    //Print($" v.x.r {angle} | {r}");

                    if (m_dbl - 1 == Math.Truncate(Math.Abs(vx.Y)) || m_dbl - 1 == Math.Truncate(Math.Abs(vx.Z))) {
                        Print($" v.x.r {angle} | {vx}");
                        break;
                    }
                    else if (m_dbl + 1 == Math.Truncate(Math.Abs(vx.Y)) || m_dbl + 1 == Math.Truncate(Math.Abs(vx.Z))) {
                        Print($" v.x.r {angle} | {vx}");
                        break;
                    }
                }

                //var vx = n.Rotate("x", 90);
                //Print($" v.x | {vx}");

                //var vy = n.Rotate("y", 90);
                //Print($" v.y | {vy}");

                //var vz = n.Rotate("z", 90);
                //Print($" v.z | {vz}");
            }

            static void Trail22(BigInteger n, BigInteger a, BigInteger b) {
                var m = (a + b) / 2;
                var m_dbl = m.ToDouble();

                var n_integerSqrt = n.IntegerSqrt();
                var remainder = n - BigInteger.Pow(n_integerSqrt, 2);

                Print($" m   | {m}");
                Print($" r   | {remainder}");
                Print();

                var kite_a = remainder;
                var kite_b = n_integerSqrt;
                var kite_p = 2 * (kite_a + kite_b);

                Print($" a   | {kite_a}");
                Print($" b   | {kite_b}");
                Print($" p   | {kite_p}");
            }

            static void Trail23(BigInteger n, BigInteger a, BigInteger b) {
                var m = (a + b) / 2;
                var m_dbl = m.ToDouble();

                var n_integerSqrt = n.IntegerSqrt();
                var remainder = n - BigInteger.Pow(n_integerSqrt, 2);

                Print($" m   | {m}");
                Print($" r   | {remainder}");
                Print();

                n.Arc(1d);

            }

            static void Trail24(BigInteger n, BigInteger a, BigInteger b) {
                var sqrt_2 = Math.Sqrt(2);
                var sqrt_3 = Math.Sqrt(3);
                var sqrt_5 = Math.Sqrt(5);
                var sqrt_6 = Math.Sqrt(6);

                var sqrt_6h = sqrt_6 / 2;

                var sqrt_10 = Math.Sqrt(10);
                var phi = (1d + Math.Sqrt(5)) / 2d;

                var n_dbl = n.ToDouble();
                var m = (a + b) / 2;

                //Print($" m   | {m}");
                //Print();

                var d = n_dbl;
                var s = BigInteger.Zero;

                while (d > 1) {
                    d = (d + (d / sqrt_6h)) / 2;

                    Print($" d {d,16}");
                    Prompt();

                    s++;
                }

                Print($" s {s,16}");

                //var mp = BigInteger.ModPow(s, d, k);

                //Print($" mp  | {mp}");


            }

            static void Trail25(BigInteger n, BigInteger a, BigInteger b) {
                var sqrt_2 = Math.Sqrt(2);
                var sqrt_3 = Math.Sqrt(3);
                var sqrt_5 = Math.Sqrt(5);
                var sqrt_6 = Math.Sqrt(6);

                var sqrt_6h = sqrt_6 / 2;

                var sqrt_10 = Math.Sqrt(10);
                var phi = (1d + Math.Sqrt(5)) / 2d;

                var n_dbl = n.ToDouble();
                var m = (a + b) / 2;

                //Print($" m   | {m}");
                //Print();

                var sqrt = Math.Sqrt(n_dbl);
                var k = 1d;
                var s = 0d;
                var v = 0d;
                var lv = v;

                while (v < n_dbl) {
                    var ks = (k * sqrt_2);
                    var ksl = (sqrt + (sqrt / ks)) / 2;
                    var ksh = (sqrt + (sqrt * ks)) / 2;
                    var ksv = (ksl + ksh) / 2;

                    v = ksl * ksh;
                    sqrt = Math.Sqrt(v);

                    Print($" {ksh,6} * {ksl,6} = {v,16} ~ {ksv}");
                    Prompt();

                    if (v < lv) {
                        break;
                    }

                    lv = v;

                    k++;
                    s++;
                }

                Print($" s {s,16}");

                //var mp = BigInteger.ModPow(s, d, k);

                //Print($" mp  | {mp}");


            }

            static void Trail26(BigInteger n, BigInteger a, BigInteger b) {

                var c = PrimeNumbers.Random(numberOfDigits: 8, prime: true);
                Print($" c {c}");

                var i = BigInteger.One;
                var d = c;
                while (d > 1) {
                    var k = (d % 10);
                    if (k == 0) {
                        k = 5;
                    }

                    var r = d % k;

                    if (r == 0) {
                        if (k == 1) {
                            d -= 1;
                        }
                        else {
                            d /= k;
                        }
                    }
                    else {
                        d -= r;
                        //d /= k;
                    }
                    

                    Print($" i {i,4} | d {d,16} | {c.Gcd(d)}");
                    Prompt();

                    i++;
                }

            }

            static void Trail27(BigInteger n, BigInteger a, BigInteger b) {


                var k = new BigInteger(1);
                var d = n;
                while (d > k) {
                    if ((d - 1) % k == 0) {
                        d = (d - 1) / k;
                        Print($" -k {k,8} | d {d,16} | g {n.Gcd(d)}");
                        Prompt();
                    }
                    else if ((d + 1) % k == 0) {
                        d = (d + 1) / k;
                        Print($" +k {k,8} | d {d,16} | g {n.Gcd(d)}");
                        Prompt();
                    }

                    k += 2;
                }


            }
            
            static void Trail28(BigInteger n, BigInteger a, BigInteger b) {
                var sqrt_2 = Math.Sqrt(2);
                var sqrt_3 = Math.Sqrt(3);
                var sqrt_5 = Math.Sqrt(5);
                var sqrt_6 = Math.Sqrt(6);
                var sqrt_10 = Math.Sqrt(10);

                var phi = (1d + Math.Sqrt(5)) / 2d;
                var phi2 = (Math.Sqrt(5)) / 4d;

                var n_dbl = n.ToDouble();
                var m = (a + b) / 2;

                Print($" phi | {phi}");
                Print($" ph2 | {phi2}");

                //Print($" m   | {m}");
                //Print();

                var sqrt = Math.Sqrt(n_dbl);
                var sqrth = sqrt * phi;
                var sqrtl = sqrt / phi;
                var sqrtm = (sqrth + sqrtl) / 2;

                Print($" sqm | {sqrtm}");

                var sqrtlimit = new BigInteger(sqrt / sqrt_10);

                var s = BigInteger.One;
                var v = BigInteger.Zero;

                while (true) {
                    sqrth = (sqrth + (sqrth / phi)) / 2;
                    sqrtl = (sqrtl + (sqrtl / phi)) / 2;

                    v = new BigInteger((sqrth * sqrtl));

                    var g = n.Gcd(v);
                    if (g != 1) {
                        Print($" v {v,16} | g {n.Gcd(v)}");
                    }

                    //Print($" {sqrth,16} * {sqrtl,16} = {v,16} | g {n.Gcd(v)}");
                    //Print($" v {v,16} | g {n.Gcd(v)}");
                    s++;
                    if (v < sqrtlimit) {
                        break;
                    }
                }

                Print($" s {s,16}");
            }

            static void Trail29(BigInteger n, BigInteger a, BigInteger b) {
                var sqrt_2 = Math.Sqrt(2);
                var sqrt_3 = Math.Sqrt(3);
                var sqrt_5 = Math.Sqrt(5);
                var sqrt_6 = Math.Sqrt(6);
                var sqrt_10 = Math.Sqrt(10);

                var phi = (1d + Math.Sqrt(5)) / 2d;

                var n_dbl = n.ToDouble();
                var m = (a + b) / 2;

                var sqrt = Math.Sqrt(n_dbl);
                var sqrt_h = new BigInteger(sqrt * sqrt_10);

                while (!sqrt_h.IsProbablyPrime()) {
                    sqrt_h++;
                }

                //Print($" h {sqrt_h,16}");

                var j = n - ((n + (n / 2)) / 2);
                var j_divisors = j.Divisors(-1);

                Print($" j {j,16}");

                foreach (var d in j_divisors) {
                    Print($" d {d,16}");
                }
            }

            static void Trail30(BigInteger n, BigInteger a, BigInteger b) {


                var d = n;
                var n_sqrd = n * n;
                var k = BigInteger.One;
                while (d <= n_sqrd) {
                    d += n;
                    k++;

                    var d_string = d.ToString();
                    if (d_string.Contains(a.ToString()))  {
                        Print($" k {k,6} | d_a {d_string}");
                        break;
                    }
                    else if (d_string.Contains(b.ToString())) {
                        Print($" k {k,6} | d_b {d_string}");
                        break;
                    }
                }

            }

            static void Trail31(BigInteger n, BigInteger a, BigInteger b) {
                var n_integerSqrt = n.IntegerSqrt();

                var k = BigInteger.One;
                var s = BigInteger.One;
                var rc = BigInteger.Zero;
                var sq = BigInteger.Zero;
                var re = BigInteger.Zero;

                //(rc + (k * 4)) < n
                //rc < n
                //k < n_integerSqrt

                while (rc < n) {
                    if (k == 1) {
                        rc += k * 3;
                    }
                    else {
                        rc += k * 4;
                    }

                    sq = BigInteger.Pow(k + 1, 2) * 2;
                    sq += BigInteger.Pow(k, 2) * 2;
                    sq -= 3;
                    // sq = (rc * 2) + 1; ... =/

                    re = ((k + 1) * (k / 2)) * 2;
                    re += ((k - 1) * (k / 2)) * 2;

                    //k += 2;
                    k += 1;
                    s++;
                }

                Print($"rc {rc,16} √ {rc.IntegerSqrt()}");
                Print($"sq {sq,16} √ {sq.IntegerSqrt()}");
                Print($"re {re,16} √ {re.IntegerSqrt()}");
                Print($" k {k,16}");
                Print($" s {s,16}");
            }

            static BigInteger Trial(BigInteger n) {
                var og = new BigDecimal(n);

                var k = new BigInteger(4);
                var o = k;
                var result = BigInteger.Zero;

                // 2, result * 1
                // 4, result * 
                // 8, result * 
                // 16, result * 
                // 32, result * 4
                //var target = n - (n / 2);

                while (o < og) {
                    og -= o;
                    o += k;
                    result++;

                    //Print($" n {n} | o {o} | r {result}");
                }

                Print($" n {og} | o {o} | r {result}");

                return result;
            }

            static BigInteger Trial2(BigInteger n) {
                var og = n;
                var p = BigInteger.One;
                var result = BigInteger.Zero;

                foreach (var primeNumber in PrimeNumbers.Get()) {
                    if (n <= primeNumber) {
                        break;
                    }

                    n -= primeNumber;
                    p = primeNumber;
                    result++;
                }

                Print($" n {n} | p {p} | r {result}");

                return result;
            }

            static void Trail3(BigInteger n, BigInteger a, BigInteger b) {
                var numberOfDigits = n.Digits().Count;

                var t = BigInteger.Pow(10, (numberOfDigits / 2) + 1);

                if (t % 2 == 0) {
                    //t = BigInteger.Pow(10, (numberOfDigits / 2));
                }

                var d = n / t;
                var u = n % t;
                var q = d * t;

                var s = BigInteger.Zero;

                Print($" d {d}");
                Print($" u {u}");
                Print($" q {q}");

                while (true) {
                    var ar = q % a;
                    var br = q % b;

                    if (ar <= 1 || br <= 1) {
                        Print($" i {s} | q {q} | ar {ar} | br {br}");
                        Prompt();
                        break;
                    }

                    q -= u;
                    s++;
                }
            }

            static void Trail4(BigInteger n, BigInteger a, BigInteger b) {
                var numberOfDigits = n.Digits().Count;
                var t = BigInteger.Pow(10, (numberOfDigits / 2) + 1);

                if (numberOfDigits % 2 != 0) {
                    t = BigInteger.Pow(10, (numberOfDigits / 2) + 2);
                }

                BigDecimal.Precision = numberOfDigits * 2;

                var ge6 = n.GeOf(6);

                var kk = 2;

                Print($" n {n,12} | gm {(n - kk).PmOf(kk)} gm {n.PmOf(kk)} gm {(n + kk).PmOf(kk)} | ge {ge6}");
                Print($" a {a,12} | gm {a.PmOf(kk)}");
                Print($" b {b,12} | gm {b.PmOf(kk)}");
                Print();

                var n6 = n / ge6.Exponent;

                var n6r = (n6 % a, n6 % b);

                Print($"n6 {n6} | {n6r}");
                Print();

                var qa = n / t;
                var qb = n % t;

                Print($"qa {qa.ToString().PadRight(numberOfDigits)}");
                Print($"qb {qb.ToString().PadLeft(numberOfDigits)}");
                Print();

                var ua = new BigDecimal(qb, qa);

                var uar = (ua.Mantissa % a, ua.Mantissa % b);

                Print($"ua {ua} | r {uar}");
                Print();

                

                Print();
            }

            static void Trail5(BigInteger n, BigInteger a, BigInteger b) {

                var d = n / 2;

                var aLongResults = a.LongMultiply(b, padded: true);
                var bLongResults = b.LongMultiply(a, padded: true);

                PrintLongResults(n, a, b, aLongResults);

                PrintLongResults(n, a, b, bLongResults);
                Print();

                return;

                var matchDigits = 2;
                var t = BigInteger.Pow(10, matchDigits);
                var u = BigInteger.Pow(10, matchDigits * 2);

                var at = a % t;
                var bt = b % t;

                var dt = n % t;
                var du = n % u;

                Print($" t {t,6} | u {u,6}");
                Print($"dt {dt,6} | du {du,6}");
                Print();

                foreach (var result in _timesTable) {
                    if (result.Z % t == dt) {

                        var qr = BigInteger.DivRem(n, result.Z);

                        var df = BigInteger.Abs(result.Z - du);

                        var msg = $" r {result.X,2} * {result.Y,2} = {result.Z,4} | df {df,4} | qr {qr}";

                        if ((result.X == at || result.X == bt) && (result.Y == at || result.Y == bt)) {
                            Print($"{msg} ~");
                        }
                        else {
                            Print($"{msg}");
                        }
                    }
                }

                static void PrintLongResults(BigInteger n, BigInteger a, BigInteger b, List<BigInteger> longResults) {
                    var sum = BigInteger.Zero;

                    foreach (var longResult in longResults) {
                        sum += longResult;

                        var h = n - longResult;
                        if (longResult != 0) {
                            var remainders = (h % a, h % b);

                            //var ln = new BigDecimal(n, longResult);

                            Print($" + {longResult,8} | s {sum,8} | - {h,8} r {remainders}");
                        }
                        else {
                            Print($" + {longResult,8} | s {sum,8} | - {h,8}");
                        }
                    }
                }
            }

            static void Trail6(BigInteger n, BigInteger a, BigInteger b) {

                var d = n;
                var sqrtSum = BigInteger.Zero;
                for (int i = 2; i < 10; i++) {
                    d /= i;
                    var dIntegerSqrt = d.IntegerSqrt();
                    sqrtSum += dIntegerSqrt;

                    Print($" d {d,8} | √ {dIntegerSqrt,8} | s {sqrtSum,8}");
                }

                Print();

            }

            static void Trail7(BigInteger n, BigInteger a, BigInteger b) {
                var f = new BigInteger(101);
                var d = new BigDecimal(n, f);

                var remainders = (nr: d.Mantissa % n, ar: d.Mantissa % a, br: d.Mantissa % b);
                
                Print($" d {d} | m {d.Mantissa} | {remainders}");

                Print();

            }

            static void Trail8(BigInteger n, BigInteger a, BigInteger b) {
                var a_digits = a.Digits(littleEndian: true);
                var b_digits = b.Digits(littleEndian: true);

                var t = 1;
                var sum = BigInteger.Zero;
                for (int i = 0; i < a_digits.Count; i++) {
                    var a_digit = a_digits[i];
                    var b_digit = b_digits[i];

                    var c = (a_digit * b_digit);

                    sum += c;

                    if (c != 0 && sum != 0) {
                        //Print($" {a_digit,4} * {b_digit,4} = {c,8} r {n % c} | s {sum,8} r {n % sum}");
                    }

                    Print($" {a_digit,4} * {b_digit,4} = {c,8} | s {sum,8} r {n % sum}");

                    //t *= 10;
                }


            }

            static void Trail9(BigInteger n, BigInteger a, BigInteger b) {
                var n_digits = n.Digits(littleEndian: false);

                var product = BigInteger.One;
                foreach (var setOfDigits in n_digits.Chunk(4)) {
                    var p = setOfDigits.JoinDigits();
                    var p_divisors = p.Divisors(-1);

                    //var d_remainders = p_divisors.Where(x => x > 1).Select(x => $"{x} r {n % x}");

                    Print($" {p}");

                    foreach (var p_divisor in p_divisors) {
                        var aqr = BigInteger.DivRem(a, p_divisor);
                        var bqr = BigInteger.DivRem(b, p_divisor);

                        if (aqr.Remainder == 1 || bqr.Remainder == 1) {
                            Print($" - {p_divisor} | aqr {aqr} | bqr {bqr}");
                        }

                        //Print($" - {p_divisor} | aqr {aqr} | bqr {bqr}");
                    }
                }
            }

            static void Trail10(BigInteger n, BigInteger a, BigInteger b) {
                var n_digits = n.Digits(littleEndian: false);
                var k = n / 2;

                var k_divisors = k.Divisors(-1);

                foreach (var p_divisor in k_divisors) {
                    var aqr = BigInteger.DivRem(a, p_divisor);
                    var bqr = BigInteger.DivRem(b, p_divisor);

                    if (aqr.Remainder == 1 || bqr.Remainder == 1) {
                        Print($" / {p_divisor} | aqr {aqr} | bqr {bqr}");
                    }
                    else if (a % p_divisor == (a - 1) || b % p_divisor == (b - 1)) {
                        Print($" % {p_divisor} | aqr {aqr} | bqr {bqr}");
                    }
                    //Print($" - {p_divisor} | aqr {aqr} | bqr {bqr}");
                }
            }

            static void Trail11(BigInteger n, BigInteger a, BigInteger b) {

                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();

                var k = n;
                var n_digits = k.Digits(littleEndian: true);

                var jt = BigInteger.One;
                var j = BigInteger.Zero;
                var y = BigInteger.Zero;
                foreach (var n_digit in n_digits.Take(n_digits.Count)) {
                    y = n / jt;
                    j += n_digit * jt;

                    var x = BigInteger.Abs(y - j);

                    Print($" x {x,8}");

                    //if (j.IsProbablyPrime()) {
                    //    //var j_divisors = (j - 1).Divisors(-1).Where(x => x < n_integerSqrt);

                    //    Print($" j {j,8} | ~");
                    //}
                    //else {
                    //    //var j_divisors = j.Divisors(-1).Where(x => x < n_integerSqrt);

                    //    Print($" j {j,8} | ");
                    //}

                    //y = n / jt;

                    //if (y.IsProbablyPrime()) {
                    //    //var y_divisors = (y - 1).Divisors(-1).Where(x => x < n_integerSqrt);

                    //    Print($" y {y,8} | ~");
                    //}
                    //else {
                    //    //var y_divisors = y.Divisors(-1).Where(x => x < n_integerSqrt);

                    //    Print($" y {y,8} | ");
                    //}

                    jt *= 10;
                }
            }

            static void Trail12(BigInteger n, BigInteger a, BigInteger b) {
                var n_integerSqrt = n.IntegerSqrt();

                var a_divisors = (a - 1).Divisors(-1);
                var b_divisors = (b - 1).Divisors(-1);

                var divisors = new List<BigInteger>();
                divisors.AddRange(a_divisors);
                divisors.AddRange(b_divisors);
                var ordered = divisors.Select(x => (x, n % x)).OrderBy(x => x.Item2).Reverse();

                foreach (var divisor in ordered) {
                    Print($" {divisor.Item1,4} | {divisor.Item2,4}");

                }
            }

            static void Trail13(BigInteger n, BigInteger a, BigInteger b) {
                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();

                var t = BigInteger.Pow(10, n_numberOfDigits);
                //var e = n + t;

                Print($" t {t,8}");

                for (int i = 1; i < 360; i++) {
                    var e = n + (t * i);
                    var remainders = (AR: e % a, BR: e % b);
                    var powers = (PA: remainders.AR.IsPowerOf(), PB: remainders.BR.IsPowerOf());
                    
                    if ((powers.PA != 0 && powers.PA != 1) || (powers.PB != 0 && powers.PB != 1)) {
                        Print($" i {i,8}");
                        Print($" e {e,8} | r {remainders} | p {powers}");
                    }

                    //Print($" t {t,8}");
                    //Print($" e {e,8} | r {remainders} | p {powers}");
                }

                Print($" done");
            }

            static void Trail14(BigInteger n, BigInteger a, BigInteger b) {
                var n_drt = n.DigitalRoot();
                var n_integerSqrt = n.IntegerSqrt();
                var n_boundary = n_integerSqrt / 3;
                var n_sqrtSqrt = n_integerSqrt.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();

                var adp = a.DerivedBy();
                var bdp = b.DerivedBy();

                //Print($" a {a,8} | d {adp} | r {n % adp}");
                //Print($" b {b,8} | d {bdp} | r {n % bdp}");
                Print();

                var d = n;

                //while (d % 2 == 0) {
                //    d /= 2;
                //}

                var d_integerSqrt = d.IntegerSqrt();

                //var q = (a / 6) * (b / 6);

                //var u = new BigDecimal(n, q);

                Print($" d {d,8}");
                //Print($" q {q,8} | {u}");

                var i = 0;
                var k = 2;
                while (k < b) {

                    if (d % k == 0) {
                        var kd = d / k;
                        var abr = (kd % a, kd % b);

                        Print($" k {k,4} | {kd,8} / r {abr}");
                    }
                    else {
                        var o = 0;
                        if (k % 2 != 0) {
                            o = 1;
                        }

                        var kr = d % k;
                        var kl = (d.ModRight(k) - d) - o;

                        if (kr == kl) {
                            var kd = n - k;
                            var abr = (kd % a, kd % b);

                            Print($" k {k,4} | {kd,8} - r {abr}"); // | r {kr,4} | l {kl,4}
                        }
                    }

                    k += 1;
                }

                Print($" done");

                //var ndp = (n - 1).DerivedBy();
                //var nd = ndp.DerivedBy();
                //var pd = nd.DerivedBy();

                //Print($" n {ndp,8}");
                //Print($" n {nd,8}");
                //Print($" n {pd,8}");

                //var d = n - (n / 4);
                //var ddp = d.DerivedBy();

                //Print($" d {ddp,8}");


                //Print($" a {ad,12} | r {n % ad.DerivedPrime,4} | dr {d % ad.DerivedPrime,4}");
                //Print($" b {bd,12} | r {n % bd.DerivedPrime,4} | dr {d % bd.DerivedPrime,4}");
            }

            static void Trail15(BigInteger n, BigInteger a, BigInteger b) {

                var derived = new Dictionary<BigInteger, BigInteger>();

                var k = new BigInteger(2);
                var s = BigInteger.One;
                while (true) {
                    while (!k.IsProbablyPrime()) {
                        k++;
                    }

                    //var dp = k.DerivedByBoth();

                    ////if (derived.ContainsKey(dp)) {
                    ////    derived[dp] += 1;
                    ////}
                    ////else {
                    ////    derived.Add(dp, 1);
                    ////}

                    ////s *= dp;
                    ////s += k;

                    //Print($" k {k,8} | dp {dp,8}"); //  | {derived[dp],6}
                    Prompt();

                    k++;
                }

                //var ad = a.DerivedBy();
                //var bd = b.DerivedBy();



            }
        }

        private static void PowersOfTwo() {


            var powersOf_2 = new PowersOf(2);
            var precision = 1;

            while (true) {
                while (!powersOf_2.Exponent.IsProbablyPrime()) {
                    powersOf_2.Next();
                }

                var isMp = MersennePrimes.Contains(powersOf_2.Exponent);
                var mpTag = isMp ? "~" : "!";

                var n = powersOf_2.Value;
                var mp = n - 1;

                precision = (int)powersOf_2.Exponent.NextPrimeNumber();
                precision = 38;

                BigDecimal.Precision = precision;
                //BigDecimal.Precision = n.NumberOfDigits();

                //var sqrt = n.Sqrt();
                var mp_sqrt = mp.Sqrt();

                // 2^23 = 29
                // 2^29 = 32
                // 2^37 = 
                //var exponent = precision;
                //var rec = powersOf_2.Exponent.Reciprocal(exponent);
                //var rc = new BigDecimal(rec.Q, -(exponent - 1));

                var rc_b = new BigDecimal(1, powersOf_2.Exponent); // reciprocal

                Print($" {mpTag}   2^{powersOf_2.Exponent} | {mp}");
                Print($"   √ {mp_sqrt}");
                Print($"  pc {precision}");
                //Print($"  rc {rc}");
                Print($" rcb {rc_b}");
                var dp = isMp ? 1 : mp.DerivedPrime(powersOf_2.Exponent);

                var ag = mp.Divide(rc_b);
                if (ag.R != 0) {
                    var k = ag.R / mp;
                    var dpe = (k * powersOf_2.Exponent) + 1;

                    Print($"  ag {ag}");
                    Print($"   k {k} | {dpe}");
                    Print($"  dp {dp}");
                }

                precision += 2;
                powersOf_2.Next();
                Print();
                Prompt();
            }

        }

        private static void PowersOfTwo_v2() {


            var powersOf_2 = new PowersOf(2);
            var precision = 1;

            while (true) {
                while (powersOf_2.Exponent < 23) {
                    powersOf_2.Next();
                }

                while (!powersOf_2.Exponent.IsProbablyPrime()) {
                    powersOf_2.Next();
                }

                var isMp = MersennePrimes.Contains(powersOf_2.Exponent);
                var mpTag = isMp ? "~" : "!";

                var n = powersOf_2.Value;
                var mp = n - 1;

                //precision = (int)powersOf_2.Exponent + 3;
                precision = 29;

                BigDecimal.Precision = precision;
                //BigDecimal.Precision = n.NumberOfDigits();

                //var sqrt = n.Sqrt();
                var mp_sqrt = mp.Sqrt();

                // 2^23 = 29
                // 2^29 = 32
                // 2^37 = 
                var exponent = precision;
                var rec = powersOf_2.Exponent.Reciprocal(exponent);
                var rc_a = new BigDecimal(rec.Q, -exponent);

                var rc_b = new BigDecimal(1, powersOf_2.Exponent); // reciprocal

                Print($" {mpTag}   2^{powersOf_2.Exponent} | {mp}");
                Print($"   √ {mp_sqrt}");
                Print($"  pc {precision}");
                Print($" rec {rec}");
                Print($" rca {rc_a}");
                Print($" rcb {rc_b}");

                var ag = mp.DivideRec(rec.Q, exponent);
                if (ag.R != 0) {
                    var k = ag.R / mp;
                    var dpe = (k * powersOf_2.Exponent) + 1;

                    Print($"  ag {ag}");
                    Print($"   k {k} | {dpe}");
                }

                var bg = mp.Divide(rc_b);
                if (bg.R != 0) {
                    var k = bg.R / mp;
                    var dpe = (k * powersOf_2.Exponent) + 1;

                    Print($"  bg {bg}");
                    Print($"   k {k} | {dpe}");
                }

                var dp = isMp ? 1 : mp.DerivedPrime(powersOf_2.Exponent);

                Print($"  dp {dp}");

                precision += 2;
                powersOf_2.Next();
                Print();
                Prompt();
            }

        }

        private static void PowersOfTwo_v3() {

            var powersOf_2 = new PowersOf(2);
            BigDecimal.Precision = 32;

            while (true) {
                while (powersOf_2.Exponent < 7) {
                    powersOf_2.Next();
                }

                while (!powersOf_2.Exponent.IsProbablyPrime()) {
                    powersOf_2.Next();
                }

                var isMp = MersennePrimes.Contains(powersOf_2.Exponent);
                var mpTag = isMp ? "~" : "!";

                var n = powersOf_2.Value;
                var mp = n - 1;
                //var dr = (mp - 1).DigitalRoot();

                //var sqrt = n.Sqrt();
                var mp_sqrt = mp.IntegerSqrt();

                // 2^23 = 29
                // 2^29 = 32
                // 2^37 = 

                Print($" {mpTag}   2^{powersOf_2.Exponent} | {mp}");
                //Print($" 2^{powersOf_2.Exponent} | {mp}");
                Print($"   √ {mp_sqrt,16} | r {mp % mp_sqrt}");
                Print();

                //if (isMp) {
                //    powersOf_2.Next();
                //    Print();
                //    continue;
                //}


                //if (isMp) {
                //    powersOf_2.Next();
                //    Print();
                //    continue;
                //}

                //Trail39(mp, powersOf_2.Base, powersOf_2.Exponent);

                //Trial_v41(mp, powersOf_2.Base, powersOf_2.Exponent);
                //Trial_v42(mp, powersOf_2.Base, powersOf_2.Exponent);
                //Trial_v43(mp, powersOf_2.Base, powersOf_2.Exponent);
                //Trial_v46(mp, powersOf_2.Base, powersOf_2.Exponent);
                //Trial_v47(mp, powersOf_2.Base, powersOf_2.Exponent);

                //Trial_v49(mp, powersOf_2.Base, powersOf_2.Exponent);
                Trial_v44(mp, powersOf_2.Base, powersOf_2.Exponent);

                //Trail31(mp);
                //Trail32(mp, powersOf_2.Exponent);
                //Trail33(mp, powersOf_2.Exponent);
                //Trail35(mp, powersOf_2.Exponent);
                //Trail36(mp, powersOf_2.Exponent);
                //Trail37(mp, powersOf_2.Exponent);
                //Trail38(mp, powersOf_2.Exponent);
                //Trail39(mp, powersOf_2.Exponent);
                //Trail40(mp, powersOf_2.Base, powersOf_2.Exponent);

                powersOf_2.Next();
                //Print();
                Prompt();
            }

            static void Trail31(BigInteger n) {
                var n_integerSqrt = n.IntegerSqrt();

                var k = BigInteger.One;
                var s = BigInteger.One;
                var rc = BigInteger.Zero;
                var sq = BigInteger.Zero;
                var re = BigInteger.Zero;

                //(rc + (k * 4)) < n
                //rc < n
                //k < n_integerSqrt

                while (rc < n) {
                    if (k == 1) {
                        rc += k * 3;
                    }
                    else {
                        rc += k * 4;
                    }

                    sq = BigInteger.Pow(k + 1, 2) * 2;
                    sq += BigInteger.Pow(k, 2) * 2;
                    sq -= 3;
                    // ... sq = (rc * 2) + 1; ...

                    re = ((k + 1) * (k / 2)) * 2;
                    re += ((k - 1) * (k / 2)) * 2;

                    //k += 2;
                    k += 1;
                    s++;
                }

                Print($"rc {rc,16} √ {rc.IntegerSqrt()}");
                Print($"sq {sq,16} √ {sq.IntegerSqrt()}");
                Print($"re {re,16} √ {re.IntegerSqrt()}");
                //Print($" k {k,16}");
                Print($" s {s,16}");
            }

            static void Trail32(BigInteger n, BigInteger e) {
                var results = new List<BigInteger>();

                var es = e * 2;

                var st = BigInteger.One;

                var f = new BigInteger(4);
                var s = new BigInteger(4);
                for (BigInteger i = 0; i < e - 2; i++) {
                    //if (s != 0) {
                    //    st *= s;
                    //}

                    s = BigInteger.ModPow(s, 2, n) - 2;

                    if (s != 0) {
                        results.Add(s);
                    }

                    f += 2;
                }

                if (s == 0) {
                    // Prime
                }

                Print($"   s {s} | f {f}");
                //Print($"  st {st}");
                Print($"   r {string.Join(", ", results)}");
            }

            static void Trail33(BigInteger n, BigInteger e) {
                var results = new List<BigInteger>();

                for (int k = 2; k <= 10; k += 2) {
                    var es = e * k;

                    if (es % 10 != 4) {
                        results.Add(es);
                    }

                }

                Print($" r {string.Join(", ", results)}");
            }

            static void Trail35(BigInteger n, BigInteger e) {
                var x = (n - 1) / e;
                var e_x = e - 1;
                var e_p = n - 1;

                var s = x;
                while (e_x > 0) {
                    var e_pow = BigInteger.Pow(2, (int)e_x);
                    var e_par = (e_pow - 1);

                    if (e_par == 0) {
                        break;
                    }

                    var r = e_p % e_par;
                    if (r == 0) {
                        Print($" 2^{e_x,-8} {e_par,9}");
                    }

                    e_x--;
                }

                Print();
            }

            static void Trail36(BigInteger n, BigInteger e) {
                //var sqrt_2 = Math.Sqrt(2);
                //var sqrt_e = Math.Sqrt(e.ToDouble());

                var isMp = MersennePrimes.Contains(e);
                //if (!isMp) {
                //    return;
                //}

                var x = (n - 1) / e;
                var n_sqrt = n.IntegerSqrt();
                var x_sqrt = x.IntegerSqrt();
                var x2 = x;

                var e_x = n - 1;
                var e_y = e * 2;
                var e_z = BigInteger.Zero;
                var e_sqrd = e * e;
                var e_lmt = x;

                var isPow2 = false;
                var xSr = BigInteger.Zero;
                var attempts = BigInteger.Zero;

                (BigInteger X, BigInteger Y, BigInteger Z) e_r;
                while (attempts <= e_lmt) {
                    attempts++;

                    if (e_x % e_y != 0) {
                        e_y += 1;
                        continue;
                    }

                    if (e_y % e == 0) {
                        e_lmt = e_x / e_y;
                        e_y += 1;
                        continue;
                    }

                    e_r = e_y.Pos(e);
                    xSr = BigInteger.ModPow(e, e_y, n);
                    isPow2 = xSr != 1 && xSr.IsPowerOfTwo;

                    Print($" e_y {e_y,16} | r {xSr,8} {isPow2,6} | e_r {e_r}");

                    //var isPow2 = xSr.IsPowerOfTwo;
                    //if (isPow2) {
                    //    Print($" e_y {e_y,16} | y_r {y_r} | e_r {e_r} | r {xSr,8} {isPow2}");
                    //    //Prompt();
                    //}

                    //if ((e_r.X.IsPowerOfTwo || e_r.X % 3 == 0) || (e_r.Y.IsPowerOfTwo || e_r.Y % 3 == 0)) {
                    //    xSr = BigInteger.ModPow(e, e_y, n);

                    //    var isPow2 = xSr != 1 && xSr.IsPowerOfTwo;
                    //    if (isPow2) {
                    //        Print($" e_y {e_y,16} | e_r {e_y.AbsPos(e)} | r {xSr,8} | e_r {isPow2}");
                    //    }
                    //}

                    e_y += 1;
                }

                Print(" ~ done");
                Prompt();
            }

            static void Trail37(BigInteger n, BigInteger e) {

                var x = (n - 1) / e;
                var sqrtLimits = n.SqrtLimits();

                Print($" min {sqrtLimits.Min,16}");
                Print($" cen {sqrtLimits.Cen,16}");
                Print($" max {sqrtLimits.Max,16}");
                Print();

                var e_x = n - 1;
                var e_y = e * 2;
                var e_z = BigInteger.Zero;
                var e_sqrd = e * e;
                var e_lmt = x;

                var isPow2 = false;
                var xSr = BigInteger.Zero;
                var attempts = BigInteger.Zero;
                (BigInteger X, BigInteger Y, BigInteger Z) e_r;

                BigInteger h = 0, l = 0, c = 0;
                for (h = sqrtLimits.Max, l = 2; l < h; h--, l++) {

                    if (e_x % l == 0) {
                        if (l % e == 0) {
                            c = e_x / l;
                            if (c > sqrtLimits.Max) {
                                e_r = c.Pos(e);
                                xSr = BigInteger.ModPow(e, c, n);
                                isPow2 = xSr != 1 && xSr.IsPowerOfTwo;

                                Print($" e_c {c,16} | r {xSr,8} {isPow2,6} | e_r {e_r}");
                            }
                        }
                        else {
                            e_r = l.Pos(e);
                            xSr = BigInteger.ModPow(e, l, n);
                            isPow2 = xSr != 1 && xSr.IsPowerOfTwo;

                            Print($" e_l {l,16} | r {xSr,8} {isPow2,6} | e_r {e_r}");
                        }
                    }

                    if (e_x % h == 0) {
                        if (h % e != 0) {
                            e_r = h.Pos(e);
                            xSr = BigInteger.ModPow(e, h, n);
                            isPow2 = xSr != 1 && xSr.IsPowerOfTwo;

                            Print($" e_h {h,16} | r {xSr,8} {isPow2,6} | e_r {e_r}");
                        }
                    }

                }

                Print(" ~ done");
                Prompt();

                //var isPow2 = xSr.IsPowerOfTwo;
                //if (isPow2) {
                //    Print($" e_y {e_y,16} | y_r {y_r} | e_r {e_r} | r {xSr,8} {isPow2}");
                //    //Prompt();
                //}

                //if ((e_r.X.IsPowerOfTwo || e_r.X % 3 == 0) || (e_r.Y.IsPowerOfTwo || e_r.Y % 3 == 0)) {
                //    xSr = BigInteger.ModPow(e, e_y, n);

                //    var isPow2 = xSr != 1 && xSr.IsPowerOfTwo;
                //    if (isPow2) {
                //        Print($" e_y {e_y,16} | e_r {e_y.AbsPos(e)} | r {xSr,8} | e_r {isPow2}");
                //    }
                //}
            }

            static void Trail38(BigInteger n, BigInteger e) {
                var x = (n - 1) / e;
                var sqrtLimits = x.SqrtLimits();

                Print($" min {sqrtLimits.Min,16}");
                Print($" cen {sqrtLimits.Cen,16}");
                Print($" max {sqrtLimits.Max,16}");
                Print();

                var isMp = MersennePrimes.Contains(e);
                if (!isMp) {
                    return;
                }

                var e_x = n - 1;
                var e_y = e * 2;
                var e_z = BigInteger.Zero;
                var e_sqrd = e * e;
                var e_lmt = x;

                var isPow2 = false;
                var xSr = BigInteger.Zero;
                var attempts = BigInteger.Zero;
                (BigInteger X, BigInteger Y, BigInteger Z) e_r;

                BigInteger h = 0, l = 0, c = 0;
                for (h = sqrtLimits.Max + e, l = e; l < h; h--, l++) {

                    if (e_x % l == 0) {
                        if (l % e != 0) {
                            e_r = l.Pos(e);
                            xSr = BigInteger.ModPow(e, l, n);
                            isPow2 = xSr != 1 && xSr.IsPowerOfTwo;

                            if (isPow2) {
                                Print($" e_l {l,16} | r {xSr,8} {isPow2,6} | e_r {e_r}");
                            }
                        }
                    }

                    if (e_x % h == 0) {
                        if (h % e != 0) {
                            e_r = h.Pos(e);
                            xSr = BigInteger.ModPow(e, h, n);
                            isPow2 = xSr != 1 && xSr.IsPowerOfTwo;

                            if (isPow2) {
                                Print($" e_h {h,16} | r {xSr,8} {isPow2,6} | e_r {e_r}");
                            }
                        }
                    }

                }

                Print(" ~ done");
                Prompt();

                //var isPow2 = xSr.IsPowerOfTwo;
                //if (isPow2) {
                //    Print($" e_y {e_y,16} | y_r {y_r} | e_r {e_r} | r {xSr,8} {isPow2}");
                //    //Prompt();
                //}

                //if ((e_r.X.IsPowerOfTwo || e_r.X % 3 == 0) || (e_r.Y.IsPowerOfTwo || e_r.Y % 3 == 0)) {
                //    xSr = BigInteger.ModPow(e, e_y, n);

                //    var isPow2 = xSr != 1 && xSr.IsPowerOfTwo;
                //    if (isPow2) {
                //        Print($" e_y {e_y,16} | e_r {e_y.AbsPos(e)} | r {xSr,8} | e_r {isPow2}");
                //    }
                //}
            }

            static void Trail39(BigInteger n, BigInteger @base, BigInteger e) {
                //var isMp = MersennePrimes.Contains(e);

                //if (!isMp) {
                //    return;
                //}

                var d = n - 1;
                var x = (n - 1) / e;
                //var sqrtLimits = x.SqrtLimits();

                //Print($"   x {x,16}");
                //Print();

                var result = CheckPrimality(n, @base, e);
                //Print();
                //Print($"  rs {"---",16} | {result}");

                if (n.IsProbablyPrime() && !result) {
                    Print();
                    Print($" ! ERROR | false negitive");

                    Prompt();
                }
                else if (!n.IsProbablyPrime() && result) {
                    Print();
                    Print($" ! ERROR | false positive");

                    Prompt();
                }
                Print();
                

                //foreach (var powerOf in powerOf_2.Exponent.Factorize()) {
                //    Print($"  pe {powerOf,16}");
                //}
                //Print();

                //var powerOf_E = new PowerOf(e, 0);
                //while (powerOf_E.Value < n) {
                //    powerOf_E.Next();
                //}

                //while (powerOf_E.Exponent <= x) {
                //    var poe = powerOf_E.Value.Pos(n);

                //    if (poe.X == 1 || poe.Y == 1) {
                //        Print($" {powerOf_E,16} | poe {poe}");
                //        break;
                //    }
                //    else if (poe.X.IsPowerOfTwo || poe.Y.IsPowerOfTwo) {
                //        Print($" {powerOf_E,16} | poe {poe}");
                //        break;
                //    }

                //    powerOf_E.Next();
                //}

                //var dp_l = isMp ? 1 : n.DerivedPrime(e);
                //var dp_lk = dp_l > 1 ? (dp_l - 1) / e : 1;
                //Print($"  dl {dp_l,16} | e * {dp_lk,12}");

                //var dp_h = n / dp_l;
                //var dp_hk = (dp_h - 1) / e;
                //Print($"  dh {dp_h,16} | e * {dp_hk,12}");
                //Print();

                static bool CheckPrimality(BigInteger canidate, BigInteger baseOf, BigInteger exponent) {
                    if (canidate <= 0) {
                        return false;
                    }

                    var a = new BigInteger(2);
                    if (baseOf == 2) {
                        a = new BigInteger(3);
                    }

                    var v = BigInteger.Pow(a, (int)baseOf);
                    if (v > canidate) {
                        return false; // Too small for this method, but may be prime.
                    }

                    Print($" v {v,16}");

                    var two_e = exponent * 2;
                    var s = v;
                    for (int i = 1; i <= exponent; i++) {
                        s = BigInteger.ModPow(s, baseOf, canidate);

                        //var sp = s.Pos(exponent);

                        Print($"   i {i,3} | s {s,16}");
                        //Print($"   i {i,3} | s {s,16} x {s.IsPowerOf(),3} | sp {sp,14}");

                        //if (i == exponent - 3) {
                        //    foreach (var powerOf in s.Factorize()) {
                        //        Print($"  xs {powerOf,16}");
                        //    }
                        //    Print();
                        //}

                        if (s == v) {
                            return true; // Prime
                        }
                    }



                    //if (a == 2) {
                    //    if (s.IsPowerOfTwo) {
                    //        return true;
                    //    }
                    //}
                    //else if (s == v) {
                    //    return true; // Prime
                    //}

                    return false; // Composite
                }
            }

            static void Trail40(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);

                if (isMp) {
                    return;
                }

                var two_e = e * 2;
                var n_sqrt = n.IntegerSqrt();

                var d = n - 1;
                var x = (n - 1) / e;

                Print($"   x {x,16}");
                Print();

                //var i = BigInteger.One;
                //var z = BigInteger.One;
                //var k = e;
                //var s = BigInteger.Zero;
                //var s_sqrd = BigInteger.Zero;
                //var s_x = BigInteger.Zero;
                //var s_x_sqrt = BigInteger.Zero;
                //var x_h = BigInteger.Zero;
                //var x_l = BigInteger.Zero;
                //while (k < x) {

                //    s = k + 1;
                //    s_sqrd = BigInteger.Pow(s, 2);
                //    s_x = BigInteger.Abs(s_sqrd - n);
                //    s_x_sqrt = s_x.IntegerSqrt();
                //    x_h = s + s_x_sqrt;
                //    x_l = s - s_x_sqrt;

                //    if (x_l > 1) {
                //        if (n % x_l == 0 && n % x_h == 0) {
                //            Print($"   s {s,16} q {s_sqrd,8} i {i,8} z {z,8} | l {x_l,8} h {x_h,8}");
                //            Print($"   s {s_x_sqrt,16} x {s_x,8} s_pos {n.Pos(s_x_sqrt)}");

                //            //foreach (var fct in (s_x_sqrt).Factorize()) {
                //            //    Print($"   f {fct,16} | {n.Pos(fct.Calculate())}");
                //            //}

                //            break;
                //        }
                //    }

                //    //Print($"   k {k,8} l {x_l,8} h {x_h,8}");

                //    k += e;
                //    i++;

                //    if (x_l < 0) {
                //        z++;
                //    }
                //}
                //Print();

                var q = 3 * e;
                var g = q;

                for (int i = 0; i < e; i++) {


                    g *= q;
                }

                //var o = BigInteger.One;
                //var result = BigInteger.Zero;
                //var q = BigInteger.Zero;

                //while (q < n) {
                //    q += o;
                //    o += 1;
                //    result++;

                //    //Print($"   i {result,8} q {q,8} o {o,8}");
                //}
                //Print($"   q {q,16} i {result,8} o {o,8}");
                //Print();



                //var o = BigInteger.One;
                //var q = BigInteger.Zero;

                //var m = BigInteger.Zero;
                //var i = BigInteger.One;
                //while (m < n) {
                //    if ((n - q) % o == 0) {
                //        m += q;
                //        Print($"   o {o,16}");
                //    }

                //    //m += q;
                //    o += 2;
                //    q += 1;

                //    i++;
                //}

                //Print($"   m {m,16} i {i,8} q {q,8} o {o,8}");

                Print();
                Prompt();
            }

            static void Trial_v41(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);

                if (isMp) {
                    return;
                }

                var two_e = e * 2;
                var thr_e = e * 3;
                var n_sqrt = n.IntegerSqrt();
                //var s_r = n % n_sqrt;
                //var s_d = n_sqrt - s_r;

                //var s_dr = s_d % e == 1 ? "<" : ">";
                //Print($"   r {s_r,16}");
                //Print($"   ~ {s_d,16} {s_dr}");

                var d = n - 1;
                var x = (n - 1) / e;

                Print($"   x {x,16}");
                Print();

                //var t_h5 = (n + 5).GetPowerOf(3);
                //var t_h2 = (n + 2).GetPowerOf(3);
                //var t_l1 = (n - 1).GetPowerOf(3);
                //var t_l4 = (n - 4).GetPowerOf(3);

                //Print($"   -4 {t_l4,-16} -1 {t_l1,-16} +2 {t_h2,-16} +5 {t_h5,-16}");
                //Print();

                var i = BigInteger.One;
                var z = BigInteger.One;
                var k = e;
                var s = BigInteger.Zero;
                var s_sqrd = BigInteger.Zero;
                var s_x = BigInteger.Zero;
                var s_x_sqrt = BigInteger.Zero;
                var x_h = BigInteger.Zero;
                var x_l = BigInteger.Zero;
                while (k < x) {

                    s = k + 1;
                    s_sqrd = BigInteger.Pow(s, 2);
                    s_x = BigInteger.Abs(s_sqrd - n);
                    s_x_sqrt = s_x.IntegerSqrt();
                    x_h = s + s_x_sqrt;
                    x_l = s - s_x_sqrt;

                    if (x_l > 1) {
                        if (n % x_l == 0 && n % x_h == 0) {
                            Print($"   s {s,16} q {s_sqrd,16} i {i,8} z {z,8} | l {x_l,8} h {x_h,8}");
                            Print($"   s {s_x_sqrt,16} x {s_x,16} sps {n.Pos(s_x_sqrt).X}");

                            var v = BigInteger.One;
                            foreach (var power in s_x_sqrt.Factorize()) {
                                power.Calculate();

                                v *= power.Value;

                                Print($" s_p {power,16} v {v,16} vps {n.Pos(v)}");
                            }

                            break;
                        }
                    }

                    //Print($"   k {k,8} l {x_l,8} h {x_h,8}");

                    k += e;
                    i++;

                    if (x_l < 0) {
                        z++;
                    }
                }

                Print();
                //Prompt();
            }

            static void Trial_v42(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);

                if (isMp) {
                    return;
                }

                var two_e = e * 2;
                var n_sqrt = n.IntegerSqrt();

                var d = n - 1;
                var x = (n - 1) / e;

                var m = e;
                var g = BigInteger.Zero;

                while (g < n) {
                    if (d % m == 0) {
                        Print($"   m {m,16}");
                    }

                    m += e;
                    g += two_e;
                }

                Print();
                //Prompt();
            }

            static void Trial_v43(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);

                if (isMp) {
                    return;
                }

                var d = n - 1;
                var x = (n - 1) / 2;

                var two_e = e * 2;
                var thr_e = e * 3;
                var n_sqrt = n.IntegerSqrt();

                var k = new PowerOf(3, 0);
                k.Next();

                while (k.Value < n_sqrt) {

                    var eps = k.Value.Pos(e);
                    if (eps.X.IsPowerOfTwo || eps.Y.IsPowerOfTwo) {
                        Print($"   k {k,16} {eps,16} | {n % k.Value}");
                    }

                    k.Next();
                }

                Print();
            }

            static void Trial_v44(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);

                if (isMp) {
                    return;
                }

                var two_e = e * 2;
                var thr_e = e * 3;
                var ttw_e = thr_e * 2;
                var hlf_e = (e - 1) / 2;

                var n_sqrt = n.IntegerSqrt();

                var d = n - 1;

                var x = (n - 1) / e;
                var y = (n - 1) / thr_e;
                var z = BigInteger.Pow(thr_e, 2) * e;

                Print($"   x {x,16}");
                Print($"   y {y,16}");
                Print($"   z {z,16}");
                Print();
                Print($"   k {thr_e,16}");
                Print();

                var i = BigInteger.One;

                var k = thr_e;
                var k_sqrd = BigInteger.Zero;

                var s = BigInteger.Zero;
                var s_sqrt = BigInteger.Zero;

                var x_h = BigInteger.Zero;
                var x_l = BigInteger.Zero;

                while (k < x) {
                    k_sqrd = BigInteger.Pow(k, 2);

                    s = n + k_sqrd;
                    s_sqrt = s.IntegerSqrt();

                    x_h = k + s_sqrt;
                    x_l = s_sqrt - k;

                    //if (i > thr_e) {
                    //    if (i - (k % i) == thr_e) {
                    //        Print($"   ~ {i} ~~~~~~~~~~~~~~~~~~~~~");
                    //    }
                    //}

                    // && n % x_h == 0
                    if (n % x_l == 0 || n % x_h == 0) {
                        Print($"   √ {k,16} k {k_sqrd,16} | i {i,8} r {i % e,8}");

                        Print($"   √ {s_sqrt,16} s {s,16} | {x_l,8} * {x_h,-8}");
                        Print();

                        //Print($"   q {new BigDecimal(y, k).WholeValue,16}");

                        //var v = BigInteger.One;
                        //foreach (var power in k.Factorize()) {
                        //    //if (power.Base == e) {
                        //    //    continue;
                        //    //}

                        //    power.Calculate();

                        //    v *= power.Value;

                        //    Print($"     {power,16} v {v,16}");
                        //}
                        //Print();

                        break;
                    }

                    k += ttw_e;
                    i++;
                }

                Print();
                //Prompt();
            }

            static void Trial_v45(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);

                if (isMp) {
                    return;
                }

                //var two_e = e * 2;
                //var n_sqrt = n.IntegerSqrt();

                //var d = n - 1;
                //var d_sqrd = BigInteger.Pow(d, 2);

                //var l = BigInteger.Zero;
                //var h = BigInteger.Zero;
                //var m = BigInteger.Zero;
                //var g = BigInteger.Zero;
                //while (d < d_sqrd) {
                //    h = d;
                //    l = h / 2;
                //    m = (h + l) / 2;
                //    g = h + l + m;

                //    Print($"   l {l,16} h {h,16} m {m,16} g {g,16}");

                //    d *= 2;
                //}

                var thr_e = 3 * e;
                var o = BigInteger.Zero;
                var q = BigInteger.Zero;

                var m = BigInteger.Zero;
                var i = BigInteger.One;
                while (m < n) {
                    //if (n % (o + 1) == 0) {
                    //    //m += q;
                    //    Print($"   o {o,16} | {n.Pos(o)}");
                    //}

                    m += o;
                    o += 3;
                    q += 1;

                    i++;
                }

                Print($"   m {m,16} i {i,8} q {q,8} o {o,8}");

                Print();
                //Prompt();
            }

            static void Trial_v46(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);
                if (isMp) {
                    return;
                }

                //var two_e = e * 2;
                //var n_sqrt = n.IntegerSqrt();

                //var d = n - 1;
                //var d_sqrd = BigInteger.Pow(d, 2);

                //var l = BigInteger.Zero;
                //var h = BigInteger.Zero;
                //var m = BigInteger.Zero;
                //var g = BigInteger.Zero;
                //while (d < d_sqrd) {
                //    h = d;
                //    l = h / 2;
                //    m = (h + l) / 2;
                //    g = h + l + m;

                //    Print($"   l {l,16} h {h,16} m {m,16} g {g,16}");

                //    d *= 2;
                //}

                var thr_n = 4 * n;
                var thr_e = 3 * e;
                var o = BigInteger.Zero;
                var q = thr_e;

                var m = BigInteger.Zero;
                var i = BigInteger.One;
                for (int j = 0; j < 6; j++) {
                    var qps = thr_n.Pos(q);

                    Print($"   q {q,16} {qps}");

                    q *= 3;
                }

                //Print($"   m {m,16} i {i,8} q {q,8} o {o,8}");

                Print();
                //Prompt();
            }

            static void Trial_v47(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);
                if (isMp) {
                    return;
                }

                var thr_e = 3 * e;

                var powerOf_2 = new PowerOf(2, 0);
                powerOf_2.Next();

                for (BigInteger k = 2; k < e * 3; k++) {
                    var x = n * k;
                    var s = x.IntegerSqrt();
                    var d = n / powerOf_2.Value;



                    Print($"   d {d,16} s {s,16} | {s % thr_e}");

                    powerOf_2.Next();
                }

                Print();
                //Prompt();
            }

            static void Trial_v48(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);
                if (isMp) {
                    return;
                }

                var n_sqrt = n.IntegerSqrt();
                var d = (n + 1) / 2;
                var d_e = 0;
                while (d > 1) {
                    if (d.IsPowerOf(3)) {
                        var power = d.GetPowerOf(3);
                        d_e += (int)power.Exponent;
                        Print($"   d {d,16} | {power}");
                    }
                    //Print($"   d {d,16}");

                    d = (d + (d / 2)) / 2;
                }



                Print($"   3^{d_e,16} | {BigInteger.Pow(3, (int)d_e)}");

                Print();
                //Prompt();
            }

            static void Trial_v49(BigInteger n, BigInteger @base, BigInteger e) {
                var isMp = MersennePrimes.Contains(e);
                if (isMp) {
                    return;
                }

                var thr_n = 3 * n;
                var powerOf_3 = new PowerOf(3, 0);
                powerOf_3.Next();

                while (powerOf_3.Value < thr_n) {
                    powerOf_3.Next();
                }

                var h = powerOf_3.Value;
                var l = powerOf_3.Value / powerOf_3.Base;
                var z = h - l;

                Print($"   h {h,16} | {powerOf_3}");
                Print($"   l {l,16} | {l * (e * (e + 1)),16}");
                Print($"   z {z,16}");

                Print();
                //Prompt();
            }

        }

        private static void PowersOf3_v1() {

            var powersOf_3 = new PowersOf(3);

            while (true) {
                while (!powersOf_3.Exponent.IsProbablyPrime()) {
                    powersOf_3.Next();
                }

                var p = powersOf_3.Value - 2;
                var p_sqrt = p.IntegerSqrt();

                Print($"   2^{powersOf_3.Exponent} | {p}");
                Print($"   √ {p_sqrt,16}");
                Print();

                Trial_v44(p, powersOf_3.Base, powersOf_3.Exponent);

                Prompt();
                powersOf_3.Next();
            }

            static void Trial_v44(BigInteger n, BigInteger @base, BigInteger e) {
                //var isMp = MersennePrimes.Contains(e);

                //if (isMp) {
                //    return;
                //}

                var two_e = e * 5;
                var thr_e = e * 3;
                var ttw_e = thr_e * 2;

                var n_sqrt = n.IntegerSqrt();

                var d = n - 1;
                var x = (n - 1) / e;
                var y = (n - 1) / thr_e;

                //var y_s = BigInteger.ModPow(5, y, n);

                Print($"   x {x,16}");
                Print($"   y {y,16}");
                Print($"   k {thr_e,16}");
                Print();

                var i = BigInteger.One;

                var k = e;
                var k_sqrd = BigInteger.Zero;
                var s = BigInteger.Zero;
                var s_sqrt = BigInteger.Zero;

                var x_h = BigInteger.Zero;
                var x_l = BigInteger.Zero;

                while (k < x) {
                    k_sqrd = BigInteger.Pow(k - 2, 2);

                    s = BigInteger.Abs(n + k_sqrd);
                    s_sqrt = s.IntegerSqrt();

                    x_h = k + s_sqrt;
                    x_l = s_sqrt - k;

                    if (x_l > 1) {
                        if (n % x_l == 0 && n % x_h == 0) {
                            Print($"   √ {k,16} k {k_sqrd,16} i {i,8} | l {x_l,8} h {x_h,8}");
                            Print($"   √ {s_sqrt,16} s {s,16}");

                            Print($"   i {$"{e}^1",16} v {e,16} nps {n.Pos(e)}");

                            var v = e;
                            foreach (var power in k.Factorize()) {
                                if (power.Base == e) {
                                    continue;
                                }

                                power.Calculate();

                                v *= power.Value;

                                Print($"   i {power,16} v {v,16} nps {n.Pos(v)}");
                            }

                            //v /= e;

                            //Print($"   v {v,16} vps {n.Pos(v)}");

                            break;
                        }
                    }

                    //Print($"   k {k,8} l {x_l,8} h {x_h,8}");

                    k += e;
                    i++;
                }

                Print();
            }
        }

        private static void PowersOfProbablyNot() {

            var n = new BigInteger(3);

            while (true) {
                var x = ((n - 1) / 2) * (n - 1);

                if (x.IsPowerOfTwo) {
                    Print($" p {n} | {x}");
                    Prompt();
                }

                n++;
            }

            //foreach (var p in PrimeNumbers.Get()) {
            //    var x = ((p - 1) / 2) * (p - 1);

            //    if (x.IsPowerOfTwo) {
            //        Print($" p {p} | {x}");
            //        Prompt();
            //    }

            //}

        }

        private static List<(BigInteger X, BigInteger Y, BigInteger Z)> TimesTable(int from, int to) {
            var results = new List<(BigInteger X, BigInteger Y, BigInteger Z)>();

            for (BigInteger x = from; x <= to; x++) {
                for (BigInteger y = x; y <= to; y++) {
                    results.Add((x, y, x * y));
                }
            }

            results = results.OrderBy(result => result.Z).ToList();
            return results;
        }

        private static (BigInteger D, BigInteger DigitalRoot) InvestigateMore(BigInteger n, int chunkSize) {
            var n_digits = n.Digits();
            var chunksOfDigits = n_digits.Chunk(chunkSize).ToList();
            chunksOfDigits.Reverse();

            var dc = BigInteger.Zero;
            var ds = BigInteger.Zero;
            var drts = BigInteger.One;

            var ci = chunksOfDigits.Count - 1;
            foreach (var pairOfDigits in chunksOfDigits) {
                var i = pairOfDigits.Length - 1;
                var dr = BigInteger.Zero;

                if (dc != 0) {
                    if (pairOfDigits[i] == 0) {
                        var zi = i;
                        while (zi >= 0 && pairOfDigits[zi] == 0) {
                            pairOfDigits[zi] = 9;
                            zi--;
                        }

                        if (zi >= 0 && pairOfDigits[zi] != 0) {
                            pairOfDigits[zi] -= dc;
                            dc = 0;
                        }
                        else {
                            dc = 1;
                        }
                    }
                    else {
                        pairOfDigits[i] -= dc;
                        dc = 0;
                    }
                }

                var dk = BigInteger.One;
                while (i >= 0) {
                    if (ci != 0 && i == 0 && pairOfDigits[i] == 0) {
                        dr += (1 * dk);
                        dc = 1;
                    }
                    else {
                        dr += (pairOfDigits[i] * dk);
                    }
                    dk *= 10;
                    i--;
                }

                ds += dr;

                var drt = dr.DigitalRoot();
                drts *= drt;

                //Print($" ci {ci} | dr {dr,8} | r {n % dr} | c {dc,3}");

                ci--;
            }

            //Print($"ds {ds,8} | drt {ds.DigitalRoot()}");

            return (ds, ds.DigitalRoot());
        }

        private static void BaseIntegerTesting() {
            var maxUInt = new BigInteger(uint.MaxValue);

            //Print($"mu {maxUInt}");

            var rng = new Random();

            var stopWatch = new Stopwatch();
            var sign = true;

            while (true) {
                //(int leftLength, int rightLength) = (rng.Next(3, 3), rng.Next(1, 1));
                (int leftLength, int rightLength) = (3, 2);

                (BigInteger l, BigInteger r) = (PrimeNumbers.Random(leftLength, prime: false), PrimeNumbers.Random(rightLength, prime: false));

                //(BigInteger l, BigInteger r) = (BigInteger.Parse("8151"), BigInteger.Parse("57"));

                //l = sign ? -l : l;
                //r = sign ? r : -r;

                //if (r == 1) {
                //    r++;
                //}

                var a = BigInteger.Parse(l.ToString());
                var b = BigInteger.Parse(r.ToString());

                var x = BaseInteger.Parse(l.ToString());
                var y = BaseInteger.Parse(r.ToString());

                Clear();

                stopWatch.Restart(); var c = a - b; stopWatch.Stop();
                var cElapsed = $"{stopWatch.Elapsed}";

                stopWatch.Restart(); var z = x - y; stopWatch.Stop();
                var zElapsed = $"{stopWatch.Elapsed}";

                Print($" L {x,12}");
                Print($" R {y,12}");

                //var zStr = string.Join(".", z.Digits);

                if (z.ToString() != c.ToString()) {
                    Print();
                    Print($" Z {z,12} | {zElapsed}", ConsoleColor.Red);
                    Print();
                    Print($" C {c,12} | {cElapsed}", ConsoleColor.Red);
                }
                else {
                    Print();
                    Print($" Z {z,12} | {zElapsed}");
                    Print();
                    Print($" C {c,12} | {cElapsed}");
                }

                //Print($" X {(BaseInteger.LongDivision(x, y)),12} | {cElapsed}");

                sign = !sign;

                //Task.Delay(1000 / 30).Wait();
                Prompt();
            }

            //for (BigInteger n = ten; n < ten * 2; n++) {
            //    var bits = n.GetBits();
            //    var bitString = string.Join(", ", bits);

            //    Print($" n {n} | {bitString}");
            //    Prompt();
            //}

            //foreach (var p in PrimeNumbers(maxUInt * 100000)) {
            //    Print($" p {p}");

            //    var u = new UltraInteger(p);

            //    Print($" u {u}");

            //    Prompt();
            //}

            Prompt();
        }

        private static void BaseTesting() {
            var q = PrimeNumbers.Random(numberOfDigits: 1, prime: false);

            while (true) {
                var a = PrimeNumbers.Random(numberOfDigits: 6, prime: true);
                var b = PrimeNumbers.Random(numberOfDigits: 6, prime: true);
                var n = a * b;
                var d = a > b ? a - b : b - a;

                Clear();
                Print($" n {n} | drt {n.DigitalRoot()}");
                Print($" a {a} | drt {a.DigitalRoot()}");
                Print($" b {b} | drt {b.DigitalRoot()}");
                Print($" d {d} | drt {d.DigitalRoot()}");

                var digitalProduct = n.DigitalProduct();
                var dp = digitalProduct.Sum * digitalProduct.Product;

                Print($" srt {digitalProduct.Sum} | prt {digitalProduct.Product} = {dp}");
                Print($" dp {dp} | r {n % dp}");
                //var b_digits = b.Digits();

                //foreach (var b_digit in b_digits) {
                //    var result = Divide(n, (int)b_digit);

                //    Print($" q {result.Quotient} | r {result.Remainder}");
                //}

                //Print();

                Prompt();
            }

            static (BigInteger Quotient, BigInteger Remainder) Divide(BigInteger n, int digit) {
                var qs = new StringBuilder();

                var n_digits = n.Digits();
                var carry = 0;
                var d = BigInteger.Zero;
                foreach (var pairOfDigits in n_digits.Chunk(2)) {
                    d = BigInteger.Zero;

                    if (pairOfDigits.Length == 2) {
                        d = ((pairOfDigits[0] * 10) + pairOfDigits[1]);
                    }
                    else {
                        d = pairOfDigits[0];
                    }

                    if (carry > 0) {
                        d += carry * (int)Math.Pow(10, pairOfDigits.Length);
                    }

                    (int dq, int dr) = Math.DivRem((int)d, digit);

                    var qsp = dq.ToString().PadLeft(pairOfDigits.Length, '0');
                    qs.Append(qsp);

                    //Print($" d {d,3} / {digit,1}  | c {carry,2} | q {qsp,2} | r {dr,2}");

                    carry = dr;
                }

                return (BigInteger.Parse(qs.ToString()), carry);
            }
        }

        private static void PerfectNumbers() {
            BigDecimal.Precision = 16;

            var stopWatch = new Stopwatch();
            var e = new BigInteger(1);
            var n = new BigDecimal(10);

            var t = BigInteger.Zero;
            var irrationality = new Irrationality();

            while (true) {
                //while (!e.IsProbablyPrime()) {
                //    n *= 2;
                //    e++;
                //}

                Clear();

                var mp = n; // + 1;

                stopWatch.Restart();
                var a = mp.WholeValue.NumberOfDigits(); stopWatch.Stop();
                Print($" e {e} | a {a} | {stopWatch.Elapsed}");
                
                stopWatch.Restart();
                var b = mp.WholeValue.ToString().Length; stopWatch.Stop();
                Print($" e {e} | b {b} | {stopWatch.Elapsed}");


                //if (a != c) {
                //    Print($" e {e} | d {mp.NumberOfDigits()} | c {c}");
                //    Prompt();
                //}

                Prompt();

                //if (!mp.IsProbablyPrime()) {
                //    Print($" e {e} | mp {mp}");

                //    var twoE = 2 * e;
                //    var k = twoE;

                //    while (mp % (k + 1) != 0) {
                //        k += twoE;
                //    }

                //    Print($" k {k}");

                //    var eDivisors = (e + 1).Divisors(-1);
                //    Print($" d {string.Join(", ", eDivisors.Where(x => k % x == 0))}");

                //    Prompt();
                //}

                n *= 10;
                e++;
            }
        }

        private static void BigVectors() {
            BigDecimal.Precision = 16;


            var integerLength = 6;

            while (true) {
                Clear();

                var rsa = RsaInteger.Generate(leftLength: integerLength, rightLength: integerLength).Take(1).First();
                var numberOfDigits = rsa.N.NumberOfDigits();
                var integerSqrt = rsa.N.IntegerSqrt();

                var n = rsa.N;
                var r = rsa.R;
                var l = rsa.L;

                var sp = new BigDecimal(integerSqrt, rsa.N);
                var rp = new BigDecimal(rsa.R, n);
                var lp = new BigDecimal(rsa.L, n);

                var rdi = new BigDecimal(sp.Mantissa, exponent: sp.Exponent + (numberOfDigits / 2));
                Print($"rdi {rdi}");

                Print($" n {n} ({numberOfDigits})");
                Print($" √ {integerSqrt} | % {sp}");
                Print($" r {r} | % {rp}");
                Print($" l {l} | % {lp}");
                Print();

                var ten = BigInteger.Pow(10, numberOfDigits - 1);

                Print($"√2 {Constants.SqrtOf2}");
                Print($"√0 {Constants.SqrtOf10}");

                var t1 = Constants.SqrtOf2 / Constants.SqrtOf10;
                var t2 = Constants.SqrtOf10 / Constants.SqrtOf2;
                Print($"t1 {t1}");
                Print($"t2 {t2}");
                Print();

                var powersOf10 = new PowersOf(10);

                while (true) {
                    var x = powersOf10.Next();

                    var bits = x.GetPrivateField<uint[]>("_bits");
                    if (bits == null) { // TOO SMALL
                        continue;
                    }

                    Print($" x {x}");

                    foreach (var bit in bits) {
                        Print($" b {bit} | r {n / bit}");
                    }

                    Print();

                    NumericsHelpers.DangerousMakeTwosComplement(bits);
                    foreach (var bit in bits) {
                        Print($" b {bit} | r {n / bit}");
                    }

                    Print();

                    Prompt();
                }


                //var powersOf = new PowersOfDecimals(10);

                //var nSqrd = n * n;
                //var k = new BigDecimal(n);
                //var s = rdi;
                //var iterations = BigInteger.Zero;
                //while (true) {
                //    //k *= s;
                //    k = (n * ((powersOf.Value - 1) / 3));
                //    powersOf.Next();

                //    var m = k.WholeValue;
                //    var lr = m % l;
                //    var rr = m % r;

                //    if (lr <= 10 || rr <= 10) {
                //        Print($" i {iterations} | m {m} | lr {lr} | rr {rr}");
                //        break;
                //    }

                //    s += rdi;
                //    iterations++;
                //}

                //var rp = radian / pd;
                //var rn = rp + n;

                //Print($"rp {rp} | gcd {rp.WholeValue.Gcd(n)}");
                //Print($"rn {rn} | gcd {rn.WholeValue.Gcd(n)}");

                //Print($"rd {radians} | gcd {radians.WholeValue.Gcd(n)}");

                //Print($"rr {rr} | gcd {rr.WholeValue.Gcd(n)}");

                //Print($"nr {nr} | gcd {nr.WholeValue.Gcd(n)}");

                //Print($" r {nr / r} | r");
                //Print($" l {nr / l} | r");
                Print();

                Prompt();
            }
        }

        private static void UnmultiplyBackward() {
            // 19801501 * 50150449 = 993054166023949
            // 115049849 * 608560489 = 70014792366816161

            var n = BigInteger.Parse("70014792366816161");
            var numOfDigits = n.NumberOfDigits();
            var integerSqrt = n.IntegerSqrt();
            var digitalRoot = n.DigitalRoot();
            var digitalProduct = n.DigitalProduct();

            Print($" n {n}:{digitalProduct} | digits {numOfDigits}");
            Print($" √ {integerSqrt}");

            var unmultiply = new Unmultiply(n);
            // left: 1, right: 9

            // 101 * 49(sq) = 4949
            // 501 * 449 = 224949
            // 1501 * 50449 = 75723949
            // 1501 * 10449
            // 101501:0 * 50449:0 = 5120623949:0
            // 801501:0 * 150449:0 = 120585023949:311040
            // 9801501:0 * 10150449:0

            foreach (var result in unmultiply.Resultsv2(left: 1, right: 9)) {

                //Print($" {result.Left}:{digitalProduct % result.Left.DigitalProduct()} * {result.Right}:{digitalProduct % result.Right.DigitalProduct()} = {result.Value}:{digitalProduct % result.Value.DigitalProduct()}");
            }

            Prompt();
        }

        private static void UnmultiplyTesting2() {
            var stopWatch = new Stopwatch();
            // Failed?:
            // 376570435171267
            // 320507624732947 = 1939097 * 165287051
            // 655243211124301 = 3018989 * 217040609

            foreach (var rsaInt in RsaInteger.Generate(leftLength: 6, rightLength: 6)) {
                var n = rsaInt.N;
                var numOfDigits = n.NumberOfDigits();
                var integerSqrt = n.IntegerSqrt();

                var unmultiply = new Unmultiply(n);

                UnmultiplyResult finalResult = null;

                stopWatch.Restart();

                Execute(() => {
                    finalResult = unmultiply.Try();
                });

                while (true) {
                    Console.Clear();

                    Print($" n {n} | digits {numOfDigits}");
                    Print($" √ {integerSqrt}");

                    Print($" r {rsaInt.R}");
                    Print($" l {rsaInt.L}");

                    Print($" i {unmultiply.Iterations} | stack {unmultiply.StackCount}");

                    if (finalResult != null) {
                        break;
                    }

                    Thread.Sleep(1000 / 30);
                }

                if (finalResult == null) {
                    unmultiply.Stop();
                }

                if (finalResult != null) {
                    Print($" T {finalResult.Left} * {finalResult.Right} = {finalResult.Value} | {stopWatch.Elapsed}");
                }

                Print($" ~ done");
                Prompt();
            }
        }

        private static void UnmultiplyTesting() {
            var stopWatch = new Stopwatch();

            BigDecimal.Precision = 12;

            //Rng(9);

            // 19801501 * 50150449 = 993054166023949
            // 11912569 * 73602769 = 876798064303561
            // 48838991 * 91408441 = 4464296027323031
            // 30724483 * 98600857 = 3029460354681931

            // 148208191 * 943933631 = 139898695874571521

            // 115049849 * 608560489 = 70014792366816161
            // 257033449 * 558321889 = 143507400781865161
            // 558321889 * 558321889 = 311723331736528321

            // 142914236976659 * 837316179717533 = 119664402932542307266534062247
            // 163528139777869 * 984344909416061 = 160968091936623422383040954009

            // RSA-100
            // 1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139
            // 37975227936943673922808872755445627854565536638199
            // 40094690950920881030683735292761468389214899724061

            // d 11912569 | i 8849104 | 00:00:00.5482508
            // d 11912569 | i 8849104 | 00:00:00.6962422
            // d 11912569 | i 7079283 | 00:00:00.6416539

            // 17095570477 * 75444843049 = 1289772631470383064373
            //  2901761809 *  8932218571 =   25919170718968354939

            //   148208191 *   943933631 =     139898695874571521
            // d 148208191 | i        0 | 00:00:03.3301951
            // d 148208191 | i        0 | 00:00:03.3108210
            // d 148208191 | i        0 | 00:00:03.1785193

            // d 148208191 | i 90328860 | 00:00:03.7621485
            // d 148208191 | i 90328860 | 00:00:03.7628687
            //              i 129259041

            Prompt();
            //stopWatch.Start();

            var n = BigInteger.Parse("139898695874571521");
            var numOfDigits = n.NumberOfDigits();
            var integerSqrt = n.IntegerSqrt();

            var d = BigInteger.Zero;
            var iterations = BigInteger.Zero;

            Execute(() => {
                d = n.SqrtBoundary(integerSqrt, ref iterations);

                stopWatch.Stop();
            });

            Print($" d {d} | i {iterations} | {stopWatch.Elapsed}");

            while (true) {
                Clear();
                Print($" n {n} | digits {numOfDigits}");
                Print($" √ {integerSqrt}");

                Print($" d {d} | i {iterations} | {stopWatch.Elapsed}");

                Task.Delay(1000 / 30).Wait();
            }

            Prompt();
            //var unmultiply = new Unmultiply(n);

            //UnmultiplyResult finalResult = null;

            ////finalResult = unmultiply.Process(reverse: true);

            //Execute(() => {
            //    //finalResult = unmultiply.Process(reverse: true);
            //    finalResult = unmultiply.ProcessParallel(reverse: true);
            //});

            //while (true) {
            //    Clear();
            //    Print($" n {n} | digits {numOfDigits}");
            //    Print($" √ {integerSqrt}");
            //    Print($" i {unmultiply.Iterations} | elims {unmultiply.Eliminations} | stack {unmultiply.StackCount} | {unmultiply.RateOfFailure()}");

            //    if (finalResult != null) {
            //        break;
            //    }

            //    Task.Delay(1000 / 30).Wait();
            //}

            //if (finalResult != null) {
            //    Print($" i {unmultiply.Iterations} | {finalResult.Left} * {finalResult.Right} = {finalResult.Value} | {stopWatch.Elapsed}");
            //}

            //Print($" done | {stopWatch.Elapsed}");
            //Prompt();
        }

        private static void UnmultiplySqrt() {
            BigDecimal.Precision = 9;

            // 7 * 23 = 161
            // n *  3 = 483

            // 298489 = 421 * 709
            // 895467 =   n *   3

            //var k = new BigInteger(298489);

            //var n = k + k.Complement(9);

            //var x = new BigInteger(298489);

            //var gcd = n.Gcd(k);
            //var lcm = n.Gcd(k);

            //var nSqrt = n.IntegerSqrt();
            //var kSqrt = k.IntegerSqrt();
            //var nkR = nSqrt / kSqrt;

            //Print($" n {n} | k {k} | gcd {gcd} | lcm {lcm}");
            //Print($" n {nSqrt} | k {kSqrt} | nkR {nkR}");

            var p = new BigInteger(19);
            var n = BigInteger.Pow(2, (int)p);



            while (true) {
                if (!p.IsProbablyPrime()) {
                    n *= 2;
                    p++;
                    continue;
                }

                var mp = n - 1;
                var integerSqrt = mp.IntegerSqrt();

                Print($" p {p} | n {mp}");
                Print($"   √ {integerSqrt}");

                var iterations = BigInteger.Zero;
                //var boundary = mp.SqrtBoundary(integerSqrt, ref iterations);
                //var boundary = mp.MpBoundary(p, integerSqrt, ref iterations);
                //Print($"   b {boundary} | i {iterations}");

                if (!mp.IsProbablyPrime()) {
                    var unmultiply = new Unmultiply(mp);
                    var finalResult = unmultiply.ProcessParallel(reverse: true);

                    Print($"   {finalResult.Left} * {finalResult.Right}");
                }
                else {
                    Print($"   1 * n");
                }

                Prompt();

                n *= 2;
                p++;
            }

            Prompt();

            //foreach (var rsa in RsaInteger.Generate(leftLength: 9, rightLength: 9)) {
            //    Clear();

            //    var n = rsa.N;
            //    var digits = n.Digits();
            //    var integerSqrt = n.IntegerSqrt();

            //    var d = integerSqrt - rsa.L;

            //    Print($" n {n} | 9s {n.Complement(9)}");
            //    Print($" √ {integerSqrt}");

            //    Print($" = {rsa.L} * {rsa.R}");
            //    Print($" d {d}");

            //    Print($" r {rsa.R}");
            //    Print($" l {rsa.L}");

            //    var iterations = BigInteger.Zero;
            //    var sqrtB = n.SqrtBoundary(integerSqrt, ref iterations);

            //    var per = new BigDecimal(iterations, n);

            //    Print($" i {iterations} | s {sqrtB} | % {per}");

            //    Prompt();
            //}


        }

        private static void MinMaxSqrts() {
            // 19801501 * 50150449 = 993054166023949
            //  7334273 * 98835307 = 724885123576811
            //  5734579 * 83187779 = 477046890510041
            BigDecimal.Precision = 16;// 16 was goo
            //Rng(8);

            var i = BigInteger.Zero;
            var highSqrtCount = BigInteger.Zero;
            var lowSqrtCount = BigInteger.Zero;

            // BigDecimal.Pi

            // pi | i 3251 | high: 325 | % 0.09996924023377422
            // 3           | too low
            // 2.650436705 | too low

            var sqrtOf2 = BigDecimal.SquareRoot(5, BigDecimal.Precision);



            var pi = BigDecimal.Pi;

            while (true) {
                var rsa = RsaInteger.Generate(leftLength: 3, rightLength: 3).Take(1).First();
                var n = rsa.N;

                var nMinusOne = n - 1;
                var nPlusOne = n + 1;
                //var maxL = nMinusOne.MaxPowerOfAlt(2, out BigInteger exponentL);
                //var maxR = nPlusOne.MaxPowerOfAlt(2, out BigInteger exponentR);

                var numberOfDigits = n.NumberOfDigits();
                var integerSqrt = n.IntegerSqrt();

                //var highSqrt = integerSqrt + (integerSqrt * c).WholeValue;
                var highSqrt = (integerSqrt * pi).WholeValue;

                var hDiff = (rsa.R - integerSqrt) / 6;
                var hsDiff = (highSqrt - rsa.R) / 6;
                if (hsDiff < hDiff) {
                    highSqrtCount++;
                }

                var lowSqrt = (integerSqrt / BigDecimal.Pi).WholeValue;

                var lDiff = (integerSqrt - rsa.L) / 6;
                var lsDiff = (rsa.L - lowSqrt) / 6;
                if (lsDiff < lDiff) {
                    lowSqrtCount++;
                }

                var hsa = highSqrt > rsa.R;
                var lsa = lowSqrt < rsa.L;
                i++;

                Clear();
                Print($"  n {n} | l {numberOfDigits}");

                var sn = lowSqrt * highSqrt;

                Print($" sn {sn} | ~ {n - sn}");

                Print($"  √ {integerSqrt,9}");

                var asdf = new BigDecimal(rsa.R, n);

                Print($"  ~ {asdf,9} | Ln {BigInteger.Log2(n)}");

                Print($"  r {rsa.R,9} | {highSqrt,9} | {hsa}");
                Print($"  l {rsa.L,9} | {lowSqrt,9} | {lsa}");

                Print($"  i {i} | high: {highSqrtCount} | % {new BigDecimal(highSqrtCount, i)}");
                Print($"  i {i} | low: {lowSqrtCount} | % {new BigDecimal(lowSqrtCount, i)}");
                Print($"");

                Task.Delay(1000 / 30).Wait();

                if (!hsa || !lsa) {
                    Print($" !");
                    Prompt();
                }

                Prompt();

                //var iterations1 = BigInteger.Zero;
                //var sixKv1 = n.SixBoundaryv1(integerSqrt, ref iterations1);

                //var iterations2 = BigInteger.Zero;
                //var sixKv2 = n.SixBoundaryv2(integerSqrt, ref iterations2);

                //Prompt();

                //i++;
            }

            Prompt();

            static void PrintSqrts(BigInteger n, BigInteger integerSqrt) {
                var tSqrt = integerSqrt;
                while (true) {
                    tSqrt = tSqrt.IntegerSqrt();

                    Print($"  √ {tSqrt} | {n % tSqrt}");

                    if (tSqrt <= 1) {
                        break;
                    }
                }
            }
        }

        private static void TestMinMaxSqrts() {
            BigDecimal.Precision = 16;

            var integerLength = 6;

            var i = BigInteger.Zero;
            while (true) {
                var rsa = RsaInteger.Generate(leftLength: integerLength, rightLength: integerLength).Take(1).First();

                var n = rsa.N;
                var unmultiply = new Unmultiply(n);
                var integerSqrt = unmultiply._integerSqrt;

                var minSqrt = unmultiply._minSqrt;
                var maxSqrt = unmultiply._maxSqrt;
                var mmS = minSqrt * maxSqrt;
                var mmSqrt = mmS.IntegerSqrt();


                Clear();
                Print($"  n {n}");
                Print($"  √ {integerSqrt,9}");
                Print($"");

                Print($"  n {mmS,9}");
                Print($" h√ {maxSqrt,9}");
                Print($"  √ {mmSqrt,9}");
                Print($" l√ {minSqrt,9}");
                Print($"");

                Print($"  r {rsa.R,9}");
                Print($"  l {rsa.L,9}");
                Print($"");

                var d = (n - 1) / 2;
                var dSqrt = d.IntegerSqrt();

                Print($"  d {d,9}");
                Print($"  √ {dSqrt,9}");

                var e = integerSqrt * dSqrt;
                var eSqrt = e.IntegerSqrt();

                Print($"  e {e,9}");
                Print($"  √ {eSqrt,9}");

                var f = eSqrt * dSqrt;
                var fSqrt = f.IntegerSqrt();

                Print($"  f {f,9}");
                Print($"  √ {fSqrt,9}");


                Prompt();
            }
        }

        private static void TestMinMaxSqrts2() {
            BigDecimal.Precision = 16;

            var integerLength = 6;

            while (true) {
                var rsa = RsaInteger.Generate(leftLength: integerLength, rightLength: integerLength).Take(1).First();

                var n = rsa.N;
                var unmultiply = new Unmultiply(n);
                var integerSqrt = unmultiply._integerSqrt;
                var sqrdSqrt = integerSqrt * integerSqrt;
                var dist = n - sqrdSqrt;
                var nPlusOne = n + 1;
                var nMinusOne = n - 1;

                var d = (n - 1) / 2;

                var sqrtPn = integerSqrt.NextPrimeNumber();

                var minSqrt = unmultiply._minSqrt;
                var minDiff = rsa.L - minSqrt;

                var maxSqrt = unmultiply._maxSqrt;
                var maxDiff = maxSqrt - rsa.R;

                var minMax = minDiff + maxDiff;

                Clear();
                Print($"  n {n}");
                Print($"  √ {integerSqrt,9} | {dist} | {n / dist}");
                Print($"");

                Print($"  d {d} | √ {d.IntegerSqrt()}");
                Print($"max {maxSqrt,9} | < {maxDiff,9}");
                Print($"min {minSqrt,9} | > {minDiff,9}");

                //var rSqrd = rsa.R * rsa.R;
                //var lSqrd = rsa.L * rsa.L;
                //var srs = rSqrd - lSqrd;

                //var rp = new BigDecimal(n, rSqrd);
                //var lp = new BigDecimal(lSqrd, n);

                var rDist = rsa.R - integerSqrt;
                var lDist = integerSqrt - rsa.L;
                var tDist = rDist + lDist;

                Print($"  r {rsa.R,9} | > {rDist,9}");
                Print($"  l {rsa.L,9} | < {lDist,9}");
                Print($"  t {"",9} | = {tDist,9}");
                Print($"");

                ShowStuff(n, integerSqrt, rsa.L, rsa.R, minSqrt, maxSqrt);

                Prompt();
            }

            static void ShowPies(BigInteger n) {
                //var k = BigDecimal.SquareRoot(BigDecimal.Pi, BigDecimal.Precision);
                var k = BigDecimal.Pi;

                for (int i = 1; i < 31; i++) {
                    var pies = k * i;
                    var np = (n / pies).WholeValue;
                    var npr = n % np;

                    Print($"np {np} | r {npr} | np.gcd {n.Gcd(n + np)} | npr.gcd {n.Gcd(n + npr)}");
                }
            }

            static void ShowSixes(BigInteger n) {
                var k = 6;

                for (int i = 1; i < 31; i++) {
                    var pies = k * i;
                    var np = (n / pies);
                    var npr = n % np;

                    Print($"np {np} | r {npr} | np.gcd {n.Gcd(np)} | npr.gcd {n.Gcd(npr)}");
                }
            }

            static void ShowMinMax(BigInteger n, BigInteger integerSqrt, BigInteger minSqrt, BigInteger maxSqrt) {
                var nMinusOne = n - 1;
                var x = new BigDecimal(maxSqrt, minSqrt) / integerSqrt;

                //x = BigDecimal.SquareRoot(x, 9);

                var a = (n * x).WholeValue;

                a = (a - (n % a)) + a;

                Print($" x {x}");
                Print($" a {a} | gcd {a.Gcd(n)}");

                //var d = (n - 1) / 2;
                //var k = d;

                //while (k > minSqrt) {
                //    k = k - (k / 2);

                //    var r = (n % k);
                //    while (r > 1) {
                //        r = (n % r);

                //        Print($" r {r} | {nMinusOne % r}");
                //    }

                //    Print($" k {k} | r {r} | gcd {n.Gcd(r)} | gcd {n.Gcd(k)}");
                //}

                //while (d > 1) {
                //    Print($" d {d} | r {nMinusOne % sqrt} | gcd {n.Gcd(d)}");

                //    d /= 2;
                //}

                //var k = n - d;

                //for (int i = 1; i < 31; i++) {
                //    var kr = n % k;

                //    Print($" k {k} | r {kr} | gcd {(n - 1).Gcd(k)}");

                //    k -= d;
                //}
            }

            // investigate more
            static void ShowSqrts(BigInteger n, BigInteger integerSqrt, BigInteger minSqrt) {
                var t = new BigDecimal(1, -6); // e needs to be at least number of digits for integerSqrt
                var k = t + 1;

                Print($" t {t}");
                Print($" k {k}");

                var count = 0;
                var i = BigInteger.One;
                while (k < 5) {
                    k += t;

                    var x = (n * k).WholeValue;

                    var gcd = x.Gcd(n);
                    if (gcd != n && gcd != 1) {
                        Print($" i {i} | k {k} | x {x} | r {n % x}  | gcd {gcd}");
                        count++;

                        if (count > 0) {
                            break;
                        }
                    }

                    i++;
                }
            }

 
            static void ShowSqrts2(BigInteger n, BigInteger integerSqrt, BigInteger minSqrt, BigInteger maxSqrt) {
                var bigN = n * 7560;
                var d = bigN / 2;
                var dSqrt = bigN.IntegerSqrt();

                var k = dSqrt - 1;
                var j = dSqrt + 1;
                var i = new BigInteger(1);
                while (k < bigN) {
                    if (bigN % k == 0) {
                        Print($"  i {i} | k {k} | {n.Gcd(k)}");
                        break;
                    }
                    else if (bigN % j == 0) {
                        Print($"  i {i} | j {j} | {n.Gcd(j)}");
                        break;
                    }

                    k -= 1;
                    j += 1;
                    i++;
                }

                Print($"done");
            }

            // works...
            static void ShowPrimeSqrts(BigInteger n, BigInteger integerSqrt, BigInteger l, BigInteger r, BigInteger minSqrt, BigInteger maxSqrt) {
                Print($"");

                var d = n / 2;

                var i = new BigInteger(1);
                var s = new BigDecimal(1);
                foreach (var p in PrimeNumbers.Get(start: n * 3)) { //minimum: integerSqrt
                    var pSqrt = BigDecimal.SquareRoot(p, 31);

                    //s += pSqrt;
                    // (1 + SqrtOf5) / 2;
                    //var grp = (1 + pSqrt) / ((p - 1) / 2);

                    //Print($"grp {grp}");
                    //Prompt();

                    var a = n * pSqrt;
                    //var b = n * (pSqrt * 2);

                    var ag = a.WholeValue.Gcd(n);
                    //var bg = b.WholeValue.Gcd(n);

                    if (ag != 1) { //  || bg != 1
                        Print($" i {i,6} | pSqrt {p} | ir {n % i} | a {a.WholeValue.Gcd(n)}"); //  | b {b.WholeValue.Gcd(n)}
                        break;
                    }

                    i++;
                }

                Print($"done");
            }

            // works...
            static void ShowGoldenSqrts(BigInteger n, BigInteger integerSqrt, BigInteger l, BigInteger r, BigInteger minSqrt, BigInteger maxSqrt) {
                var nSize = n.NumberOfDigits();

                var t = (n / (BigDecimal.Pi * 2)).WholeValue;

                Print($"");

                //Print($" t {t} | lr {t % l} | rr {t % r}");

                //var goldenRatio = Constants.GoldenRatio / n;

                //var goldenK = goldenRatio * BigInteger.Pow(10, integerSqrt.NumberOfDigits() - 1);
                //var goldenRatio = Constants.GoldenRatio / n;
                //var goldenK = (n.GoldenRatio()) + 1;

                var goldenRatio = Constants.SqrtOf3;
                var goldenK = goldenRatio + (minSqrt * integerSqrt);

                Print($"gK {goldenK}");
                Print($"gK {goldenK}");

                var i = new BigInteger(1);
                var s = new BigDecimal(1);

                while (true) {
                    var goldenN = n * goldenK;
                    var goldenG = goldenN.WholeValue.Gcd(n);

                    if (goldenG != 1) {
                        Print($" i {i} | gn {goldenN.WholeValue} | gnr {n % goldenN.WholeValue} | gg {goldenG}");
                        Print($"gK {goldenK}");
                        break;
                    }


                    goldenK += goldenRatio;
                    i++;
                }

                Print($"done");
            }

            // works...
            static void ShowGold(BigInteger n, BigInteger integerSqrt, BigInteger l, BigInteger r, BigInteger minSqrt, BigInteger maxSqrt) {

                Print($"");

                var goldenK = (minSqrt * maxSqrt) - minSqrt;

                Print($"gK {goldenK}");

                var i = new BigInteger(1);
                var s = new BigDecimal(1);

                while (true) {
                    var goldenG = goldenK.Gcd(n);

                    if (goldenG != 1) {
                        Print($" i {i} | gK {goldenK} | gnr {n % goldenK} | g1 {goldenG}");
                        break;
                    }

                    goldenK += 9;
                    i++;
                }

                Print($"done");
            }

            // probably def no
            static void ShowGoldenRoots(BigInteger n, BigInteger integerSqrt, BigInteger l, BigInteger r, BigInteger minSqrt, BigInteger maxSqrt) {
                Print($"");

                var k = n * 3;
                k = k.NextPrimeNumber();

                var i = new BigInteger(1);
                while (true) {
                    var m = n % k;

                    if (m != 0 && m < integerSqrt) {
                        var kSqrt = BigDecimal.SquareRoot(k, 31);
                        var a = n * kSqrt;
                        var ag = a.WholeValue.Gcd(n);

                        if (ag != 1) {
                            Print($" i {i,6} | kSqrt {k} | ir {n % a.WholeValue} | ag {ag}"); //  | b {b.WholeValue.Gcd(n)}
                            break;
                        }
                    }

                    k += 1;
                }

                //var i = new BigInteger(1);
                //var s = new BigDecimal(1);
                //foreach (var p in PrimeNumbers(minimum: n * 3)) { //minimum: integerSqrt
                //    var pSqrt = BigDecimal.SquareRoot(p, 31);

                //    //s += pSqrt;
                //    // (1 + SqrtOf5) / 2;
                //    //var grp = (1 + pSqrt) / ((p - 1) / 2);

                //    //Print($"grp {grp}");
                //    //Prompt();

                //    var a = n * pSqrt;
                //    //var b = n * (pSqrt * 2);

                //    var ag = a.WholeValue.Gcd(n);
                //    //var bg = b.WholeValue.Gcd(n);

                //    if (ag != 1) { //  || bg != 1
                //        Print($" i {i,6} | pSqrt {p} | ir {n % i} | a {a.WholeValue.Gcd(n)}"); //  | b {b.WholeValue.Gcd(n)}
                //        break;
                //    }

                //    i++;
                //}

                Print($"done");
            }


            static void ShowStuff(BigInteger n, BigInteger integerSqrt, BigInteger l, BigInteger r, BigInteger minSqrt, BigInteger maxSqrt) {
                var fibNumbers = new FibonacciNumbers();
                var pellNumbers = new PellNumbers();
                var collatzCon = new CollatzConjecture();

                BigDecimal.Precision = 31;

                var t = new BigDecimal(n);

                var f = new BigInteger(5).Sqrt();

                var a = new BigDecimal(2.5);

                var b = new BigDecimal(-0.23);



                Print($" ~ done");
            }
        }

        // More?
        private static void SquareAndGoldenRatios() {
            BigDecimal.Precision = 10;

            //var goldenK = BigInteger.Parse("2184000001");
            //var goldenRatio = new Irrationality() {
            //    I = BigInteger.Parse("2184000000"),
            //    Value = BigDecimal.Parse("3.1981685862647414865834"),
            //    Sign = true
            //};

            var goldenK = BigInteger.Parse("1");
            var goldenRatio = new Irrationality() {
                I = BigInteger.Parse("0"),
                Value = BigDecimal.Parse("0"),
                Sign = true
            };

            Execute(() => {
                while (true) {
                    var g = goldenK.GoldenRatio();
                    goldenRatio.Next(g);
                    goldenK += 1;
                }
            });

            //var squareK = BigInteger.Parse("12169000001");
            //var squareRatio = new Irrationality() {
            //    I = BigInteger.Parse("12169000000"),
            //    Value = BigDecimal.Parse("0.60489409626506199590708"),
            //    Sign = true
            //};

            //Execute(() => {
            //    while (true) {
            //        var g = squareK.SquareRatio();
            //        squareRatio.Next(g);
            //        squareK += 1;
            //    }
            //});

            while (true) {
                Clear();

                Print($"");
                Print($" i {goldenRatio.I}");
                Print($" k {goldenK}");
                Print($" v {goldenRatio.Value}");
                Print($" s {goldenRatio.Sign}");

                //Print($"");
                //Print($" i {squareRatio.I}");
                //Print($" k {squareK}");
                //Print($" v {squareRatio.Value}");
                //Print($" s {squareRatio.Sign}");

                Task.Delay(1000 / 30).Wait();
            }
        }

        private static void RSA100() {
            // 1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139
            // 15226050279225333605356183781326374297180681149613
            // 80688657908494580122963258952897654000350692006139
            // 95914708187719913728319442734224028297531373155752



            var n = BigInteger.Parse("1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139");
            var d1 = BigInteger.Parse("37975227936943673922808872755445627854565536638199");
            var d2 = BigInteger.Parse("40094690950920881030683735292761468389214899724061");

            var integerSqrt = n.IntegerSqrt();

            var primeRoot = integerSqrt;
            while (!primeRoot.IsProbablyPrime()) {
                primeRoot++;
            }

            Print($" n {n}");
            Print($" s {integerSqrt}");
            Print($"");
            Print($" d {d1}");
            Print($" d {d2}");
            Print($"");

            foreach (var factor in (n - 1).Factors()) {
                Print($" f {factor}");
            }

            Print($"");
            Prompt();

            //while (true) {
            //    pn -= primeRoot;

            //    var d = pn / primeRoot;

            //    Print($"pn {pn}");
            //    Print($" d {d}");

            //    Prompt();
            //}

            //Print($" d {d1}");
            //Print($" d {d2}");
            Print($"");



            //var nSqrd = n * n;

            //while (nSqrd.ToString().Length > d1.ToString().Length) {
            //    nSqrd /= 2;
            //}

            //Print($"ns {nSqrd}");
            //Print($"");

            //var nString = string.Join("", "1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139".ToCharArray().Reverse());
            //var nString = "1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139";
            //for (var i = 2; i <= nString.Length; i++ ) {
            //    var pString = nString.Substring(0, i);

            //    var p = BigInteger.Parse(pString);

            //    if (!p.IsProbablyPrime() && (p + 1).IsProbablyPrime()) {
            //        p += 1;
            //    }

            //    if (p.IsProbablyPrime()) {
            //        var dp = p - 1;
            //        var k = new BigInteger(2);

            //        while (!dp.IsProbablyPrime()) {
            //            while (dp % k == 0) {
            //                dp /= k;
            //            }

            //            k++;

            //            if (k > dp.IntegerSqrt()) {
            //                break;
            //            }
            //        }

            //        Print($" p {p}");
            //        Print($" d {d1 % p}");
            //        Print($" d {d2 % p}");
            //        Print($"");

            //        Print($"dp {dp}");
            //        Print($" d {d1 % dp}");
            //        Print($" d {d2 % dp}");
            //        Print($"");
            //    }
            //    else {
            //        //Print($" p {pString} | ");
            //    }


            //    //Prompt();
            //}

            Print($" COMPLETE");
            Prompt();
        }

        private static void RSA130() {
            // 4169543159
            // 41695 / n = 100001 - 96608 = 3393 / n = 1228866
            // 43159 / n = 96608
            // l + r     = 84854 ~ 85058
            // l * r     = 17995 14505 = 261017475

            // d 64567
            // d 64577

            // 4 1 6 9 5 4 3 1 5 9
            // 35640

            //           9                         9         9           9   9       9     9   9           9      9
            //                         8  8           8  8       8  88     8    8         8   8
            //          7                7      7   7                   7                       7
            //     6            6    6        6         6     6    6  6               6          6        6    6
            //  5    5      5     5 5                                  5       5         5  5     5     5
            //                                   4          4               4 4                    4
            //               333   3    3   3  3                3                      3               3         3
            //   22    2  22                 2    2                                22   2    2              2
            // 1                      1    1         1    11   1                  1                             1
            //      0 0          0                     0          0       0      0                  000  0   00
            // 1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139

            //           9                         9         9
            //         9   9       9     9   9           9      9
            //                         8  8           8  8
            // 8  88     8    8         8   8
            //          7                7      7   7
            //        7                       7
            //     6            6    6        6         6     6
            //   6  6               6          6        6    6
            //  5    5      5     5 5
            //       5       5         5  5     5     5
            //                                   4          4
            //            4 4                    4
            //               333   3    3   3  3                3
            //                       3               3         3
            //   22    2  22                 2    2
            //                   22   2    2              2
            // 1                      1    1         1    11   1
            //                  1                             1
            //      0 0          0                     0
            //  0       0      0                  000  0   00

            var numberChars = new List<char>() {
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            };

            numberChars.Reverse();

            var numberStrings = new List<string>() {
                "15226050279225333605356183781326374297180681149613", // Left
                "80688657908494580122963258952897654000350692006139", // Right
                //"40094690950920881030683735292761468389214899724061", // DH
                //"37975227936943673922808872755445627854565536638199", // DL
            };


            var nString = "40094690950920881030683735292761468389214899724061";

            foreach (var numberChar in numberChars) {

                foreach (var numberString in numberStrings) {
                    var nCopy = new StringBuilder(numberString);

                    for (int i = 0; i < nCopy.Length; i++) {
                        if (nCopy[i] != numberChar) {
                            nCopy = nCopy.Replace($"{numberChar}", "");

                            //nCopy[i] = ' ';
                        }
                    }

                    Print($"// {nCopy}");
                }

            }

            Prompt();

            //  80688657908494580122963258952897654000350692006139
            //  15226050279225333605356183781326374297180681149613
            //  95914708187719913728319442734224028297531373155752+
            //  65462607629269246517607075171571279703170010856526-
            // 161377315816989160245926517905795308000701384012278+
            //  30452100558450667210712367562652748594361362299226-
            //  
            // 5.
            // 149691037031037353441505 =
            // 748455185155186767207525


            var u = BigInteger.Parse("161377315816989160245926517905795308000701384012278");

            
            var n = BigInteger.Parse("1522605027922533360535618378132637429718068114961380688657908494580122963258952897654000350692006139");












             var left = BigInteger.Parse("15226050279225333605356183781326374297180681149613");
            var right = BigInteger.Parse("80688657908494580122963258952897654000350692006139");
            var length = n.ToString().Length;
            var targetLength = length / 2;

            BigDecimal.Precision = length;

            var v = new BigDecimal(right, left);

            var integerSqrt = n.IntegerSqrt();
            var digitalRoot = integerSqrt.DigitalRoot();

            var d1 = BigInteger.Parse("37975227936943673922808872755445627854565536638199"); // - 43159 = 21408 | 22882 + 21408 = 44290
            var d2 = BigInteger.Parse("40094690950920881030683735292761468389214899724061"); // - 41695 = 22882 | 22882 - 21408 =  1474

            Print($"u % d1 = {(n % u)}");

            var sum = BigInteger.Zero;

            //Clear();
            Print($" n {n}");
            Print($" √ {integerSqrt}");
            Print($" d {d1}");
            Print($" d {d2}");

            Print($" v {v.Mantissa}");

            var vSqrt = v.Mantissa.IntegerSqrt();

            Print($" t {vSqrt}");

            //Print($"n");

            //var f = 2;
            //foreach (var divisor in n.Shifter()) {
            //    sum += divisor;

            //    Print($" d {divisor} | {sum}");
            //    //Print($" r {n % divisor} | f {f}");

            //    f++;
            //    Prompt();
            //}

            Print($"");

            Prompt();
        }

        private static void SqrtIntegers() {
            var i = new BigInteger(3);
            var n = new BigInteger(1);
            var k = new BigInteger(3);

            while (true) {
                n = k;

                var integerSqrt = n.IntegerSqrt();
                if (n % integerSqrt == 0) {
                    Print($" - {n} | √ {integerSqrt}");
                }
                else {
                    Print($"   {n} | √ {integerSqrt}");
                }

                k += 1;
                i++;
                Prompt();
            }

            //var digitSum = new BigInteger(0);
            //foreach (var primeNumber in PrimeNumbers()) {
            //    n = primeNumber * primeNumber;

            //    var p = (n - 2);

            //    if (p % 10 == 1 || p % 10 == 9) {
            //        digitSum += p % 10;
            //    }
            //    else {
            //        digitSum += 1;
            //    }

            //    var pPrime = p.IsPrime() ? "~" : " ";
            //    //var divisors = p.Divisors();
            //    //var divisorList = string.Join(", ", divisors);

            //    Print($" ~ {primeNumber} | {n} | {pPrime} | {digitSum}");

            //    i++;

            //    Prompt();
            //}



        }

        private static void ExpandingCircle() {
            var size = new BigDecimal(6378000); // 6,378,000 in meters

            var acceleration = new BigDecimal(9.807); // meters/s

            // Speed of Light: 299,800,000    meters/s
            //                  30,570,001.01 seconds
            //                         353.81 days

            //                           0.9687 % SoL?

            //         847,324.8 meters per day.
            //         738,963   days since January 1, 1 AD
            // 626,141,676,182.4 since January 1, 1 AD
            //          98,172.1 times the size

            var radius = new BigDecimal(0);

            var diameter = radius + radius;
            var circumference = BigDecimal.Pi * diameter;

            while (true) {
                var currentTime = DateTime.UtcNow;
                var unixTime = ((DateTimeOffset)currentTime).ToUnixTimeSeconds();

                radius = unixTime / acceleration;
                diameter = radius + radius;
                circumference = BigDecimal.Pi * diameter;

                Print($" ~ {radius}");
                Print($" s {radius / size}");

                //Print($" d {diameter}");
                //Print($" c {circumference}");


                Prompt();
            }


            Prompt();

        }

        private static void A193651() {
            var i = new BigInteger(0);

            var n = new BigInteger(1);
            var k = new BigInteger(1);

            var s = new BigInteger(0);
            var sign = true;

            while (true) {
                n *= k;
                s = n - (n / 2);

                Print($" i {i,3} | k {k} | s {s}");

                k += 2;
                i++;
                sign = !sign;

                Prompt();
            }

            Prompt();
        }

        private static void ASomething2() {
            var i = new BigInteger(0);

            var n = new BigInteger(1);
            var k = new BigInteger(1);

            var s = new BigInteger(0);
            var sign = true;

            var primeNumbers = PrimeNumbers.Get();
            //what.ElementAt
            var sequence = new List<BigInteger>();

            while (true) {
                var p = primeNumbers.ElementAt((int)i);
                n = (k * k);

                s += (2 * p);

                sequence.Add(s);

                Print($" i {i,3} | k {k} | p {p} | s {string.Join(",", sequence)}");

                k += 1;
                i++;
                sign = !sign;

                Prompt();
            }

            Prompt();
        }

        private static void DigitStuff() {
            var i = new BigInteger(1);

            var baseOf = new BigInteger(2);
            var n = baseOf;

            n *= baseOf;
            i += 1;

            while (true) {
                var p = (n - 1);
                var x = (p - 1);

                var y = x / i;

                //var pPrime = p.IsPrime() ? "Prime" : "     ";

                var d = (p / i);
                var r = BigInteger.ModPow(3, d, p);

                Print($" i {i,3} | p {p,6} | r {r,6} | {r.IsPowerOfTwo} | y {y % i}");

                //var yPrime = y.IsPrime() ? "Prime" : "";

                //Print($"        y {y,3} | {yPrime}");
                //n += 2;
                //n *= 3;

                n *= baseOf;

                i += 1;

                Prompt();
            }

            Prompt();
        }

        private static void PerfectNumberStuff() {
            var i = BigInteger.Zero;
            var n = new BigInteger(10);

            while (true) {
                var divisors = n.Divisors(-1);

                if (divisors.Count % 2 == 0) {
                    Print($" n {n}");
                    Print($"   {string.Join(", ", divisors)}");

                    //var s = divisors.Multiply();
                    //var a = divisors.AltSum();
                    //var r = s / n;

                    //Print($"   s {s} | a {a}");

                    Prompt();
                }

                n++;
            }

            //foreach (var perfectNumber in perfectNumbers) {
            //    n = (perfectNumber * perfectNumber) + (perfectNumber - 1);

            //    Print($"   n {perfectNumber}");
            //    Print($" n^2 {n}");

            //    Prompt();
            //}

            Prompt();
        }

        private static void CollatzStuff() {
            //Rng();

            // 431 * 587 = 252997

            foreach (var rsaInt in RsaInteger.Generate(leftLength: 3, rightLength: 3)) {
                var i = new BigInteger(1);
                var n = rsaInt.N;

                var d = (n - (n % 2)) + 2;

                while (d % 2 == 0) {
                    d /= 2;
                }

                var a = d;

                var aDivisors = a.Divisors(-1);

                Print($"  n {n} = {rsaInt.L} * {rsaInt.R}");
                Print($"  d {d}");
                Print($"  a {a} | {string.Join(", ", aDivisors)}");

                Prompt();
            }
        }

        private static void PrimalityCheck() {
            var stopWatchA = new Stopwatch();
            var stopWatchB = new Stopwatch();

            var elapsedTotalA = new TimeSpan();
            var elapsedTotalB = new TimeSpan();

            var i = BigInteger.Zero;
            var n = new BigInteger(101);

            var showSqrts = false;

            var isPrime = false;
            var isPrimeAlt = false;
            while (true) {
                Clear();

                var integerSqrt = n.IntegerSqrt(SqrtMethod.NewtonPlus);

                Print($" i {i,10} | {n}");

                stopWatchA.Restart();
                isPrime = n.IsPrime();
                stopWatchA.Stop(); elapsedTotalA = elapsedTotalA.Add(stopWatchA.Elapsed);

                stopWatchB.Restart();
                isPrimeAlt = n.IsPrimeAlt();
                stopWatchB.Stop(); elapsedTotalB = elapsedTotalB.Add(stopWatchB.Elapsed);

                Print($"   p {isPrime,10} | {stopWatchA.Elapsed} | {elapsedTotalA}");
                Print($" ~ p {isPrimeAlt,10} | {stopWatchB.Elapsed} | {elapsedTotalB}");

                if (showSqrts) {
                    foreach (SqrtMethod sqrtMethod in Enum.GetValues<SqrtMethod>()) {
                        var result = n.IntegerSqrt(sqrtMethod);

                        Print($" {sqrtMethod,12} | {result}");
                    }
                }

                Thread.Sleep(60);

                //Prompt();
                i++;
                n += 32;
            }
        }

        private static void Shapes() {
            BigDecimal.Precision = 200;

            var numberShapes = new NumberShapes();
            numberShapes.Next();

            var t = BigDecimal.Zero;

            var vl = BigDecimal.Zero;
            var dl = BigDecimal.Zero;
            while (true) {

                Print($"i {numberShapes.i}");
                Print($"    | S {numberShapes.Square} | C {numberShapes.Cube}");

                //var x = new BigDecimal(numberShapes.Cube - numberShapes.Square);
                //var y = new BigDecimal(numberShapes.Cube + numberShapes.Square);
                //var z = new BigDecimal(numberShapes.Square, numberShapes.Cube);
                var x = numberShapes.Square * numberShapes.Cube;
                var y = numberShapes.Square + numberShapes.Cube;

                Print($"    | X {x}");
                Print($"    | Y {y}");
                //Print($"    | Z {z}");

                numberShapes.Next();
                Console.ReadKey();
            }
        }

        private static BigInteger _n = new BigInteger(1);

        private static BigInteger _square = new BigInteger(1);
        private static BigInteger _cube = new BigInteger(1);

        private static BigDecimal _v = new BigDecimal(1);
        private static BigDecimal _t = new BigDecimal(1);

        private static BigDecimal _s = BigDecimal.Zero;

        private static void PieStuff() {
            BigDecimal.Precision = 16;

            var s = new BigInteger(2);
            var k = new BigInteger(2);
            var n = new BigInteger(1);
            var currentValue = new BigDecimal(1);
            var valueSum = new BigDecimal(0);

            var direction = true;

            while (true) {
                //Console.Clear();

                //currentValue = new BigDecimal(4 * k, n * k);
                currentValue = new BigDecimal(4, n);

                if (direction) {
                    valueSum += currentValue;
                }
                else {
                    valueSum -= currentValue;
                }

                if (k % 2 == 0) {
                    Console.Clear();
                    Print($" k {k} | n {n}");

                    Print($"  currentValue {currentValue}");
                    Print($"      valueSum {valueSum}");
                    //Print($"     valueSum^ {(valueSum * valueSum)}");

                    Console.ReadKey();
                }

                k += s;
                n += 2;
                direction = !direction;
            }
        }

        private static void Stuff() {
            BigDecimal.Precision = 16;

            var i = new BigInteger(0);
            var s = new BigInteger(1);
            var k = new BigInteger(1);
            var n = new BigInteger(1);

            var high = new BigInteger(3);
            var low = new BigInteger(1);

            var currentValue = new BigDecimal(1);
            var valueSum = new BigDecimal(0);

            var stepCount = 1;
            var direction = true;

            var pellNumbers = new PellNumbers();
            pellNumbers.Next();

            var pv = new BigDecimal(0);
            while (true) {
                //Console.Clear();

                currentValue = new BigDecimal(high, low);

                if (direction) {
                    valueSum += currentValue;
                }
                else {
                    valueSum -= currentValue;
                }

                if (i % 1 == 0) {
                    Console.Clear();
                    Print($" high {high} | low {low}");

                    Print($"  currentValue {currentValue}");
                    Print($"      valueSum {valueSum}");
                    //Print($"     valueSum^ {(valueSum * valueSum)}");

                    Console.ReadKey();
                }

                low += 1;
                high = (low * 2) + 1;
                i++;

                direction = !direction;
            }
        }

        private static void Circles() {
            BigDecimal.Precision = 23;

            var numberShapes = new NumberShapes();
            //numberShapes.Next();

            //var numberGap = new NumberGap();
            //numberGap.Next();

            var direction = true;

            _n = new BigInteger(1);

            Execute(() => {
                while (true) {
                    //if (_n.IsPrime()) {
                    //    //_square = _n * _n;
                    //    //_cube = _n * _n * _n;
                    //}

                    //_square = _n * _n;
                    //_cube = _n * _n * _n;
                    _v = new BigDecimal(9, _n * _n * _n);

                    if (direction) {
                        _t += _v;
                    }
                    else {
                        _t -= _v;
                    }

                    //_s = _t;
                    _s = _t * _t;

                    direction = !direction;
                    //Console.ReadKey();

                    _n += 2;
                    //numberShapes.Next();
                    //numberGap.Next();
                }
            });

            while (true) {
                Console.Clear();

                Print($"i {_n}");
                Print($"    v {_v}");
                Print($"    t {_t}");

                Print($"    t^   {_s}");
                Print($"    t/pi {_t / BigDecimal.Pi}");
                Print($"    pi/t {BigDecimal.Pi / _t}");
                //Print($"    t2 {_t * 2}");

                Thread.Sleep(1000 / 30);
            }


        }
    }
}
