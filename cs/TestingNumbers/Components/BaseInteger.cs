using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Net.Quic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestingNumbers.Components {
    public readonly struct BaseInteger {
        // https://github.com/dotnet/runtime/blob/main/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigInteger.cs

        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types
        // MaxLength: 536,870,897 uints
        // MaxLength: 2,147,483,591 bytes
        internal static int MaxLength => Array.MaxLength / sizeof(byte);

        internal readonly int _sign;
        internal readonly byte[] _digits;

        internal static BaseInteger _zero => new BaseInteger(new byte[] { 0 }, 1);
        internal static BaseInteger _one => new BaseInteger(new byte[] { 1 }, 1);
        internal static BaseInteger _two => new BaseInteger(new byte[] { 2 }, 1);
        internal static BaseInteger _ten => new BaseInteger(new byte[] { 0, 1 }, 1);

        internal static byte[] _singleZero => new byte[] { 0 };
        internal static byte[] _singleDigit = new byte[] { 0 };

        public int Sign {
            get {
                return _sign;
            }
        }

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

        public static BaseInteger Zero { get { return _zero; } }
        public static BaseInteger One { get { return _one; } }
        public static BaseInteger Two { get { return _two; } }
        public static BaseInteger Ten { get { return _ten; } }

        public BaseInteger(BigInteger n) {
            if (n == 0) {
                this = Zero;
                return;
            }

            _sign = n < 0 ? -1 : 1;

            var digits = new List<byte>();
            while (n != 0) {
                digits.Add((byte)(n % 10));

                n /= 10;
            }

            _digits = new byte[digits.Count];

            Array.Copy(digits.ToArray(), _digits, digits.Count);
        }

        public BaseInteger(int n) {
            if (n == 0) {
                this = Zero;
                return;
            }

            _sign = n < 0 ? -1 : 1;

            var digits = new List<byte>();
            while (n != 0) {
                digits.Add((byte)(n % 10));

                n /= 10;
            }

            _digits = new byte[digits.Count];

            Array.Copy(digits.ToArray(), _digits, digits.Count);
        }

        public BaseInteger(ReadOnlySpan<byte> digits, int sign) {
            _sign = sign;
            _digits = new byte[digits.Length];

            digits.CopyTo(_digits);
        }

        public static BaseInteger Parse(string value) {
            if (string.IsNullOrEmpty(value)) {
                return Zero;
            }

            var sign = !value.StartsWith('-') ? 1 : -1;
            var digits = new List<byte>();
            for (int i = value.Length - 1; i >= 0; i--) {
                var d = char.GetNumericValue(value[i]);
                if (d != -1) {
                    digits.Add((byte)d);
                }
            }

            return new BaseInteger(digits.ToArray(), sign); // little-endian 1234 = [4, 3, 2, 1]
        }

        public override string ToString() {
            var sb = new StringBuilder();
            for (int i = _digits.Length - 1; i >= 0; i--) {
                sb.Append(_digits[i]);
            }

            if (_sign == 1) {
                return sb.ToString();
            }
            else {
                return string.Format("-{0}", sb);
            }
        }

        private static (BaseInteger Min, BaseInteger Max, int Sign) Cross(BaseInteger left, BaseInteger right) {
            if (left._digits.Length == right._digits.Length) {
                for (int i = left._digits.Length - 1; i >= 0; i--) {
                    if (left._digits[i] > right._digits[i]) {
                        return (right, left, left._sign);
                    }
                    else if (left._digits[i] < right._digits[i]) {
                        return (left, right, right._sign);
                    }
                }

                return (left, right, right._sign);
            }
            else if (left._digits.Length < right._digits.Length) {
                return (left, right, right._sign);
            }
            else {
                return (right, left, left._sign);
            }
        }

        private static (byte[] left, byte[] right) Swap(byte[] left, byte[] right) {
            // left and right may be padded with zeros.

            var leftSdi = left.SignificantDigitIndex();
            var rightSdi = right.SignificantDigitIndex();

            if (leftSdi == rightSdi) {
                for (int i = leftSdi; i >= 0; i--) {
                    if (left[i] > right[i]) {
                        return (left, right);
                    }
                    else if (left[i] < right[i]) {
                        return (right, left);
                    }
                }

                return (left, right);
            }
            else if (leftSdi < rightSdi) {
                return (right, left);
            }

            return (left, right);
        }

        public static bool operator >(BaseInteger left, BaseInteger right) {
            if (left._sign != right._sign) {
                if (left._sign > right._sign) {
                    return true;
                }
                else {
                    return false;
                }
            }

            if (left._digits.Length == right._digits.Length) {
                for (int i = left._digits.Length - 1; i >= 0; i--) {
                    if (left._digits[i] > right._digits[i]) {
                        return true;
                    }
                    else if (left._digits[i] < right._digits[i]) {
                        return false;
                    }
                }
            }

            if (left._digits.Length > right._digits.Length) {
                return true;
            }
            else {
                return false;
            }
        }

        public static bool operator <(BaseInteger left, BaseInteger right) {
            if (left._sign != right._sign) {
                if (left._sign < right._sign) {
                    return true;
                }
                else {
                    return false;
                }
            }

            if (left._digits.Length == right._digits.Length) {
                for (int i = left._digits.Length - 1; i >= 0; i--) {
                    if (left._digits[i] < right._digits[i]) {
                        return true;
                    }
                    else if (left._digits[i] > right._digits[i]) {
                        return false;
                    }
                }
            }

            if (left._digits.Length < right._digits.Length) {
                return true;
            }
            else {
                return false;
            }
        }

        public static BaseInteger operator +(BaseInteger left, BaseInteger right) {
            if (left._sign == right._sign) {
                return Add(left, right);
            }
            else {
                return Subtract(left, right);
            }
        }

        public static BaseInteger operator ++(BaseInteger n) {
            return Add(n, One);
        }

        public static BaseInteger operator -(BaseInteger left, BaseInteger right) {
            return Subtract(left, right);
        }

        public static BaseInteger operator --(BaseInteger n) {
            return Subtract(n, One);
        }

        public static BaseInteger operator *(BaseInteger left, BaseInteger right) {
            return Multiply(left, right);
        }

        public static BaseInteger operator /(BaseInteger left, BaseInteger right) {
            return LongDivide3(left, right);
        }

        public static BaseInteger operator %(BaseInteger left, BaseInteger right) {
            return Modulus(left, right);
        }

        public static BaseInteger Add(BaseInteger left, BaseInteger right) {
            (left, right, _) = Cross(left, right);

            Span<byte> bytes = new byte[right._digits.Length + 1];

            int i = 0;
            int carry = 0;
            int result = 0;
            while (i < left._digits.Length) {
                result = (left._digits[i] + right._digits[i]) + carry;

                bytes[i] = (byte)(result % 10);
                carry = result / 10;

                //Program.Print($" {right._digits[i],3} + {left._digits[i],-3} = {result,3} | carry {carry,-3}");

                i++;
            }

            if (i < right._digits.Length) {
                SpanHelper.CopyTail(right._digits, bytes, i);
            }

            while (carry > 0) {
                result = bytes[i] + carry;

                bytes[i] = (byte)(result % 10);
                carry = result / 10;

                i++;
            }

            bytes = bytes.TrimEnd(_singleZero);

            if (bytes.Length == 0) {
                return Zero;
            }

            return new BaseInteger(bytes, sign: 1);
        }

        public static void Add(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right, Span<byte> product) {
            int i = 0;
            int carry = 0;
            int result = 0;
            while (i < left.Length) {
                result = (left[i] + right[i]) + carry;

                product[i] = (byte)(result % 10);
                carry = result / 10;

                //Program.Print($" {right[i],3} + {left[i],-3} = {result,3} | carry {carry,-3}");

                i++;
            }

            if (i < right.Length) {
                right.CopyTail(product, i);
            }

            while (carry > 0) {
                result = product[i] + carry;

                product[i] = (byte)(result % 10);
                carry = result / 10;

                i++;
            }
        }

        public static void Add(Span<byte> left, ReadOnlySpan<byte> right) {
            int i = 0;
            int carry = 0;
            int result = 0;
            while (i < left.Length) {
                result = (left[i] + right[i]) + carry;

                left[i] = (byte)(result % 10);
                carry = result / 10;

                //Program.Print($" {right[i],3} + {left[i],-3} = {result,3} | carry {carry,-3}");

                i++;
            }

            if (i < right.Length) {
                right.CopyTail(left, i);
            }

            while (carry > 0) {
                result = left[i] + carry;

                left[i] = (byte)(result % 10);
                carry = result / 10;

                i++;
            }
        }

        public static BaseInteger Subtract(BaseInteger left, BaseInteger right) {
            (Span<byte> leftBytes, Span<byte> rightBytes) = Swap(left._digits, right._digits);

            //Span<byte> bytes = new byte[left._digits.Length + 1];

            Subtract(leftBytes, rightBytes);

            leftBytes = leftBytes.TrimEnd(_singleZero);

            if (leftBytes.Length == 0) {
                return Zero;
            }

            return new BaseInteger(leftBytes, 1);
        }

        public static void Subtract(Span<byte> left, ReadOnlySpan<byte> right) {
            int i = 0;
            int borrow = 0;
            (int l, int r) = (0, 0);
            while (i < right.Length) {
                l = left[i];
                r = right[i] - borrow;

                if (r < l) {
                    r += 10;
                    borrow = 1;
                }
                else {
                    borrow = 0;
                }

                //Program.Print($" {r,3} - {l,-3} = {(r - l),3} | borrow {borrow,-3}");

                left[i] = (byte)(r - l);
                i++;
            }

            if (i < right.Length) {
                right.CopyTail(left, i);
            }

            while (borrow > 0) {
                l = borrow;
                r = left[i];

                if (r < l) {
                    r += 10;
                    borrow = 1;
                }
                else {
                    borrow = 0;
                }

                //Program.Print($" {r,3} - {l,-3} = {(r - l),3} | borrow {borrow,-3}");

                left[i] = (byte)(r - l);

                i++;
            }
        }

        public static BaseInteger Multiply(BaseInteger left, BaseInteger right) {
            (Span<byte> leftBytes, Span<byte> rightBytes) = Swap(left._digits, right._digits);
            Span<byte> product = new byte[(leftBytes.Length + rightBytes.Length) + 1];

            Multiply(leftBytes, rightBytes, product);

            product = product.TrimEnd(_singleZero);
            if (product.Length == 0) {
                return Zero;
            }

            var sign = left._sign != right._sign ? -1 : left._sign;

            return new BaseInteger(product, sign);
        }

        private static void Multiply(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right, Span<byte> product) {
            int carry = 0;
            int result = 0;

            (int l, int r) = (0, 0);
            for (int x = 0; x < left.Length; x++) {
                if (left[x] == 0) continue;

                l = left[x];
                for (int y = 0, z = x; y < right.Length; y++, z++) {
                    r = right[y];

                    result = (l * r) + (product[z] + carry);

                    //Program.Print($" ({left[x],3} * {right[y],-3}) + {(product[z] + carry),-3} = {result,3}");

                    product[z] = (byte)(result % 10);
                    carry = result / 10;
                }

                if (carry != 0) {
                    product[x + right.Length] = (byte)carry;
                    carry = 0;
                }
            }
        }

        private static void Multiply(ReadOnlySpan<byte> left, byte right, Span<byte> product) {
            int carry = 0;
            int result = 0;
            for (int x = 0; x < left.Length; x++) {
                if (left[x] == 0) continue;

                result = (left[x] * right) + (product[x] + carry);

                //Program.Print($" ({left[x],3} * {right,-3}) + {(product[x] + carry),-3} = {result,3}");

                product[x] = (byte)(result % 10);
                carry = result / 10;

                if (carry != 0) {
                    product[x + 1] = (byte)carry;
                    carry = 0;
                }
            }
        }

        private static void Multiply_v2(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right, Span<byte> product) {
            int carry = 0;
            int result = 0;

            (int l, int r) = (0, 0);
            for (int x = 0; x < left.Length; x++) {
                if (left[x] == 0) continue;

                l = left[x];
                for (int y = 0, z = x; y < right.Length; y++, z++) {
                    r = right[y];

                    result = (l * r) + (product[z] + carry);

                    //Program.Print($" ({left[x],3} * {right[y],-3}) + {(product[z] + carry),-3} = {result,3}");

                    product[z] = (byte)(result % 10);
                    carry = result / 10;
                }

                if (carry != 0) {
                    product[x + right.Length] = (byte)carry;
                    carry = 0;
                }
            }
        }

        public static BaseInteger LongDivide3(BaseInteger dividend, BaseInteger divisor) {
            var totalLength = (dividend._digits.Length + divisor._digits.Length) + 1;

            Span<byte> product = new byte[totalLength];
            Span<byte> quotient = new byte[totalLength];

            divisor._digits.CopyTo(product);
            Program.Print($" ! {string.Join("", product.ToArray().Reverse()),3}");

            var q = 1;

            #region Tens
            var e = dividend._digits.Length - divisor._digits.Length;
            var si = product.Length - 1;
            while (si >= 0 && product[si] == 0) {
                si--;
            }

            while (e > 1) {
                for (int i = si; i >= 0; i--) {
                    product[i + 1] = product[i];
                    product[i] = 0;
                }

                Program.Print($" ~ {string.Join("", product.ToArray().Reverse()),3}");

                q *= 10;
                si++;
                e--;
            }
            #endregion

            Program.Print($" e {e,3} | si {si,3}");

            var sie = si + e;

            var m = (dividend._digits[sie] * 100) + (dividend._digits[sie - 1] * 10) + dividend._digits[sie - 2];
            var j = (product[si] * 10) + product[si - 1];

            //(int gq, int gr) = Math.DivRem(m, j); // | gq {gq} gr {gr}
            var g = m / j;
            q *= g;

            Program.Print($" m {m,3} / j {j,-3} = {g,-3}");
            Program.Print($" q {q,3}");

            if (g > 1) {
                var k = new BaseInteger(g);
                Multiply_v1(product, k._digits).CopyTo(product);

                Program.Print($" * {string.Join("", product.ToArray().Reverse()),3}");
            }

            (int a, int b) = (0, 0);

            var result = 0;
            var carry = 0;
            var y = 0;
            while (true) {
                y = 0;
                carry = 0;
                for (int x = 0; x < divisor._digits.Length; x++) {
                    a = product[x];
                    b = divisor._digits[x];

                    result = (a + b) + carry;

                    //Program.Print($" ({b} * 2) + {carry} = {result,3} ");

                    product[x] = (byte)(result % 10);
                    carry = result / 10;

                    y++;
                }

                while (carry > 0) {
                    result = product[y] + carry;

                    product[y] = (byte)(result % 10);
                    carry = result / 10;
                    y++;
                }

                q++;

                if (y == dividend._digits.Length) {
                    y -= 1;
                    break;
                }
            }


            //Program.Print($" + {string.Join("", product.ToArray().Reverse()),3}");


            //while (!LessThanOrEqual(product, dividend._digits)) {
            //    Subtract(product, divisor._digits).CopyTo(product);
            //    q -= 1;

            //    Program.Print($" - {string.Join("", product.ToArray().Reverse()),3}");
            //}

            //Program.Print($" q {q,3}");

            //Program.Print();
            //Program.Print($" {xs,3} / {ys,-3} = {qs,3} r {rs,-3}");

            product = product.TrimEnd(_singleZero);
            if (product.Length == 0) {
                return Zero;
            }

            return new BaseInteger(q);

            static bool LessThanOrEqual(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) {
                int i = left.Length - 1;
                while (i > 0 && left[i] == 0) {
                    i--;
                }

                if (i + 1 > right.Length) {
                    return false;
                }

                for (; i >= 0; i--) {
                    if (left[i] < right[i]) {
                        return true;
                    }
                    else if (left[i] > right[i]) {
                        return false;
                    }
                }

                return true;
            }
        }

        private static Span<byte> Multiply_v1(ReadOnlySpan<byte> left, ReadOnlySpan<byte> right) {
            Span<byte> product = new byte[(left.Length + right.Length) + 1];

            int carry = 0;
            int result = 0;
            (int l, int r) = (0, 0);
            for (int x = 0; x < left.Length; x++) {
                if (left[x] == 0) continue;

                l = left[x];
                for (int y = 0, z = x; y < right.Length; y++, z++) {
                    r = right[y];

                    result = (r * l) + (product[z] + carry);

                    //Program.Print($" ({left[x],3} * {right[y],-3}) + {(bytes[z] + carry),-3} = {result,3}");

                    product[z] = (byte)(result % 10);
                    carry = result / 10;
                }

                if (carry != 0) {
                    product[x + right.Length] = (byte)carry;
                    carry = 0;
                }
            }

            //Program.Print($" k {string.Join("", bytes.ToArray().Reverse()),3}");
            return product.TrimEnd(_singleZero);
        }

        public static BaseInteger Modulus(BaseInteger left, BaseInteger right) {
            //(left, right, _) = Cross(left, right);

            Span<byte> bytes = new byte[(left._digits.Length + right._digits.Length) + 1];

            int i = 0;
            int carry = 0;
            int result = 0;
            for (int x = 0; x < left._digits.Length; x++) {
                if (left._digits[x] == 0) continue;

                for (int y = 0, z = x; y < right._digits.Length; y++, z++) {
                    result = left._digits[x] % right._digits[y];

                    Program.Print($" ({left._digits[x],3} % {right._digits[y],-3}) + {(bytes[z]),-3} = {result,3}");

                    bytes[z] = (byte)result;
                    //carry = result;

                    i++;
                }

                if (carry != 0) {
                    bytes[x + right._digits.Length] = (byte)carry;
                    carry = 0;
                }
            }

            while (carry > 0) {
                result = bytes[i] + carry;

                bytes[i] = (byte)(result % 10);
                carry = result / 10;

                i++;
            }

            bytes = bytes.TrimEnd(_singleZero);

            if (bytes.Length == 0) {
                return Zero;
            }

            var sign = left._sign != right._sign ? -1 : left._sign;

            return new BaseInteger(bytes, sign);
        }
    }
}
