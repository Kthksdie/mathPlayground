using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Components {
    public static class OEIS {

        public static BigInteger A105280(int n) {
            if (n <= 0) {
                return BigInteger.Zero;
            }

            var result = new BigInteger(11);
            for (int i = 0; i < n - 1; i++) {
                result = 11 * result + 11;
            }

            return result;
        }
    }
}
