using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using PrimeNumbers.Extensions;
using PrimeNumbers.Helpers;
using PrimeNumbers.Entities;
using PrimeNumbers.Utilities;
using static PrimeNumbers.Extensions.BigIntegerExtensions;
using System.Windows.Forms;
using System.Threading;
using System.Net.Http;
using System.Text.RegularExpressions;
using Fractions;
using System.Security.Cryptography;
using static PrimeNumbers.Helpers.Graph3D;

namespace PrimeNumbers {
    class Program {
        #region Properties
        private static bool _debugMode = true;

        private static string _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
        private static string _currentDataPath = $@"{_currentDirectory}\Data";

        private static string _progressFilePath = $@"{_currentDataPath}\Progress.json";

        private static string _mersennePrimesFilePath = $@"{_currentDataPath}\MersennePrimes.json";
        private static string _verifiedMersennePrimesFilePath = $@"{_currentDataPath}\VerifiedMersennePrimes.json";

        private static string _verifiedMersennePrimesPath = $@"{_currentDataPath}\Mersenne";

        private static Progress _progress = new Progress();
        private static List<PrimeNumber> _primeNumbers = new List<PrimeNumber>();
        private static List<PrimeNumber> _verifiedPrimeNumbers = new List<PrimeNumber>();
        private static List<BigInteger> _specialDigits = new List<BigInteger>() {
            1, 3, 7, 9
        };

        private static int _primaryDegreeOfParallelism = 2;
        private static int _secondaryDegreeOfParallelism = 16;
        private static int _witnesses = 6;

        private static Stopwatch _overallStopwatch = new Stopwatch();

        private static TimeSpan _timeLimit = new TimeSpan(0, 0, 10);

        private static Dictionary<int, BigInteger> _typeCounts = new Dictionary<int, BigInteger>();
        private static Dictionary<int, int> _specialDigitCounts = new Dictionary<int, int>();

        private static FlipFlop _flipFlop = new FlipFlop();

        // Coord Plane: https://www.desmos.com/calculator/q8mwzeylbk
        #endregion

        #region Brute Properties
        private static BigInteger _index = new BigInteger();
        private static BigInteger _testedIndex = new BigInteger();

        private static List<PrimeGap> _primeGaps = new List<PrimeGap>();

        private static int _hits = 0;
        private static int _misses = 0;
        private static int _gap = 0;
        private static int _largestGap = 0;

        private static BigInteger _skippedCount = new BigInteger();
        private static BigInteger _duplicateCount = new BigInteger();
        #endregion

        #region Series Properties
        private static List<BigInteger> _seriesNumbers = new List<BigInteger>() {
            0, 4, 6, 2
        };

        private static List<BigInteger> _currentNumbers = new List<BigInteger>();

        private static BigInteger _seriesCount = new BigInteger();
        private static BigInteger _seriesGap = new BigInteger();
        private static BigInteger _largestSeriesGap = new BigInteger();

        private static List<string> _primeSeriesStrings = new List<string>();
        #endregion

        static void Main(string[] args) {
            Console.Title = "Prime Numbers";
            Console.WindowWidth = 162;
            Console.OutputEncoding = System.Text.Encoding.UTF8;



            //Initialize();

            //Thought1();
            //Thought2();
            //Thought3();
            //Thought4();

            //var graphy = new Graphy();

            //Selling();
            //Problem();

            //GetVerifiedMersennePrimes();
            //ZetaTesting();
            //IrrationalNumberGenerator();

            //Primalization(_primaryDegreeOfParallelism,
            //    _secondaryDegreeOfParallelism,
            //    _witnesses);

            //RandomPrimalization();
            //BrutePrimalization();

            //Statistics();

            //Thought6();
            Test();

            Logging.Log($"Finished.");
            Console.ReadKey();
        }

        public static void Test() {
            var n = BigInteger.Parse("35794234179725868774991807832568455403003778024228226193532908190484670252364677411513516111204504060317568667");
            var d1 = BigInteger.Parse("5846418214406154678836553182979162384198610505601062333");
            var d2 = BigInteger.Parse("6122421090493547576937037317561418841225758554253106999");

            var integerSqrt = n.Sqrt();

            var t = NextP(integerSqrt, 1);
            var b = NextP(integerSqrt, -1);

            Console.WriteLine($" n {n}");

            Console.WriteLine($" t {t}");
            Console.WriteLine($" b {b}");

            var td = (t * t) - n;
            var bd = n - (b * b);
            var sd = td + bd;

            Console.WriteLine($"td {td}");
            Console.WriteLine($"bd {bd}");

            Console.WriteLine($"sd {sd}");

            var x = n - sd;

            Console.WriteLine($" x {x}");

            // Need to use better Primality Test.
            BigInteger NextP(BigInteger s, BigInteger direction) {
                while (!s.IsProbablyPrime().IsPrime) { // IsLikelyPrime
                    s += direction;
                }

                return s;
            }
        }

