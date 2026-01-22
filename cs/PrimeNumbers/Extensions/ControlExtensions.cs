using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrimeNumbers.Extensions {
    public static class ControlExtensions {
        public static void PerformInvoke(this Control control, MethodInvoker action) {
            control.Invoke(action);
        }
    }
}
