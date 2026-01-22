using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Components {
    public static class BaseIntegerHelper {
        public static int SignificantDigitIndex(this byte[] bytes) {
            var i = bytes.Length - 1;
            while (i > 0 && bytes[i] == 0) {
                i--;
            }

            return i;
        }

        public static int SignificantDigitIndex(this Span<byte> bytes) {
            var i = bytes.Length - 1;
            while (i > 0 && bytes[i] == 0) {
                i--;
            }

            return i;
        }

        public static int SignificantDigitIndex(this ReadOnlySpan<byte> bytes) {
            var i = bytes.Length - 1;
            while (i > 0 && bytes[i] == 0) {
                i--;
            }

            return i;
        }

        public static void CopyTail(this ReadOnlySpan<byte> source, Span<byte> dest, int start) {
            source.Slice(start).CopyTo(dest.Slice(start));
        }
    }
}
