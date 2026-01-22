using Numberality.Con.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Components {

    public class ReverseMultiply {
        public BigInteger N {
            get { return _n; }
        }

        public BigInteger Prospects {
            get { return _prospects; }
        }

        public BigInteger MaxProspects {
            get { return _maxProspects; }
        }

        public BigInteger Eliminations {
            get { return _eliminations; }
        }

        public Stopwatch Watch {
            get { return _stopWatch; }
        }

        private BigInteger _n { get; set; }

        private BigInteger _prospects { get; set; }

        private BigInteger _maxProspects { get; set; }

        private BigInteger _eliminations { get; set; }

        private Stopwatch _stopWatch { get; set; } = new Stopwatch();

        private List<Product> _initalProspects { get; set; }

        private Stack<Prospect> _currentProspects { get; set; }

        private int _maxTarget;
        private BigInteger[] _digitTargets;
        private BigInteger[] _lengthTargets;
        private BigInteger[] _stepTargets;

        public ReverseMultiply(BigInteger n) {
            _n = n;

            _initalProspects = InitializeProspects();
            _currentProspects = new Stack<Prospect>(_initalProspects.Select(x => new Prospect(2, x)));

            var x = _n.NumberOfDigits() - 3;
            if (x <= 0) {
                _maxProspects = 11 * _initalProspects.Count;
            }
            else {
                _maxProspects = OEIS.A105280(x) * _initalProspects.Count;
            }

            InitializeTargets();
        }

        private void InitializeTargets() {
            var t = 2;
            var lt = new BigInteger(100);
            var digitTargets = new List<BigInteger>();
            var lengthTargets = new List<BigInteger>();
            var stepTargets = new List<BigInteger>();

            while (true) {
                var digitTarget = _n % lt;
                var lengthTarget = lt;
                var stepTarget = lt / 10;

                digitTargets.Add(digitTarget);
                lengthTargets.Add(lengthTarget);
                stepTargets.Add(stepTarget);

                lt *= 10;
                t++;
                if (lt > _n) {
                    break;
                }
            }

            _digitTargets = digitTargets.ToArray();
            _lengthTargets = lengthTargets.ToArray();
            _stepTargets = stepTargets.ToArray();
            _maxTarget = t;
        }

        private List<Product> InitializeProspects() {
            var prospects = new List<Product>();

            var n_integerSqrt = _n.IntegerSqrt();
            var nt = BigInteger.Pow(10, n_integerSqrt.NumberOfDigits() - 1);
            //var st = (n_integerSqrt / 2 / nt) * nt;

            var ti = 1;
            var lt = BigInteger.Pow(10, ti);
            var dt = _n % lt;

            foreach (var product in Integers.Products(max: lt - 1)) {
                if (product.P % lt == dt) {
                    //prospects.Add(product);
                    prospects.Add(new Product(product.C + nt, product.E + nt));
                    //prospects.Add(new Product(product.C + nt, product.E + st));
                }
            }

            return prospects;
        }

        public Product TrySolve() {
            var solution = new Product(0, 0);

            _stopWatch.Restart();

            //var sqrt_length = _n.IntegerSqrt().NumberOfDigits();
            //var ones_max = (sqrt_length / 2) + 1;

            int nextTarget;
            Prospect current;

            var done = false;
            while (!done) {
                current = _currentProspects.Pop();
                nextTarget = current.TargetDigits + 1;
                if (nextTarget >= _maxTarget) {
                    continue;
                }

                _prospects++;

                foreach (var product in GetProspects_v2(current.Product, current.TargetDigits)) {
                    if (product.P > _n) {
                        continue;
                    }
                    else if (product.P == _n) {
                        _stopWatch.Stop();

                        solution = product;
                        done = true;
                        break;
                    }
                    //else if (product.C.Ones() >= ones_max || product.E.Ones() >= ones_max) {
                    //    _eliminations++;
                    //    continue;
                    //}
                    //else if (nextTarget > 4 && _n.IsQuadraticResidue(product.C) && _n.IsQuadraticResidue(product.E)) {
                    //    _eliminations++;
                    //    continue;
                    //}

                    _currentProspects.Push(new Prospect(nextTarget, product));
                }

                if (_currentProspects.Count == 0) {
                    break;
                }
            }

            return solution;
        }

        public async Task<Product> TrySolveAsync() {
            var solution = new Product(0, 0);

            await Task.Factory.StartNew(() => {
                solution = TrySolve();
            }, TaskCreationOptions.LongRunning);

            return solution;
        }

        public async Task<Product> TrySolve_MultithreadedAsync() {
            var solution = new Product(0, 0);

            _stopWatch.Restart();

            var options = new ParallelOptions() {
                MaxDegreeOfParallelism = Environment.ProcessorCount / 2
            };

            var initialProspects = new List<Prospect>(_initalProspects.Select(x => new Prospect(2, x)));
            var parallelProspects = new List<Prospect>();
            foreach (var prospect in initialProspects) {
                foreach (var product in GetProspects_v2(prospect.Product, prospect.TargetDigits)) {
                    parallelProspects.Add(new Prospect(prospect.TargetDigits + 1, product));
                }
            }

            var done = false;
            await Parallel.ForEachAsync(parallelProspects, options, async (initialProspect, ct) => {
                var stack = new Stack<Prospect>();
                stack.Push(initialProspect);

                int nextTarget;
                Prospect current;

                while (!done) {
                    current = stack.Pop();
                    nextTarget = current.TargetDigits + 1;
                    if (nextTarget >= _maxTarget) {
                        continue;
                    }

                    //_prospects++;

                    foreach (var product in GetProspects_v2(current.Product, current.TargetDigits)) {
                        if (product.P > _n) {
                            continue;
                        }
                        else if (product.P == _n) {
                            _stopWatch.Stop();

                            solution = product;
                            done = true;
                            break;
                        }

                        stack.Push(new Prospect(current.TargetDigits + 1, product));
                    }

                    if (stack.Count == 0) {
                        //Eliminations++;
                        break;
                    }
                }
            });

            return solution;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private List<Product> GetProspects(in Product currentProspect, int targetDigits) {
            var products = new List<Product>();

            var ti = targetDigits - 2;
            var lt = _lengthTargets[ti];
            var st = _stepTargets[ti];
            var dt = _digitTargets[ti];

            var cl_limit = lt + currentProspect.C;
            var el_limit = lt + currentProspect.E;

            BigInteger cl, el, p;
            for (cl = currentProspect.C; cl <= cl_limit; cl += st) {
                for (el = currentProspect.E; el <= el_limit; el += st) {
                    p = cl * el;
                    if (p % lt == dt) {
                        products.Add(new Product(cl, el, p));
                        break;
                    }
                }
            }

            return products;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<Product> GetProspects_v2(in Product currentProspect, int targetDigits) {
            var products = new List<Product>();

            var ti = targetDigits - 2;
            var lt = _lengthTargets[ti];
            var st = _stepTargets[ti];
            var dt = _digitTargets[ti];

            var cl_limit = lt + currentProspect.C;
            var el_limit = lt + currentProspect.E;

            var cl = currentProspect.C;
            var el = BigInteger.Zero;

            var p = BigInteger.Zero;
            while (cl <= cl_limit) {
                el = currentProspect.E;

                while (el < el_limit) {
                    p = cl * el;
                    if (p % lt == dt) {
                        products.Add(new Product(cl, el, p));
                        break;
                    }

                    el += st;
                }

                cl += st;
            }

            return products;
        }
    }
}
