using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xkom
{
    class QuerryResult
    {

        public string NazwaProduktu { get; set; }
        public string Zdjecie { get; set; }
        public string Opis { get; set; }
        public double Cena { get; set; }
        public string Kategoria { get; set; }






        /*
         dr["Nazwa produktu"] = item.NazwaProduktu;
                        dr["Zdjecie"] = item.Zdjecie;
                        string Opis = item.Opis;
                        var opsTab = Opis.Split(',');
                        StringBuilder sb = new StringBuilder();
                        foreach (var tekst in opsTab)
                        {
                            sb.Append(tekst.Trim() + "\n");
                        }

                        dr["Opis"] = sb.ToString();
                        dr["Cena"] = item.CenaBrutto;
                        dr["Kategoria"] = item.Kategoria + " " + item.Podkategoria;*/
    }
}
