using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Interfaces;

namespace TestingNumbers.Sequences {
    public class FibonacciNumbers : ISequence {
        public BigInteger I { get; set; }

        public BigInteger High { get; set; }

        public BigInteger Low { get; set; }

        public BigInteger Current { get; set; }

        public FibonacciNumbers() {
            Reset();
        }

        public void Reset() {
            I = 1;
            High = 1;
            Low = 0;
            Current = 0;
        }

        public BigInteger Next() {
            Current = Low + High;

            Low = High;
            High = Current;

            I++;

            return Current;
        }

        public IEnumerable<BigInteger> All() {
            while (true) {
                yield return Next();
            }
        }
    }
}
