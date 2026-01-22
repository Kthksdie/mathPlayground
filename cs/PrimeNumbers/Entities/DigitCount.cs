using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Entities {
    public class DigitCount {

        public DigitCount() {
            Digit = 0;
            Count = 0;
            Percent = 0;
        }

        public int Digit { get; set; }

        public int Count { get; set; }

        public double Percent { get; set; }
    }
}
