using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Helpers {
    public static class Unused {
        public static void MersennePrimes() {
            //var overallStopWatch = new Stopwatch();
            //overallStopWatch.Start();

            //// Best params so far:
            //// maxDegreeOfParallelism = 2;
            //// subMaxDegreeOfParallelism = 3;
            //// witnesses = 3;

            //var primaryDegreeOfParallelism = 1;
            //var secondaryDegreeOfParallelism = 3;

            //// Min: 3, Max: 10
            //var witnesses = 3;

            //var parallelOptions = new ParallelOptions() {
            //    MaxDegreeOfParallelism = primaryDegreeOfParallelism
            //};

            //InfiniteParallel.While(parallelOptions, _progress.Sequence, (binaryValue, sequence, state) => {
            //    // TODO: Refine?
            //    //lock (InfiniteParallel.ParallelLock) {
            //    //    while (SystemInternals.GetIdleTime() <= new TimeSpan(0, 0, 10)) {
            //    //        Task.Delay(new TimeSpan(0, 0, 5));
            //    //        continue;
            //    //    }
            //    //}

            //    var numberValue = binaryValue.ParseBinary();

            //    var displayValue = $"{numberValue}";

            //    if (displayValue.Length >= 50) {
            //        displayValue = $"{displayValue.Left(24)}...{displayValue.Right(23)}";
            //    }

            //    var primeResult = numberValue.IsProbablyPrime_Parallel(witnesses: witnesses, maxDegreeOfParallelism: secondaryDegreeOfParallelism);

            //    var numOfDigits = numberValue.NumberOfDigits();
            //    var durationPerDigit = new TimeSpan(primeResult.Duration.Ticks / numOfDigits);

            //    var loggingDetail = $"{displayValue,50}, D: {numOfDigits,9}, T: {primeResult.Duration,16}, P: {durationPerDigit,16}: B: {overallStopWatch.Elapsed,16}";

            //    lock (InfiniteParallel.ParallelLock) {
            //        if (primeResult.IsPrime) {
            //            var mersennePrime = new MersennePrime() {
            //                ID = _mersennePrimes.Count + 1,
            //                NumberOfDigits = numOfDigits,
            //                Number = numberValue.ToString(),
            //                Discovered = DateTime.UtcNow,
            //                Duration = primeResult.Duration,
            //                DurationPerDigit = durationPerDigit,
            //                DurationOverall = overallStopWatch.Elapsed,
            //                Sequence = sequence
            //            };

            //            Logging.Log($"{sequence,9}, {mersennePrime.ID,4}: {loggingDetail}", Color.Yellow);

            //            if (!_mersennePrimes.Any(x => x.Number == mersennePrime.Number)) {
            //                _mersennePrimes.Add(mersennePrime);
            //            }

            //            if (!_debugMode) {
            //                Logging.WriteToJsonFile(_mersennePrimesPath, _mersennePrimes);
            //            }
            //        }
            //        else {
            //            Logging.Log($"{sequence,9},       {loggingDetail}", rewrite: true);
            //        }

            //        _progress.Sequence++;

            //        if (!_debugMode) {
            //            Logging.WriteToJsonFile(_progressDataPath, _progress);
            //        }
            //    }
            //});
        }
    }
}
