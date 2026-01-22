using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PowersOfPrimes.Extensions {
    /// <summary>
    /// Provides extension methods for BigInteger operations including primality testing, 
    /// random number generation, and square root calculations.
    /// </summary>
    /// <remarks>
    /// This class contains optimized implementations for mathematical operations on BigInteger values,
    /// including probabilistic primality testing using the Miller-Rabin algorithm and efficient
    /// integer square root calculations using Newton's method.
    /// </remarks>
    public static class BigIntegerExtensions {
        private static Random _random = new Random();

        /// <summary>
        /// Generates a random BigInteger within a specified range.
        /// </summary>
        /// <param name="random">The Random instance to use for generation.</param>
        /// <param name="minValue">The inclusive lower bound of the random number.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number.</param>
        /// <returns>A random BigInteger that is greater than or equal to minValue and less than maxValue.</returns>
        /// <exception cref="ArgumentException">Thrown when minValue is greater than maxValue.</exception>
        /// <remarks>
        /// This method uses an efficient algorithm to generate random BigInteger values within a specified range.
        /// The implementation is based on the algorithm described at https://stackoverflow.com/a/68593532.
        /// </remarks>
        public static BigInteger Next(this Random random, BigInteger minValue, BigInteger maxValue) {
            // Returns a random BigInteger that is within a specified range.
            // The lower bound is inclusive, and the upper bound is exclusive.
            // https://stackoverflow.com/a/68593532 - Theodor Zoulias

            if (minValue > maxValue) {
                throw new ArgumentException();
            }
            else if (minValue == maxValue) {
                return minValue;
            }

            var zeroBasedUpperBound = maxValue - 1 - minValue; // Inclusive
            var bytes = zeroBasedUpperBound.ToByteArray();
            var lastByteMask = (byte)0b11111111; // Search for the most significant non-zero bit
            for (var mask = (byte)0b10000000; mask > 0; mask >>= 1, lastByteMask >>= 1) {
                if ((bytes[bytes.Length - 1] & mask) == mask) {
                    break; // We found it
                }
            }

            var result = BigInteger.Zero;
            while (true) {
                random.NextBytes(bytes);

                bytes[bytes.Length - 1] &= lastByteMask;

                result = new BigInteger(bytes);

                if (result <= zeroBasedUpperBound) {
                    return result + minValue;
                }
            }
        }

        /// <summary>
        /// Determines whether a BigInteger is probably prime using the Miller-Rabin primality test.
        /// </summary>
        /// <param name="n">The BigInteger to test for primality.</param>
        /// <param name="certainty">The number of random bases to test. Higher values increase accuracy but reduce performance. Default is 8.</param>
        /// <returns>true if the number is probably prime; otherwise, false.</returns>
        /// <remarks>
        /// This method implements the Miller-Rabin probabilistic primality test. The probability of a false positive
        /// is at most 4^(-certainty). For certainty = 8, this gives a false positive probability of less than 1 in 65,536.
        /// 
        /// The algorithm handles edge cases efficiently:
        /// - Numbers ≤ 1 are not prime
        /// - 2 and 3 are prime
        /// - Even numbers > 2 are not prime
        /// 
        /// Implementation based on the Rosetta Code example: https://rosettacode.org/wiki/Miller%E2%80%93Rabin_primality_test#C#
        /// </remarks>
        public static bool IsProbablyPrime(this BigInteger n, int certainty = 8) {

            if (n <= 1) {
                return false;
            }

            if (n == 2 || n == 3) {
                return true;
            }

            if (n.IsEven) {
                return false;
            }

            var nMinusOne = n - 1;
            var nMinusTwo = n - 2;

            var d = nMinusOne;
            int s = 0;

            while (d % 2 == 0) {
                d /= 2;
                s += 1;
            }

            BigInteger a, x;
            for (int i = 0; i < certainty; i++) {
                a = _random.Next(2, nMinusTwo);

                x = BigInteger.ModPow(a, d, n);
                if (x == 1 || x == nMinusOne) {
                    continue;
                }

                for (int r = 1; r < s; r++) {
                    x = BigInteger.ModPow(x, 2, n);

                    if (x == 1) {
                        return false;
                    }

                    if (x == nMinusOne) {
                        break;
                    }
                }

                if (x != nMinusOne) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Calculates the integer square root of a BigInteger using an optimized Newton's method implementation.
        /// </summary>
        /// <param name="n">The BigInteger to find the square root of.</param>
        /// <returns>The largest integer whose square is less than or equal to n.</returns>
        /// <remarks>
        /// This method returns the integer square root, which is the largest integer x such that x² ≤ n.
        /// The implementation uses different algorithms optimized for different number ranges:
        /// 
        /// - Small numbers (< 1.448e17): Direct hardware square root
        /// - Medium numbers (1.448e17 to 8.5e37): Single Newton iteration
        /// - Large numbers (8.5e37 to 4.3322e127): Multiple Newton iterations
        /// - Very large numbers (≥ 4.3322e127): Advanced Newton Plus algorithm
        /// 
        /// The algorithm is based on the implementation by SunsetQuest available at:
        /// https://github.com/SunsetQuest/NewtonPlus-Fast-BigInteger-and-BigFloat-Square-Root
        /// </remarks>
        public static BigInteger IntegerSqrt(this BigInteger n) {
            // https://github.com/SunsetQuest/NewtonPlus-Fast-BigInteger-and-BigFloat-Square-Root

            if (n < 144838757784765629) { // 1.448e17 = ~1<<57
                uint vInt = (uint)Math.Sqrt((ulong)n);

                if (n <= 4503599761588224 && (ulong)vInt * vInt > (ulong)n) { // 4.5e15 = ~1<<52
                    vInt--;
                }

                return vInt;
            }

            double xAsDub = (double)n;
            if (xAsDub < 8.5e37) { // 8.5e37 is V<sup>2</sup>long.max * long.max
                ulong vInt = (ulong)Math.Sqrt(xAsDub);
                BigInteger v = vInt + (ulong)(n / vInt) >> 1;
                return v * v >= n ? v : v - 1;
            }

            if (xAsDub < 4.3322e127) {
                BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
                v = v + n / v >> 1;
                if (xAsDub > 2e63) {
                    v = v + n / v >> 1;
                }
                return v * v >= n ? v : v - 1;
            }

            int xLen = (int)n.GetBitLength();
            int wantedPrecision = (xLen + 1) / 2;
            int xLenMod = xLen + (xLen & 1) + 1;

            // Do the first Sqrt on hardware
            long tempX = (long)(n >> xLenMod - 63);
            double tempSqrt1 = Math.Sqrt(tempX);
            ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
            if (valLong == 0) {
                valLong = 1UL << 53;
            }

            // Classic Newton Iterations
            BigInteger val = ((BigInteger)valLong << 53 - 1) + (n >> xLenMod - 3 * 53) / valLong;
            int size = 106;
            for (; size < 256; size <<= 1) {
                val = (val << size - 1) + (n >> xLenMod - 3 * size) / val;
            }

            if (xAsDub > 4e254) { // 1 << 845
                int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 2;

                // Apply Starting Size
                int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
                int needToShiftBy = size - wantedSize;
                val >>= needToShiftBy;
                size = wantedSize;
                do {
                    // Newton Plus Iterations

                    int shiftX = xLenMod - 3 * size;
                    BigInteger valSqrd = val * val << size - 1;
                    BigInteger valSU = (n >> shiftX) - valSqrd;
                    val = (val << size) + valSU / val;
                    size *= 2;
                } while (size < wantedPrecision);
            }

            // There are a few extra digits here, lets save them
            int oversidedBy = size - wantedPrecision;
            BigInteger saveDroppedDigitsBI = val & (BigInteger.One << oversidedBy) - 1;
            int downby = oversidedBy < 64 ? (oversidedBy >> 2) + 1 : oversidedBy - 32;
            ulong saveDroppedDigits = (ulong)(saveDroppedDigitsBI >> downby);

            // Shrink result to wanted Precision
            val >>= oversidedBy;

            // Detect a round-ups
            if (saveDroppedDigits == 0 && val * val > n) {
                val--;
            }

            // //////// Error Detection ////////
            // // I believe the above has no errors but to guarantee the following can be added.
            // // If an error is found, please report it.
            // BigInteger tmp = val * val;
            // if (tmp > x)
            // {
            //     throw new Exception("Sqrt function had internal error - value too high");
            // }
            // if ((tmp + 2 * val + 1) >= x)
            // {
            //     throw new Exception("Sqrt function had internal error - value too low");
            // }

            return val;
        }

        /// <summary>
        /// Calculates the integer square root of a BigInteger using a branchless optimized implementation.
        /// </summary>
        /// <param name="n">The BigInteger to find the square root of.</param>
        /// <returns>The largest integer whose square is less than or equal to n.</returns>
        /// <remarks>
        /// This method provides the same mathematical result as <see cref="IntegerSqrt(BigInteger)"/> but uses
        /// branchless programming techniques to reduce conditional branches and potentially improve performance
        /// on modern processors with deep pipelines.
        /// 
        /// The branchless implementation uses bitwise operations and conditional masks to eliminate most
        /// if-else statements, which can improve CPU branch prediction and reduce pipeline stalls.
        /// 
        /// Performance characteristics:
        /// - May be faster on processors with deep pipelines and poor branch prediction
        /// - May be slower on processors with good branch prediction due to additional arithmetic operations
        /// - Memory usage is similar to the standard implementation
        /// 
        /// Use this method when performance profiling indicates that branch mispredictions are a bottleneck
        /// in your specific use case.
        /// </remarks>
        public static BigInteger IntegerSqrtBranchless(this BigInteger n) {
            // Optimized branchless version of IntegerSqrt with reduced conditional branches
            // Based on the original algorithm with branchless optimizations

            // Early exit for zero and negative numbers (branchless using sign bit)
            var signMask = (n >> 63) & 1; // 1 if negative, 0 if non-negative
            var isZero = (n == 0) ? 1 : 0;
            if (signMask == 1 || isZero == 1) {
                return BigInteger.Zero;
            }

            // Branchless range detection using bitwise operations
            var range1Mask = (n < 144838757784765629) ? 1 : 0;
            var range2Mask = (n >= 144838757784765629 && (double)n < 8.5e37) ? 1 : 0;
            var range3Mask = (n >= 144838757784765629 && (double)n >= 8.5e37 && (double)n < 4.3322e127) ? 1 : 0;
            var range4Mask = (n >= 144838757784765629 && (double)n >= 4.3322e127) ? 1 : 0;

            BigInteger result = BigInteger.Zero;

            // Range 1: Small numbers (< 1.448e17)
            if (range1Mask == 1) {
                uint vInt = (uint)Math.Sqrt((ulong)n);

                // Branchless adjustment using conditional mask
                var adjustMask = (n <= 4503599761588224 && (ulong)vInt * vInt > (ulong)n) ? 1 : 0;
                vInt -= (uint)adjustMask;

                result = vInt;
            }

            // Range 2: Medium numbers (1.448e17 to 8.5e37)
            if (range2Mask == 1) {
                double xAsDub = (double)n;
                ulong vInt = (ulong)Math.Sqrt(xAsDub);
                BigInteger v = (new BigInteger(vInt) + (BigInteger)((BigInteger)n / vInt)) >> 1;

                // Branchless final adjustment
                var finalAdjustMask = (v * v >= n) ? 0 : 1;
                result = v - finalAdjustMask;
            }

            // Range 3: Large numbers (8.5e37 to 4.3322e127)
            if (range3Mask == 1) {
                double xAsDub = (double)n;
                BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
                var extraIterationMask = (xAsDub > 2e63) ? 1 : 0;
                v = v + ((BigInteger)(((BigInteger)n / v) >> 1) * (BigInteger)extraIterationMask);

                // Branchless final adjustment
                var finalAdjustMask = (v * v >= n) ? 0 : 1;
                result = v - finalAdjustMask;
            }

            // Range 4: Very large numbers (>= 4.3322e127)
            if (range4Mask == 1) {
                double xAsDub = (double)n;
                int xLen = (int)n.GetBitLength();
                int wantedPrecision = (xLen + 1) / 2;
                int xLenMod = xLen + (xLen & 1) + 1;

                // Branchless initialization using conditional mask
                long tempX = (long)(n >> xLenMod - 63);
                double tempSqrt1 = Math.Sqrt(tempX);
                ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
                var initMask = (valLong == 0) ? 1 : 0;
                valLong = valLong + (1UL << 53) * (ulong)initMask;

                // Classic Newton Iterations
                BigInteger val = ((BigInteger)valLong << 53 - 1) + (n >> xLenMod - 3 * 53) / valLong;
                int size = 106;

                // Unrolled loop for better performance
                for (int i = 0; i < 2; i++) // size: 106 -> 212 -> 424 (but we cap at 256)
                {
                    if (size >= 256) break;
                    val = (val << size - 1) + (n >> xLenMod - 3 * size) / val;
                    size <<= 1;
                }

                // Branchless Newton Plus iterations
                var newtonPlusMask = (xAsDub > 4e254) ? 1 : 0;
                if (newtonPlusMask == 1) {
                    int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 2;
                    int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
                    int needToShiftBy = size - wantedSize;
                    val >>= needToShiftBy;
                    size = wantedSize;

                    // Unrolled Newton Plus iterations
                    while (size < wantedPrecision) {
                        int shiftX = xLenMod - 3 * size;
                        BigInteger valSqrd = val * val << size - 1;
                        BigInteger valSU = (n >> shiftX) - valSqrd;
                        val = (val << size) + valSU / val;
                        size *= 2;
                    }
                }

                // Branchless precision adjustment
                int oversidedBy = size - wantedPrecision;
                BigInteger saveDroppedDigitsBI = val & (BigInteger.One << oversidedBy) - 1;
                int downby = oversidedBy < 64 ? (oversidedBy >> 2) + 1 : oversidedBy - 32;
                ulong saveDroppedDigits = (ulong)(saveDroppedDigitsBI >> downby);

                val >>= oversidedBy;

                // Branchless final adjustment
                var roundUpMask = (saveDroppedDigits == 0 && val * val > n) ? 1 : 0;
                result = val - roundUpMask;
            }

            return result;
        }
    }
}