        public static void Initialize() {
            _overallStopwatch.Start();

            if (File.Exists(_verifiedMersennePrimesFilePath)) {
                _verifiedPrimeNumbers = Logging.GetJsonFromFile<List<PrimeNumber>>(_verifiedMersennePrimesFilePath);
            }

            if (_debugMode) {
                return;
            }

            PeriodicTask.Run(() => {
                Logging.SaveJsonAsFile(_mersennePrimesFilePath, _primeNumbers);
                Logging.SaveJsonAsFile(_progressFilePath, _progress);
                //Console.Beep();

            }, new TimeSpan(0, 1, 0));

            if (File.Exists(_progressFilePath)) {
                _progress = Logging.GetJsonFromFile<Progress>(_progressFilePath);
                if (_progress == null) {
                    _progress = new Progress();
                }

                //Logging.Log($"    Sequence:   {BigInteger.Parse(_progress.Sequence).ToPrettyString()}");
            }

            if (File.Exists(_mersennePrimesFilePath)) {
                _primeNumbers = Logging.GetJsonFromFile<List<PrimeNumber>>(_mersennePrimesFilePath);
                _primeNumbers = _primeNumbers.OrderBy(x => x.NumberOfDigits).ToList();

                for (int i = 0; i < _primeNumbers.Count; i++) {
                    var mersennePrime = _primeNumbers[i];
                    mersennePrime.ID = i;
                }
                
                Logging.SaveJsonAsFile(_mersennePrimesFilePath, _primeNumbers);

                //Logging.Log($"    Discovered: {_mersennePrimes.Count}");
            }

            _primaryDegreeOfParallelism = Convert.ToInt32(ConfigurationManager.AppSettings["PrimaryDegreeOfParallelism"]);
            _secondaryDegreeOfParallelism = Convert.ToInt32(ConfigurationManager.AppSettings["SecondaryDegreeOfParallelism"]);
            _witnesses = Convert.ToInt32(ConfigurationManager.AppSettings["Witnesses"]);
        }

        public static void Statistics() {
            Logging.Log($"");
            Logging.Log($"<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>");

            var mSum = new BigInteger(1);
            var sum = new BigInteger(0);
            foreach (var typeCount in _typeCounts.OrderBy(x => x.Key)) {
                if (typeCount.Key != 0) {
                    sum += typeCount.Value;

                    if (sum != 0) {
                        mSum *= sum;
                    }
                }


                Logging.Log($"    {typeCount.Key,3}: {typeCount.Value,-9}, {GetChanceAsString((int)typeCount.Value)} %");

                //var vN = sum.AsSpecial();
                //var primeResult = vN.IsProbablyPrime_Parallel(false, 6, 1);

                //if (primeResult.IsPrime) {
                //    Logging.Log($"    {typeCount.Key,3}: {typeCount.Value,-9}, % {GetChanceAsString((int)typeCount.Value)}, s: {vN,9}", Color.Yellow);
                //}
                //else {
                //    Logging.Log($"    {typeCount.Key,3}: {typeCount.Value,-9}, % {GetChanceAsString((int)typeCount.Value)}, s: {vN,9}");
                //}
            }

            Logging.Log($"<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>");

            Logging.Log($"     Found: {_primeNumbers.Count} of {_testedIndex}");
            Logging.Log($"    Chance: {GetHitChanceAsString()} %");

            //Logging.Log($"<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>");

            //foreach (var primeGap in _primeGaps) {
            //    Logging.Log($"{primeGap.Gap,9}: {primeGap.GapNumbers.Count,-9}");

            //    //foreach (var gapNumber in primeGap.GapNumbers) {
            //    //    Logging.Log($"            {gapNumber.Index,9}: {gapNumber.Number.ToPrettyString(),-32}");
            //    //}
            //}

            //Logging.Log($"<<<<<<<<<<<<<<<<<<<<>>>>>>>>>>>>>>>>>>>>");
        }

        public static void Selling() {
            var startingSalePrice = 140000;
            var remainingMortgage = 136000;
            var rentDeposit = 1000;
            var njCommision = 0.06;
            var njCommisionValue = $"{(njCommision * 100.0):0.00} %";

            Logging.Log($"    Starting Sale Price: {startingSalePrice}");
            Logging.Log($"    Remaining Mortgage: {remainingMortgage}");
            Logging.Log($"    Rent Deposit:       {rentDeposit}");
            Logging.Log($"    NJ Commision:       {njCommisionValue}");

            var possibleSalesPrices = new List<double>();
            possibleSalesPrices.Add(startingSalePrice);

            for (int i = 0; i < 15; i++) {
                startingSalePrice += 5000;

                possibleSalesPrices.Add(startingSalePrice);
            }

            foreach (var salesPrice in possibleSalesPrices.OrderByDescending(x => x)) {
                var @return = salesPrice - remainingMortgage;
                var commision = @return * njCommision;
                @return -= commision;

                var salesPriceValue = $"{salesPrice:C}";
                var returnValue = $"{@return:C}";
                var commisionValue = $"{commision:C}";

                Logging.Log($"        Sales Price: {salesPriceValue,10} | Comission: {commisionValue,10} | Return: {returnValue,10}");
            }
        }

        public static void Problem() {

            var primeNumbers = new List<BigInteger>();

            var hits = 0;
            var misses = 0;
            var duplicates = 0;

            var gap = 0;
            var maxGap = 0;

            var n = 0.0m;
            while (true) {
                if (n % 2 == 0) {
                    var r = n / 2;

                    //Logging.Log($"    n / 2     = {r,9}");
                }
                else {
                    var r = (decimal)Math.E * n + 1;

                    var s = r.RemoveLeftSide().RemoveLeadingZeros().RemoveTrailingZeros().AsSpecial();
                    var numberOfDigits = s.NumberOfDigits();
                    var numValue = $"{numberOfDigits}";

                    var primeResult = s.IsProbablyPrime_Parallel(6, 1);

                    if (primeResult.IsPrime) {
                        if (!primeNumbers.Contains(s)) {
                            primeNumbers.Add(s);
                            hits++;
                            gap = 0;
                        }
                        else {
                            duplicates++;
                            misses++;
                            gap++;
                        }

                        Logging.Log($"    {(int)n,18}: {s.ToPrettyString(),32} d: {numValue,9} h: {hits,9} m: {misses,9} d: {duplicates,9} g: {gap,9} G: {maxGap,9}", Color.Yellow);
                    }
                    else {
                        misses++;
                        gap++;

                        //Logging.Log($"    {n,9}: {s.ToPrettyString(),32} d: {numValue,9} h: {hits,9} m: {misses,9} d: {duplicates,9} g: {gap,9} G: {maxGap,9}");
                    }
                }

                if (gap > maxGap) {
                    maxGap = gap;
                }

                n++;

                if (duplicates > 0) {
                    break;
                }
            }
        }

