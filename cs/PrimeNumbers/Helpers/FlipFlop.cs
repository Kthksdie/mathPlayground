using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Helpers {
    public class FlipFlop {
        private bool _value = false;
        private bool _variation = false;

        public bool Value() {
            return _value ^= true;
        }

        public bool IsTrue() {
            _variation ^= true;
            return _variation;
        }

        public bool IsFalse() {
            _variation ^= false;
            return !_variation;
        }
    }
}
