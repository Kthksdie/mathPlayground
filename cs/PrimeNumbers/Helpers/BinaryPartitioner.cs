using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Helpers {
    public class BinaryPartitioner : Partitioner<string> {
        public static int Sequence = 0;

        public BinaryPartitioner() {
            Sequence = 0;
        }

        public BinaryPartitioner(int sequence) {
            Sequence = sequence;
        }

        public override IList<IEnumerator<string>> GetPartitions(int partitionCount) {
            if (partitionCount < 1) {
                throw new ArgumentOutOfRangeException("partitionCount");
            }

            return (from i in Enumerable.Range(0, partitionCount)
                    select InfiniteEnumerator()).ToArray();
        }

        public override bool SupportsDynamicPartitions { get { return true; } }

        public override IEnumerable<string> GetDynamicPartitions() {
            return new InfiniteEnumerators();
        }

        private static IEnumerator<string> InfiniteEnumerator() {
            while (true) {
                Sequence++;

                yield return new string('1', Sequence);
            }
        }

        private class InfiniteEnumerators : IEnumerable<string> {
            public IEnumerator<string> GetEnumerator() {
                return InfiniteEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }
    }
}