        public static void Thought1() {
            var hits = 0;
            var misses = 0;

            var s = 2;

            var i = new BigInteger(1);

            var z = new BigFraction(1.1);
            while (true) {
                var d = BigInteger.Pow(i, s);
                var zV = new BigFraction(1, d);

                z = zV + z;

                //var lastDigit = d.LastDigit();
                //if (lastDigit != 0) {
                //    i++;
                //    continue;
                //}

                var d1 = d + 1;
                var sumOfDigits = d1.SumOfDigits();
                var numberOfDigits = d1.NumberOfDigits();
                var uniqueCount = d1.UniqueCount();
                var uniqueDiff = numberOfDigits - uniqueCount;
                var oddEvenString = d1.ToOddEvenString();
                var oddEvenValue = oddEvenString.ParseBinary();

                var primeResult = d1.IsProbablyPrime_Parallel(6, 1);

                var zValya = z.ToDecimal();

                var loggingDetails = $"{i,-18} v: {zValya,-32} v2: {oddEvenValue.ToPrettyString(),-32} s: {sumOfDigits,-9} n: {numberOfDigits,-3} u: {uniqueCount,-3} d: {uniqueDiff,-3}";

                if (primeResult.IsPrime) {
                    hits++;

                    Logging.Log($"{loggingDetails}", Color.Yellow);
                }
                else {
                    misses++;

                    Logging.Log($"{loggingDetails}");

                    //var nP = d.NextPrime(6, 1);
                    //var diff = nP - d;
                    //hits++;

                    //Logging.Log($"    {i,-18} p: {nP.ToPrettyString(),-32} f: {diff,-32} h: {hits,-18}, m: {misses,-18}", Color.Yellow);
                }

                if (zValya.ToString() == "∞" || zValya.ToString() == "NaN") {
                    break;
                }

                i++;
            }
        }

        public static void Thought2() {

            var primeNumbers = new List<BigInteger>() {
                1, 3, 5, 7, 11
            };
            
            var hits = 0;
            var misses = 0;

            //var s = 2;

            //var i = new BigInteger(1);
            while (true) {
                var takenNumbers = primeNumbers.Take(2).ToList();

                foreach (var primeNumber in takenNumbers) {
                    primeNumbers.Remove(primeNumber);
                }

                var number = takenNumbers.First() + takenNumbers.Last();

                number -= 1;

                if (primeNumbers[0] == number) {

                }
                else {
                    primeNumbers.Insert(0, number);
                }

                var sumOfDigits = number.SumOfDigits();
                var numberOfDigits = number.NumberOfDigits();
                var uniqueCount = number.UniqueCount();
                var uniqueDiff = numberOfDigits - uniqueCount;
                var oddEvenString = number.ToOddEvenString();
                var oddEvenValue = oddEvenString.ParseBinary();

                var primeResult = number.IsProbablyPrime_Parallel(6, 1);

                var loggingDetails = $"{number.ToPrettyString(),-32} v2: {oddEvenValue.ToPrettyString(),-32} s: {sumOfDigits,-9} n: {numberOfDigits,-3} u: {uniqueCount,-3} d: {uniqueDiff,-3}";

                if (primeResult.IsPrime) {
                    hits++;

                    Logging.Log($"{loggingDetails}", Color.Yellow);
                }
                else {
                    misses++;

                    Logging.Log($"{loggingDetails}");
                }

                number++;
            }
        }

        public static void Thought6() {

            var maxItems = 64;

            //Table start. xmlns='http://www.w3.org/1999/xhtml'

            string html = "<html xmlns='http://www.w3.org/1999/xhtml'>" +
                "<head>" +
                "<style  type='text/css'>" +
                "body {background-color: black; color: white;}" +
                "</style>" +
                "</head>" +
                "<body>";

            html += "<table cellpadding='15' cellspacing='0' style='border: 1px solid #ccc;font-size: 9pt;font-family:arial'>";
            for (int i = 0; i <= maxItems; i++) {

                html += "<tr>";

                for (int v = 1; v <= maxItems; v++) {

                    if (i > 0) {
                        var itemResult = i * v;
                        var itemValue = new BigInteger(itemResult);

                        var primeResult = itemValue.IsProbablyPrime_Parallel(7, 2);

                        if (primeResult.IsPrime) {
                            html += $"<td style='font-weight:bold 800; color: yellow;'>{itemResult}</td>";
                        }
                        else if (i == v) {
                            html += $"<td style='color: green;'>{itemResult}</td>";
                        }
                        else if (itemValue.IsPowerOfTwo) {
                            html += $"<td style='color: blue;'>{itemResult}</td>";
                        }
                        else if (itemValue % 3 == 0) {
                            html += $"<td style='color: DarkGrey;'>{itemResult}</td>";
                        }
                        else {
                            html += $"<td style='color: DarkSlateGrey;'>{itemResult}</td>";
                        }

                    }
                    else {
                        //html += $"<td></td>"; // {v}
                    }
                }

                html += "</tr>";
            }

            html += "</table>" +
                "</body>" +
                "</html>";

            File.WriteAllText(@"E:\Console.htm", html);

        }

