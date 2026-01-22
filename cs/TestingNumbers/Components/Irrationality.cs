using ExtendedNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Components {
    public class Irrationality {
        public BigInteger I = BigInteger.Zero;

        public BigDecimal Value = 0;

        public bool Sign = true;

        public Irrationality() {

        }

        public void Next(BigDecimal value) {
            if (Sign) {
                Value += value;
            }
            else {
                Value -= value;
            }

            Sign = !Sign;
            I++;
        }
    }
}
