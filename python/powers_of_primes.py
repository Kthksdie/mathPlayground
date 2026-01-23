import time
import threading
import queue
from typing import Iterator, NamedTuple

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
    exponent = target_exponent
    power_of = PowerOf(base_of, exponent, current_val)
    
    while True:
        yield power_of
        
        # Calculate next
        power_of.value *= base_of
        power_of.exponent += 1

def check_primality(candidate: int, base_of: int, exponent: int) -> bool:
    """
    Works for primes in the form of:
        (k^n) - (k - 1)
        (k^p - 1)/k - 1    *k cannot be p^n; where n > 1*

    -- https://github.com/Kthksdie | Method inspired by Lucas-Lehmer's for 2^p - 1, and by Alan Gee's for (3^p - 1)/2.
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

class ResultData(NamedTuple):
    details: str
    num_of_digits: int
    elapsed: float
    status: str
    tested_count: int
    positive_count: int

def processing_thread_func(result_queue: queue.Queue, stop_event: threading.Event):
    tested_count = 0
    positive_count = 0
    
    iterator = powers_of(3, 0)
    
    while not stop_event.is_set():
        if tested_count > 5000:
            break
            
        try:
            power_of = next(iterator)
        except StopIteration:
            break
            
        candidate = power_of.value - (power_of.base_of - 1)
        details = f"{power_of.base_of}^{power_of.exponent}-{(power_of.base_of - 1)}"
        
        num_of_digits = number_of_digits(candidate)
        
        check_watch_start = time.perf_counter()
        
        is_prime = check_primality(candidate, power_of.base_of, power_of.exponent)
        
        elapsed = time.perf_counter() - check_watch_start
        
        if is_prime:
            positive_count += 1

        status = "PRIME" if is_prime else "NOT PRIME"
        
        data = ResultData(
            details=details,
            num_of_digits=num_of_digits,
            elapsed=elapsed,
            status=status,
            tested_count=tested_count,
            positive_count=positive_count
        )
        
        result_queue.put(data)
        tested_count += 1
    
    # Signal that processing is done (optional, or just rely on stop_event/main join)

def output_thread_func(result_queue: queue.Queue, stop_event: threading.Event):
    while not stop_event.is_set() or not result_queue.empty():
        try:
            # Wait for 0.1s so we can check stop_event periodically
            data = result_queue.get(timeout=0.1)
        except queue.Empty:
            continue
            
        print("\033c", end="")
        print(f"\n{data.details} | digits {data.num_of_digits} | # {data.tested_count} | ~ {data.positive_count}" +
              f"\n{data.details} | digits {data.num_of_digits} | {data.elapsed:.4f}s | {data.status}", flush=True)
        
        result_queue.task_done()

def execute():
    result_queue = queue.Queue()
    stop_event = threading.Event()
    
    proc_thread = threading.Thread(target=processing_thread_func, args=(result_queue, stop_event))
    out_thread = threading.Thread(target=output_thread_func, args=(result_queue, stop_event))
    
    proc_thread.start()
    out_thread.start()
    
    try:
        proc_thread.join()
        # Once processing is done, we can tell output to stop? 
        # Actually output thread loop condition checks queue empty.
        # But we need to make sure output thread doesn't exit prematurely if processing is slow, 
        # though processing is CPU bound and we have data flowing.
        # Ideally, we should signal output thread to stop only after processing is done AND queue is empty.
        
        # Let's wait for queue to be empty first
        result_queue.join() 
        # Then signal stop
        stop_event.set()
        out_thread.join()
        
    except KeyboardInterrupt:
        print("\nAborted.")
        stop_event.set()
        proc_thread.join()
        out_thread.join()

if __name__ == "__main__":
    execute()