        private static Dictionary<char, string> GetMorseCodeSets() {
            var dot = '.';
            var dash = '−';

            return new Dictionary<char, string>() {
                {'a', string.Concat(dot, dash)},
                {'b', string.Concat(dash, dot, dot, dot)},
                {'c', string.Concat(dash, dot, dash, dot)},
                {'d', string.Concat(dash, dot, dot)},
                {'e', dot.ToString()},
                {'f', string.Concat(dot, dot, dash, dot)},
                {'g', string.Concat(dash, dash, dot)},
                {'h', string.Concat(dot, dot, dot, dot)},
                {'i', string.Concat(dot, dot)},
                {'j', string.Concat(dot, dash, dash, dash)},
                {'k', string.Concat(dash, dot, dash)},
                {'l', string.Concat(dot, dash, dot, dot)},
                {'m', string.Concat(dash, dash)},
                {'n', string.Concat(dash, dot)},
                {'o', string.Concat(dash, dash, dash)},
                {'p', string.Concat(dot, dash, dash, dot)},
                {'q', string.Concat(dash, dash, dot, dash)},
                {'r', string.Concat(dot, dash, dot)},
                {'s', string.Concat(dot, dot, dot)},
                {'t', string.Concat(dash)},
                {'u', string.Concat(dot, dot, dash)},
                {'v', string.Concat(dot, dot, dot, dash)},
                {'w', string.Concat(dot, dash, dash)},
                {'x', string.Concat(dash, dot, dot, dash)},
                {'y', string.Concat(dash, dot, dash, dash)},
                {'z', string.Concat(dash, dash, dot, dot)},
                {'0', string.Concat(dash, dash, dash, dash, dash)},
                {'1', string.Concat(dot, dash, dash, dash, dash)},
                {'2', string.Concat(dot, dot, dash, dash, dash)},
                {'3', string.Concat(dot, dot, dot, dash, dash)},
                {'4', string.Concat(dot, dot, dot, dot, dash)},
                {'5', string.Concat(dot, dot, dot, dot, dot)},
                {'6', string.Concat(dash, dot, dot, dot, dot)},
                {'7', string.Concat(dash, dash, dot, dot, dot)},
                {'8', string.Concat(dash, dash, dash, dot, dot)},
                {'9', string.Concat(dash, dash, dash, dash, dot)}
            };
        }

        public static void Thought3() {
            var morseCodeSets = GetMorseCodeSets();
            var morseString = new List<char>();
            var graphForm = new GraphForm();

            var scatterPlots = new List<cScatter>();

            var iterations = 1000;

            var i = new BigInteger(1);

            var startingZ = iterations / 2 * -1d;
            var normalize = eNormalize.Separate;

            var s = 2;
            //var hits = 0;
            //var misses = 0;
            while (true) {
                var alphaNumber = Math.Pow((double)i, s);
                var number = new BigInteger(alphaNumber);

                var location = CalcLocation(i, 0, false);
                normalize = location.E;

                var z = startingZ;

                var primeResult = number.IsProbablyPrime_Parallel(6);
                if (primeResult.IsPrime) {
                    _hits++;

                    if (_gap == 1) {
                        morseString.Add(' ');
                    }
                    else {
                        morseString.Add('-');
                    }

                    //morseString.Add('-');

                    _gap = 0;

                    Logging.Log($"{i,9} {number.ToPrettyString(),32}", Color.Yellow);

                    scatterPlots.Add(new cScatter(location.X, location.Y, z, Brushes.RoyalBlue));
                }
                else {
                    _misses++;
                    if (_gap++ > _largestGap) {
                        _largestGap = _gap;
                    }

                    morseString.Add('.');

                    Logging.Log($"{i,9} {number.ToPrettyString(),32}");

                    scatterPlots.Add(new cScatter(location.X, location.Y, z, Brushes.White));
                }

                startingZ += 0.1d;
                i++;
                if (i > iterations) {
                    break;
                }
            }

            graphForm.SetScatterPoints(scatterPlots, normalize);

            //morseCodeSets = morseCodeSets.OrderByDescending(x => x.Value.Length).ToDictionary(x => x.Key, x => x.Value);

            //var codeResult = string.Join("", morseString);
            //var valueResult = codeResult;

            //foreach (var codeSet in morseCodeSets) {
            //    while (valueResult.Contains(codeSet.Value)) {
            //        valueResult = valueResult.Replace(codeSet.Value, $"{codeSet.Key}");
            //    }
            //}

            //Logging.Log($"{valueResult}");
        }

