using ExtendedNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Extensions;

namespace TestingNumbers.Components {
    public static class Constants {
        public static BigDecimal SqrtOf2;
        public static BigDecimal SqrtOf3;
        public static BigDecimal SqrtOf5;
        public static BigDecimal SqrtOf10;

        public static BigDecimal GoldenRatio;
        public static BigDecimal Φ;

        public static char SqrtSymbol = '√';

        public static char DigitalRtSymbol = 'ð';

        public static Dictionary<int, int> Digitas = new Dictionary<int, int>() {
            { 0, 1 },
            { 1, 2 },
            { 2, 1 },
            { 3, 4 },
            { 4, 3 },
            { 5, 2 },
            { 6, 1 },
            { 7, 2 },
            { 8, 1 },
            { 9, 2 },
        };

        public static Dictionary<int, int> Digitos = new Dictionary<int, int>() {
            { 0, 1 },
            { 1, 3 },
            { 2, 3 },
            { 3, 7 },
            { 4, 7 },
            { 5, 7 },
            { 6, 7 },
            { 7, 9 },
            { 8, 9 },
            { 9, 1 },
        };

        static Constants() {
            SqrtOf2 = new BigInteger(2).Sqrt();
            SqrtOf3 = new BigInteger(3).Sqrt();
            SqrtOf5 = new BigInteger(5).Sqrt();
            SqrtOf10 = new BigInteger(10).Sqrt();

            GoldenRatio = (1 + SqrtOf5) / 2;
            Φ = GoldenRatio;
        }
    }
}
