import math
import argparse
import random
import time
from dataclasses import dataclass
from typing import List, Generator

# ------------------------------------------------------------------------------
# ------------------------------------------------------------------------------
# Helper Functions and Classes
# ------------------------------------------------------------------------------

def isqrt(n: int) -> int:
    """Returns the integer square root of n."""
    if n < 0:
        raise ValueError
    if n == 0:
        return 0
    # math.isqrt is available in Python 3.8+
    return math.isqrt(n)

def number_of_digits(n: int) -> int:
    """Returns the number of digits in n."""
    if n == 0:
        return 1
    if n < 0:
        n = -n
    return len(str(n))

def is_probable_prime(n: int, k: int = 40) -> bool:
    """
    Miller-Rabin primality test.
    Returns True if n is probably prime, False if composite.
    k is the number of rounds (accuracy).
    """
    if n <= 1: return False
    if n <= 3: return True
    if n % 2 == 0: return False

    # Find r and d such that n - 1 = 2^r * d
    r, d = 0, n - 1
    while d % 2 == 0:
        r += 1
        d //= 2

    for _ in range(k):
        a = random.randint(2, n - 2)
        x = pow(a, d, n)
        if x == 1 or x == n - 1:
            continue
        for _ in range(r - 1):
            x = pow(x, 2, n)
            if x == n - 1:
                break
        else:
            return False
    return True

def generate_prime_candidate(min_val: int, max_val: int) -> int:
    """Generates a probable prime between min_val and max_val."""
    while True:
        p = random.randint(min_val, max_val)
        # Optimization: make sure it's odd
        if p % 2 == 0:
            p += 1
        if p > max_val:
            continue
            
        if is_probable_prime(p):
            return p

@dataclass(frozen=True)
class Product:
    c: int
    e: int
    p: int

    def __str__(self):
        return f"{self.c} * {self.e} = {self.p}"

@dataclass
class Prospect:
    target_digits: int
    product: Product

# ------------------------------------------------------------------------------
# Core Logic
# ------------------------------------------------------------------------------