        private static dynamic CalcLocation(BigInteger n, int method, bool ignoreZ) {
            var p = (double)n;
            
            switch (method) {
                case 0:
                    return new {
                        X = Math.Sin(p) * p,
                        Y = Math.Cos(p) * p,
                        Z = ignoreZ ? 0 : p,
                        E = eNormalize.MaintainXYZ
                    };
                case 1:
                    return new {
                        X = Math.Sin(p) * p,
                        Y = Math.Cos(p) * p,
                        Z = ignoreZ ? 0 : p % 2,
                        E = eNormalize.MaintainXYZ
                    };
                case 2:
                    return new {
                        X = Math.Sin((double)_gap) * p,
                        Y = Math.Cos((double)_gap) * p,
                        Z = ignoreZ ? 0 : p,
                        E = eNormalize.MaintainXYZ
                    };
                case 3: // Neat
                    return new {
                        X = Math.Sin(p) * (double)_gap,
                        Y = Math.Cos(p) * (double)_gap,
                        Z = 0d,
                        E = eNormalize.MaintainXYZ
                    };
                case 4: // Something
                    return new {
                        X = Math.Sin(p) * (double)_gap,
                        Y = Math.Cos(p) * (double)_gap,
                        Z = ignoreZ ? 0 : p,
                        E = eNormalize.MaintainXYZ
                    };
                case 5: // No?
                    return new {
                        X = Math.Sin(p) * (double)_hits,
                        Y = Math.Cos(p) * (double)_hits,
                        Z = ignoreZ ? 0 : p,
                        E = eNormalize.MaintainXYZ
                    };
                case 6: // No?
                    return new {
                        X = Math.Sin((double)_hits) * p,
                        Y = Math.Cos((double)_hits) * p,
                        Z = ignoreZ ? 0 : p,
                        E = eNormalize.MaintainXYZ
                    };
                case 7: // No?
                    return new {
                        X = Math.Sin(p) * (double)_misses,
                        Y = Math.Cos(p) * (double)_misses,
                        Z = ignoreZ ? 0 : p,
                        E = eNormalize.MaintainXYZ
                    };
                case 8: // No?
                    return new {
                        X = Math.Sin((double)_misses) * p,
                        Y = Math.Cos((double)_misses) * p,
                        Z = ignoreZ ? 0 : p,
                        E = eNormalize.MaintainXYZ
                    };
                default:
                    break;
            }

            return new {
                X = 0,
                Y = 0,
                Z = 0
            };
        }

        //public bool IsPerfectSquare(BigInteger num) {
        //    var root = num.Sqrt();

        //    return (int)Math.Pow(root, 2) == num;
        //}

        public static void Thought4() {

            var resolvedCount = 0;
            var maxStepCount = 0;
            while (true) {
                var initialNumber = new BigInteger().ToRandom(6) * -1;

                //Logging.Log($"{resolvedNumbers.Count,9} {initialNumber.ToPrettyString(),32} ", rewrite: true);


                var number = initialNumber;

                var stepCount = 0;
                while (number < -1) {
                    if (stepCount++ > maxStepCount) {
                        maxStepCount = stepCount;
                    }

                    if (number.IsEven) {
                        number = (number / -2) * -1;
                    }
                    else {
                        number = -3 * -number - 1;
                    }

                    Logging.Log($"{resolvedCount,9} {initialNumber.ToPrettyString(),32} {stepCount,9} {maxStepCount,9}", rewrite: true);
                }

                resolvedCount++;
            }


        }

        public static void Primalization(int primaryDegreeOfParallelism, int secondaryDegreeOfParallelism, int witnesses = 10) {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var parallelOptions = new ParallelOptions() {
                MaxDegreeOfParallelism = primaryDegreeOfParallelism
            };

            var moddedValues = new List<BigInteger>();
            var lastPrime = new BigInteger(0);

            var progress = _progress.Index;

            //var numberValue = new BigInteger(0);
            var powerOfTwo = BigInteger.Parse(_progress.Sequence);
            while (true) {
                var numberValue = (powerOfTwo += powerOfTwo) - 1;
                var sequence = numberValue + 1;

                //numberValue += 1;
                //var sequence = numberValue;

                var displayValue = numberValue.ToPrettyString();
                var numOfDigits = numberValue.NumberOfDigits();

                lock (InfiniteParallel.ParallelLockPrimary) {
                    Logging.Log($"{_progress.Index,18},     {displayValue,32}, D: {numOfDigits,9}, O: {_overallStopwatch.Elapsed,16}", rewrite: true);
                }

                if (!_verifiedPrimeNumbers.All(x => numOfDigits > x.NumberOfDigits) && !_verifiedPrimeNumbers.Any(x => x.NumberOfDigits == numOfDigits)) {
                    _progress.Index++;
                    _progress.Sequence = sequence.ToString();
                    continue;
                }

                var primeResult = numberValue.IsProbablyPrime_Parallel(witnesses: witnesses, maxDegreeOfParallelism: secondaryDegreeOfParallelism);
                var durationPerDigit = new TimeSpan(primeResult.Duration.Ticks / numOfDigits);

                var loggingDetail = $"{displayValue,32}, D: {numOfDigits,9}, T: {primeResult.Duration,16}, P: {durationPerDigit,16}, O: {_overallStopwatch.Elapsed,16}";

                lock (InfiniteParallel.ParallelLockPrimary) {
                    if (primeResult.IsPrime) {
                        var mersennePrime = new PrimeNumber() {
                            ID = _primeNumbers.Count + 1,
                            NumberOfDigits = numOfDigits,
                            Number = numberValue.ToString(),
                            Discovered = DateTime.UtcNow,
                            Duration = primeResult.Duration,
                            DurationPerDigit = durationPerDigit,
                            DurationOverall = _overallStopwatch.Elapsed,
                            Sequence = _progress.Index
                        };

                        if (!_primeNumbers.Any(x => x.Number == mersennePrime.Number)) {
                            _primeNumbers.Add(mersennePrime);
                        }

                        Logging.Log($"{_progress.Index,18}, {mersennePrime.ID,2}: {loggingDetail}", Color.Yellow, rewrite: true);
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                    }
                    else {
                        //Logging.Log($"{_progress.Index,18},     {loggingDetail}", rewrite: true);
                        //Console.SetCursorPosition(0, Console.CursorTop + 1);
                    }

                    _progress.Index++;
                    _progress.Sequence = sequence.ToString();
                }
            }
        }

