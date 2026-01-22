using Numberality.Con.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Components {
    public struct Xydous {
        private BigInteger _x { get; set; } = BigInteger.Zero;

        private BigInteger _n { get; set; } = BigInteger.Zero;

        private bool _pality { get; set; } = false;

        public BigInteger X { get { return _x; } }

        public BigInteger N { get { return _n; } }

        public bool Pality { get { return _pality; } }

        public Xydous() {
            // 
        }

        public bool Step() {
            _n++;
            _pality = _n.IsProbablyPrime();

            if (_pality) {
                _x++;
            }

            return _pality;
        }

        public void Advance() {
            while (!this.Step()) {
                // Do Nothing
            }
        }
    }
}
