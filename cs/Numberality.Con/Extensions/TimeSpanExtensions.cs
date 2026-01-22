using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Extensions {
    public static class TimeSpanExtensions {

        public static TimeSpan Avg(this List<TimeSpan> timeSpans) {
            if (timeSpans.Count == 0) {
                return TimeSpan.Zero;
            }

            return new TimeSpan((long)timeSpans.Select(ts => ts.Ticks).Average());
        }
    }
}