        public static void AlternativePrimalization(int primaryDegreeOfParallelism, int secondaryDegreeOfParallelism, int witnesses = 10) {
            var parallelOptions = new ParallelOptions() {
                MaxDegreeOfParallelism = primaryDegreeOfParallelism
            };

            var wilsonPrimes = new List<int>() {
                5, 13, 563
            };

            var index = new BigInteger();
            var number = new BigInteger();

            var spaceCount = 0;

            while (true) {
                number++;

                var displayValue = number.ToPrettyString();
                var numOfDigits = number.NumberOfDigits();

                var v = "1".PadRight(numOfDigits, '0');
                var y = new BigInteger(Convert.ToInt32(v));

                // , d: {numOfDigits,9}
                var loggingDetail = $"{displayValue,32} {y,9}";

                var primeResult = number.IsProbablyPrime_Parallel(7, 2);

                if (primeResult.IsPrime) {
                    var mersennePrime = new PrimeNumber() {
                        ID = _primeNumbers.Count + 1,
                        NumberOfDigits = numOfDigits,
                        Number = number.ToString(),
                        Discovered = DateTime.UtcNow,
                        Duration = primeResult.Duration,
                        DurationOverall = _overallStopwatch.Elapsed,
                        Sequence = (int)index
                    };

                    Logging.Log($"{loggingDetail}", Color.Yellow);

                    spaceCount = 0;
                }
                else {
                    spaceCount++;

                    Logging.Log($"{loggingDetail}");
                }

                index++;
            }
        }

        public static void RandomPrimalization() {

            var showSuccess = false;
            var showFailure = false;

            var maxGenerations = 10000;
            var maxIterations = 10000;

            var bestSeed = 0;
            var bestChance = 0d;

            var seed = new BigInteger(999999);
            while (true) {
                seed = GenerateNumber(seed, _index, _gap, _largestGap, Sequence.S4);

                var rng = new Random(8030036);

                var hitCount = 0;
                var missCount = 0;

                for (int i = 0; i < maxGenerations; i++) {
                    var nextNumber = RandomBigInteger(rng, int.MaxValue).AsSpecial();

                    if (SinglePrimalization(nextNumber, 1, showSuccess, showFailure)) {
                        hitCount++;
                    }
                    else {
                        missCount++;
                    }
                }

                var checkCount = hitCount + missCount;
                var chance = (hitCount / (double)checkCount) * 100.0;

                var loggingDetail = $"s: {seed,-13} h: {hitCount,6} m: {missCount,6} c: {$"{chance:0.00}",6} %";

                if (chance > bestChance) {
                    bestChance = chance;
                    bestSeed = (int)seed;

                    Logging.Log($"    {loggingDetail} B: {bestSeed,-13} C: {$"{bestChance:0.00}",6} %", Color.MediumSlateBlue);
                }
                else {
                    Logging.Log($"    {loggingDetail} B: {bestSeed,-13} C: {$"{bestChance:0.00}",6} %");
                }

                seed += 1;
                _index++;

                if (_testedIndex > maxIterations) {
                    break;
                }
                else if (seed <= 0) {
                    break;
                }
            }
        }

        public static BigInteger RandomBigInteger(Random rng, BigInteger numOfBits) {
            //var rng = new Random(seed);

            var bytes = new Byte[numOfBits.ToByteArray().LongLength];
            rng.NextBytes(bytes);

            var result = new BigInteger(bytes);

            return result * result.Sign;
        }

        public static void BrutePrimalization() {
            var maxNumOfTests = 100000;

            var baseNumber = new BigInteger(1);

            while (true) {
                baseNumber = GenerateNumber(baseNumber, _index, _gap, _largestGap, Sequence.Plus);

                var primaryNumber = baseNumber;
                SinglePrimalization(primaryNumber, 0, true, true);

                //var d = BigInteger.Pow(primaryNumber, 2);

                //SinglePrimalization(d, 0, true, true);

                //var oddEvenString = d.ToOddEvenString();
                //var oddEvenValue = oddEvenString.ParseBinary();

                _index++;

                if (_testedIndex > maxNumOfTests) {
                    break;
                }
            }
        }

