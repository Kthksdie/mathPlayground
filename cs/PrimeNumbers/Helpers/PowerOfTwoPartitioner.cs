using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Helpers {
    /// <summary>
    /// Infinitely generate Power of Two BigIntegers.
    /// </summary>
    public class PowerOfTwoPartitioner : Partitioner<BigInteger> {
        public static BigInteger Sequence = new BigInteger(1);

        public PowerOfTwoPartitioner() {
            Sequence = new BigInteger(1);
        }

        public PowerOfTwoPartitioner(BigInteger sequence) {
            Sequence = sequence;
        }

        public override IList<IEnumerator<BigInteger>> GetPartitions(int partitionCount) {
            if (partitionCount < 1) {
                throw new ArgumentOutOfRangeException("partitionCount");
            }

            return (from i in Enumerable.Range(0, partitionCount)
                    select InfiniteEnumerator()).ToArray();
        }

        public override bool SupportsDynamicPartitions { get { return true; } }

        public override IEnumerable<BigInteger> GetDynamicPartitions() {
            return new InfiniteEnumerators();
        }

        private static IEnumerator<BigInteger> InfiniteEnumerator() {
            while (true) {
                Sequence += Sequence;

                yield return (Sequence - 1);
            }
        }

        private class InfiniteEnumerators : IEnumerable<BigInteger> {
            public IEnumerator<BigInteger> GetEnumerator() {
                return InfiniteEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }
    }
}
