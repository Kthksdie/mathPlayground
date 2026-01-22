using ExtendedNumerics;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using TestingNumbers.Extensions;

namespace TestingNumbers.Components {
    public class Unmultiply {
        public BigInteger _n = BigInteger.Zero;
        public BigInteger _integerSqrt = BigInteger.Zero;
        public BigInteger _maxSqrt = BigInteger.Zero;
        public BigInteger _minSqrt = BigInteger.Zero;

        public BigInteger Iterations = BigInteger.Zero;

        public BigInteger Eliminations = BigInteger.Zero;

        private Stopwatch _globalWatch = new Stopwatch();

        private bool _stop = false;

        public Unmultiply(BigInteger n) {
            _n = n;
            _integerSqrt = _n.IntegerSqrt();

            BigDecimal.Precision = 16;

            var tSqrt = _integerSqrt + 1;

            _maxSqrt = (tSqrt * BigDecimal.Pi).WholeValue;
            _minSqrt = (tSqrt / BigDecimal.Pi).WholeValue;
        }

        public BigInteger BruteForce() {
            _globalWatch.Restart();

            var iterations = BigInteger.Zero;

            if (_n % _integerSqrt == 0) {
                _globalWatch.Stop();

                Program.Print($" k {_integerSqrt} | Sqrt | i 0 | {_globalWatch.Elapsed}");
                return _integerSqrt;
            }

            var cenK = _integerSqrt - _integerSqrt % 6;
            var cenLastDigit = (int)(cenK % 10);

            var minK = _minSqrt - _minSqrt % 6;
            var minLastDigit = (int)(minK % 10);

            var path = "";

            while (true) {
                if (cenLastDigit != 6) {
                    if (_n % (cenK + 1) == 0) {
                        cenK += 1; path = "CenK"; break;
                    }
                    else if (_n % (cenK - 1) == 0) {
                        cenK -= 1; path = "CenK"; break;
                    }
                }
                else if (_n % (cenK + 1) == 0) {
                    cenK += 1; path = "CenK"; break;
                }

                cenK -= 6;
                cenLastDigit = _digites[cenLastDigit];

                if (minLastDigit != 6) {
                    if (_n % (minK + 1) == 0) {
                        minK += 1; path = "MinK"; break;
                    }
                    else if (_n % (minK - 1) == 0) {
                        minK -= 1; path = "MinK"; break;
                    }
                }
                else if (_n % (minK + 1) == 0) {
                    minK += 1; path = "MinK"; break;
                }

                minK += 6;
                minLastDigit = _digitas[minLastDigit];

                if (minK >= cenK) {
                    path = "NpK"; break;
                }

                iterations++;
            }

            _globalWatch.Stop();

            var k = BigInteger.One;
            if (path == "CenK") {
                k = cenK;
            }
            else if (path == "MinK") {
                k = minK;
            }

            Program.Print($"  k {k} | {path} | i {iterations} | {_globalWatch.Elapsed}");

            return k;
        }

        private static readonly Dictionary<int, int> _digites = new Dictionary<int, int>() {
            { 6, 0 },
            { 0, 4 },
            { 4, 8 },
            { 8, 2 },
            { 2, 6 },
        };

        public BigInteger SqrtDescendCenter() {
            var k = _integerSqrt - _integerSqrt % 6;
            var lastDigit = (int)(k % 10);

            while (true) {
                if (lastDigit != 6) {
                    if (_n % (k + 1) == 0) {
                        k += 1;
                        break;
                    }
                    else if (_n % (k - 1) == 0) {
                        k -= 1;
                        break;
                    }
                }
                else if (_n % (k + 1) == 0) {
                    k += 1;
                    break;
                }

                k -= 6;
                lastDigit = _digites[lastDigit];
            }

            //Program.Print($" k {k} | i {iterations} | v2 {_globalWatch.Elapsed}");

            return k;
        }

        private static readonly Dictionary<int, int> _digitas = new Dictionary<int, int>() {
            { 6, 2 },
            { 2, 8 },
            { 8, 4 },
            { 4, 0 },
            { 0, 6 },
        };

        public BigInteger SqrtAscendMin() {
            var k = _minSqrt - _minSqrt % 6;
            var lastDigit = (int)(k % 10);

            while (true) {
                if (lastDigit != 6) {
                    if (_n % (k + 1) == 0) {
                        k += 1;
                        break;
                    }
                    else if (_n % (k - 1) == 0) {
                        k -= 1;
                        break;
                    }
                }
                else if (_n % (k + 1) == 0) {
                    k += 1;
                    break;
                }

                k += 6;
                lastDigit = _digitas[lastDigit];
            }

            //Program.Print($" k {k} | i {iterations} | v2 {_globalWatch.Elapsed}");

            return k;
        }

