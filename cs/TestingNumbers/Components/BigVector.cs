using ExtendedNumerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TestingNumbers.Extensions;

namespace TestingNumbers.Components {
    public class BigVector : IEquatable<BigVector> {

        public BigInteger X { get; set; } = BigInteger.Zero;
        public BigInteger Y { get; set; } = BigInteger.Zero;

        public BigVector() {
            X = BigInteger.Zero;
            Y = BigInteger.Zero;
        }

        public BigVector(BigInteger x, BigInteger y) {
            X = x;
            Y = y;
        }

        public BigVector(BigInteger n) {
            X = n;
            Y = n;
        }

        public override string ToString() {
            return $"(X: {X}, Y: {Y})";
        }

        public override int GetHashCode() {
            return X.GetHashCode() + Y.GetHashCode();
        }

        public override bool Equals(object? obj) {
            return obj == null ? false : Equals(obj as BigVector);
        }

        public bool Equals(BigVector? v) {
            return v != null && X == v.X && Y == v.Y;
        }

        public static bool operator ==(BigVector a, BigVector b) {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(BigVector a, BigVector b) {
            return a.X != b.X || a.Y != b.Y;
        }

        public static BigVector operator +(BigVector a, BigVector b) {
            return Add(a, b);
        }

        public static BigVector operator -(BigVector a, BigVector b) {
            return Subtract(a, b);
        }

        public static BigVector operator *(BigVector a, BigVector b) {
            return Multiply(a, b);
        }

        public static BigVector operator /(BigVector a, BigVector b) {
            return Divide(a, b);
        }

        public static BigVector operator %(BigVector a, BigVector b) {
            return Mod(a, b);
        }

        public static BigVector Add(BigVector a, BigVector b) {
            return new BigVector(a.X + b.X, a.Y + b.Y);
        }

        public static BigVector Subtract(BigVector a, BigVector b) {
            return new BigVector(a.X - b.X, a.Y - b.Y);
        }

        public static BigVector Multiply(BigVector a, BigVector b) {
            return new BigVector(a.X * b.X, a.Y * b.Y);
        }

        public static BigVector Divide(BigVector a, BigVector b) {
            return new BigVector(a.X / b.X, a.Y / b.Y);
        }

        public static BigVector Mod(BigVector a, BigVector b) {
            return new BigVector(a.X % b.X, a.Y % b.Y);
        }

        // no...
        public static BigVector Intersect(BigVector a, BigVector b) {
            // https://paulbourke.net/geometry/pointlineplane/javascript.txt

            // Start?
            var x1 = a.X;
            var y1 = a.Y;
            // Finish?
            var x2 = b.X;
            var y2 = b.Y;

            // Start?
            var x3 = a.X;
            var y3 = a.Y;
            // Finish?
            var x4 = b.X;
            var y4 = b.Y;

            // Check if none of the lines are of length 0
            //if ((x1 == x2 && y1 == y2) || (x3 == x4 && y3 == y4)) {
            //    return new BigVector();
            //}

            var yx = (y4 - y3) * (x2 - x1);
            var xy = (x4 - x3) * (y2 - y1);
            var denominator = (yx - xy);

            Program.Print($"yx {yx}");
            Program.Print($"xy {xy}");
            Program.Print($"de {denominator}");

            //var denominator = ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1)); // Original

            // Lines are parallel
            if (denominator == 0) {
                return new BigVector();
            }

            var ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / denominator;
            var ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / denominator;

            Program.Print($"ua {ua}");
            Program.Print($"ub {ub}");

            // is the intersection along the segments
            if (ua < 0 || ua > 1 || ub < 0 || ub > 1) {
                return new BigVector();
            }

            // Return a object with the x and y coordinates of the intersection
            var x = x1 + ua * (x2 - x1);
            var y = y1 + ua * (y2 - y1);

            return new BigVector(x, y);
        }

        public static BigInteger Distance(BigVector a, BigVector b) {
            return ((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y)).IntegerSqrt();
        }

        public BigInteger Magnitude() {
            return ((X * X) + (Y * Y)).IntegerSqrt();
        }
    }
}
