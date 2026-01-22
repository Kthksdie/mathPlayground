using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TestingNumbers.Components {
    public struct Product {

        public BigInteger X { get; set; }

        public BigInteger Y { get; set; }

        public BigInteger Z { get; set; }

        public Product(BigInteger x, BigInteger y) {
            X = x;
            Y = y;
            Z = x * y;
        }

        public static List<Product> Table(int from, int to) {
            var results = new List<Product>();

            for (BigInteger x = from; x <= to; x++) {
                for (BigInteger y = x; y <= to; y++) {
                    results.Add(new Product(x < 10 ? 100 + x : x, y));
                }
            }

            //results = results.OrderBy(result => result.Z).ToList();

            return results;
        }
    }
}
