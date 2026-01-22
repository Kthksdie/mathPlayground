using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Components {
    public static class Gemini {
        public static Tuple<byte[], byte[]> Divide(Span<byte> dividend, Span<byte> divisor) {
            // Handle empty arrays or divisor of zero
            if (dividend.Length == 0 || divisor.Length == 0 || divisor[divisor.Length - 1] == 0) {
                throw new ArgumentException("Invalid dividend or divisor");
            }

            var d = divisor.ToArray();
            Array.Resize<byte>(ref d, dividend.Length);

            divisor = d;

            // Handle dividend smaller than divisor
            if (CompareMagnitude(dividend, divisor) < 0) {
                return new Tuple<byte[], byte[]>(new byte[] { 0 }, dividend.ToArray());
            }

            // Initialize variables
            var quotient = new List<byte>();
            var remainder = new byte[dividend.Length];
            
            Array.Copy(dividend.ToArray(), 0, remainder, 0, dividend.Length);

            // Long division loop
            for (int i = dividend.Length - 1; i >= 0; i--) {
                // Bring the next digit into the remainder
                ShiftLeft(remainder, 1);
                remainder[0] = dividend[i];

                

                // Repeatedly subtract the divisor until the remainder is less
                while (CompareMagnitude(remainder, divisor) >= 0) {
                    Program.Print($" d {string.Join(" ", divisor.ToArray())} | r {string.Join(" ", remainder)}");

                    Subtract(remainder, divisor);
                    quotient.Insert(0, 1); // Insert digit 1 in quotient for each subtraction
                }

                // Remove the leading 1 if the remainder becomes zero
                if (remainder[0] == 0) {
                    if (quotient.Count > 0) {
                        quotient.RemoveAt(0);
                    }
                }
            }

            return new Tuple<byte[], byte[]>(quotient.ToArray(), remainder);
        }

        // Helper methods for comparison and arithmetic on byte arrays
        private static int CompareMagnitude(Span<byte> a, Span<byte> b) {
            if (a.Length != b.Length) {
                return a.Length.CompareTo(b.Length);
            }

            for (int i = 0; i < a.Length; i++) {
                if (a[i] != b[i]) {
                    return a[i].CompareTo(b[i]);
                }
            }

            return 0;
        }

        private static void ShiftLeft(byte[] arr, int positions) {
            Array.Copy(arr, 0, arr, positions, arr.Length - positions);
            for (int i = 0; i < positions; i++) {
                arr[i] = 0;
            }
        }

        private static void Subtract(Span<byte> arr, Span<byte> subtrahend) {
            for (int i = arr.Length - 1; i >= 0; i--) {
                int borrow = 0;
                if (arr[i] < subtrahend[i] + borrow) {
                    borrow = 1;
                    arr[i] = (byte)(10 + arr[i] - subtrahend[i] - borrow);
                }
                else {
                    arr[i] = (byte)(arr[i] - subtrahend[i] - borrow);
                }
            }
        }

    }
}
