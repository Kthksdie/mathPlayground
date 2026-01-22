using Numberality.Con.Components;
using Numberality.Con.Enumerations;
using Numberality.Con.Extensions;
using Numberality.Con.Utilities;
using System.Buffers.Text;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace Numberality.Con {
    internal class Program {
        // √

        private static ConsoleColor _defaultFontColor = ConsoleColor.White;

        public static void Print() {
            Console.WriteLine("");
        }

        public static void Print(string message) {
            Console.WriteLine(message);
        }

        public static void Print(string message, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = _defaultFontColor;
        }

        public static void Clear() {
            Console.Clear();
            Console.WriteLine("\x1b[3J");
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
        }

        public static void Prompt() {
            Console.ReadKey();
        }


        static void Main(string[] args) {
            Startup();

            //Test_v1();
            //Test_v2();
            Test_v3();

            Prompt();
        }

        static void Startup() {
            ConsoleUtility.MoveWindowToCenter();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = _defaultFontColor;
        }

        public static Task Execute(Action action) {
            return Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
        }

        static void Test() {

            const int numberOfDigits = 9;

            var elapsedTimes = new List<TimeSpan>();

            (Product Solution, TimeSpan Elapsed) fastest = (new Product(0, 0), new TimeSpan(1, 0, 0));
            (Product Solution, TimeSpan Elapsed) slowest = (new Product(0, 0), new TimeSpan(0, 0, 0));

            var a = BigInteger.Zero;
            var b = BigInteger.Zero;
            while (true) {
                Clear();

                // 66231877 * 16846079 = 1115747432260283 | 00:00:03.4417954
                //  8937521 *  5461121 =   48808883621041 | 00:00:09.7316557
                //  7579657 *  6512731

                var rng = true;
                if (rng) {
                    a = Integers.Random(numberOfDigits, prime: true);
                    b = Integers.Random(numberOfDigits, prime: true);
                }
                else {
                    a = BigInteger.Parse("7579657");
                    b = BigInteger.Parse("6512731");
                }

                var n = (a * b);
                //var n = BigInteger.Parse("4697085165547666455778961193578674054751365097816639741414581943064418050229216886927397996769537406063869583");

                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();

                var reverseMultiply = new ReverseMultiply(n);

                var solutionTask = reverseMultiply.TrySolveAsync();
                //var solutionTask = reverseMultiply.TrySolve_RecursiveAsync();
                //var solutionTask = reverseMultiply.TrySolve_MultithreadedAsync();

                while (true) {
                    Print($" n {n,numberOfDigits} | l {n_numberOfDigits}");
                    Print($" √ {n_integerSqrt,numberOfDigits}");
                    //Print($" a {a,numberOfDigits}");
                    //Print($" b {b,numberOfDigits}");
                    Print();

                    Print($" Prospects: {reverseMultiply.Prospects} / {reverseMultiply.MaxProspects}");
                    //Print($" Eliminations: {reverseMultiply.Eliminations}");
                    Print($" Current:      {reverseMultiply.Watch.Elapsed,12}");

                    Print();
                    Print($" Fastest: {fastest.Elapsed} | {fastest.Solution}");
                    Print($" Slowest: {slowest.Elapsed} | {slowest.Solution}");

                    var average = elapsedTimes.Avg();

                    Print($" Average: {average} | Count: {elapsedTimes.Count}");

                    if (solutionTask.IsCompleted) {
                        break;
                    }
                    else {
                        Task.Delay(1000 / 30).Wait();
                        Clear();
                    }
                }

                var solution = solutionTask.Result;
                elapsedTimes.Add(reverseMultiply.Watch.Elapsed);

                Print();
                Print($" ~ {solution.C,numberOfDigits} * {solution.E,numberOfDigits} = {solution.P,numberOfDigits}");

                if (slowest.Elapsed < reverseMultiply.Watch.Elapsed) {
                    slowest = (solution, reverseMultiply.Watch.Elapsed);
                }

                if (fastest.Elapsed > reverseMultiply.Watch.Elapsed) {
                    fastest = (solution, reverseMultiply.Watch.Elapsed);
                }

                //if (elapsedTimes.Count > 10) {
                //    elapsedTimes.Clear();
                //    Prompt();
                //}

                if ((solution.C == 0 || solution.E == 0) || (solution.C == 1 || solution.E == 1)) {
                    Print("ERROR");
                    Prompt();
                }

                Task.Delay(360).Wait();
                //Prompt();
            }
        }

        static void Test_v1() {
            var n = BigInteger.One;
            while (true) {
                Print($" ~ {n}");

                var b = BigInteger.One;
                var e = BigInteger.Zero;

                var factors = n.Factorize();
                foreach (var powerOf in factors) {
                    b *= powerOf.Base;
                    e += powerOf.Exponent;

                    Print($"   {powerOf} = {powerOf.Calculate()}");
                }

                var finalOf = new PowerOf(b, e);
                Print($" * {b}^{e} = {finalOf.Calculate()}");

                Prompt();
                n++;
            }
        }

        static void Test_v2() {

            var n = 64d;
            while (true) {
                var radius = n / 2d;
                var circumference = (2d * Math.PI) * radius;
                var spawnDistance = radius + (Math.PI * circumference);

                Print($" ~ {n}");
                Print($" g {spawnDistance}");

                var t = n;
                var s = 0;
                var d = n; // child size
                while (t > 1) {
                    d += t / 2d;
                    t /= 2d;
                    s++;
                }

                Print($"    s {s} | d {d}");

                Prompt();
                n /= 2d;
            }
        }

        /// <summary>
        /// Scaling using # of p (by index).
        /// </summary>
        static void Test_v3() {
            var s = new StringBuilder();
            var ores = new List<byte>();


            foreach (var n in Integers.Numbers()) {
                if (n.Primality == Primality.Prime) {
                    s.Append(1);

                    Print($" ~ {n,8} | {n.Primality,8} | {s}");
                }
                else {
                    s.Append(0);

                    Print($"   {n,8} | {"",8} | {s}");
                }

                Prompt();
            }
        }

        static void Trails_v1() {
            var numberOfDigits = 6;

            var a = BigInteger.Zero;
            var b = BigInteger.Zero;

            while (true) {
                var random = true;
                if (random) {
                    a = Integers.Random(numberOfDigits, prime: true);
                    b = Integers.Random(numberOfDigits, prime: true);
                }
                else {
                    a = BigInteger.Parse("498227");
                    b = BigInteger.Parse("776221");
                }

                var n = (a * b);
                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();

                Clear();
                Print($" n {n,12} | l {n_numberOfDigits}");
                Print($" √ {n_integerSqrt,12}");
                Print($" a {a,12}");
                Print($" b {b,12}");
                Print();

                var reverseMultiply = new ReverseMultiply(n);
                var solution = reverseMultiply.TrySolveAsync().GetAwaiter().GetResult();

                Print();

                Print($" Elapsed:      {reverseMultiply.Watch.Elapsed,12}");

                Print($" ~ {solution.C,4} * {solution.E,4} = {solution.P,4}");

                Prompt();
            }
        }

        static void Trails_v2() {
            var a = BigInteger.Zero;
            var b = BigInteger.Zero;

            while (true) {

                var random = true;
                var numberOfDigits = 9;

                if (random) {
                    a = Integers.Random(numberOfDigits, prime: true);
                    b = Integers.Random(numberOfDigits, prime: true);
                }
                else {
                    a = BigInteger.Parse("498227");
                    b = BigInteger.Parse("776221");
                }

                var n = (a * b);
                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();

                Clear();
                Print($" n {n,12} | l {n_numberOfDigits}");
                Print($" √ {n_integerSqrt,12}");
                Print($" a {a,12}");
                Print($" b {b,12}");
                Print();

                var nt = BigInteger.Pow(10, n.NumberOfDigits() - 4);
                var nst = BigInteger.Pow(10, n_integerSqrt.NumberOfDigits() - 4);
                var cst = BigInteger.Pow(10, 2);

                var nr = n / nt / cst;

                Print($"nr  {nr,-12}");
                Print($"cst {cst,-12}");

                var underCount = 0;
                var overCount = 0;
                for (BigInteger x = 999; x >= 100; x--) {

                    for (BigInteger y = 100; y <= x; y++) {
                        var z = x * y;

                        //Print($" {x,4} * {y,4} = {z,4}");

                        while (z > nr) {
                            z /= 10;
                        }

                        //Print($" {x,4} * {y,4} = {z,4}");
                        if (z == nr) {
                            z = (x * y) * nst;

                            if (z < n) {
                                underCount++;
                            }
                            else {
                                overCount++;
                            }

                            //Print($" {x,4} * {y,4} = {z,4}");
                            //count++;
                        }

                        //Print($" {x,4} * {y,4} = {z,4}");
                        //count++;
                    }

                }

                Print($" u {underCount,12}");
                Print($" o {overCount,12}");

                Prompt();
            }
        }

        static void Trails_v3() {
            var a = BigInteger.Zero;
            var b = BigInteger.Zero;

            while (true) {

                var random = true;
                var numberOfDigits = 6;

                if (random) {
                    a = Integers.Random(numberOfDigits, prime: true);
                    b = Integers.Random(numberOfDigits, prime: true);
                }
                else {
                    a = BigInteger.Parse("498227");
                    b = BigInteger.Parse("776221");
                }

                var n = (a * b);
                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();

                Clear();
                Print($" n {n,12} | l {n_numberOfDigits}");
                Print($" √ {n_integerSqrt,12}");
                Print($" a {a,12}");
                Print($" b {b,12}");
                Print();

                var t = 2;
                var lt = new BigInteger(100);
                var digitTargets = new List<BigInteger>();
                var lengthTargets = new List<BigInteger>();
                var stepTargets = new List<BigInteger>();

                while (true) {
                    var digitTarget = n % lt;
                    var lengthTarget = lt;
                    var stepTarget = lt / 10;

                    digitTargets.Add(digitTarget);
                    lengthTargets.Add(lengthTarget);
                    stepTargets.Add(stepTarget);

                    Print($" {t,2} | {stepTarget,12} {lengthTarget,12} {digitTarget,12}");

                    lt *= 10;
                    t++;
                    if (lt > n) {
                        break;
                    }
                }


                Prompt();
            }
        }

        static void Trails_v4() {
            var digits = new Dictionary<BigInteger, BigInteger>();

            foreach (var p in Integers.Products(9)) {
                //Print($"{p}");

                var lastDigit = p.P % 10;
                if (!digits.ContainsKey(lastDigit)) {
                    digits.Add(lastDigit, BigInteger.One);
                }
                else {
                    digits[lastDigit]++;
                }

            }

            foreach (var ld in digits) {
                Print($"{ld.Key}: {ld.Value}");
            }

            //Prompt();
        }

        static void Trails_v5() {
            var a = BigInteger.Zero;
            var b = BigInteger.Zero;

            var sl = 10m;
            var sh = 0m;

            while (true) {

                var random = true;
                var numberOfDigits = 2;

                if (random) {
                    a = Integers.Random(numberOfDigits, prime: true);
                    b = Integers.Random(numberOfDigits, prime: true);
                    
                    while (a == b) {
                        b = Integers.Random(numberOfDigits, prime: true);
                    }
                }
                else {
                    a = BigInteger.Parse("498227");
                    b = BigInteger.Parse("776221");
                }

                if (b > a) {
                    (a, b) = (b, a);
                }

                var n = (a * b);
                var n_integerSqrt = n.IntegerSqrt();
                var n_integerCbrt = n.IntegerCbrt();
                var n_numberOfDigits = n.NumberOfDigits();

                Clear();
                Print($" n {n,12} | l {n_numberOfDigits}");
                Print($" √ {n_integerSqrt,12} | square");
                Print($" √ {n_integerCbrt,12} | cube");

                var ab_h = a + b;
                var ab_m = ab_h / 2;

                Print();
                Print($" a {a,12}");
                Print($" b {b,12}");
                Print();
                Print($" h {ab_h,12}");
                Print($" m {ab_m,12}");
                Print();

                Triangle_v2(n, a, b);
                //Circle(n, a, b);

                //var isoTriangle = new IsoscelesTriangle(n);
                //PrintT(isoTriangle);

                //var isoTriangle2 = new IsoscelesTriangle(n * 2);
                //PrintT(isoTriangle2);

                // Scalene Triangle

                // Right Triangle
                //var leg_a = a;
                //var leg_b = b;
                //var leg_c = ((a * a) + (b * b)).IntegerSqrt();
                //var leg_z = leg_c * leg_c;

                //Print($" {leg_z,12} | leg_z");
                //Print($" {leg_c,12} | leg_c");
                //Print();

                // 1.7320508075688772935274463415059
                //  17320508075688772935274463415059

                //var asdf = 1.7320508075688772935274463415059;

                //var sqrt_2 = Math.Sqrt(2);
                //var sqrt_3 = Math.Sqrt(3);
                //var sqrt_5 = Math.Sqrt(5);
                //var phi = (1d + sqrt_5) / 2d;
                //var phi_sqrt = Math.Sqrt(phi);

                //var pi_sqrt = Math.Sqrt(Math.PI);

                ////       2n              = base?
                //// .. a + b or a^2 + b^2 = side a and b?
                ////        n or √n        = height?

                //var q2 = n.Multiply(sqrt_2) / 2;
                //var q3 = n.Multiply(sqrt_3) / 3;
                //var q5 = n.Multiply(sqrt_5) / 5;
                //var qg = n.Multiply(phi_sqrt).Divide(phi);
                //var qp = n.Multiply(pi_sqrt).Divide(Math.PI);

                //Print($"q2 {q2,12} | √ {q2.IntegerSqrt()} | g {BigInteger.DivRem(n, q2)}");
                //Print($"q3 {q3,12} | √ {q3.IntegerSqrt()} | g {BigInteger.DivRem(n, q3)}");
                //Print($"q5 {q5,12} | √ {q5.IntegerSqrt()} | g {BigInteger.DivRem(n, q5)}");
                //Print($"qg {qg,12} | √ {qg.IntegerSqrt()} | g {BigInteger.DivRem(n, qg)}");
                //Print($"qp {qp,12} | √ {qp.IntegerSqrt()} | g {BigInteger.DivRem(n, qp)}");

                //var k = BigInteger.One;

                //var n_rem = BigInteger.Pow(n_integerCbrt, 3);

                //var isoTriangleD = new IsoscelesTriangle(n);
                //while (isoTriangleD.Remainder > 0) {
                //    isoTriangleD = new IsoscelesTriangle(n_rem - k);
                //    k++;
                //}

                //PrintT(isoTriangleD);

                Prompt();
                //Task.Delay(1000 / 30).Wait();
            }

            static void Triangle(BigInteger n, BigInteger right, BigInteger left) {
                var c = right;
                var a = left;
                var b = right + left;
                var height = b / 2;
                var perimeter = b + c + a;
                var area = (b * height) / 2;
                var area_sqrt = area.IntegerSqrt();

                Print($" {c,12} | side c      |");
                Print($" {a,12} | side a      | ");
                Print($" {b,12} | base        | {n.Pos(b)}");
                Print($" {height,12} | height      | {n.Pos(height)}");
                Print($" {perimeter,12} | perimeter   | {n.Pos(perimeter)}");
                Print($" {area,12} | area        | ar {area.Pos(right)} br {area.Pos(left)}");
                Print();

                var area_d = area - n;
                var area_d_sqrt = area_d.IntegerSqrt();

                Print($" {area_d,12} | area_d | √ {area_d_sqrt,4} | {n.Pos(area_sqrt),4}");
                Print();
            }

            static void Triangle_v2(BigInteger n, BigInteger right, BigInteger left) {
                var sqrt_2a = Math.Sqrt(2);
                var sqrt_2b = Math.Sqrt(sqrt_2a);
                var sqrt_2c = Math.Sqrt(sqrt_2b);
                var sqrt_2d = Math.Sqrt(2) * 3;

                var sqrt_10a = Math.Sqrt(10);

                var n_sqrt = Math.Sqrt(n.ToDouble());

                var c = n_sqrt * (n_sqrt + 1d);
                var a = n_sqrt * (n_sqrt - 1d);
                var b = c + a;
                var height = b / 2d;
                var perimeter = b + c + a;
                var area = (b * height) / 2d;
                var area_sqrt = Math.Sqrt(area);

                Print($" {c,12} | side c      | √ {Math.Sqrt(c)}");
                Print($" {a,12} | side a      | √ {Math.Sqrt(a)}");
                Print($" {b,12} | base        | √ {Math.Sqrt(b)}");
                Print($" {height,12} | height      |");
                Print($" {perimeter,12} | perimeter   | ");
                Print($" {area,12} | area        |");
                Print();

                var fk_a = n.Multiply(sqrt_2a);
                var fk_b = n.Multiply(sqrt_2b);
                var fk_c = n.Multiply(sqrt_2c);
                var fk_d = n.Multiply(sqrt_2d);
                var fk_e = n.Multiply(sqrt_10a);

                Print($" {fk_a,12} | fk_a | √ {Math.Sqrt(fk_a.ToDouble())}");
                Print($" {fk_b,12} | fk_b | √ {Math.Sqrt(fk_b.ToDouble())}");
                Print($" {fk_c,12} | fk_c | √ {Math.Sqrt(fk_c.ToDouble())}");
                Print($" {fk_d,12} | fk_d | √ {Math.Sqrt(fk_d.ToDouble())}");
                Print($" {fk_e,12} | fk_e | √ {Math.Sqrt(fk_e.ToDouble())}");
            }

            static void Circle(BigInteger n, BigInteger right, BigInteger left) {
                var sqrt_2 = Math.Sqrt(2);
                var sqrt_5 = Math.Sqrt(5);

                var phi = (1d + sqrt_5) / 2d;
                var phi_sqrt = Math.Sqrt(phi);

                var rad = 180d / Math.PI;

                var pi = Math.PI;

                var n_dbl = double.Parse(n.ToString());
                var n_sqrt = Math.Sqrt(n_dbl);

                var radius = n_dbl;
                var diameter = 2 * radius;
                var area = pi * (radius * radius);
                var circumference = (2 * pi) * radius;

                var @base = Math.Sqrt(area);
                var height = 2 * @base;

                var j = n_dbl / circumference;

                Print($" {radius,12} | radius");
                Print($" {diameter,12} | diameter");
                Print($" {circumference,12} | circumference");
                Print($" {area,12} | area");
                Print();

                //Print($" {j,12} | j");
                Print($" {@base,12} | @base");
                Print($" {height,12} | height");

                //var points = 8d;
                //var two_pi = (2d * Math.PI);
                //var step_theta = (2d * two_pi) / points;

                //for (double theta = 0d; theta < two_pi; theta += step_theta) {
                //    var x = radius * Math.Cos(theta);
                //    var y = radius * Math.Sin(theta);

                //    var z = (x == 0 ? 1 : x) * (y == 0 ? 1 : y);

                //    Print($" {z,12}");
                //}



            }

            static void Sqrts(BigInteger n, BigInteger a, BigInteger b) {
                var n_integerSqrt = n.IntegerSqrt();

                var sqrt_2 = Math.Sqrt(2);
                var sqrt_3 = Math.Sqrt(3);

                var h_sqrt = n_integerSqrt;
                var l_sqrt = n_integerSqrt;

                for (int i = 0; i < 4; i++) {
                    l_sqrt = l_sqrt.Divide(sqrt_2);

                    Print($" {l_sqrt,12} | l_sqrt1");
                }

                for (int i = 0; i < 4; i++) {
                    l_sqrt = l_sqrt.Multiply(sqrt_2);

                    Print($" {l_sqrt,12} | l_sqrt2");
                }

                for (int i = 0; i < 4; i++) {
                    h_sqrt = h_sqrt.Multiply(sqrt_2);

                    Print($" {h_sqrt,12} | h_sqrt");
                }
            }
        }

        static void Trails_v6() {
            var random = true;
            var numberOfDigits = 2;

            while (true) {
                var a = BigInteger.Parse("498227");
                var b = BigInteger.Parse("776221");

                if (random) {
                    a = Integers.Random(numberOfDigits, prime: true);
                    b = Integers.Random(numberOfDigits, prime: true);

                    while (a == b) {
                        b = Integers.Random(numberOfDigits, prime: true);
                    }
                }

                if (b > a) {
                    (a, b) = (b, a);
                }

                var n = (a * b);
                var n_integerSqrt = n.IntegerSqrt();
                var n_integerCbrt = n.IntegerCbrt();
                var n_numberOfDigits = n.NumberOfDigits();

                Clear();
                Print($" n {n,12} | l {n_numberOfDigits}");
                Print($" √ {n_integerSqrt,12} | square");
                Print($" √ {n_integerCbrt,12} | cube");

                var ab_h = a + b;
                var ab_m = ab_h / 2;

                Print();
                Print($" a {a,12}");
                Print($" b {b,12}");
                Print();
                Print($" h {ab_h,12}");
                Print($" m {ab_m,12}");
                Print();

                Print($"gr {BigIntegerExtensions.Phi,12}");
                Print();


                var n_dbl = n.ToDouble();
                var n_sqrt = Math.Sqrt(n_dbl);

                var n_sqrt_h = n_sqrt * BigIntegerExtensions.Phi;
                var n_sqrt_l = n_sqrt / BigIntegerExtensions.Phi;

                Print($" l {n_sqrt_h,12}");
                Print($" l {n_sqrt_l,12}");

                Print();

                //var t = double.Parse(n.ToString());
                //var t = n;
                //var d = n / 2;
                //var sqrt_2 = Math.Sqrt(2);

                //var k = new BigInteger(3);
                //var s = BigInteger.Zero;

                //while (k <= t) {
                //    t -= k;
                //    k += 2;
                //    s += 1;
                //}
                //Print();

                //var z = t * k;

                //Print($" {t,12} | t | g {n.Gcd(t),12}");
                //Print($" {k,12} | k | g {n.Gcd(k),12}");
                //Print($" {s,12} | s | g {n.Gcd(s),12}");
                //Print($" {z,12} | z | g {n.Gcd(s),12}");

                Prompt();

            }

        }

        static void Trails_v7() {
            var sqrt_2 = Math.Sqrt(2);

            var b = new BigInteger(2);
            var n = new BigInteger(2);
            var e = new BigInteger(1);

            while (true) {
                if (!e.IsProbablyPrime()) {
                    n *= b;
                    e++;
                    continue;
                }

                var isMp = Integers.MersennePrimeExponents.Contains(e);
                var mpTag = isMp ? "~" : "!";

                //var ep = Math.Sqrt(double.Parse(e.ToString()));

                var p = n - 1;

                Print();
                Print($" {mpTag} {b,3}^{e,-3} {p}");

                //Print($" {"p",13} | {p}");

                var p_integerSqrt = p.IntegerSqrt();

                Print($" {"√",15} {p_integerSqrt}");

                var es = e * e;

                var ex = (p / es);

                Print($" {"ex",15} {ex}");

                if (!isMp) {
                    //var rm = new ReverseMultiply(p);

                    //var product = rm.TrySolve();

                    //var dp_l = p.DerivedPrime(e);
                    //var dp_h = p / dp_l;
                    //var dp_d = dp_h - dp_l;
                    //var dp_dr = p % dp_d;
                    //var dp_dr_sqrt = dp_dr.IntegerSqrt();

                    //Print($" {"dp_l",12} {dp_l,16}");
                    //Print($" {"dp_h",12} {dp_h,16}");

                    //Print($" {"dp_d",12} {dp_d,16} | pr {p % dp_d} √ {dp_dr_sqrt}");
                    //Print();

                    Unmultiply(p, e);
                }

                //var invalidate = false;
                //if (p.CheckLucasLehmerPrimality(e)) {
                //    Print($" ~ | {p}");
                //    invalidate = true;
                //}

                //if (p.CheckMersennePrimality(e)) {
                //    Print($" ~ | {p}");
                //    invalidate = true;
                //}

                //if (invalidate && !p.IsProbablyPrime()) {
                //    Print($" ! | {p}");
                //}

                Prompt();
                n *= b;
                e++;
            }

            static bool CheckPrimality(BigInteger canidate, BigInteger primeExponent) {
                var d = canidate;
                var s = new BigInteger(4);
                for (BigInteger i = 0; i < primeExponent - 2; i++) {
                    s = BigInteger.ModPow(s, 2, canidate) - 2;

                    d -= s;

                }

                Print($" d {d}");

                if (s == 0) {
                    return true; // Prime
                }

                return false; // Composite
            }

            static void Unmultiply(BigInteger canidate, BigInteger primeExponent) {

                if (primeExponent != 101) {
                    return;
                }

                // 101101101101
                // 336323216032
                // 7432339208719, 341117531003194129
                // 12264586153 = 44029 × 278557
                // 219687786701

                var two_E = primeExponent * 2;
                var two_E_Sqrd = two_E * two_E;

                var c_t = BigInteger.Pow(10, two_E.NumberOfDigits());
                var c_d = canidate % c_t;

                var products = new List<Product>();
                for (BigInteger x = two_E; x <= two_E_Sqrd; x += two_E) {

                    for (BigInteger y = x; y <= two_E_Sqrd; y += two_E) {
                        var x_m = x + 1;
                        var y_m = y + 1;
                        var z = x_m * y_m;

                        if (z % c_t == c_d) {
                            //Print($" {x_m} * {y_m} = {z}");

                            products.Add(new Product(x_m, y_m, z));
                        }
                    }
                }

                foreach (var product in products) {
                    if (canidate % product.C == 0) {
                        Print($" * {product}");
                        return;
                    }
                }

                products = products.OrderByDescending(x  => x.C).ToList();

                foreach (var product in products) {
                    Print($" ? {product}");

                    var x_boundary = product.C * product.C;
                    if (primeExponent % 10 == 1) {
                        x_boundary = BigInteger.Pow(two_E, 6);
                    }

                    var x_m = product.C - 1;
                    for (BigInteger x_p = x_m; x_p < two_E_Sqrd; x_p += x_m) {

                        if (canidate % (x_p + 1) == 0) {
                            Print($" * {(x_p + 1)}");
                            return;
                        }
                    }

                }

                Print($" * DONE");
            }
        }

        static void Trails_v8() {
            // 2^101
            // 7432339208719
            // 341117531003194129

            var p = BigInteger.Pow(2, 101) - 1;

            var d = BigInteger.Pow(2, 202);
            d -= (d % 202);

            var k = new BigInteger(2);
            while (true) {
                while (d % k == 0) {
                    d /= k;

                    Print($" k {k} | d {d} | {p.Gcd(k)}");
                }

                if (d.IsProbablyPrime()) {
                    break;
                }

                k += k == 2 ? 1 : 2;
            }

            Print($" done");
        }

        static void Trails_v9() {
            // 2^101
            // 7432339208719
            // 341117531003194129

            //Integers.MersennePrimeExponents
            var boundary = Integers.MersennePrimeExponents.Last();

            var p = new BigInteger(101);
            while (true) {
                while (!p.IsProbablyPrime()) {
                    p += 2;
                }

                var isMp = Integers.MersennePrimeExponents.Contains(p);
                var mpTag = isMp ? "~" : " ";

                Print($" {mpTag} {p}");

                if (!isMp) {
                    var two_P = p * 2;
                    var e = two_P;

                    while (e < boundary && !Integers.MersennePrimeExponents.Contains(e + 1)) {
                        e += two_P;
                    }

                    if (Integers.MersennePrimeExponents.Contains(e + 1)) {
                        Print($" + {e + 1}");

                        p = e + 1;
                        continue;
                    }
                    else {
                        while (!(e + 1).IsProbablyPrime()) {
                            e += two_P;
                        }

                        Print($" - {e + 1}");
                    }
                }

                p += 2;
                Prompt();
            }

            Print($" done");
        }

        static void Trails_v10() {
            // 2^101
            // 7432339208719
            // 341117531003194129

            //Integers.MersennePrimeExponents
            //var boundary = Integers.MersennePrimeExponents.Last();

            var sqrt_2 = Math.Sqrt(2);

            var b = new BigInteger(2);
            var n = new BigInteger(2);
            var e = new BigInteger(1);

            while (true) {
                if (!e.IsProbablyPrime()) {
                    n *= b;
                    e++;
                    continue;
                }

                var isMp = Integers.MersennePrimeExponents.Contains(e);
                var mpTag = isMp ? "~" : "!";

                //var ep = Math.Sqrt(double.Parse(e.ToString()));

                var p = n - 1;

                Print();
                Print($" {mpTag} {b,3}^{e,-4} {p}");

                //Print($" {"p",13} | {p}");

                var p_integerSqrt = p.IntegerSqrt();

                Print($" {"√",10} {p_integerSqrt}");

                var d = p + 1;
                var k = BigInteger.Zero;

                while (d > e) {
                    if (d % e == 0) {

                        Print($" {"s",10} {d / e}");
                        k++;
                    }

                    d /= 10;
                }

                Print($" {"k",10} {k}");
                Print($" {"d",10} {d}");

                if (!isMp) {
                    var dp_l = p.DerivedPrime(e);
                    var dp_h = p / dp_l;
                    var dp_d = dp_h - dp_l;
                    var dp_dr = p % dp_d;
                    var dp_dr_sqrt = dp_dr.IntegerSqrt();

                    Print($" {"dp_l",12} {dp_l,16}");
                    Print($" {"dp_h",12} {dp_h,16}");

                    Print($" {"dp_d",12} {dp_d,16} | pr {p % dp_d} √ {dp_dr_sqrt}");
                    Print();
                }

                Prompt();
                n *= b;
                e++;
            }

            Print($" done");
        }

        static void Trails_v11() {
            // 2^101
            // 7432339208719
            // 341117531003194129

            //Integers.MersennePrimeExponents
            //var boundary = Integers.MersennePrimeExponents.Last();

            var sqrt_2 = Math.Sqrt(2);

            var b = new BigInteger(2);
            var n = new BigInteger(2);
            var e = new BigInteger(1);

            while (true) {
                while (e < 7) {
                    n *= b;
                    e++;
                    continue;
                }

                if (!e.IsProbablyPrime()) {
                    n *= b;
                    e++;
                    continue;
                }

                var isMp = Integers.MersennePrimeExponents.Contains(e);
                var mpTag = isMp ? "~" : "!";

                var p = n - 1;
                var p_sqrt = p.IntegerSqrt();

                //var x = (p - 1) / e;
                var two_e = e * 2;
                var thr_e = e * 3;
                var six_e = e * 6;

                Print($" {mpTag} {b,3}^{e,-4} {p}");
                Print($" {"√",13} | {p_sqrt}");
                //Print($" {"x",13} | {x}");
                Print();

                var w = (p - 1) / e;
                var x = (p - 1) / two_e;
                var y = (p - 1) / thr_e;
                var z = (p - 1) / six_e;

                Print($" w | {w,16}");
                Print($" x | {x,16}");
                Print($" y | {y,16}");
                Print($" z | {z,16}");
                Print();

                if (!isMp) {
                    //Factorize(p);

                    var k = p_sqrt - (p_sqrt % thr_e);
                    var s = BigInteger.Zero;
                    var g = BigInteger.Zero;
                    var g_sqrt = BigInteger.Zero;

                    var q = BigInteger.Zero;
                    var q_sqrt = BigInteger.Zero;
                    var h_h = BigInteger.Zero;
                    var h_l = BigInteger.Zero;

                    while (k < w) {
                        s = BigInteger.Pow(k, 2);

                        g = s + p;
                        g_sqrt = g.IntegerSqrt();

                        q = g - p;
                        q_sqrt = q.IntegerSqrt();

                        h_h = g_sqrt + q_sqrt;
                        h_l = g_sqrt - q_sqrt;

                        if (p % h_h == 0) {
                            Print($"   |  {h_l,16} * {h_h,16}");

                            break;
                        }

                        k += thr_e;
                    }
                    Print();
                }
                else {
                    Print();
                }

                //Prompt();
                n *= b;
                e++;
            }

            Print($" done");

            static void Factorize(BigInteger n) {
                if (n.IsProbablyPrime()) {
                    return;
                }

                var h = BigInteger.Zero;
                var l = n;

                var k = new BigInteger(2);
                var e = BigInteger.Zero;
                var s = n;
                while (s > 1) {
                    e = BigInteger.Zero;
                    while (s % k == 0) {
                        s /= k;
                        e++;
                    }

                    if (e != 0) {
                        //s *= BigInteger.Pow(k, (int)e);

                        Print($" {k,8}^{e,-16}");

                        if (k > h) {
                            h = k;
                        }

                        if (k < l) {
                            l = k;
                        }
                    }

                    k++;
                }

                var m = (h + l) / 2;
                var d = BigInteger.Pow(m, 2) - n;
                var d_sqrt = d.IntegerSqrt();

                Print($" m | {m,16}");
                Print($" d | {d_sqrt,16} | {d,16}");

                Print();
            }

            static void Low(BigInteger n, BigInteger e, BigInteger p) {
                var two_e = e * 2;

                var boundary = (n / 2).IntegerSqrt();
                var j = new BigInteger(2);
                var c = new BigInteger(0);
                var q = new BigInteger(0);
                var ql = new BigInteger(0);
                var qh = new BigInteger(0);

                var w = new BigInteger(0);
                var k = new BigInteger(0);
                var h = new BigInteger(0);

                var d = new BigInteger(0);
                var ds = new BigInteger(0);
                var dl = new BigInteger(0);
                var dh = new BigInteger(0);

                //while (j < two_e) {
                //    j *= 2;
                //}

                while (j * 2 < boundary) {
                    q = 1;
                    ql = j - q;
                    qh = j + q;

                    w = BigInteger.Pow(qh, 1);
                    k = (p - (p % j)) + j;
                    h = k.IntegerSqrt();

                    d = k - p;
                    ds = d.IntegerSqrt();
                    dl = h - ds;
                    dh = h + ds;

                    //Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8}");
                    Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8} | l {dl,8} | h {dh,8}");

                    j *= 2;
                }
            }

            static void High(BigInteger n, BigInteger e, BigInteger p) {
                var boundary = n / 2;
                var j = (n / 2).IntegerSqrt();
                var c = new BigInteger(0);
                var q = new BigInteger(0);
                var ql = new BigInteger(0);
                var qh = new BigInteger(0);

                var w = new BigInteger(0);
                var k = new BigInteger(0);
                var h = new BigInteger(0);

                var d = new BigInteger(0);
                var ds = new BigInteger(0);
                var dl = new BigInteger(0);
                var dh = new BigInteger(0);

                while (j < boundary) {
                    q = BigInteger.Max(1, (BigInteger.Pow(4, (int)c) / 2));
                    ql = j - q;
                    qh = j + q;

                    w = BigInteger.Pow(qh, 2);
                    k = p + w;
                    h = k.IntegerSqrt();

                    d = k - p;
                    ds = d.IntegerSqrt();
                    dl = h - ds;
                    dh = h + ds;

                    if (dl > 1 && dh > 1) {
                        if (p % dl == 0 || p % dh == 0) {
                            Print($" {"~",13} | l {dl,8} | h {dh,8}");
                            //Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8} | l {dl,8} | h {dh,8}");
                            Prompt();
                            break;
                        }
                    }

                    //Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8}");
                    //Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8} | l {dl,8} | h {dh,8}");

                    j *= 2;
                    c++;
                }
            }

            static bool CheckPrimality(BigInteger canidate, BigInteger baseOf, BigInteger exponent) {
                if (canidate <= 0) {
                    return false;
                }

                var a = new BigInteger(2);
                if (baseOf == 2) {
                    a = new BigInteger(6);
                }

                var v = BigInteger.Pow(a, (int)baseOf);
                if (v > canidate) {
                    return false; // Too small for this method, but may be prime.
                }

                var s = v;
                for (int i = 0; i < exponent - 1; i++) {
                    s = BigInteger.ModPow(s, baseOf, canidate);

                    var np = canidate.Pos(s);
                    var ep = s.Pos(exponent);

                    Print($" {"s",13} | {s,16} | np {np.X,8} {np.Y,8} | ep {ep.X,8} {ep.Y,8}");
                }

                if (s == v) {
                    return true; // Prime
                }

                return false; // Composite
            }
        }

        static void Trails_v12() {
            var random = true;
            var numberOfDigits = 4;

            var m_min = 2m;
            var m_max = 1m;

            // 7-8 digits
            // mn 1.0001003109639883
            // mx 1.7087774294670846

            while (true) {
                var a = BigInteger.Parse("498227");
                var b = BigInteger.Parse("776221");

                if (random) {
                    a = Integers.Random(numberOfDigits, prime: true);
                    b = Integers.Random(numberOfDigits, prime: true);

                    while (a == b) {
                        b = Integers.Random(numberOfDigits, prime: true);
                    }
                }

                if (b > a) {
                    (a, b) = (b, a);
                }

                var n = (a * b);
                var n_integerSqrt = n.IntegerSqrt();
                var n_integerCbrt = n.IntegerCbrt();
                var n_numberOfDigits = n.NumberOfDigits();

                Clear();
                Print($" n {n,12} | l {n_numberOfDigits}");
                Print($" √ {n_integerSqrt,12}");
                Print();

                var ab_h = a + b;
                var ab_m = ab_h / 2;
                var ab_x = ab_m * ab_m;
                var ab_y = ab_x - n;
                var ab_y_sqrt = ab_y.IntegerSqrt();

                Print();
                Print($" a {a,12}");
                Print($" b {b,12}");
                Print();
                //Print($" h {ab_h,12}");
                Print($" m {ab_m,12}");
                Print();

                var s_dbl = n_integerSqrt.ToDecimal();
                var m_dbl = ab_m.ToDecimal();

                var g = (m_dbl / s_dbl);

                if (g < m_min) {
                    m_min = g;
                }

                if (g > m_max) {
                    m_max = g;
                }

                Print($" g {g,12}");
                Print();

                Print($"mn {m_min,12}");
                Print($"mx {m_max,12}");
                Print();

                Task.Delay(1000 / 30).Wait();
                //Prompt();

                //var triangle = new IsoscelesTriangle(n);
                ////while (triangle.Remainder > 0) {
                ////    triangle = new IsoscelesTriangle(triangle.Remainder);
                ////}

                //Print($" {triangle.N,16} | n");
                //Print($" {triangle.Side,16} | Side");
                //Print($" {triangle.Base,16} | Base");
                //Print($" {triangle.Height,16} | Height");
                //Print($" {triangle.Area,16} | Area");
                //Print($" {triangle.Perimeter,16} | Perimeter");
                //Print($" {triangle.Remainder,16} | Remainder");

                //var sqrt_2 = Math.Sqrt(2);
                //var m = n_integerSqrt;
                //while (m > 1) {
                //    var mh = m;
                //    var ml = m.Divide(sqrt_2);
                //    var ms = ml + mh;

                //    Print($" d {m,6} | {ml,6} | {mh,6} | {ms,6}");

                //    m = m.Divide(sqrt_2);
                //}

                //for (int i = 2; i <= 8; i++) {
                //    var mh = m;
                //    var ml = m.Divide(sqrt_2);
                //    var ms = ml + mh;

                //    Print($" d {m,6} | {ml,6} | {mh,6} | {ms,6}");

                //    m = m.Multiply(sqrt_2);
                //}




                //var m = n_integerSqrt * 4;
                //while (m > 1) {
                //    m /= 2;

                //    var dd = n_integerSqrt + m;
                //    Print($" d {m,12} | {dd,12}");
                //}

                //Prompt();

            }

        }

        static void Trails_v13() {
            var random = true;
            var numberOfDigits = 29;

            var nd = (numberOfDigits - 1);
            var tl = BigInteger.Pow(10, nd);
            var th = BigInteger.Pow(10, nd + 1) - 1;
            var tk = BigInteger.Zero;

            //Print($" tl {tl,12}");
            //Print($" th {th,12}");
            //Prompt();

            // 8:   1.7390891840607210626185958254
            // 14:  1.7392527599574610320348280685
            // 18:  1.7392527155229396128451908706
            // 20:  1.7392527128689894991700380921
            // 22:  1.7392527130985944885972285404
            // 38:  1.7392527130926086328325551329
            // 58:  1.7392527130926086325993914494

            var m_min = 2m;
            var m_max = 1m;

            while (true) {
                var a = th - tk;
                var b = tl + tk;

                if (a == b) {
                    Print($" ~ DONE");
                    Prompt();
                }

                var n = (a * b);
                var n_integerSqrt = n.IntegerSqrt();
                var n_numberOfDigits = n.NumberOfDigits();

                Clear();
                Print($" n {n,12} | l {n_numberOfDigits}");
                Print($" √ {n_integerSqrt,12}");
                Print();

                var ab_h = a + b;
                var ab_m = ab_h / 2;

                Print();
                Print($" a {a,12}");
                Print($" b {b,12}");
                Print();
                //Print($" h {ab_h,12}");
                Print($" m {ab_m,12}");
                Print();

                var s_dbl = n_integerSqrt.ToDecimal();
                var m_dbl = ab_m.ToDecimal();

                var g = (m_dbl / s_dbl);

                if (g < m_min) {
                    m_min = g;
                }

                if (g > m_max) {
                    m_max = g;
                }

                Print($" g {g,12}");
                Print();

                Print($"mn {m_min,12}");
                Print($"mx {m_max,12}");
                Print();

                Task.Delay(1000 / 30).Wait();
                //Prompt();
                tk++;
            }

        }

        static void Trails_v14() {
            // 2^101
            // 7432339208719
            // 341117531003194129

            //Integers.MersennePrimeExponents
            //var boundary = Integers.MersennePrimeExponents.Last();

            var n = new BigInteger(1);
            var e = new BigInteger(1);

            foreach (var p in Integers.PrimeNumbers()) {
                if (p < 3) {
                    continue;
                }

                n *= p;

                var n_sqrt = n.IntegerSqrt();

                Print($" n   {n,36}");
                Print($"    e {e,16} p {p,16}");
                Print($"    √ {n_sqrt,16}");
                Print();

                var k = new BigInteger(1);

                if ((n - p) % 4 == 0) {
                    k = n - p;
                }
                else if ((n + p) % 4 == 0) {
                    k = n + p;
                }

                //var k = (n - 1) / 2;
                //var x = (3 + (n / 3)) / 2;
                //var z = (p + (n / p)) / 2;

                //Print($"    k {k,16}");
                //Print($"    x {x,16}");
                //Print($"    z {z,16}");
                Print();

                Factorize(k);

                e += 1;

                Prompt();
            }

            static void Factorize(BigInteger n) {
                if (n.IsProbablyPrime()) {

                    Print($" {n,8}^{1,-16} s {n,16}");
                    Print();
                    return;
                }

                var k = new BigInteger(2);
                var e = BigInteger.Zero;
                var s = BigInteger.One;
                while (k <= n) {
                    e = BigInteger.Zero;
                    while (n % k == 0) {
                        n /= k;
                        e++;
                    }

                    if (e != 0) {
                        s *= BigInteger.Pow(k, (int)e);

                        Print($" {k,8}^{e,-16} s {s,16}");
                    }

                    k++;
                }

                Print();
            }

            static void Low(BigInteger n, BigInteger e, BigInteger p) {
                var two_e = e * 2;

                var boundary = (n / 2).IntegerSqrt();
                var j = new BigInteger(2);
                var c = new BigInteger(0);
                var q = new BigInteger(0);
                var ql = new BigInteger(0);
                var qh = new BigInteger(0);

                var w = new BigInteger(0);
                var k = new BigInteger(0);
                var h = new BigInteger(0);

                var d = new BigInteger(0);
                var ds = new BigInteger(0);
                var dl = new BigInteger(0);
                var dh = new BigInteger(0);

                //while (j < two_e) {
                //    j *= 2;
                //}

                while (j * 2 < boundary) {
                    q = 1;
                    ql = j - q;
                    qh = j + q;

                    w = BigInteger.Pow(qh, 1);
                    k = (p - (p % j)) + j;
                    h = k.IntegerSqrt();

                    d = k - p;
                    ds = d.IntegerSqrt();
                    dl = h - ds;
                    dh = h + ds;

                    //Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8}");
                    Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8} | l {dl,8} | h {dh,8}");

                    j *= 2;
                }
            }

            static void High(BigInteger n, BigInteger e, BigInteger p) {
                var boundary = n / 2;
                var j = (n / 2).IntegerSqrt();
                var c = new BigInteger(0);
                var q = new BigInteger(0);
                var ql = new BigInteger(0);
                var qh = new BigInteger(0);

                var w = new BigInteger(0);
                var k = new BigInteger(0);
                var h = new BigInteger(0);

                var d = new BigInteger(0);
                var ds = new BigInteger(0);
                var dl = new BigInteger(0);
                var dh = new BigInteger(0);

                while (j < boundary) {
                    q = BigInteger.Max(1, (BigInteger.Pow(4, (int)c) / 2));
                    ql = j - q;
                    qh = j + q;

                    w = BigInteger.Pow(qh, 2);
                    k = p + w;
                    h = k.IntegerSqrt();

                    d = k - p;
                    ds = d.IntegerSqrt();
                    dl = h - ds;
                    dh = h + ds;

                    if (dl > 1 && dh > 1) {
                        if (p % dl == 0 || p % dh == 0) {
                            Print($" {"~",13} | l {dl,8} | h {dh,8}");
                            //Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8} | l {dl,8} | h {dh,8}");
                            Prompt();
                            break;
                        }
                    }

                    //Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8}");
                    //Print($" {"k",13} | {k,8} | h {h,8} | d {d,8} | s {ds,8} | l {dl,8} | h {dh,8}");

                    j *= 2;
                    c++;
                }
            }

            static bool CheckPrimality(BigInteger canidate, BigInteger baseOf, BigInteger exponent) {
                if (canidate <= 0) {
                    return false;
                }

                var a = new BigInteger(2);
                if (baseOf == 2) {
                    a = new BigInteger(6);
                }

                var v = BigInteger.Pow(a, (int)baseOf);
                if (v > canidate) {
                    return false; // Too small for this method, but may be prime.
                }

                var s = v;
                for (int i = 0; i < exponent - 1; i++) {
                    s = BigInteger.ModPow(s, baseOf, canidate);

                    var np = canidate.Pos(s);
                    var ep = s.Pos(exponent);

                    Print($" {"s",13} | {s,16} | np {np.X,8} {np.Y,8} | ep {ep.X,8} {ep.Y,8}");
                }

                if (s == v) {
                    return true; // Prime
                }

                return false; // Composite
            }
        }

        static void Trails_v15() {
            var globalWatch = new Stopwatch();
            var powerOf_2 = new PowerOf(2, 0);
            powerOf_2.Next();

            while (true) {
                if (!powerOf_2.Exponent.IsProbablyPrime()) {
                    powerOf_2.Next();
                    continue;
                }

                //Print();

                var n = powerOf_2.Value;
                var p = n - 1;

                //Print($"   {powerOf_2,16} | {p,16}");
                //Print();

                if (p.IsProbablyPrime()) {
                    powerOf_2.Next();
                    continue;
                }

                Print();
                Print($"   {powerOf_2,16} | {p,16}");
                Print();

                //foreach (var powerOf in p.Factorize()) {
                //    Print($"   {powerOf,16}");
                //}

                globalWatch.Restart();
                var dp = p.DerivedPrime_Unbound(powerOf_2.Exponent);
                globalWatch.Stop();
                Print($"  dp {dp,16} | {globalWatch.Elapsed}");

                //var d = p / 2;
                //var k = powerOf_2.Exponent * 2;

                //var oddNumber = BigInteger.One;
                //var result = BigInteger.Zero;

                //while (d >= oddNumber) {
                //    d -= oddNumber;
                //    oddNumber += k;
                //    result++;
                //}

                //Print($"   d {d,16} o {oddNumber,16} = {result,16}");

                powerOf_2.Next();

                //Prompt();
            }

        }

        static void Trails_v16() {
            var powerOf_2 = new PowerOf(2, 0);
            powerOf_2.Next();

            while (true) {
                Print($"   {powerOf_2,16} | {powerOf_2.Value,16}");

                var k = powerOf_2.Value + 1;
                var c = BigInteger.Zero;

                var c3 = BigInteger.Zero;  // = A001045
                var c5 = BigInteger.Zero;  // = A007910
                var c7 = BigInteger.Zero;  // = A077947
                var c11 = BigInteger.Zero; // = 
                while (true) {
                    if (k.IsProbablyPrime()) {
                        c++;
                    }

                    if (k % 11 == 0) {
                        c11++;
                    }

                    k += 1;

                    if (k.IsPowerOfTwo) {
                        break;
                    }
                }

                var l = (k - powerOf_2.Value) / 2;

                Print($"   c {c,8} l {l,8} c11 {c11,8}");

                powerOf_2.Next();

                Prompt();
            }
        }

        static void Trails_v17() {
            var powerOf_2 = new PowerOf(2, 0);

            while (powerOf_2.Exponent < 7) {
                powerOf_2.Next();
            }

            while (true) {
                while (!powerOf_2.Exponent.IsProbablyPrime()) {
                    powerOf_2.Next();
                }

                Print($"   {powerOf_2,16} | {powerOf_2.Value,16}");

                var p_limit = (powerOf_2.Value - (powerOf_2.Value / 2)) / 2;

                var d = powerOf_2.Value - 1;

                var x = d / powerOf_2.Exponent;
                var y = 2 * powerOf_2.Exponent;
                var z = x - y;
                var zq = BigInteger.DivRem(z, powerOf_2.Exponent);

                Print($"   l {p_limit,16}");
                Print($"   x {x,16}");
                Print($"   y {y,16}");
                Print($"   z {z,16} qr {zq,16}");
                Print();

                var k = powerOf_2.Exponent;

                var c = BigInteger.Zero;
                while (k > 1) {
                    var cl = d.ModLeft(k) / k;
                    c += cl;

                    k -= 2;
                    while (k > 1 && k.IsProbablyPrime()) {
                        k -= 2;
                    }


                }

                Print($"   C {c,16}");
                Print();

                powerOf_2.Next();

                Prompt();
            }
        }

        static void Trails_v18() {
            // 2^p + 1 / 2 + 1 = 2p ?


            // 3^p + 1 / 3 + 1 = 6p
            var powerOf_3 = new PowerOf(3, 0);
            powerOf_3.Next();

            while (true) {
                while (!powerOf_3.Exponent.IsProbablyPrime()) {
                    powerOf_3.Next();
                }

                var n = powerOf_3.Value;
                var p = (powerOf_3.Value + 1) / (powerOf_3.Base + 1);
                var p_sqrt = p.IntegerSqrt();

                Print($"   {powerOf_3,8} | {p,-16}");
                Print($"   √ {p_sqrt,16}");
                Print();

                if (!p.IsProbablyPrime()) {
                    var h = BigInteger.Zero;
                    var l = p;

                    foreach (var power in p.Factorize()) {
                        Print($"   {power,16}");

                        if (power.Base > h) {
                            h = power.Base;
                        }

                        if (l > power.Base) {
                            l = power.Base;
                        }
                    }
                    Print();

                    //var m = (h + l) / 2;
                    //var g = BigInteger.Pow(m, 2);

                    //Print($"   {m,16} | {g,16}");

                    //foreach (var power in (m - 1).Factorize()) {
                    //    Print($"   {power,16}");

                    //    if (power.Base > h) {
                    //        h = power.Base;
                    //    }

                    //    if (l > power.Base) {
                    //        l = power.Base;
                    //    }
                    //}
                    //Print();
                }

                Print();
                Prompt();

                powerOf_3.Next();
            }
        }
    }
}