        public BigDecimal RateOfFailure() {
            if (Iterations > 0 && Eliminations > 0) {
                var tempPrecision = BigDecimal.Precision;
                BigDecimal.Precision = 10;

                var rate = new BigDecimal(Eliminations, Iterations);

                BigDecimal.Precision = tempPrecision;
                return rate * 100;
            }

            return BigDecimal.Zero;
        }

        public void Stop() {
            //_stop = true;
        }

        public IEnumerable<UnmultiplyResult> Results(BigInteger left, BigInteger right) {
            var leftDigits = left.NumberOfDigits();
            var rightDigits = right.NumberOfDigits();

            var digits = leftDigits > rightDigits ? rightDigits : leftDigits; // Current Number of Digits
            var minDigits = digits + 1;

            var dd = BigInteger.Pow(10, digits);    // Divisors Number of Digits
            var nd = BigInteger.Pow(10, minDigits); // Target Number of Digits
            var ld = _n % nd;                       // Last Digits of N

            //Program.Print($"dd {dd}");
            //Program.Print($"nd {nd}");

            // i 15363582 | 19801501 * 50150449 = 993054166023949

            var iterations = BigInteger.Zero;
            //var z = BigInteger.Zero;
            for (BigInteger x = dd + left; x <= nd + left; x += dd) {
                Iterations++;

                // y = left == 0 && right == 0 ? (x - left) + right : dd + right
                // y = dd + right
                for (BigInteger y = dd + right; y <= nd + right; y += dd) {
                    Iterations++;

                    var z = x * y;
                    if (z > _n) {
                        yield break;
                    }
                    else if (z % nd != ld) {
                        continue;
                    }

                    yield return new UnmultiplyResult() {
                        Left = x,
                        Right = y,
                        Value = z
                    };

                    if (z == _n) {
                        yield break;
                    }
                    else {
                        break;
                    }
                }
            }
        }

        public IEnumerable<UnmultiplyResult> Resultsv2(BigInteger left, BigInteger right) {
            var leftDigits = left.NumberOfDigits();
            var rightDigits = right.NumberOfDigits();

            var digits = leftDigits > rightDigits ? rightDigits : leftDigits; // Current Number of Digits
            var minDigits = digits + 1;

            var dd = BigInteger.Pow(10, digits);    // Divisors Number of Digits
            var nd = BigInteger.Pow(10, minDigits); // Target Number of Digits
            var ld = _n % nd;                       // Last Digits of N

            var results = new List<UnmultiplyResult>();

            //Program.Print($"dd {dd}");
            //Program.Print($"nd {nd}");

            var iterations = BigInteger.Zero;
            for (BigInteger x = dd + left; x <= nd + left; x += dd) {
                Iterations++;

                if (x > 1 && _n % x == 0) {
                    if (x * (_n / x) == _n) {
                        var ty = _n / x;

                        return new List<UnmultiplyResult>() {
                            new UnmultiplyResult() {
                                //Route = "n / x",
                                Left = x,
                                Right = ty,
                                Value = x * ty
                            }
                        };
                    }
                }

                for (BigInteger y = dd + right; y <= nd + right; y += dd) {
                    Iterations++;

                    if (y > 1 && _n % y == 0) {
                        if (y * (_n / y) == _n) {
                            var tx = _n / y;

                            return new List<UnmultiplyResult>() {
                                new UnmultiplyResult() {
                                    //Route = "n / y",
                                    Left = tx,
                                    Right = y,
                                    Value = tx * y
                                }
                            };
                        }
                    }

                    var z = x * y;
                    if (z > _n) {
                        Eliminations++;
                        return new List<UnmultiplyResult>();
                    }
                    else if (z % nd != ld) {
                        continue;
                    }

                    var result = new UnmultiplyResult() {
                        //Route = "default",
                        Left = x,
                        Right = y,
                        Value = z,
                        Carry = z / nd,
                    };

                    if (z == _n) {
                        return new List<UnmultiplyResult>() { result };
                    }

                    results.Add(result);
                    break;
                }
            }

            return results;
        }

        public int StackCount = 0;

