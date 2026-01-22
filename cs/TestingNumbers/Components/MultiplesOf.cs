using ExtendedNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Components {
    public class MultiplesOf {
        public BigInteger Base { get; set; }

        public BigInteger Exponent { get; set; }

        public BigInteger Value { get; set; }

        public MultiplesOf(BigInteger baseOf) {
            Base = baseOf;
            Exponent = 1;
            Value = baseOf;
        }

        public override string ToString() {
            return $"{Base}*{Exponent} | {Value}";
        }

        public string ToShortString() {
            return $"{Base}*{Exponent}";
        }

        public BigInteger Next() {
            Value += Base;
            Exponent++;

            return Value;
        }
    }

    public class MultiplesOfDecimals {
        public BigDecimal Base { get; set; }

        public BigInteger Exponent { get; set; }

        public BigDecimal Value { get; set; }

        public MultiplesOfDecimals(BigDecimal baseOf) {
            Base = baseOf;
            Exponent = 1;
            Value = baseOf;
        }

        public override string ToString() {
            return $"{Base}*{Exponent} | {Value}";
        }

        public string ToShortString() {
            return $"{Base}*{Exponent}";
        }

        public BigDecimal Next() {
            Value += Base;
            Exponent++;

            return Value;
        }
    }
}