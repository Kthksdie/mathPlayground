using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Components {
    public struct Zenteger {

        internal int[] _digits { get; set; } // Litte Endian

        internal static int[] _singleZero => new int[] { 0 };

        public List<int> Digits {
            get {
                return [.. _digits.Reverse()];
            }
        }

        public int NumberOfDigits {
            get {
                return _digits.Length;
            }
        }

        public Zenteger(int n) {
            var digits = new List<int>();

            while (n > 0) {
                digits.Add(n % 10);

                n /= 10;
            }

            _digits = digits.ToArray(); // Litte Endian
        }

        public Zenteger(BigInteger n) {
            var digits = new List<int>();

            while (n > 0) {
                digits.Add((int)(n % 10));

                n /= 10;
            }

            _digits = digits.ToArray(); // Litte Endian
        }

        public Zenteger(int[] digits) {
            _digits = digits;
        }

        public override string ToString() {
            var result = new StringBuilder();

            for (int i = _digits.Length - 1; i >= 0; i--) {
                result.Append(_digits[i]);
            }

            return result.ToString();
        }

        public static Zenteger Parse(string value) {
            if (string.IsNullOrEmpty(value)) {
                return new Zenteger(_singleZero);
            }

            var sign = !value.StartsWith('-') ? 1 : -1;
            var digits = new List<int>();
            for (int i = value.Length - 1; i >= 0; i--) {
                var d = (int)char.GetNumericValue(value[i]);
                if (d != -1) {
                    digits.Add(d);
                }
            }

            return new Zenteger(digits.ToArray());
        }
    }
}
