using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Extensions {
    public static class StringExtensions {
        public static int CountOf(this string s, char character) {
            var count = 0;

            foreach (var c in s.AsSpan()) {
                if (c == character) {
                    count++;
                }
            }

            return count;
        }

        public static string ToSomething(this string s) {
            var result = "";

            foreach (var c in s.AsSpan()) {
                if (c == '0') {
                    result += "0";

                    continue;
                }


            }

            return result;
        }
    }
}
