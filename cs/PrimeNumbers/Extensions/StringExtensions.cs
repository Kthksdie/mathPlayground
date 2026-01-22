using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Extensions {
    public static class StringExtensions {
        public static string ReplaceFirst(this string text, string search, string replace) {
            int pos = text.IndexOf(search);
            if (pos < 0) {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string Midcate(this string value, int right = 14, int left = 14, int max = 32) {
            if (value.Length >= max) {
                value = $"{value.Left(right)}....{value.Right(left)}";
            }

            return value;
        }

        public static string Truncate(this string value, int maxLength, bool ellipses = false) {
            if (string.IsNullOrEmpty(value)) {
                return value;
            }

            if (ellipses) {
                return value.Length <= maxLength ? value : $"{value.Substring(0, maxLength)}...";
            }
            else {
                return value.Length <= maxLength ? value : value.Substring(0, maxLength);
            }
        }

        public static string Left(this string input, int count) {
            return input.Substring(0, Math.Min(input.Length, count));
        }

        public static string Right(this string input, int count) {
            return input.Substring(Math.Max(input.Length - count, 0), Math.Min(count, input.Length));
        }
    }
}
