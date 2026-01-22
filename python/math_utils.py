import math
import random

class MathUtils:
    @staticmethod
    def isqrt(n: int) -> int:
        """Returns the integer square root of n."""
        if n < 0:
            raise ValueError
        if n == 0:
            return 0
        return math.isqrt(n)

    @staticmethod
    def number_of_digits(n: int) -> int:
        """Returns the number of digits in n. Returns 1 for n=0."""
        if n == 0:
            return 1
        if n < 0:
            n = -n
        return len(str(n))

    @staticmethod
    def number_of_digits_zero_is_zero(n: int) -> int:
        """Returns the number of digits in n. Returns 0 for n=0.
        This is required for the Unmultiply algorithm logic."""
        if n == 0:
            return 0
        if n < 0:
            n = -n
        return len(str(n))

    @staticmethod
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

    @staticmethod
    def generate_prime_candidate(min_val: int, max_val: int) -> int:
        """Generates a probable prime between min_val and max_val."""
        while True:
            p = random.randint(min_val, max_val)
            # Optimization: make sure it's odd
            if p % 2 == 0:
                p += 1
            if p > max_val:
                continue
                
            if MathUtils.is_probable_prime(p):
                return p
