using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xkom.DBXkom;

namespace Xkom
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Koszyk koszyk = new Koszyk();
        private Xkom_ProjektEntities xkom = new Xkom_ProjektEntities();
        private ObservableCollection<TestKategorie> TheList = new ObservableCollection<TestKategorie>();
        private List<string> checkedBox = new List<string>();
        public static string NAME_OF_PRODUCT;


        public MainWindow()
        {
            InitializeComponent();
            ShowCategorie();
            this.DataContext = this;
        }
        public void ShowCategorie()
        {
            using (var context = new Xkom_ProjektEntities())
            {
                var show = xkom.podkategorie.Join(xkom.kategorie,
                    pk => pk.kategorie_id,
                    kb => kb.id,
                    (pk, kb) => new
                    {
                        Id = kb.id,
                        Id1 = pk.id,
                        NazwaKategorie = kb.nazwa_kategorii,
                        NazwaPK = pk.nazwa_podkategorii
                    }).OrderBy(k => k.NazwaKategorie).ThenBy(pk => pk.NazwaPK);

                int count = 0;
                foreach (var item in show)
                {
                    TheList.Add(new TestKategorie
                    {
                        Id = count,
                        NazwaKategorii = item.NazwaKategorie.ToString() + " - " + item.NazwaPK.ToString()
                    });
                    count++;
                }
                Resources["TheList"] = TheList;
            }
        }
        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new Xkom_ProjektEntities())
            {
                var threeTables = from p in xkom.produkt
                                  join k in xkom.kategorie on p.kategorie_id equals k.id
                                  join pk in xkom.podkategorie on p.podkategorie_id equals pk.id
                                  join zdj in xkom.zdj_produktu on p.id equals zdj.produkt_id
                                  join opis in xkom.produkt_opis on p.id equals opis.produkt_id
                                  select new
                                  {
                                      NazwaProduktu = p.Nazwa_produktu,
                                      Zdjecie = zdj.path_do_zdj,
                                      Opis = opis.opis,
                                      Kategoria = k.nazwa_kategorii,
                                      Podkategoria = pk.nazwa_podkategorii
                                  };
                threeTables = threeTables.OrderBy(p => p.Kategoria).ThenBy(n => n.Podkategoria);


                if (!WyszukajTextBox.Text.Equals(""))
                {
                    string element = WyszukajTextBox.Text;
                    SearchByName(element);
                    
                }
                else if (checkedBox.Count > 0)
                {
                    ListOfCategories(checkedBox);
                }
                else
                {
                    DataTable dt = new DataTable();

                    dt.Columns.Add("Nazwa produktu");
                    dt.Columns.Add("Zdjecie");
                    dt.Columns.Add("Opis");
                    dt.Columns.Add("Kategoria");
                    foreach (var item in threeTables)
                    {
                        DataRow dr = dt.NewRow();

                        dr["Nazwa produktu"] = item.NazwaProduktu;
                        dr["Zdjecie"] = item.Zdjecie;
                        dr["Opis"] = item.Opis;
                        dr["Kategoria"] = item.Kategoria + " " + item.Podkategoria;

                        dt.Rows.Add(dr);
                    }
                    dg1.ItemsSource = dt.DefaultView;
                }


            }
        }

        private void SearchByName(string name)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Nazwa produktu");
            dt.Columns.Add("Zdjecie");
            dt.Columns.Add("Opis");
            dt.Columns.Add("Kategoria");
            if (checkedBox.Count > 0)
            {
                dt.Rows.Clear();
                List<string> listOfCategories = SeperateK(checkedBox);
                List<string> listOfPK = SeperatePK(checkedBox);

                var naKategorie = from p in xkom.produkt
                                  join k in xkom.kategorie on p.kategorie_id equals k.id
                                  join pk in xkom.podkategorie on p.podkategorie_id equals pk.id
                                  join zdj in xkom.zdj_produktu on p.id equals zdj.produkt_id
                                  join opis in xkom.produkt_opis on p.id equals opis.produkt_id
                                  where p.Nazwa_produktu.Contains(name) && listOfCategories.Contains(k.nazwa_kategorii) && listOfPK.Contains(pk.nazwa_podkategorii)
                                  select new
                                  {
                                      NazwaProduktu = p.Nazwa_produktu,
                                      Zdjecie = zdj.path_do_zdj,
                                      Opis = opis.opis,
                                      Kategoria = k.nazwa_kategorii,
                                      Podkategoria = pk.nazwa_podkategorii
                                  };
                foreach (var item in naKategorie)
                {
                    DataRow dr = dt.NewRow();

                    dr["Nazwa produktu"] = item.NazwaProduktu;
                    dr["Zdjecie"] = item.Zdjecie;
                    dr["Opis"] = item.Opis;
                    dr["Kategoria"] = item.Kategoria + " " + item.Podkategoria;
                    dt.Rows.Add(dr);
                }
                dg1.ItemsSource = dt.DefaultView;

            }
            else
            {
                dt.Rows.Clear();
                var threeTables = from p in xkom.produkt
                                  join k in xkom.kategorie on p.kategorie_id equals k.id
                                  join pk in xkom.podkategorie on p.podkategorie_id equals pk.id
                                  join zdj in xkom.zdj_produktu on p.id equals zdj.produkt_id
                                  join opis in xkom.produkt_opis on p.id equals opis.produkt_id
                                  where p.Nazwa_produktu.Contains(name)
                                  select new
                                  {
                                      NazwaProduktu = p.Nazwa_produktu,
                                      Zdjecie = zdj.path_do_zdj,
                                      Opis = opis.opis,
                                      Kategoria = k.nazwa_kategorii,
                                      Podkategoria = pk.nazwa_podkategorii
                                  };
                foreach (var item in threeTables)
                {


                    DataRow dr = dt.NewRow();

                    dr["Nazwa produktu"] = item.NazwaProduktu;
                    dr["Zdjecie"] = item.Zdjecie;
                    dr["Opis"] = item.Opis;
                    dr["Kategoria"] = item.Kategoria + " " + item.Podkategoria;

                    dt.Rows.Add(dr);
                }
                dg1.ItemsSource = dt.DefaultView;
            }


        }



        private void dg1_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Zdjecie")
            {
                FrameworkElementFactory image = new FrameworkElementFactory(typeof(Image));
                image.SetBinding(Image.SourceProperty, new Binding(e.PropertyName));

                e.Column = new DataGridTemplateColumn
                {
                    CellTemplate = new DataTemplate() { VisualTree = image },
                    Header = e.PropertyName
                };
            }
        }

        private void CheckBoxZone_Checked(object sender, RoutedEventArgs e)
        {
            var cos = e.OriginalSource.ToString().Substring(43, e.OriginalSource.ToString().Length - 43 - 15);
            checkedBox.Add(cos);
            string czytaj = " ";
            foreach (var item in checkedBox)
            {
                czytaj += item.ToString() + "\n";
            }

            textbox1.Text = czytaj;
        }
        private void CheckBoxZone_UnChecked(object sender, RoutedEventArgs e)
        {
            var cos = e.OriginalSource.ToString().Substring(43, e.OriginalSource.ToString().Length - 43 - 16);
            int index = checkedBox.IndexOf(cos);
            checkedBox.RemoveAt(index);
            string czytaj = " ";
            foreach (var item in checkedBox)
            {
                czytaj += item.ToString() + "\n";
            }

            textbox1.Text = czytaj;
        }

        public List<string> SeperateK(List<string> list)
        {
            List<string> result = new List<string>();

            foreach (var item in list)
            {
                string row;
                int index = item.IndexOf('-');
                row = item.Remove(index - 1, item.Length - index + 1);
                result.Add(row);
                Console.WriteLine(row);
            }

            return result;
        }

        public List<string> SeperatePK(List<string> list)
        {
            List<string> result = new List<string>();

            foreach (var item in list)
            {
                string row;
                int index = item.IndexOf('-');
                row = item.Remove(0, index + 2);
                result.Add(row);
                Console.WriteLine(row);
            }
            return result;
        }

        public void ListOfCategories(List<string> list)
        {
            List<string> listOfCategories = SeperateK(list);
            List<string> listOfPK = SeperatePK(list);

            var threeTables = from p in xkom.produkt
                              join k in xkom.kategorie on p.kategorie_id equals k.id
                              join pk in xkom.podkategorie on p.podkategorie_id equals pk.id
                              join zdj in xkom.zdj_produktu on p.id equals zdj.produkt_id
                              join opis in xkom.produkt_opis on p.id equals opis.produkt_id
                              where listOfCategories.Contains(k.nazwa_kategorii) && listOfPK.Contains(pk.nazwa_podkategorii)
                              select new
                              {
                                  NazwaProduktu = p.Nazwa_produktu,
                                  Zdjecie = zdj.path_do_zdj,
                                  Opis = opis.opis,
                                  Kategoria = k.nazwa_kategorii,
                                  Podkategoria = pk.nazwa_podkategorii
                              };

            DataTable dt = new DataTable();

            dt.Columns.Add("Nazwa produktu");
            dt.Columns.Add("Zdjecie");
            dt.Columns.Add("Opis");
            dt.Columns.Add("Kategoria");
            dt.Columns.Add("Pod kategoria");
            foreach (var item in threeTables)
            {
                DataRow dr = dt.NewRow();

                dr["Nazwa produktu"] = item.NazwaProduktu.ToString();
                dr["Zdjecie"] = item.Zdjecie.ToString();
                dr["Opis"] = item.Opis.ToString();
                dr["Kategoria"] = item.Kategoria.ToString() + " " + item.Podkategoria.ToString();

                dt.Rows.Add(dr);
            }

            dg1.ItemsSource = dt.DefaultView;
        }

        private void dg1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dg1.SelectedItem != null)
            {
                DataRowView dr = dg1.SelectedItem as DataRowView;
                DataRow dr1 = dr.Row;
                NAME_OF_PRODUCT = Convert.ToString(dr1.ItemArray[0]);
                ProductWindow pw = new ProductWindow();
                pw.Show();
            }
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                koszyk.Show();
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine("Tak");
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            koszyk.Close();
        }
    }
}
