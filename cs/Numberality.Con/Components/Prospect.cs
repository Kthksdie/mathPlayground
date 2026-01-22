using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numberality.Con.Components {
    public struct Prospect {
        public int TargetDigits { get; set; }

        public Product Product { get; set; }

        public Prospect(int targetDigits, Product product) {
            TargetDigits = targetDigits;
            Product = product;
        }
    }
}
