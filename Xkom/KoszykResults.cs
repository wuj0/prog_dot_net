using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xkom
{
    class KoszykResults
    {
        public string NazwaProduktu { get; set; }
        public double? NettoPcs { get; set; }
        public double? Netto { get; set; }
        public decimal BruttoPcs { get; set; }
        public decimal Brutto { get; set; }
        public int Pcs { get; set; }

        public override bool Equals(object obj)
        {
            return obj is KoszykResults results &&
                   NazwaProduktu == results.NazwaProduktu &&
                   NettoPcs == results.NettoPcs &&
                   Netto == results.Netto &&
                   BruttoPcs == results.BruttoPcs &&
                   Brutto == results.Brutto;
        }

        public override int GetHashCode()
        {
            int hashCode = 264909473;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NazwaProduktu);
            hashCode = hashCode * -1521134295 + NettoPcs.GetHashCode();
            hashCode = hashCode * -1521134295 + Netto.GetHashCode();
            hashCode = hashCode * -1521134295 + BruttoPcs.GetHashCode();
            hashCode = hashCode * -1521134295 + Brutto.GetHashCode();
            hashCode = hashCode * -1521134295 + Pcs.GetHashCode();
            return hashCode;
        }
    }
}
