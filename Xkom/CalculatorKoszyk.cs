using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xkom
{
    class CalculatorKoszyk
    {




        private CalculatorKoszyk() { }

        private static CalculatorKoszyk _instance;

        public static CalculatorKoszyk GetInstance()
        {
            if(_instance == null)
            {
                _instance = new CalculatorKoszyk();
            }
            return _instance;
        }



    }
}
