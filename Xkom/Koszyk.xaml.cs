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
        
        public Koszyk()
        {
            InitializeComponent();
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
                var netto = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Netto;
                KoszykResults.Add(new KoszykResults
                {
                    NazwaProduktu = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().NazwaProduktu,
                    NettoPcs = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Netto,
                    Netto = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Netto * item.Value,
                    BruttoPcs = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Brutto,
                    Brutto = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Brutto * item.Value,
                    Pcs = item.Value

                });
                try
                {
                    wartoscBruttoPairs.Add(item.Key, brutto * item.Value);
                }
                catch (ArgumentException)
                {
                    wartoscBruttoPairs[item.Key] = brutto * item.Value;
                }
                try
                {
                    wartoscNettoPairs.Add(item.Key, netto * item.Value);
                }
                catch (ArgumentException)
                {
                    wartoscNettoPairs[item.Key] = netto * item.Value;
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
            int numberOfLastOrder = 0;
            try
            {
                numberOfLastOrder = xkom.zamowienia.Max(z => z.zamowienie_id);
            }
            catch (InvalidOperationException)
            {
                numberOfLastOrder = 0;
            }

            int count = 0;
            foreach (var item in koszyk)
            {
                zamowienia ord = new zamowienia
                {
                    zamowienie_id = numberOfLastOrder + 1,
                    produkt_id = item.Key,
                    cena_id = priceId[count],
                    ilosc = item.Value
                };
                count++;
                xkom.zamowienia.Add(ord);
            }
            xkom.SaveChanges();
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!MainWindow.isClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }
    }
}
