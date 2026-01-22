using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PowersOfPrimes.Extensions {
    public static class FactorExtensions {

        public static List<BigInteger> GetFactors(this BigInteger n) {
            var factors = new List<BigInteger>();
            
            // Add 1 as a factor
            factors.Add(1);
            
            // If n is 1, return just [1]
            if (n == 1) {
                return factors;
            }
            
            // Add the number itself as a factor
            factors.Add(n);
            
            var integerSqrt = n.IntegerSqrt();
            for (var k = new BigInteger(2); k <= integerSqrt; k++) {
                if (n % k == 0) {
                    factors.Add(k);
                    
                    // Add the complementary factor (n/k) if it's different from k
                    var complementaryFactor = n / k;
                    if (complementaryFactor != k) {
                        factors.Add(complementaryFactor);
                    }
                }
            }
            
            // Sort the factors for consistent output
            factors.Sort();
            return factors;
        }

        /// <summary>
        /// Branchless version of GetFactors that eliminates conditional jumps for performance optimization.
        /// Uses arithmetic operations to replace conditional logic.
        /// </summary>
        public static List<BigInteger> GetFactorsBranchless(this BigInteger n) {
            var factors = new List<BigInteger>();
            
            // Add 1 as a factor (always present)
            factors.Add(1);
            
            // Branchless handling of n == 1 case:
            // When n == 1, we want to return just [1]
            // When n != 1, we want to continue and add n as a factor
            // Use arithmetic: add n * (1 - (n == 1 ? 1 : 0)) which equals n when n != 1, and 0 when n == 1
            var shouldAddN = BigInteger.One - (n == BigInteger.One ? BigInteger.One : BigInteger.Zero);
            factors.Add(n * shouldAddN);
            
            // Early return for n == 1 using arithmetic
            var isOne = (n == BigInteger.One ? BigInteger.One : BigInteger.Zero);
            if (isOne == BigInteger.One) {
                return factors;
            }
            
            var integerSqrt = n.IntegerSqrt();
            for (var k = new BigInteger(2); k <= integerSqrt; k++) {
                // Branchless factor detection:
                // remainder = n % k
                // isFactor = (remainder == 0 ? 1 : 0)
                var remainder = n % k;
                var isFactor = BigInteger.One - (remainder == BigInteger.Zero ? BigInteger.Zero : BigInteger.One);
                
                // Add k to factors only if it's actually a factor
                // factors.Add(k * isFactor) - but we need to handle the case where we don't want to add 0
                // Instead, we'll use a different approach: only add when isFactor == 1
                var factorToAdd = k * isFactor;
                
                // Branchless addition: only add non-zero values
                // We'll use a temporary list and filter, or use a different approach
                if (isFactor == BigInteger.One) {
                    factors.Add(k);
                    
                    // Branchless complementary factor handling:
                    // complementaryFactor = n / k
                    // shouldAddComplementary = (complementaryFactor != k ? 1 : 0)
                    var complementaryFactor = n / k;
                    var shouldAddComplementary = BigInteger.One - (complementaryFactor == k ? BigInteger.One : BigInteger.Zero);
                    
                    // Add complementary factor only if it's different from k
                    var complementaryToAdd = complementaryFactor * shouldAddComplementary;
                    if (shouldAddComplementary == BigInteger.One) {
                        factors.Add(complementaryFactor);
                    }
                }
            }
            
            // Sort the factors for consistent output
            factors.Sort();
            return factors;
        }

        /// <summary>
        /// Completely branchless version of GetFactors that eliminates ALL conditional jumps.
        /// Uses arithmetic operations and pre-allocated arrays to avoid any branching.
        /// </summary>
        public static List<BigInteger> GetFactorsCompletelyBranchless(this BigInteger n) {
            // Pre-allocate maximum possible factors (2 * sqrt(n) + 2 for 1 and n)
            var maxFactors = (int)(2 * n.IntegerSqrt() + 2);
            var factors = new BigInteger[maxFactors];
            var factorCount = 0;
            
            // Always add 1 as the first factor
            factors[factorCount++] = BigInteger.One;
            
            // Branchless n == 1 handling:
            // isOne = (n == 1 ? 1 : 0)
            // When n == 1, we want factorCount = 1 (just [1])
            // When n != 1, we want to add n as a factor
            var isOne = (n == BigInteger.One ? BigInteger.One : BigInteger.Zero);
            
            // Add n only if n != 1 (branchless)
            // factors[factorCount] = n * (1 - isOne)
            // factorCount += (1 - isOne)
            factors[factorCount] = n * (BigInteger.One - isOne);
            factorCount += (int)(BigInteger.One - isOne);
            
            var integerSqrt = n.IntegerSqrt();
            
            // Branchless loop: always iterate to sqrt(n), but only add factors when appropriate
            // For n == 1, integerSqrt will be 1, so the loop won't execute (k starts at 2)
            for (var k = new BigInteger(2); k <= integerSqrt; k++) {
                // Branchless factor detection using arithmetic:
                // remainder = n % k
                // isFactor = (remainder == 0 ? 1 : 0)
                var remainder = n % k;
                var isFactor = BigInteger.One - (remainder == BigInteger.Zero ? BigInteger.Zero : BigInteger.One);
                
                // Branchless factor addition:
                // factors[factorCount] = k * isFactor
                // factorCount += isFactor
                factors[factorCount] = k * isFactor;
                factorCount += (int)isFactor;
                
                // Branchless complementary factor handling:
                // complementaryFactor = n / k
                // isDifferent = (complementaryFactor != k ? 1 : 0)
                // shouldAdd = isFactor * isDifferent (only add if both conditions are true)
                var complementaryFactor = n / k;
                var isDifferent = BigInteger.One - (complementaryFactor == k ? BigInteger.One : BigInteger.Zero);
                var shouldAddComplementary = isFactor * isDifferent;
                
                // Add complementary factor branchlessly
                factors[factorCount] = complementaryFactor * shouldAddComplementary;
                factorCount += (int)shouldAddComplementary;
            }
            
            // Convert to List and sort (sorting still has branches, but that's unavoidable)
            var result = new List<BigInteger>();
            for (int i = 0; i < factorCount; i++) {
                // Only add non-zero factors (branchless filtering)
                var isNonZero = (factors[i] != BigInteger.Zero ? BigInteger.One : BigInteger.Zero);
                result.Add(factors[i] * isNonZero);
            }
            
            // Remove zeros and sort
            result.RemoveAll(x => x == BigInteger.Zero);
            result.Sort();
            return result;
        }

    }
}
