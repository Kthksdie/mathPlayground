using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Entities {
    public enum Sequence {
        Plus,
        PowersOfTwo,
        NextPowerOfTwo,
        NextSpecial,
        NextEven,
        NextOdd,
        S4,
        S5,
        S6,
        S7,
        S8,
        S9,
        S10,
        S11,
        S12,
        S13,
        S14
    }

    public class PrimeNumber {

        public PrimeNumber() {
            // 
        }

        public int ID { get; set; }

        public int NumberOfDigits { get; set; }

        public string Number { get; set; }

        public DateTime Discovered { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan DurationPerDigit { get; set; }

        public TimeSpan DurationOverall { get; set; }

        public int Sequence { get; set; }
    }
}
