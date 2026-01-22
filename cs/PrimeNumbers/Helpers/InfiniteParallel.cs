using PrimeNumbers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeNumbers.Helpers {
    public static class InfiniteParallel {
        public static readonly object ParallelLockPrimary = new object();
        public static readonly object ParallelLockSecondary = new object();

        private static int _currentIndex = 0;

        // Random generator (thread safe)
        public static readonly ThreadLocal<Random> s_Gen = new ThreadLocal<Random>(() => { return new Random(); });

        // Random generator (thread safe)
        public static Random Gen {
            get {
                return s_Gen.Value;
            }
        }

        public static void IdlePause(int waitDelay, int idleDuration) {
            // TODO: Refine? lol
            lock (ParallelLockPrimary) {
                while (SystemInternals.GetIdleTime() <= new TimeSpan(0, 0, idleDuration)) {
                    Task.Delay(new TimeSpan(0, 0, waitDelay));
                    continue;
                }
            }
        }

        public static void While(ParallelOptions parallelOptions, int sequence, Action<string, int, ParallelLoopState> action) {
            var condition = new Func<bool>(() => {
                return true;
            });

            Parallel.ForEach(new BinaryPartitioner(sequence), parallelOptions, (value, loopState) => {
                if (condition()) {
                    action(value, sequence, loopState);
                    sequence++;
                }
                else {
                    loopState.Stop();
                }
            });
        }

        public static void While(ParallelOptions parallelOptions, Progress progress, Action<BigInteger, int, ParallelLoopState> action) {
            _currentIndex = progress.Index;

            var condition = new Func<bool>(() => {
                return true;
            });

            var sequence = BigInteger.Parse(progress.Sequence);
            Parallel.ForEach(new PowerOfTwoPartitioner(sequence), parallelOptions, (value, loopState) => {
                if (condition()) {
                    action(value, _currentIndex, loopState);

                    _currentIndex++;
                }
                else {
                    loopState.Stop();
                }
            });
        }

        public static void While2(ParallelOptions parallelOptions, Progress progress, Action<BigInteger, int, ParallelLoopState> action) {
            _currentIndex = progress.Index;

            var condition = new Func<bool>(() => {
                return true;
            });

            var sequence = BigInteger.Parse(progress.Sequence);
            Parallel.ForEach(new BigIntegerPartitioner(sequence), parallelOptions, (value, loopState) => {
                if (condition()) {
                    action(value, _currentIndex, loopState);

                    _currentIndex++;
                }
                else {
                    loopState.Stop();
                }
            });
        }
    }
}
