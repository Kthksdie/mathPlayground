using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Extensions {
    public static class LinqExtensions {
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size) {
            T[] bucket = null;
            var count = 0;

            foreach (var item in source) {
                if (bucket == null) {
                    bucket = new T[size];
                }

                bucket[count++] = item;
                if (count != size) {
                    continue;
                }

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0) {
                yield return bucket.Take(count).ToArray();
            }
        }
    }
}
