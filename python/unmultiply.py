import math
import argparse
import random
import time
from dataclasses import dataclass
from typing import List, Generator, Optional, Tuple

# ------------------------------------------------------------------------------
# Helper Functions
# ------------------------------------------------------------------------------

from math_utils import MathUtils

# ------------------------------------------------------------------------------
# Helper Functions
# ------------------------------------------------------------------------------

# ------------------------------------------------------------------------------
# Classes
# ------------------------------------------------------------------------------

@dataclass
class UnmultiplyResult:
    idx: int = 0
    left: int = 0
    right: int = 0
    value: int = 0
    carry: int = 0

class Unmultiply:
    # Static dictionaries for BruteForce steps
    _digites = {6: 0, 0: 4, 4: 8, 8: 2, 2: 6}
    _digitas = {6: 2, 2: 8, 8: 4, 4: 0, 0: 6}

    def __init__(self, n: int):
        self.n = n
        self.integer_sqrt = MathUtils.isqrt(n)
        
        # Approximate Pi usage based on C# code logic using precision 16
        # Assuming float precision is sufficient
        t_sqrt = self.integer_sqrt + 1
        self.max_sqrt = int(t_sqrt * math.pi)
        self.min_sqrt = int(t_sqrt / math.pi)
        
        self.iterations = 0
        self.eliminations = 0
        self.stack_count = 0
        self.start_time = 0
        self.end_time = 0
        self.stop_flag = False

    def brute_force(self) -> int:
        """
        Attempt to find a factor using a wheel-based search near sqrt.
        Ported from Unmultiply.cs BruteForce.
        """
        start_time = time.time()
        iterations = 0

        if self.n % self.integer_sqrt == 0:
            print(f" k {self.integer_sqrt} | Sqrt | i 0 | {time.time() - start_time:.4f}")
            return self.integer_sqrt

        cen_k = self.integer_sqrt - (self.integer_sqrt % 6)
        cen_last_digit = int(cen_k % 10)

        min_k = self.min_sqrt - (self.min_sqrt % 6)
        min_last_digit = int(min_k % 10)

        path = ""
        k = 1

        while True:
            # Check center
            if cen_last_digit != 6:
                if self.n % (cen_k + 1) == 0:
                    cen_k += 1
                    path = "CenK"
                    break
                elif self.n % (cen_k - 1) == 0:
                    cen_k -= 1
                    path = "CenK"
                    break
            elif self.n % (cen_k + 1) == 0:
                cen_k += 1
                path = "CenK"
                break
            
            cen_k -= 6
            cen_last_digit = self._digites[cen_last_digit]

            # Check min
            if min_last_digit != 6:
                if self.n % (min_k + 1) == 0:
                    min_k += 1
                    path = "MinK"
                    break
                elif self.n % (min_k - 1) == 0:
                    min_k -= 1
                    path = "MinK"
                    break
            elif self.n % (min_k + 1) == 0:
                min_k += 1
                path = "MinK"
                break
            
            min_k += 6
            min_last_digit = self._digitas[min_last_digit]

            if min_k >= cen_k:
                path = "NpK"
                break
            
            iterations += 1

        if path == "CenK":
            k = cen_k
        elif path == "MinK":
            k = min_k

        print(f"  k {k} | {path} | i {iterations} | {time.time() - start_time:.4f}")
        return k

    def results(self, left: int, right: int) -> Generator[UnmultiplyResult, None, None]:
        """
        Generator yielding potential unmultiply steps.
        Ported from Unmultiply.cs Results (and logic of Resultsv2 for eliminations).
        """
        # C# Logic: var digits = leftDigits > rightDigits ? rightDigits : leftDigits;
        # Crucial: Treating 0 as 0 digits for the purpose of the initial step
        left_digits = MathUtils.number_of_digits_zero_is_zero(left)
        right_digits = MathUtils.number_of_digits_zero_is_zero(right)
        
        current_digits = max(left_digits, right_digits)
        min_digits = current_digits + 1
        
        dd = 10 ** current_digits
        nd = 10 ** min_digits
        ld = self.n % nd

        # Iterating x from dd + left to nd + left with step dd
        # This basically iterates through prepending digits 1..10 to left
        x = dd + left
        while x <= nd + left:
            self.iterations += 1
            
            # Optimization from Resultsv2: Exact check
            if x > 1 and self.n % x == 0:
                if x * (self.n // x) == self.n:
                    ty = self.n // x
                    yield UnmultiplyResult(left=x, right=ty, value=x*ty)
                    # In C# Resultsv2 this returns immediately.
                    # We continue here to match iterator behavior but return high priority match?
                    # Actually, if we find exact factor, we can probably stop.
                    # But yield allows caller to decide.
            
            y = dd + right
            while y <= nd + right:
                self.iterations += 1
                
                # Optimization from Resultsv2: Exact check
                if y > 1 and self.n % y == 0:
                     if y * (self.n // y) == self.n:
                        tx = self.n // y
                        yield UnmultiplyResult(left=tx, right=y, value=tx*y)
                
                z = x * y
                if z > self.n:
                    # In Resultsv2, this causes yield break (and increment eliminations)
                    self.eliminations += 1
                    # Since y increases, z increases. Further y will also be > n.
                    break 
                elif z % nd != ld:
                    y += dd
                    continue
                
                # Suffix matches
                res = UnmultiplyResult(left=x, right=y, value=z, carry=z // nd)
                
                yield res
                
                if z == self.n:
                    # Found exact match
                    return
                
                # In C# Results/Resultsv2: "break" (inner loop) after finding a match
                # Reasoning: for a fixed x, and fixed next-digit-position, 
                # there is usually only one y digit that satisfies the modulo equation.
                break
            
                y += dd
            
            x += dd

    def process(self, reverse: bool = True) -> UnmultiplyResult:
        """
        Main solver process using a stack-based DFS.
        Ported from Unmultiply.cs Process.
        """
        self.start_time = time.time()
        s = UnmultiplyResult(left=0, right=0, value=0)
        
        stack = [s]
        self.stack_count = 1
        
        while True:
            if self.stop_flag:
                return None
            
            if not stack:
                break
                
            current = stack.pop()
            self.stack_count -= 1
            
            # Get results generator
            res_gen = self.results(current.left, current.right)
            results_list = list(res_gen)
            
            if reverse:
                results_list.reverse()
                
            for result in results_list:
                if self.stop_flag:
                    return None
                
                if result.value == self.n:
                    self.end_time = time.time()
                    return result
                
                stack.append(result)
                self.stack_count += 1
        
        self.end_time = time.time()
        return s

# ------------------------------------------------------------------------------
# Main
# ------------------------------------------------------------------------------

def main():
    parser = argparse.ArgumentParser(description="Unmultiply Port (Factors by suffix reconstruction)")
    parser.add_argument("--rng-digits", type=int, help="Generate a random number with this many digits (composite of two halves).")
    parser.add_argument("--number", type=int, help="Specific number to factorize.")
    
    args = parser.parse_args()
    
    target_n = 0
    
    if args.number:
        target_n = args.number
    elif args.rng_digits:
        half_digits = args.rng_digits // 2
        min_f = 10**(half_digits - 1)
        max_f = (10**half_digits) - 1
        
        print(f"Generating probable primes roughly {half_digits} digits long...")
        f1 = MathUtils.generate_prime_candidate(min_val=min_f, max_val=max_f)
        f2 = MathUtils.generate_prime_candidate(min_val=min_f, max_val=max_f)

        target_n = f1 * f2
        print(f"Generated target: {target_n} (from {f1} * {f2})")
    else:
        print("Please provide --rng-digits or --number")
        return

    print(f"Solving for N = {target_n}")
    
    solver = Unmultiply(target_n)
    
    # Try brute force first? C# doesn't call it in process, but let's leave it available.
    # The requirement is to run the main logic.
    
    solution = solver.process(reverse=True)
    
    if solution and solution.value == target_n:
        print(f"Solution Found: {solution.value}")
        print(f"Factors: {solution.left}, {solution.right}")
        print(f"Time: {solver.end_time - solver.start_time:.4f}s")
    else:
        print("No solution found.")

if __name__ == "__main__":
    main()
