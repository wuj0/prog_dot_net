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

        DataTable dataTable = new DataTable();
        private Dictionary<int, int> koszyk = ProductWindow.KOSZYK;
        private Xkom_ProjektEntities xkom = new Xkom_ProjektEntities();
        double? wartoscNetto = 0.0;
        decimal wartoscBrutto = 0;
        public Koszyk()
        {
            //if (koszyk.Count != 0)
            //{
                InitializeComponent();
                //CreateTable();
            //}
            //else
            //{
            //    MessageBox.Show("Nie masz jeszcze produktów w koszyku.",
            //                                "Confirmation",
            //                                MessageBoxButton.OK,
            //                                MessageBoxImage.Information);
            //    this.Close();
            //}
                     
        }

        public void Refresh()
        {
            CreateTable();
        }

        private DataTable AddToCart(Dictionary<int, int> keys)
        {
            
            DataTable dt = new DataTable();

            dt.Columns.Add("Nazwa produktu");
            dt.Columns.Add("Netto pcs");
            dt.Columns.Add("Netto");
            dt.Columns.Add("Brutto pcs");
            dt.Columns.Add("Brutto");
            dt.Columns.Add("Wartość brutto pcs");
            dt.Columns.Add("Wartość brutto");
            dt.Columns.Add("Pcs");
            
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
                
                DataRow dr = dt.NewRow();
                dr["Nazwa produktu"] = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().NazwaProduktu;
                dr["Netto pcs"] = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Netto;
                dr["Netto"] = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Netto * item.Value;
                dr["Brutto pcs"] = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Brutto;
                dr["Brutto"] = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Brutto * item.Value;
                dr["Wartość brutto pcs"] = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().WartośćBrutto;
                dr["Wartość brutto"] = threeTables.Where(p => p.Id == item.Key).FirstOrDefault().WartośćBrutto * item.Value;
                dr["Pcs"] = keys.FirstOrDefault().Value;

                wartoscNetto += (threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Netto * item.Value);
                wartoscBrutto += (threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Brutto * item.Value);
                buyId.Add(threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Id);
                priceId.Add(threeTables.Where(p => p.Id == item.Key).FirstOrDefault().Cid);
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private void CreateTable()
        {
            
            dataTable = AddToCart(koszyk);
            
            dGKoszyk.ItemsSource = dataTable.DefaultView;

            TextNetto.Text = wartoscNetto.ToString();
            TextBrutto.Text = wartoscBrutto.ToString();
            

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {   DataRowView row = (DataRowView)dGKoszyk.SelectedItem;
                int removeProduct = xkom.produkt.Where(predicate: p => p.Nazwa_produktu == row.Row.Field<string>("Nazwa produktu")).FirstOrDefault().id;
                var cos = row.Row.Field<string>("Nazwa produktu");
                dataTable.Rows.Remove(row.Row);
                koszyk.Remove(removeProduct);
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
            var numberOfLastOrder = xkom.zamowienia.Max(z => z.zamowienie_id);


            int count = 0;
            foreach (var item in koszyk)
            {
                zamowienia ord = new zamowienia
                {
                    zamowienie_id = numberOfLastOrder + 1,
                    produkt_id = item.Key,
                    cena_id = priceId[count],
                    ilosc = item.Value,
                };
                count++;
                xkom.zamowienia.Add(ord);
            }
            xkom.SaveChanges();
        }
    }
}
