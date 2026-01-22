using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Entities {
    public class Progress {

        public Progress() {
            Index = 0;
            Sequence = "1";
            Duration = new TimeSpan();
        }

        public int Index { get; set; }

        public string Sequence { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