        public UnmultiplyResult Process(bool reverse = true) {
            var s = new UnmultiplyResult() {
                Left = 0,
                Right = 0,
                Value = 0
            };

            var stack = new Stack<UnmultiplyResult>();
            stack.Push(s);

            StackCount = 1;

            while (true) {
                if (_stop) { return null; }

                var current = stack.Pop();
                StackCount--;

                var results = Results(current.Left, current.Right);
                if (reverse) {
                    results = results.Reverse();
                }

                //Program.Clear();
                //Program.Print($" n {_n}");
                //Program.Print($" r 50150449");
                //Program.Print($" l 19801501");

                //Program.Print($" P {current.Left} * {current.Right} = {current.Value} | c {current.Carry} | qr {_n.IsQuadraticResidue(current.Value)}");

                foreach (var result in results) {
                    if (_stop) { return null; }

                    if (result.Value == _n) {
                        return result;
                    }

                    stack.Push(result);
                    StackCount++;

                    //Program.Print($" C {result.Left} * {result.Right} = {result.Value} | c {result.Carry} | qr {_n.IsQuadraticResidue(result.Value)}");
                }

                //Program.Prompt();

                if (stack.Count == 0) {
                    break;
                }
            }

            return s;
        }

        public UnmultiplyResult ProcessParallel(bool reverse = true) {
            var s = new UnmultiplyResult() {
                Left = 0,
                Right = 0,
                Value = 0
            };

            var initialResults = Resultsv2(left: 0, right: 0);
            if (reverse) {
                initialResults = initialResults.Reverse();
            }

            var cancellationTokenSource = new CancellationTokenSource();

            var tasks = new List<Task>();

            foreach (var initialResult in initialResults) {
                var canTry = true;
                //if (initialResult.Left != initialResult.Right) {
                //    if (initialResults.Any(x => x.Left == initialResult.Right && x.Right == initialResult.Left)) {
                //        canTry = false;
                //    }
                //}

                if (canTry) {
                    var task = Task.Factory.StartNew(() => {
                        var stack = new Stack<UnmultiplyResult>();
                        stack.Push(initialResult);

                        while (true) {
                            if (s.Value == _n) {
                                return;
                            }

                            var current = stack.Pop();
                            var results = Resultsv2(current.Left, current.Right);
                            if (reverse) {
                                results = results.Reverse();
                            }

                            foreach (var result in results) {
                                if (result.Value == _n) {
                                    s = new UnmultiplyResult() {
                                        Left = result.Left,
                                        Right = result.Right,
                                        Value = result.Value
                                    };

                                    return;
                                }

                                stack.Push(result);
                            }

                            if (stack.Count == 0) {
                                StackCount++;
                                return;
                            }

                            if (cancellationTokenSource.IsCancellationRequested) {
                                return;
                            }
                        }
                    }, cancellationTokenSource.Token);

                    tasks.Add(task);
                }
            }

            while (true) {
                if (s.Value == _n) {
                    cancellationTokenSource.Cancel();
                    break;
                }

                Task.Delay(1000 / 30).Wait();
            }

            return s;
        }

        private List<(int X, int Y, int Z)> _timesTable = TimesTable(from: 1, to: 9);

        public List<UnmultiplyResult> GetResults(UnmultiplyResult currentResult) {
            var results = new List<UnmultiplyResult>();



            return results;
        }

        public UnmultiplyResult Try() {
            var s = new UnmultiplyResult() {
                Left = 0,
                Right = 0,
                Value = 0
            };

            var stack = new Stack<UnmultiplyResult>();
            stack.Push(s);
            StackCount++;

            while (true) {
                var current = stack.Pop();
                StackCount--;

                var results = GetResults(current);
                foreach (var result in results) {
                    if (result.Value == _n) {
                        return result;
                    }

                    stack.Push(result);
                    StackCount++;

                    //Program.Print($" C {result.Left} * {result.Right} = {result.Value} | c {result.Carry} | qr {_n.IsQuadraticResidue(result.Value)}");
                }

                //Program.Prompt();

                if (stack.Count == 0) {
                    break;
                }
            }

            return s;
        }

        private static List<(int X, int Y, int Z)> TimesTable(int from, int to) {
            var results = new List<(int X, int Y, int Z)>();

            for (int x = from; x <= to; x++) {
                for (int y = x; y <= to; y++) {
                    results.Add((x, y, x * y));
                }
            }

            results = results.OrderBy(result => result.Z).ToList();
            return results;
        }
    }

    public class UnmultiplyResult {
        public int Idx { get; set; }

        public BigInteger Left = BigInteger.Zero;
        public BigInteger Right = BigInteger.Zero;
        public BigInteger Value = BigInteger.Zero;

        public BigInteger Carry = BigInteger.Zero;

        public UnmultiplyResult() {
            //Id = Guid.NewGuid();
            //ParentId = Guid.Empty;
        }
    }

    public class CustomStack<T> {
        public List<T> Items = new List<T>();

        public void Push(T item) {
            Items.Add(item);
        }

        public T Pop() {
            if (Items.Count > 0) {
                T temp = Items[Items.Count - 1];
                Items.RemoveAt(Items.Count - 1);
                return temp;
            }
            else
                return default;
        }

        public void Remove(int itemAtPosition) {
            Items.RemoveAt(itemAtPosition);
        }
    }
}