        public static bool SinglePrimalization(BigInteger number, int type, bool showSuccess = false, bool showFailure = false) {
            var duplicate = false;
            if (_primeNumbers.Any(x => x.Number == number.ToString())) {
                duplicate = true;
            }

            var displayValue = number.ToPrettyString();

            var primeResult = new PrimeResult() {
                IsPrime = true
            };

            if (!duplicate) {
                primeResult = number.IsProbablyPrime_Parallel(6, 1);
            }

            //var sumOfDigits = number.SumOfDigits();
            var numberOfDigits = number.NumberOfDigits();
            //var uniqueCount = number.UniqueCount();
            //var uniqueDiff = numberOfDigits - uniqueCount;
            //var oddEvenString = number.ToOddEvenString();
            //var oddEvenValue = oddEvenString.ParseBinary();

            var durationPerDigit = new TimeSpan(primeResult.Duration.Ticks / numberOfDigits);

            if (!_typeCounts.ContainsKey(type)) {
                _typeCounts.Add(type, 0);
            }

            var typeIndex = $"";
            if (type == 0) {
                typeIndex = $"{"",-9}{"",-3}";
            }
            else if (type > 0) {
                typeIndex = $"{_index,-9}+{type,-2}";
            }

            var loggingDetail = $"{typeIndex} {number.ToPrettyString(),-32} d: {numberOfDigits,9}";

            if (primeResult.IsPrime) {
                var mersennePrime = new PrimeNumber() {
                    ID = _primeNumbers.Count + 1,
                    NumberOfDigits = numberOfDigits,
                    Number = number.ToString(),
                    Discovered = DateTime.UtcNow,
                    Duration = primeResult.Duration,
                    DurationPerDigit = durationPerDigit,
                    DurationOverall = _overallStopwatch.Elapsed,
                    Sequence = (int)_index
                };

                if (!_primeNumbers.Any(x => x.Number == mersennePrime.Number)) {
                    _primeNumbers.Add(mersennePrime);

                    _typeCounts[type]++;

                    //var lastDigit = (int)number.LastDigit();
                    //if (lastDigit != 5) {
                    //    if (!_specialDigitCounts.ContainsKey(lastDigit)) {
                    //        _specialDigitCounts.Add(lastDigit, 1);
                    //    }
                    //    else {
                    //        _specialDigitCounts[lastDigit]++;
                    //    }
                    //}

                }
                else {
                    _duplicateCount++;
                }

                if (_gap > 0) {
                    var primeGap = _primeGaps.FirstOrDefault(x => x.Gap == _largestGap);
                    if (primeGap == null) {
                        primeGap = new PrimeGap() {
                            Gap = _largestGap,
                            GapNumbers = new List<GapNumber>()
                        };

                        _primeGaps.Add(primeGap);
                    }

                    primeGap.GapNumbers.Add(new GapNumber() {
                        Index = _index,
                        Number = number
                    });
                }

                _gap = 0;

                if (showSuccess) {
                    Logging.Log($"        {loggingDetail} {GetHitChanceAsString(),-4} %", Color.Yellow);
                }
            }
            else {
                _gap++;

                if (_gap > _largestGap) {
                    _largestGap = _gap;
                }

                if (showFailure) {
                    Logging.Log($"        {loggingDetail} {GetHitChanceAsString(),-4} %");
                }
            }

            _testedIndex++;

            return primeResult.IsPrime;
        }

        private static BigInteger _baseNumber = new BigInteger(0);

        public static BigInteger GenerateNumber(BigInteger number, BigInteger index, BigInteger gap, BigInteger largestGap, Sequence sequence) {

            switch (sequence) {
                case Sequence.Plus:
                    number += 1;
                    break;
                case Sequence.PowersOfTwo:
                    if (number == 0) {
                        number = 1;
                    }

                    number += number;
                    break;
                case Sequence.NextPowerOfTwo:
                    if (number < 2) {
                        number = 2;
                    }

                    number += number;

                    break;
                case Sequence.NextSpecial:
                    number += 1;

                    if (number >= 10) {
                        number = number.AsNextSpecial();
                    }

                    break;
                case Sequence.NextEven:
                    number += 1;

                    while (!number.IsEven) {
                        number += 1;
                    }

                    break;
                case Sequence.NextOdd:
                    number += 1;

                    while (number.IsEven) {
                        number += 1;
                    }

                    break;
                case Sequence.S4: // Interesting
                    number += 10000 * _index;

                    break;
                case Sequence.S5: // Interesting
                    number += 1;

                    while (true) {
                        if (_specialDigits.Contains(number % 10)) {
                            break;
                        }

                        number += 1;
                    }

                    break;
                case Sequence.S6:
                    number += 1;

                    while (number.IsEven) {
                        number += 1;
                    }

                    break;
                default:
                    break;
            }

            _baseNumber += 1;

            return number;
        }

        public static double GetHitChance() {
            var primeNumbers = (double)_primeNumbers.Count;
            var numbers = (double)_testedIndex + 1;

            var chance = (primeNumbers / numbers) * 100d;

            // {GetHitChance():0.00}%
 
            var valueString = $"{Math.Truncate(chance):0.00}";

            return chance.Truncate(2);
        }

        public static string GetHitChanceAsString() {
            var chance = GetHitChance();

            var valueString = $"{chance:0.00}";
            if (!valueString.Contains(".")) {
                valueString = $"%{valueString}.00";
            }

            return valueString.PadLeft(7);
        }

        public static double GetChance(int value) {
            var primeNumbers = (double)value;
            var numbers = (double)_testedIndex + 1;

            var chance = (primeNumbers / numbers) * 100d;

            var valueString = $"{Math.Truncate(chance):0.00}";

            return chance.Truncate(2);
        }

        public static string GetChanceAsString(int value) {
            var chance = GetChance(value);

            var valueString = $"{chance:0.00}";
            if (!valueString.Contains(".")) {
                valueString = $"%{valueString}.00";
            }

            return valueString.PadLeft(7);
        }

        private static void GetVariations() {
            var numberSeries = new List<int>() {
                //0, 2, 4, 6, 8
                1, 3, 7, 9
            };

            var variations = new List<List<int>>();

            var combos = numberSeries.Combinations().ToList();
            foreach (var combo in combos) {
                //variations.Add(combo.ToList());

                var permos = combo.Permutations().ToList();
                foreach (var permo in permos) {
                    variations.Add(permo.ToList());
                }
            }

            var varios = new List<List<int>>();
            foreach (var variation in variations) {
                if (!varios.Contains(variation)) {
                    varios.Add(variation);

                    Logging.Log($"{(varios.IndexOf(variation) - 1),6}: {string.Join(", ", variation)}");
                }
            }

            Logging.Log($"variations: {varios.Count}");
        }

