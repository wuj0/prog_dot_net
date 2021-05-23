using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xkom.DBXkom;

namespace Xkom
{
    /// <summary>
    /// Logika interakcji dla klasy AddEditProduct.xaml
    /// </summary>
    public partial class AddEditProduct : Window
    {
        Xkom_ProjektEntities xkom = new Xkom_ProjektEntities();
        public AddEditProduct()
        {
            InitializeComponent();
        }

        private void test()
        {
            //xkom.dodawanie_produktow("sdasdasd", Convert.ToDecimal(123), "Akcesoria", "Zasilanie", 123, 200);
        }

        private string IsGood()
        {
            if (NameTBAdd.Text.Length > 8)
            {

                if (KodTBAdd.Text.Length == 6)
                {

                    if (KategoriaTBAdd.Text.Length > 3)
                    {

                        if (PodkategoriaTBAdd.Text.Length > 3)
                        {

                            if (CenaTBAdd.Text.Length > 0)
                            {

                                if (IloscTBAdd.Text.Length > 1)
                                {
                                    return "Ok";
                                }
                                else
                                {
                                    return "Muszisz podać ilość!";
                                }
                            }
                            else
                            {
                                return "Muszisz podać cene!";
                            }
                        }
                        else
                        {
                            return "Muszisz podać podkategorie!";
                        }
                    }
                    else
                    {
                        return "Muszisz podać kategorie!";
                    }
                }
                else
                {
                    return "Kod produktu musi zawierać dokładnie 6 znaków!";
                }
            }
            else
            {
                return "Naza produktu musi zawierać więcej niż 10 znaków!";
            }
        }

        private void KodTBAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            KodTBAdd.Text = IsNumber(KodTBAdd.Text);
        }

        private string IsNumber(string input)
        {
            Regex regex1 = new Regex("^[0-9]+$");

            if (regex1.IsMatch(input))
            {
                return input;
            }
            else
            {
                if (ErrorTB != null)
                {
                    ErrorTB.Text = "Akcpetowalne są tylko liczby!";
                }

                int cos1 = input.Length;
                string cos = "";
                try
                {
                    cos = input.Remove(cos1 - 1, 1);
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                return cos;
            }
        }

        private void CenaTBAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            CenaTBAdd.Text = IsNumber(CenaTBAdd.Text);
        }

        private void IloscTBAdd_TextChanged(object sender, TextChangedEventArgs e)
        {
            IloscTBAdd.Text = IsNumber(IloscTBAdd.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string result = IsGood();

            if (result.Equals("Ok"))
            {
                xkom.dodawanie_produktow(NameTBAdd.Text, 
                    Convert.ToDecimal(KodTBAdd.Text), 
                    KategoriaTBAdd.Text,
                    PodkategoriaTBAdd.Text,
                    Convert.ToInt32(IloscTBAdd.Text),
                    Convert.ToDouble(CenaTBAdd.Text));

                ErrorTB.Text = result;
            }
            else
            {
                ErrorTB.Text = result;
            }
        }


    }
}
