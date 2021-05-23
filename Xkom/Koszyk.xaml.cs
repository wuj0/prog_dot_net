using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
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
    /// Logika interakcji dla klasy Koszyk.xaml
    /// </summary>


    public partial class Koszyk : Window
    {
        private List<int> buyId = new List<int>();
        private List<int> priceId = new List<int>();

        private Dictionary<int, int> koszyk = ProductWindow.KOSZYK;
        private Xkom_ProjektEntities xkom = new Xkom_ProjektEntities();
        private ObservableCollection<KoszykResults> KoszykResults = new ObservableCollection<KoszykResults>();

        private Dictionary<int, double> wartoscNettoPairs = new Dictionary<int, double>();
        private Dictionary<int, double> wartoscBruttoPairs = new Dictionary<int, double>();

        private string mailKlienta = "";

        public Koszyk()
        {
            InitializeComponent();
            var date = DateTime.Now;
        }

        public void Refresh()
        {
            KoszykResults.Clear();
            AddToCart(koszyk);
        }

        private void AddToCart(Dictionary<int, int> keys)
        {
            var threeTables = from p in xkom.produkt
                              join c in xkom.cena on p.id equals c.produkt_id
                              join il in xkom.ilosc on p.id equals il.produkt_id
                              select new
                              {
                                  Cid = c.id,
                                  Id = p.id,
                                  NazwaProduktu = p.Nazwa_produktu,
                                  Netto = c.cena_netto,
                                  Brutto = c.cena_brutto,
                                  WartośćBrutto = c.Wartosc_netto,
                                  NaStanie = il.ilosc1
                              };

            foreach (var item in keys)
            {
                var brutto = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Brutto;
                brutto = Math.Round(brutto, 2, MidpointRounding.ToEven);
                var netto = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Netto;
                netto = Math.Round(netto, 2, MidpointRounding.ToEven);
                KoszykResults.Add(new KoszykResults
                {
                    NazwaProduktu = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().NazwaProduktu,
                    NettoPcs = netto,
                    Netto = netto * item.Value,
                    BruttoPcs = brutto,
                    Brutto = brutto * item.Value,
                    Pcs = item.Value

                });
                try
                {
                    wartoscBruttoPairs.Add(item.Key, Math.Round(brutto * item.Value, 2,
                                             MidpointRounding.ToEven));
                }
                catch (ArgumentException)
                {
                    wartoscBruttoPairs[item.Key] = Math.Round(brutto * item.Value, 2,
                                             MidpointRounding.ToEven);
                }
                try
                {
                    wartoscNettoPairs.Add(item.Key, Math.Round(netto * item.Value, 2,
                                             MidpointRounding.ToEven));
                }
                catch (ArgumentException)
                {
                    wartoscNettoPairs[item.Key] = Math.Round(netto * item.Value, 2,
                                             MidpointRounding.ToEven);
                }
                buyId.Add(threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Id);
                priceId.Add(threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Cid);
            }

            foreach (var item in wartoscBruttoPairs)
            {
                Console.WriteLine(item.Key.ToString() + "  " + item.Value.ToString());
            }

            Resources["KoszykResults"] = KoszykResults;
            TextNetto.Text = LiczNetto(wartoscNettoPairs).ToString();
            TextBrutto.Text = LiczBrutto(wartoscBruttoPairs).ToString();
        }
        private double LiczBrutto(Dictionary<int, double> keyValuePairs)
        {
            double result = 0;
            foreach (var item in keyValuePairs)
            {
                result += (double)item.Value;
            }

            return result;
        }

        private double LiczNetto(Dictionary<int, double> keyValuePairs)
        {
            double result = 0;
            foreach (var item in keyValuePairs)
            {
                result += (double)item.Value;
            }

            return result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                KoszykResults results = (KoszykResults)dGKoszyk.SelectedItem;
                string szukaj = results.NazwaProduktu;

                var bruttoRemove = results.Brutto;
                double actualBrutto = Convert.ToDouble(TextBrutto.Text);
                actualBrutto -= bruttoRemove;

                var nettoRemove = results.Netto;
                double actualNetto = Convert.ToDouble(TextNetto.Text);
                actualNetto -= nettoRemove;

                TextNetto.Text = actualNetto.ToString();
                TextBrutto.Text = actualBrutto.ToString();

                int removeProduct = xkom.produkt.Where(predicate: p => p.Nazwa_produktu == szukaj).FirstOrDefault().id;
                var cos = results.NazwaProduktu;
                KoszykResults.Remove(results);
                koszyk.Remove(removeProduct);
                Console.WriteLine(KoszykResults.Count.ToString());
                if (KoszykResults.Count < 1)
                {
                    TextNetto.Text = "0";
                    TextBrutto.Text = "0";
                    wartoscBruttoPairs.Clear();
                    wartoscNettoPairs.Clear();
                }
            }
            catch (InvalidCastException)
            {
                var message = MessageBox.Show("Brak produktów w koszyku.",
                                            "Confirmation",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Information);
                if (message == MessageBoxResult.OK)
                {

                }
            }
        }

        private void BuyBtn_Click(object sender, RoutedEventArgs e)
        {
            if (mailKlienta.Length > 1)
            {
                int numberOfLastOrder = xkom.zamowienia.Max(z => z.zamowienie_id) + 1;

                foreach (var item in koszyk)
                {
                    
                    xkom.dodawanie_zamowienia(item.Key, mailKlienta, item.Value, numberOfLastOrder);
                }
            }
            else
            {
                ErrorLog.Text = "Aby dokonać zakupu muszisz się zalogować!";
            }

        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!MainWindow.isClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void LoginBTN_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailTB.Text;
            var pswd = PswdTB.Password;

            Console.WriteLine(pswd);

            var result = xkom.sprawdzanie_konta(email, pswd).FirstOrDefault();

            if (result.Equals("Haslo do Konta jest poprawne!"))
            {
                mailKlienta = email;
                ErrorLog.Text = "Zostałeś zalogowany!";
                EmailTB.Clear();
                PswdTB.Clear();
            }
            else
            {
                ErrorLog.Text = "Niepoprawne dane logowania!";
            }

        }

        private void LogoutBTN_Click(object sender, RoutedEventArgs e)
        {
            mailKlienta = "";

            ErrorLog.Text = "Zostałeś wylogowany!";
        }
    }
}
