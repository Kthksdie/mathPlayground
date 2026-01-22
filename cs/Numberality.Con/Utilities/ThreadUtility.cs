using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Utilities {
    public static class ThreadUtility {
        public static ThreadLocal<Random> Random = new ThreadLocal<Random>(() => {
            return new Random();
        });


    }
}
