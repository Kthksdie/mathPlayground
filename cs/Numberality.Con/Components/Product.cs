using Numberality.Con.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Components {
    public struct Product {
        public BigInteger C { get; }

        public BigInteger E { get; }

        public BigInteger P { get; }

        private string _stringValue = string.Empty;

        public Product(BigInteger multiplicand, BigInteger multiplier) {
            C = multiplicand;
            E = multiplier;
            P = multiplicand * multiplier;
        }

        public Product(BigInteger multiplicand, BigInteger multiplier, BigInteger product) {
            C = multiplicand;
            E = multiplier;
            P = product;
        }

        public override string ToString() {
            if (_stringValue == string.Empty) {
                _stringValue = $"{C} * {E} = {P}";
            }

            return _stringValue;
        }
    }
}
