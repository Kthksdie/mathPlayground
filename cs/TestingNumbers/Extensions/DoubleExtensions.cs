using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Extensions {
    public static class DoubleExtensions {
        public static double WholeValue(this double value) {
            return double.Parse(value.ToString().Replace(".", string.Empty));
        }
    }
}
