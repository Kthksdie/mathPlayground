using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Entities {
    public class PrimeResult {
        public PrimeResult() {
            IsPrime = false;
            FailedAt = "";
            Duration = new TimeSpan();
        }

        public bool IsPrime { get; set; }

        public TimeSpan Duration { get; set; }

        public string FailedAt { get; set; }
    }
}
