using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Xkom.DBXkom;

namespace Xkom
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool isClosing = false;
        public static Koszyk koszyk = new Koszyk();
        public ProductWindow pw = new ProductWindow();
        private Xkom_ProjektEntities xkom = new Xkom_ProjektEntities();
        private ObservableCollection<TestKategorie> TheList = new ObservableCollection<TestKategorie>();
        private ObservableCollection<QuerryResult> QuerryResults = new ObservableCollection<QuerryResult>();
        private Dictionary<int, TestKategorie> tescik = new Dictionary<int, TestKategorie>();
        private List<string> checkedBox = new List<string>();
        public static string NAME_OF_PRODUCT;
        private int take = 10;
        private int skip = 0;
        public MainWindow()
        {
            InitializeComponent();
            ShowCategorie();
            koszyk.Hide();
            ShowDB(skip, take);
        }
        public void ShowCategorie()
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
                    NazwaKategorii = item.NazwaKategorie.ToString() + " - " + item.NazwaPK.ToString(),
                    IsChecked = false
                });
                count++;
            }

            //var result = TheList.Where(p => p.NazwaKategorii.Contains("AGD")).ToList(); //DZIALA
            var result = TheList.OrderBy(k => k.NazwaKategorii).ToList(); //DZIALA

            Resources["TheList"] = TheList;
        }
        int iloscProduktow = 0;
        int ileStron = 0;
        int ktoraStrona = 1;
        int iloscProduktowWczesniej = 0;
        private void ShowDB(int skip, int take)
        {

            QuerryResults.Clear();
        
            var threeTables = from p in xkom.produkt
                              join k in xkom.kategorie on p.kategorie_id equals k.id
                              join pk in xkom.podkategorie on p.podkategorie_id equals pk.id
                              join zdj in xkom.zdj_produktu on p.id equals zdj.produkt_id
                              join c in xkom.cena on p.id equals c.produkt_id
                              join opis in xkom.produkt_opis on p.id equals opis.produkt_id
                              select new
                              {
                                  NazwaProduktu = p.Nazwa_produktu,
                                  Zdjecie = zdj.path_do_zdj,
                                  Opis = opis.opis,
                                  Kategoria = k.nazwa_kategorii,
                                  Podkategoria = pk.nazwa_podkategorii,
                                  CenaBrutto = c.cena_brutto
                              };
            iloscProduktow = threeTables.Count();
            if (szukaj.Length > 2)
            {
                threeTables = threeTables.Where(p => p.NazwaProduktu.Contains(szukaj.ToLower()));
                iloscProduktow = threeTables.Count();
                if (iloscProduktow < skip || iloscProduktowWczesniej > iloscProduktow)
                {
                    skip = 0;
                    ktoraStrona = 1;
                }
                if (checkedBox.Count > 0)
                {
                    List<string> listOfCategories = SeperateK(checkedBox);
                    List<string> listOfPK = SeperatePK(checkedBox);
                    threeTables = threeTables.Where(p => listOfCategories.Contains(p.Kategoria) && listOfPK.Contains(p.Podkategoria)).OrderBy(p => p.NazwaProduktu);
                    iloscProduktow = threeTables.Count();
                    if (iloscProduktow < skip || iloscProduktowWczesniej > iloscProduktow)
                    {
                        skip = 0;
                        ktoraStrona = 1;
                    }
                }
            }
            else if (checkedBox.Count > 0)
            {
                List<string> listOfCategories = SeperateK(checkedBox);
                List<string> listOfPK = SeperatePK(checkedBox);
                threeTables = threeTables.Where(p => listOfCategories.Contains(p.Kategoria) && listOfPK.Contains(p.Podkategoria)).OrderBy(p => p.NazwaProduktu);
                iloscProduktow = threeTables.Count();
                if (iloscProduktow < skip || iloscProduktowWczesniej > iloscProduktow)
                {
                    skip = 0;
                    ktoraStrona = 1;
                }
            }
            //var lista = threeTables.GroupBy(p => p.Kategoria + " - " + p.Podkategoria).ToList();
            //int count = 0;
            //var kategorie = threeTables.Where(p => p.NazwaProduktu != null).Select(p => p.Kategoria + " - " + p.Podkategoria).GroupBy(p => p);

            ////DO PRZEMYSLENIA

            //foreach (var item in kategorie)
            //{

            //    TheList.Add(new TestKategorie
            //    {
            //        Id = count,
            //        NazwaKategorii = item.Key.ToString()
            //    });
            //    count++;
            //}
            //Resources["TheList"] = TheList;
            threeTables = threeTables.OrderBy(p => p.Kategoria).ThenBy(n => n.Podkategoria);
            threeTables = threeTables.Skip(skip).Take(take);

            try
            {

                foreach (var item in threeTables)
                {
                    var zdjecie = new BitmapImage(new Uri(item.Zdjecie));
                    QuerryResults.Add(new QuerryResult
                    {
                        NazwaProduktu = item.NazwaProduktu,
                        Zdjecie = item.Zdjecie,
                        Opis = item.Opis,
                        Cena = item.CenaBrutto,
                        Kategoria = item.Kategoria + " - " + item.Podkategoria
                    });
                }
            }
            catch (ArgumentException)
            {
                WarningTextBlock.Text = "Brak produktów";
            }
            iloscProduktowWczesniej = iloscProduktow;
            ileProuktówLabel.Content = iloscProduktow.ToString();
            ileStron = iloscProduktow / 10;
            if (iloscProduktow % 10 != 0)
            {
                ileStron += 1;
            }
            ileStronLabel.Content = ileStron.ToString();

            KtoraStronaLabel.Content = ktoraStrona.ToString();
            Resources["QuerryResults"] = QuerryResults;

            

            foreach (var item in TheList)
            {
                string nazwa = item.NazwaKategorii;
                Console.WriteLine(nazwa);
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowDB(skip, take);
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
                    Header = e.PropertyName,
                    Width = 110

                };

                e.Column.DisplayIndex = 1;

            }

            if (!(e.PropertyName == "Zdjecie"))
            {
                e.Cancel = true;
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
            ShowDB(skip, take);
        }
        private void CheckBoxZone_UnChecked(object sender, RoutedEventArgs e)
        {
            var cos = e.OriginalSource.ToString().Substring(43, e.OriginalSource.ToString().Length - 43 - 16);
            int index = checkedBox.IndexOf(cos);
            checkedBox.RemoveAt(index);
            string czytaj = "";
            foreach (var item in checkedBox)
            {
                czytaj += item.ToString() + "\n";
            }
            textbox1.Text = czytaj;
            ShowDB(skip, take);
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

        public void ListOfCategories()
        {
            List<string> listOfCategories = SeperateK(checkedBox);
            List<string> listOfPK = SeperatePK(checkedBox);

            DataTable dt = new DataTable();
            dt.Columns.Add("Nazwa produktu");
            dt.Columns.Add("Zdjecie");
            dt.Columns.Add("Opis");
            dt.Columns.Add("Cena");
            dt.Columns.Add("Kategoria");

            var naKategorie = from p in xkom.produkt
                              join k in xkom.kategorie on p.kategorie_id equals k.id
                              join pk in xkom.podkategorie on p.podkategorie_id equals pk.id
                              join zdj in xkom.zdj_produktu on p.id equals zdj.produkt_id
                              join c in xkom.cena on p.id equals c.produkt_id
                              join opis in xkom.produkt_opis on p.id equals opis.produkt_id
                              where listOfCategories.Contains(k.nazwa_kategorii) && listOfPK.Contains(pk.nazwa_podkategorii)
                              select new
                              {
                                  NazwaProduktu = p.Nazwa_produktu,
                                  Zdjecie = zdj.path_do_zdj,
                                  Opis = opis.opis,
                                  Kategoria = k.nazwa_kategorii,
                                  Podkategoria = pk.nazwa_podkategorii,
                                  CenaBrutto = c.cena_brutto
                              };
            foreach (var item in naKategorie)
            {
                DataRow dr = dt.NewRow();

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
                dr["Kategoria"] = item.Kategoria + " " + item.Podkategoria;

                dt.Rows.Add(dr);
            }
            dg1.ItemsSource = dt.DefaultView;
        }

        private void dg1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dg1.SelectedItem != null)
            {
                QuerryResult querry = (QuerryResult)dg1.SelectedItem;
                NAME_OF_PRODUCT = querry.NazwaProduktu;
                
                pw.Show();
            }
        }

        private void btn1_Click(object sender, RoutedEventArgs e)
        {
            koszyk.Visibility = Visibility.Visible;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            isClosing = true;
            koszyk.Close();
            pw.Close();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Thickness margin = dg1.Margin;
            margin.Left = 365;
            dg1.Width -= 45;
            dg1.Margin = margin;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Thickness margin = dg1.Margin;
            margin.Left = 320;
            dg1.Width += 45;
            dg1.Margin = margin;
        }



        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!((skip / 10) == ileStron))
            {
                skip += 10;
                ktoraStrona++;
                ShowDB(skip, take);
            }
            else
            {
                WarningTextBlock.Text = "Jesteś na ostatniej stronie!";
                ShowDB(skip, take);
            }

            
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (skip == 0)
            {
                ShowDB(0, take);
                WarningTextBlock.Text = "Jesteś na pierwszej stronie!";
            }
            else
            {
                ktoraStrona--;
                skip -= 10;
                ShowDB(skip, take);

            }
        }
        string szukaj = "";

        private void WyszukajTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = isDigit();
            if (search.Length > 2)
            {
                szukaj = search;
                ShowDB(skip, take);
            }
            if (search.Length < 1)
            {
                checkedBox.Clear();
                szukaj = "";
                ShowDB(skip, take);
            }
        }

        private string isDigit()
        {
            Regex regex = new Regex("^[0-9A-Za-z ]+$");
            Regex regex1 = new Regex("^[\\w ]+$");

            if (regex1.IsMatch(WyszukajTextBox.Text))
            {
                return WyszukajTextBox.Text;
            }
            else
            {
                WarningTextBlock.Text = "Akcpetowalne są tylko liczby i litery";
                return WyszukajTextBox.Text = "";
            }
        }

        
    }
}

