using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Entities {
    public class PrimeGap {
        public BigInteger Gap { get; set; }

        public List<GapNumber> GapNumbers { get; set; }
    }

    public class GapNumber {
        public BigInteger Index { get; set; }

        public BigInteger Number { get; set; }
    }
}
