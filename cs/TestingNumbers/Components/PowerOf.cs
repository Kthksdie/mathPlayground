using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Components {
    public struct PowerOf {
        public BigInteger Base { get; set; }

        public BigInteger Exponent { get; set; }

        public BigInteger Value { get; set; }

        public PowerOf(BigInteger baseOf, BigInteger exponent) {
            Base = baseOf;
            Exponent = exponent;
            Value = 1;
        }

        public override string ToString() {
            return $"{Base}^{Exponent}";
        }

        public BigInteger Calculate() {
            Value = BigInteger.Pow(Base, (int)Exponent);

            return Value;
        }

        public void Next() {
            Value *= Base;
            Exponent++;
        }
    }
}
