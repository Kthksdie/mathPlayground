using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Helpers
{
    public static class SpanHelper
    {
        public static void CopyTail(ReadOnlySpan<byte> source, Span<byte> dest, int start)
        {
            source.Slice(start).CopyTo(dest.Slice(start));
        }
    }
}