        /// <summary>
        /// Convert *.txt file versions to .json
        /// </summary>
        public static void GetVerifiedMersennePrimes() {
            var files = Directory.GetFiles(_verifiedMersennePrimesPath, "*.txt").OrderBy(x => new FileInfo(x).Length).ToList();

            foreach (string file in files) {
                if (files.IndexOf(file) + 1 <= _verifiedPrimeNumbers.Count) {
                    Logging.Log($"    Skipped: {(files.IndexOf(file) + 1)}", true);
                    continue;
                }

                var fileIndex = files.IndexOf(file);

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                string contents = File.ReadAllText(file);
                contents = contents.Replace($"\n", "");
                contents = contents.Replace($" ", "");
                contents = contents.Trim();

                var numberValue = BigInteger.Parse(contents);
                var numOfDigits = numberValue.NumberOfDigits();

                var mersennePrime = new PrimeNumber() {
                    ID = _primeNumbers.Count + 1,
                    NumberOfDigits = numOfDigits,
                    Number = numberValue.ToString(),
                    Discovered = DateTime.UtcNow,
                    Duration = stopWatch.Elapsed,
                    DurationPerDigit = new TimeSpan(stopWatch.Elapsed.Ticks / numOfDigits),
                    DurationOverall = _overallStopwatch.Elapsed,
                    Sequence = -1
                };

                _verifiedPrimeNumbers.Add(mersennePrime);

                Logging.Log($"{_verifiedPrimeNumbers.Count,9}: {numberValue.ToPrettyString(), 32} D: {mersennePrime.NumberOfDigits, 12}, P: {mersennePrime.DurationPerDigit,16}, O: {mersennePrime.DurationOverall,16}");

                Logging.SaveJsonAsFile(_verifiedMersennePrimesFilePath, _verifiedPrimeNumbers);
            }
        }

        public static void CheckExistingPrimes() {

            var index = 0;
            foreach (var mersennePrime in _verifiedPrimeNumbers) {
                var numberValue = BigInteger.Parse(mersennePrime.Number);

                var baseString = numberValue.ToBaseString(36);
                var prettyBaseString = baseString.Midcate(20, 20, 44);

                Logging.Log($"{(index + 1),9}: {numberValue.ToPrettyString(),32} D: {mersennePrime.NumberOfDigits,12}, B: {prettyBaseString,44}, O: {_overallStopwatch.Elapsed,16}");

                index++;
            }

        }

        public static void AnyNumberSeries(BigInteger currentNumber) {
            _currentNumbers.Add(currentNumber);

            if (_seriesNumbers.Count > _currentNumbers.Count) {
                return;
            }

            var seriesCount = 0;

            var currentNumbers = string.Join("", _currentNumbers.Select(x => $"{x.ToString().PadLeft(5, '0')}"));
            var seriesNumbers = string.Join("", _primeSeriesStrings);

            while (currentNumbers.Contains(seriesNumbers)) {
                currentNumbers = currentNumbers.ReplaceFirst(seriesNumbers, "");
                seriesCount++;
            }


            //for (int i = 0; i < _currentNumbers.Count; i++) {
            //    var matches = new List<BigInteger>() {
            //        _seriesNumbers.FirstOrDefault()
            //    };

            //    foreach (var seriesNumber in _seriesNumbers.Skip(1)) {
            //        var nextIndex = i + _seriesNumbers.IndexOf(seriesNumber);
            //        if (nextIndex > (_currentNumbers.Count - 1)) {
            //            break;
            //        }

            //        var nextNumber = _currentNumbers[nextIndex];

            //        //Logging.Log($"{nextIndex}: {nextNumber}");

            //        if (nextNumber == seriesNumber) {
            //            matches.Add(seriesNumber);
            //        }
            //        else {
            //            break;
            //        }
            //    }

            //    if (matches.Count == _seriesNumbers.Count) {
            //        seriesCount++;
            //    }
            //}

            if (seriesCount > _seriesCount) {
                _seriesCount = seriesCount;
                _seriesGap = 0;
            }
            else {
                _seriesGap++;
            }

            if (_seriesGap > _largestSeriesGap) {
                _largestSeriesGap = _seriesGap;
            }
        }

        public static void IrrationalNumberGenerator() {

            var e = new BigDecimal();
            e += (Math.PI - 2) /1.3;

            var index = new BigInteger();

            var number = new BigDecimal();
            while (true) {
                index += 1;

                number = (int)index * e;
                number.Normalize();

                var numberParams = number.ToString().Split('E');

                var n = string.Join("", numberParams[0].Take((int)index));

                var value = BigInteger.Parse(n);

                var displayValue = value.ToPrettyString();
                var numOfDigits = value.NumberOfDigits();
                var sumOfDigits = value.SumOfDigits();
                var altSumOfDigits = value.AlternateSumOfDigits();

                var loggingDetail = $"{displayValue,32}, D: {numOfDigits,9}";

                lock (InfiniteParallel.ParallelLockPrimary) {
                    Logging.Log($"    {loggingDetail}", Color.LightSlateGray, rewrite: true);

                    var numberResult = value.IsProbablyPrime_Parallel(witnesses: 6, maxDegreeOfParallelism: 6);
                    if (numberResult.IsPrime) {
                        Logging.Log($"    {loggingDetail}", Color.SlateBlue, rewrite: true);
                        Console.SetCursorPosition(0, Console.CursorTop + 1);
                    }
                    else {
                        Logging.Log($"    {loggingDetail}", Color.LightSlateGray, rewrite: true);
                    }
                }
            }
        }
    }
}