class ReverseMultiply:
    def __init__(self, n: int):
        self.n = n
        self.prospects_count = 0
        self.eliminations = 0
        self.start_time = 0
        self.end_time = 0

        self.digit_targets: List[int] = []
        self.length_targets: List[int] = []
        self.step_targets: List[int] = []
        
        # Initialize
        self.initial_prospects = self._initialize_prospects()
        # Initialize stack with initial prospects (target_digits=2)
        # In C# it was: new Stack<Prospect>(_initalProspects.Select(x => new Prospect(2, x)));
        self.current_prospects = [Prospect(2, x) for x in self.initial_prospects]
        
        self.max_prospects = len(self.initial_prospects) ** 2 # Approximation
        self._initialize_targets()

    def _initialize_targets(self):
        t = 2
        lt = 100
        
        while True:
            digit_target = self.n % lt
            length_target = lt
            step_target = lt // 10
            
            self.digit_targets.append(digit_target)
            self.length_targets.append(length_target)
            self.step_targets.append(step_target)
            
            lt *= 10
            t += 1
            if lt > self.n:
                # Include one more level to cover the full number? 
                # C# breaks when lt > n. 
                # If n=123, lt=100 (loop running), next lt=1000 (>n), breaks.
                break
        
        self.max_target = t
        
    def _initialize_prospects(self) -> List[Product]:
        prospects = []
        
        n_isqrt = isqrt(self.n)
        # nt = 10 ^ (digits(isqrt(n)) - 1)
        # Example: n=12345, sqrt=111 (3 digits), nt=100
        nt = 10 ** (number_of_digits(n_isqrt) - 1)
        
        ti = 1
        lt = 10 ** ti # 10
        dt = self.n % lt # last digit
        
        # Integers.Products(max: lt - 1) 
        # Assuming this means searching for c, e in [0..9] (since lt-1 = 9)
        # such that (c * e) % lt == dt
        limit = lt - 1
        
        # Note: We iterate 0 to limit inclusive
        for c in range(limit + 1):
            for e in range(limit + 1):
                p = c * e
                if p % lt == dt:
                    # prospects.Add(new Product(product.C + nt, product.E + nt));
                    # Use the high-order offset logic from C#
                    prospects.append(Product(c + nt, e + nt, p)) # p here is just c*e small product? 
                    # Note: In C# `new Product` recalculates P: C*E.
                    # self.Product(c+nt, e+nt, ...) needs the full product
                    # The C# struct `Product(c, e)` calculates P = c*e.
                    # My dataclass doesn't auto-calc, so I should be careful.
                    # But wait, looking at my Python Product:
                    # c: int, e: int, p: int.
                    # I should likely instantiate just c and e, and calc P.
                    
        # Fix: The appended prospects need correct P
        fixed_prospects = []
        for p in prospects:
            # We must recalculate P based on the offset C and E
            real_c = p.c
            real_e = p.e
            real_p = real_c * real_e
            fixed_prospects.append(Product(real_c, real_e, real_p))
            
        return fixed_prospects

    def _get_prospects_v2(self, current_prospect: Product, target_digits: int) -> List[Product]:
        products = []
        
        # ti = targetDigits - 2
        ti = target_digits - 2
        
        if ti >= len(self.length_targets):
            # Should not happen if max_target checks are correct
            return products

        lt = self.length_targets[ti]
        st = self.step_targets[ti]
        dt = self.digit_targets[ti]
        
        cl_limit = lt + current_prospect.c
        el_limit = lt + current_prospect.e
        
        cl = current_prospect.c
        
        # Loops:
        # while cl <= cl_limit
        #   el = currentProspect.E
        #   while el < el_limit   (Note: C# uses < el_limit? Let's check source)
        #     checks...
        #     el += st
        #   cl += st
        
        # Checking C# source:
        # line 279: while (cl <= cl_limit)
        # line 282: while (el < el_limit)  <-- Wait, line 251 (v1) used <=. v2 uses < ?
        # line 251: for (el = ...; el <= el_limit; ...)
        # line 282: while (el < el_limit)
        # This difference is suspicious.
        # If st=10, limit=Current+100.
        # If we iterate 0, 10, ... 90 (10 steps).
        # < Limit (if limit is +100) means up to +90.
        # <= Limit means up to +100.
        # +100 implies we are wrapping around or overflowing the digit?
        # Usually we want 10 steps (0 to 9 for that digit).
        # if st=10, 10 steps is +0, +10, ... +90.
        # So < limit (where limit = start + 100) covers +0..+90.
        # <= limit would cover +100 which overlaps with next digit?
        # I will stick to `<` for `el` as in v2, but check `cl`.
        # C# v2 line 279: `cl <= cl_limit`.
        # This means `cl` iterates 11 times? 
        # If so, it might be checking carry-overs or something?
        # But `el` iterates 10 times.
        # Let's replicate EXACTLY C# v2.
        
        while cl <= cl_limit:
            el = current_prospect.e
            while el < el_limit:
                p = cl * el
                if p % lt == dt:
                    products.append(Product(cl, e=el, p=p))
                    # break inner loop?
                    # C# line 286: break; 
                    # This means once we find a matching suffix for EL, we stop checking ELs?
                    # Since we only vary the digit at `st`, there should be only one matches modulo?
                    break
                
                el += st
            cl += st
            
        return products

    def try_solve(self) -> Product:
        solution = Product(0, 0, 0)
        self.start_time = time.time()
        
        # _currentProspects is a Stack (LIFO). Python list as stack: append/pop.
        
        done = False
        while not done:
            if not self.current_prospects:
                break
                
            current = self.current_prospects.pop()
            next_target = current.target_digits + 1
            
            if next_target >= self.max_target:
                continue
                
            self.prospects_count += 1
            
            prospects_v2 = self._get_prospects_v2(current.product, current.target_digits)
            for product in prospects_v2:
                if product.p > self.n:
                    continue
                elif product.p == self.n:
                    self.end_time = time.time()
                    solution = product
                    done = True
                    break
                
                # Push back to stack
                self.current_prospects.append(Prospect(next_target, product))
                
        return solution

# ------------------------------------------------------------------------------
# CLI
# ------------------------------------------------------------------------------

def main():
    parser = argparse.ArgumentParser(description="Reverse Multiply (Factorization) Port")
    parser.add_argument("--rng-digits", type=int, help="Generate a random number with this many digits (composite of two halves).")
    parser.add_argument("--number", type=int, help="Specific number to factorize.")
    
    args = parser.parse_args()
    
    target_n = 0
    
    if args.number:
        target_n = args.number
    elif args.rng_digits:
        # Generate two factors
        # If digits = 10. We want factors roughly 5 digits (sqrt).
        # random range for factors.
        half_digits = args.rng_digits // 2
        min_f = 10**(half_digits - 1)
        max_f = (10**half_digits) - 1
        
        print(f"Generating probable primes roughly {half_digits} digits long...")
        f1 = generate_prime_candidate(min_val=min_f, max_val=max_f)
        f2 = generate_prime_candidate(min_val=min_f, max_val=max_f)

        target_n = f1 * f2
        
        print(f"Generated target: {target_n} (from {f1} * {f2})")
    else:
        print("Please provide --rng-digits or --number")
        return

    print(f"Solving for N = {target_n}")
    
    solver = ReverseMultiply(target_n)
    
    # print targets info for debug
    # print(f"Digit Targets: {solver.digit_targets}")
    
    solution = solver.try_solve()
    
    if solution.p == target_n:
        print(f"Solution Found: {solution}")
        print(f"Factors: {solution.c}, {solution.e}")
        print(f"Time: {solver.end_time - solver.start_time:.4f}s")
    else:
        print("No solution found.")

if __name__ == "__main__":
    main()
