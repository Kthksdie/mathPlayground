import time
import math
from typing import Iterator

class PowerOf:
    def __init__(self, base_of: int, exponent: int, value: int):
        self.base_of = base_of
        self.exponent = exponent
        self.value = value

def powers_of(base_of: int, target_exponent: int) -> Iterator[PowerOf]:
    """
    Generates an infinite sequence of powers of a given base number.
    """
    if base_of == 1:
        base_of = 2

    current_val = pow(base_of, target_exponent) if target_exponent > 0 else 1
    # Adjust starting value if target_exponent is 0, since we want to start loop correctly or just setup initial state
    # The C# loop:
    # var powerOf = new PowerOf(baseOf, 0, 1);
    # while (powerOf.Exponent < targetExponent) { ... }
    
    # We can just start from target_exponent
    exponent = target_exponent
    power_of = PowerOf(base_of, exponent, current_val)
    
    # If target_exponent was 0, current_val is 1 which is correct for base^0
    # If target_exponent > 0, we calculate it using pow above.
    
    while True:
        yield power_of
        
        # Calculate next
        power_of.value *= base_of
        power_of.exponent += 1
        # object is yielded by reference in C# but here we yield the object. 
        # In C# it modifies the same object. Python yield returns the object. 
        # Since I'm modifying the object after yield, it might differ if consumed lazily/stored.
        # But for the loop usage in Execute(), it is fine as it is consumed immediately.
        # However, to be safe and cleaner in Python, let's yield a new object or just update.
        # The C# code modifies the *same* instance. 
        # "yield return powerOf;" then "powerOf.Value *= baseOf;"
        # So in the loop `foreach (var powerOf in ...)` the user gets the object.
        # In Python if I yield `power_of` and then modify it, the user sees the modified version if they held onto it?
        # Actually in `foreach` execution, the body executes before the iterator resumes.
        # So it's fine.

def check_primality(candidate: int, base_of: int, exponent: int) -> bool:
    """
    Method inspired by Lucas-Lehmer's for 2^p - 1, and by Alan Gee's for (3^p - 1)/2.
        Works for:
            (k^n) - (k - 1)
            (k^p - 1)/k - 1    *k cannot be p^n; where n > 1*
    @JKthksdie
    """
    if candidate <= 0:
        return False

    a = 3 if base_of == 2 else 2
    
    v = pow(a, base_of)
    
    if v > candidate:
        return False # Too small | may return false positives
        
    s = v
    
    count = exponent - 1
    while count > 0:
        s = pow(s, base_of, candidate)
        count -= 1
        
    return s == v

def number_of_digits(n: int) -> int:
    if n == 0:
        return 1
    if n < 0:
        n = -n
    return len(str(n))

def execute():
    check_watch_start = 0.0
    
    tested_count = 0
    positive_count = 0
    
    for power_of in powers_of(3, 0):
        candidate = power_of.value - (power_of.base_of - 1)
        details = f"{power_of.base_of}^{power_of.exponent}-{(power_of.base_of - 1)}"
        
        num_of_digits = number_of_digits(candidate)
        
        check_watch_start = time.perf_counter()
        
        is_prime = check_primality(candidate, power_of.base_of, power_of.exponent)
        
        elapsed = time.perf_counter() - check_watch_start
        
        if is_prime:
            positive_count += 1

        status = "PRIME" if is_prime else "NOT PRIME"

        print("\033c", end="")
        print(f"\n{details} | digits {num_of_digits} | # {tested_count} | ~ {positive_count}" +
        f"\n{details} | digits {num_of_digits} | {elapsed:.4f}s | {status}")

        tested_count += 1
        if tested_count > 5000:
            break

if __name__ == "__main__":
    try:
        execute()
    except KeyboardInterrupt:
        print("\nAborted.")
