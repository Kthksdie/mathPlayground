using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Extensions;

namespace TestingNumbers.Components {
    public class CollatzConjecture {
        public BigInteger I = BigInteger.One;

        public BigInteger N = new BigInteger(1);

        public BigInteger R = BigInteger.Zero; // Result

        public BigInteger UpCount = BigInteger.Zero;
        public BigInteger DownCount = BigInteger.Zero;

        public CollatzConjecture() {

        }

        public IEnumerable<BigInteger> Try(BigInteger n) {
            I = BigInteger.One;

            n -= 1; // TEST

            N = n;
            R = n;

            UpCount = BigInteger.Zero;
            DownCount = BigInteger.Zero;

            while (true) {
                if (R % 2 == 0) {
                    R /= 2;

                    DownCount++;
                }
                else if (R > 0) {
                    R = (3 * R) + 1; // Positive Integers

                    UpCount++;
                }
                else {
                    R = (3 * R) - 1; // Negitive Integers

                    UpCount++;
                }

                I++;

                if (R == 1) {
                    break;
                }

                yield return R;
            }

            yield return 1;
        }


        private double _n = 1d;
        private double _result = 0d;

        private int _i = 1;
        private int _upCount = 0;
        private int _downCount = 0;

        public IEnumerable<double> Try_v2(BigInteger n) {
            var sqrt_2 = Math.Sqrt(2);
            var sqrt_3 = Math.Sqrt(3);

            _i = 1;
            _n = n.ToDouble();
            _result = n.ToDouble();

            _upCount = 0;
            _downCount = 0;

            while (true) {
                if (_result.WholeValue() % 2d == 0d) {
                    _result /= 2d;

                    _downCount++;
                }
                else if (_result > 0d) {
                    _result = (3d * _result) + 1d; // Positive Integers

                    _upCount++;
                }
                else {
                    _result = (3d * _result) - 1d; // Negitive Integers

                    _upCount++;
                }

                _i++;

                if (_result == 1d) {
                    break;
                }

                yield return _result;
            }

            yield return 1d;
        }
    }
}
