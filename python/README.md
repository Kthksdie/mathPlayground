# Reverse Multiply (Python)

A CLI-based port of the core logic from:
- [`/cs/Numberality.Con/Components/ReverseMultiply.cs`](./cs/Numberality.Con/Components/ReverseMultiply.cs)

## Requirements

-   Python 3.8+

## Usage

### 1. Factorize a specific number

To factorize a known number (e.g., product of two primes):

```bash
python reverse_multiply.py --number <your_number>
```

**Example:**
```bash
python reverse_multiply.py --number 8051
# Output:
# Solving for N = 8051
# ...
# Solution Found: 83 * 97 = 8051
```

### 2. Generate and Factorize

To generate a random target (product of two probable primes) and immediately attempt to factorize it:

```bash
python reverse_multiply.py --rng-digits <total_digits>
```

**Example:**
```bash
python reverse_multiply.py --rng-digits 10
# Output:
# Generating probable primes roughly 5 digits long...
# Generated target: ...
# Solving for N = ...
```

## How it works

The script uses a digit-by-digit reconstruction method:
1.  It calculates the target digits (modulo $10^k$) that the product must meet.
2.  It maintains a stack of "Prospects" (pairs of potential factor suffixes).
3.  It iteratively extends these prospects, keeping only those whose product matches the target digits of $N$.

---
*Created with ❤️ by a [Gemini](https://gemini.google.com).*