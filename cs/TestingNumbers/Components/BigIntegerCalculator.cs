using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Components {
    //https://github.com/dotnet/runtime/blob/c3e5ce97a29317d3dab98d09c310daf11a2260c7/src/libraries/System.Runtime.Numerics/src/System/Numerics/BigIntegerCalculator.AddSub.cs

    public static class BigIntegerCalculator {
        private const int CopyToThreshold = 8;
        private static readonly int StackAllocThreshold = 64;

        private static void CopyTail(ReadOnlySpan<uint> source, Span<uint> dest, int start) {
            source.Slice(start).CopyTo(dest.Slice(start));
        }

        public static void Add(ReadOnlySpan<uint> left, uint right, Span<uint> bits) {
            //Debug.Assert(left.Length >= 1);
            //Debug.Assert(bits.Length == left.Length + 1);

            Add(left, bits, ref MemoryMarshal.GetReference(bits), startIndex: 0, initialCarry: right);
        }

        public static void Add(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> bits) {
            //Debug.Assert(right.Length >= 1);
            //Debug.Assert(left.Length >= right.Length);
            //Debug.Assert(bits.Length == left.Length + 1);

            // Switching to managed references helps eliminating
            // index bounds check for all buffers.
            ref uint resultPtr = ref MemoryMarshal.GetReference(bits);
            ref uint rightPtr = ref MemoryMarshal.GetReference(right);
            ref uint leftPtr = ref MemoryMarshal.GetReference(left);

            int i = 0;
            long carry = 0;

            // Executes the "grammar-school" algorithm for computing z = a + b.
            // While calculating z_i = a_i + b_i we take care of overflow:
            // Since a_i + b_i + c <= 2(2^32 - 1) + 1 = 2^33 - 1, our carry c
            // has always the value 1 or 0; hence, we're safe here.

            do {
                carry += Unsafe.Add(ref leftPtr, i);
                carry += Unsafe.Add(ref rightPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                carry >>= 32;
                i++;
            } while (i < right.Length);

            Add(left, bits, ref resultPtr, startIndex: i, initialCarry: carry);
        }

        private static void AddSelf(Span<uint> left, ReadOnlySpan<uint> right) {
            //Debug.Assert(left.Length >= right.Length);

            int i = 0;
            long carry = 0L;

            // Switching to managed references helps eliminating
            // index bounds check...
            ref uint leftPtr = ref MemoryMarshal.GetReference(left);

            // Executes the "grammar-school" algorithm for computing z = a + b.
            // Same as above, but we're writing the result directly to a and
            // stop execution, if we're out of b and c is already 0.

            for (; i < right.Length; i++) {
                long digit = (Unsafe.Add(ref leftPtr, i) + carry) + right[i];
                Unsafe.Add(ref leftPtr, i) = unchecked((uint)digit);
                carry = digit >> 32;
            }
            for (; carry != 0 && i < left.Length; i++) {
                long digit = left[i] + carry;
                left[i] = (uint)digit;
                carry = digit >> 32;
            }

            //Debug.Assert(carry == 0);
        }

        public static void Subtract(ReadOnlySpan<uint> left, uint right, Span<uint> bits) {
            //Debug.Assert(left.Length >= 1);
            //Debug.Assert(left[0] >= right || left.Length >= 2);
            //Debug.Assert(bits.Length == left.Length);

            Subtract(left, bits, ref MemoryMarshal.GetReference(bits), startIndex: 0, initialCarry: -right);
        }

        public static void Subtract(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> bits) {
            //Debug.Assert(right.Length >= 1);
            //Debug.Assert(left.Length >= right.Length);
            //Debug.Assert(Compare(left, right) >= 0);
            //Debug.Assert(bits.Length == left.Length);

            // Switching to managed references helps eliminating
            // index bounds check for all buffers.
            ref uint resultPtr = ref MemoryMarshal.GetReference(bits);
            ref uint rightPtr = ref MemoryMarshal.GetReference(right);
            ref uint leftPtr = ref MemoryMarshal.GetReference(left);

            int i = 0;
            long carry = 0;

            // Executes the "grammar-school" algorithm for computing z = a + b.
            // While calculating z_i = a_i + b_i we take care of overflow:
            // Since a_i + b_i + c <= 2(2^32 - 1) + 1 = 2^33 - 1, our carry c
            // has always the value 1 or 0; hence, we're safe here.

            do {
                carry += Unsafe.Add(ref leftPtr, i);
                carry -= Unsafe.Add(ref rightPtr, i);
                Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                carry >>= 32;
                i++;
            } while (i < right.Length);

            Subtract(left, bits, ref resultPtr, startIndex: i, initialCarry: carry);
        }

        private static void SubtractSelf(Span<uint> left, ReadOnlySpan<uint> right) {
            //Debug.Assert(left.Length >= right.Length);
            // Assertion failing per https://github.com/dotnet/runtime/issues/97780
            // Debug.Assert(Compare(left, right) >= 0);

            int i = 0;
            long carry = 0L;

            // Switching to managed references helps eliminating
            // index bounds check...
            ref uint leftPtr = ref MemoryMarshal.GetReference(left);

            // Executes the "grammar-school" algorithm for computing z = a - b.
            // Same as above, but we're writing the result directly to a and
            // stop execution, if we're out of b and c is already 0.

            for (; i < right.Length; i++) {
                long digit = (Unsafe.Add(ref leftPtr, i) + carry) - right[i];
                Unsafe.Add(ref leftPtr, i) = unchecked((uint)digit);
                carry = digit >> 32;
            }
            for (; carry != 0 && i < left.Length; i++) {
                long digit = left[i] + carry;
                left[i] = (uint)digit;
                carry = digit >> 32;
            }

            // Assertion failing per https://github.com/dotnet/runtime/issues/97780
            //Debug.Assert(carry == 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Add(ReadOnlySpan<uint> left, Span<uint> bits, ref uint resultPtr, int startIndex, long initialCarry) {
            // Executes the addition for one big and one 32-bit integer.
            // Thus, we've similar code than below, but there is no loop for
            // processing the 32-bit integer, since it's a single element.

            int i = startIndex;
            long carry = initialCarry;

            if (left.Length <= CopyToThreshold) {
                for (; i < left.Length; i++) {
                    carry += left[i];
                    Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                    carry >>= 32;
                }

                Unsafe.Add(ref resultPtr, left.Length) = unchecked((uint)carry);
            }
            else {
                for (; i < left.Length;) {
                    carry += left[i];
                    Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                    i++;
                    carry >>= 32;

                    // Once carry is set to 0 it can not be 1 anymore.
                    // So the tail of the loop is just the movement of argument values to result span.
                    if (carry == 0) {
                        break;
                    }
                }

                Unsafe.Add(ref resultPtr, left.Length) = unchecked((uint)carry);

                if (i < left.Length) {
                    CopyTail(left, bits, i);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Subtract(ReadOnlySpan<uint> left, Span<uint> bits, ref uint resultPtr, int startIndex, long initialCarry) {
            // Executes the addition for one big and one 32-bit integer.
            // Thus, we've similar code than below, but there is no loop for
            // processing the 32-bit integer, since it's a single element.

            int i = startIndex;
            long carry = initialCarry;

            if (left.Length <= CopyToThreshold) {
                for (; i < left.Length; i++) {
                    carry += left[i];
                    Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                    carry >>= 32;
                }
            }
            else {
                for (; i < left.Length;) {
                    carry += left[i];
                    Unsafe.Add(ref resultPtr, i) = unchecked((uint)carry);
                    i++;
                    carry >>= 32;

                    // Once carry is set to 0 it can not be 1 anymore.
                    // So the tail of the loop is just the movement of argument values to result span.
                    if (carry == 0) {
                        break;
                    }
                }

                if (i < left.Length) {
                    CopyTail(left, bits, i);
                }
            }
        }

        public static void Divide(ReadOnlySpan<uint> left, uint right, Span<uint> quotient, out uint remainder) {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(quotient.Length == left.Length);

            // Executes the division for one big and one 32-bit integer.
            // Thus, we've similar code than below, but there is no loop for
            // processing the 32-bit integer, since it's a single element.

            ulong carry = 0UL;

            for (int i = left.Length - 1; i >= 0; i--) {
                ulong value = (carry << 32) | left[i];
                ulong digit = value / right;
                quotient[i] = (uint)digit;
                carry = value - digit * right;
            }
            remainder = (uint)carry;
        }

        public static void Divide(ReadOnlySpan<uint> left, uint right, Span<uint> quotient) {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(quotient.Length == left.Length);

            // Same as above, but only computing the quotient.

            ulong carry = 0UL;
            for (int i = left.Length - 1; i >= 0; i--) {
                ulong value = (carry << 32) | left[i];
                ulong digit = value / right;
                quotient[i] = (uint)digit;
                carry = value - digit * right;
            }
        }

        public static uint Remainder(ReadOnlySpan<uint> left, uint right) {
            Debug.Assert(left.Length >= 1);

            // Same as above, but only computing the remainder.
            ulong carry = 0UL;
            for (int i = left.Length - 1; i >= 0; i--) {
                ulong value = (carry << 32) | left[i];
                carry = value % right;
            }

            return (uint)carry;
        }

        public static void Divide(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> quotient, Span<uint> remainder) {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(quotient.Length == left.Length - right.Length + 1);
            Debug.Assert(remainder.Length == left.Length);

            left.CopyTo(remainder);
            Divide(remainder, right, quotient);
        }

        public static void Divide(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> quotient) {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(quotient.Length == left.Length - right.Length + 1);

            // Same as above, but only returning the quotient.

            uint[]? leftCopyFromPool = null;

            // NOTE: left will get overwritten, we need a local copy
            // However, mutated left is not used afterwards, so use array pooling or stack alloc
            Span<uint> leftCopy = (left.Length <= StackAllocThreshold ?
                                  stackalloc uint[StackAllocThreshold]
                                  : leftCopyFromPool = ArrayPool<uint>.Shared.Rent(left.Length)).Slice(0, left.Length);
            left.CopyTo(leftCopy);

            Divide(leftCopy, right, quotient);

            if (leftCopyFromPool != null)
                ArrayPool<uint>.Shared.Return(leftCopyFromPool);
        }

        public static void Remainder(ReadOnlySpan<uint> left, ReadOnlySpan<uint> right, Span<uint> remainder) {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(remainder.Length >= left.Length);

            // Same as above, but only returning the remainder.

            left.CopyTo(remainder);
            Divide(remainder, right, default);
        }

        private static void Divide(Span<uint> left, ReadOnlySpan<uint> right, Span<uint> bits) {
            Debug.Assert(left.Length >= 1);
            Debug.Assert(right.Length >= 1);
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(bits.Length == left.Length - right.Length + 1
                || bits.Length == 0);

            // Executes the "grammar-school" algorithm for computing q = a / b.
            // Before calculating q_i, we get more bits into the highest bit
            // block of the divisor. Thus, guessing digits of the quotient
            // will be more precise. Additionally we'll get r = a % b.

            uint divHi = right[right.Length - 1];
            uint divLo = right.Length > 1 ? right[right.Length - 2] : 0;

            // We measure the leading zeros of the divisor
            int shift = BitOperations.LeadingZeroCount(divHi);
            int backShift = 32 - shift;

            // And, we make sure the most significant bit is set
            if (shift > 0) {
                uint divNx = right.Length > 2 ? right[right.Length - 3] : 0;

                divHi = (divHi << shift) | (divLo >> backShift);
                divLo = (divLo << shift) | (divNx >> backShift);
            }

            // Then, we divide all of the bits as we would do it using
            // pen and paper: guessing the next digit, subtracting, ...
            for (int i = left.Length; i >= right.Length; i--) {
                int n = i - right.Length;
                uint t = (uint)i < (uint)left.Length ? left[i] : 0;

                ulong valHi = ((ulong)t << 32) | left[i - 1];
                uint valLo = i > 1 ? left[i - 2] : 0;

                // We shifted the divisor, we shift the dividend too
                if (shift > 0) {
                    uint valNx = i > 2 ? left[i - 3] : 0;

                    valHi = (valHi << shift) | (valLo >> backShift);
                    valLo = (valLo << shift) | (valNx >> backShift);
                }

                // First guess for the current digit of the quotient,
                // which naturally must have only 32 bits...
                ulong digit = valHi / divHi;
                if (digit > 0xFFFFFFFF)
                    digit = 0xFFFFFFFF;

                // Our first guess may be a little bit to big
                while (DivideGuessTooBig(digit, valHi, valLo, divHi, divLo))
                    --digit;

                if (digit > 0) {
                    // Now it's time to subtract our current quotient
                    uint carry = SubtractDivisor(left.Slice(n), right, digit);
                    if (carry != t) {
                        Debug.Assert(carry == t + 1);

                        // Our guess was still exactly one too high
                        carry = AddDivisor(left.Slice(n), right);
                        --digit;

                        Debug.Assert(carry == 1);
                    }
                }

                // We have the digit!
                if ((uint)n < (uint)bits.Length)
                    bits[n] = (uint)digit;

                if ((uint)i < (uint)left.Length)
                    left[i] = 0;
            }
        }

        private static uint AddDivisor(Span<uint> left, ReadOnlySpan<uint> right) {
            Debug.Assert(left.Length >= right.Length);

            // Repairs the dividend, if the last subtract was too much

            ulong carry = 0UL;

            for (int i = 0; i < right.Length; i++) {
                ref uint leftElement = ref left[i];
                ulong digit = (leftElement + carry) + right[i];
                leftElement = unchecked((uint)digit);
                carry = digit >> 32;
            }

            return (uint)carry;
        }

        private static uint SubtractDivisor(Span<uint> left, ReadOnlySpan<uint> right, ulong q) {
            Debug.Assert(left.Length >= right.Length);
            Debug.Assert(q <= 0xFFFFFFFF);

            // Combines a subtract and a multiply operation, which is naturally
            // more efficient than multiplying and then subtracting...

            ulong carry = 0UL;

            for (int i = 0; i < right.Length; i++) {
                carry += right[i] * q;
                uint digit = unchecked((uint)carry);
                carry >>= 32;
                ref uint leftElement = ref left[i];
                if (leftElement < digit)
                    ++carry;
                leftElement = unchecked(leftElement - digit);
            }

            return (uint)carry;
        }

        private static bool DivideGuessTooBig(ulong q, ulong valHi, uint valLo, uint divHi, uint divLo) {
            Debug.Assert(q <= 0xFFFFFFFF);

            // We multiply the two most significant limbs of the divisor
            // with the current guess for the quotient. If those are bigger
            // than the three most significant limbs of the current dividend
            // we return true, which means the current guess is still too big.

            ulong chkHi = divHi * q;
            ulong chkLo = divLo * q;

            chkHi += (chkLo >> 32);
            chkLo &= 0xFFFFFFFF;

            if (chkHi < valHi)
                return false;
            if (chkHi > valHi)
                return true;

            if (chkLo < valLo)
                return false;
            if (chkLo > valLo)
                return true;

            return false;
        }
    }
}
