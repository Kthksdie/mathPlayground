using Numberality.Con.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Components {
    public struct IsoscelesTriangle {
        public BigInteger N { get; set; }

        public BigInteger Side { get; set; }

        public BigInteger Base { get; set; }

        public BigInteger Height { get; set; }

        public BigInteger Area { get; set; }

        public BigInteger Perimeter { get; set; }

        public BigInteger Remainder { get; set; }

        public IsoscelesTriangle(BigInteger n) {
            N = n;
            Height = n.IntegerSqrt();
            Area = BigInteger.Pow(Height, 2);
            Base = (Height * 2);
            Side = (BigInteger.Pow(Base / 2, 2) + Area).IntegerSqrt();
            Perimeter = (2 * Height) + Base;
            Remainder = N - Area; // Remainder > 0 = Scalene Triangle
        }
    }
}
