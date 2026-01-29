# Math Playground Python Scripts

This directory contains Python ports of the C# components found in the parent repository.

## Scripts

### 1. Reverse Multiply (`reverseMultiply.py`)
A direct port of the logic from:
- [`/cs/Numberality.Con/Components/ReverseMultiply.cs`](../cs/Numberality.Con/Components/ReverseMultiply.cs)

**Method:** Digit-by-digit reconstruction using a stack of "Prospects" to match suffixes.

### 2. Unmultiply (`unmultiply.py`)
A direct port of the logic from:
- [`/cs/TestingNumbers/Components/Unmultiply.cs`](../cs/TestingNumbers/Components/Unmultiply.cs)

**Method:** Factors using a combination of wheel-based search near the square root and a logic to find potential factors by reconstructing suffixes (Stack-based DFS).

## Requirements

-   Python 3.8+

## Usage

Both scripts share the same command-line interface.

### 1. Factorize a specific number

To factorize a known number:

```bash
# Using Reverse Multiply
python reverseMultiply.py --number <your_number>

# Using Unmultiply
python unmultiply.py --number <your_number>
```

**Example:**
```bash
python unmultiply.py --number 8051
# Output:
# Solving for N = 8051
# ...
# Solution Found: 8051
# Factors: 83, 97
```

### 2. Generate and Factorize

To generate a random target (product of two probable primes) and immediately attempt to factorize it:

```bash
python reverseMultiply.py --rng-digits <total_digits>
# OR
python unmultiply.py --rng-digits <total_digits>
```

**Example:**
```bash
python unmultiply.py --rng-digits 10
# Output:
# Generating probable primes roughly 5 digits long...
# Generated target: ...
# Solving for N = ...
```

---
*Created with [Gemini](https://gemini.google.com) by [Kthksdie](https://x.com/jasonlee2122).*
