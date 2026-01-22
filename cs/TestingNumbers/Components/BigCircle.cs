using ExtendedNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Components {
    public class BigCircle {
        public BigInteger N { get; set; } = BigInteger.Zero;

        public BigDecimal Radius { get; set; } = BigDecimal.Zero;

        public BigDecimal Diameter { get; set; } = BigDecimal.Zero;

        public BigDecimal Circumference { get; set; } = BigDecimal.Zero;

        public BigDecimal Area { get; set; } = BigDecimal.Zero;

        public BigCircle() {
            
        }

        public BigCircle(BigInteger n) {
            N = n;
            Circumference = 2 * BigDecimal.Pi * n;
            Diameter = Circumference / BigDecimal.Pi;
            Radius = Diameter / 2;
            Area = Circumference * Circumference;
        }


    }
}
