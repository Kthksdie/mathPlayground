using System.Numerics;

namespace TestingNumbers.Interfaces {
    public interface ISequence {
        BigInteger I { get; set; }

        BigInteger High { get; set; }

        BigInteger Low { get; set; }

        BigInteger Current { get; set; }

        void Reset();

        BigInteger Next();

        IEnumerable<BigInteger> All();
    }
}