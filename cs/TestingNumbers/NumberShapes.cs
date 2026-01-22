using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Extensions;

namespace TestingNumbers
{
    public class NumberShapes {
        public BigInteger i = BigInteger.One;

        public BigInteger Square = BigInteger.One;
        public BigInteger Cube = BigInteger.One;

        public BigInteger Low = BigInteger.One;
        public BigInteger High = BigInteger.One;

        public BigInteger N = BigInteger.One;

        public NumberShapes() {
            
        }

        public void Next() {
            i++;

            while (true) {
                if (Low.IsSquare()) {
                    Square = N;
                    break;
                }
            }

            while (true) {
                High++;

                if (High.IsCube()) {
                    Cube = High;
                    break;
                }
            }
        }
    }
}
