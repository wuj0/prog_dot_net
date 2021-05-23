using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
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
            var date = DateTime.Now;
            LiveTimeLabel.Content = date;
        }
        public void ShowCategorie() // Wyswietlanie kategori w expanderze Binding
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
            foreach (var item in show) // iteracja wyniku zapytania na tworzenie obiektów Bindingu
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
            var result = TheList.OrderBy(k => k.NazwaKategorii).ToList(); //Sortowanie od A do Z

            Resources["TheList"] = TheList;
        }
        int iloscProduktow = 0;
        int ileStron = 0;
        int ktoraStrona = 1;
        int iloscProduktowWczesniej = 0;
        private void ShowDB(int skip, int take) // Wyswietlenie produktów
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
            if (szukaj.Length > 2) // Jesli uzytkownik wpisze minimum 2 znaki do TextBoxa Wyszukiwania 
            {
                threeTables = threeTables.Where(p => p.NazwaProduktu.Contains(szukaj.ToLower()));
                iloscProduktow = threeTables.Count();
                if (iloscProduktowWczesniej < iloscProduktow || iloscProduktowWczesniej > iloscProduktow)
                {
                    skip = 0;
                    ktoraStrona = 1;
                }
                if (checkedBox.Count > 0) // jesli uzytkownik wybierze dodatkowo kategorie
                {
                    List<string> listOfCategories = SeperateK(checkedBox);
                    List<string> listOfPK = SeperatePK(checkedBox);
                    threeTables = threeTables.Where(p => listOfCategories.Contains(p.Kategoria) && listOfPK.Contains(p.Podkategoria)).OrderBy(p => p.NazwaProduktu);
                    iloscProduktow = threeTables.Count();
                    if (iloscProduktowWczesniej < iloscProduktow || iloscProduktowWczesniej > iloscProduktow)
                    {
                        skip = 0;
                        ktoraStrona = 1;
                    }
                }
            }
            else if (checkedBox.Count > 0) // jesli uzytwkonik wybierze kategorie
            {
                List<string> listOfCategories = SeperateK(checkedBox);
                List<string> listOfPK = SeperatePK(checkedBox);
                threeTables = threeTables.Where(p => listOfCategories.Contains(p.Kategoria) && listOfPK.Contains(p.Podkategoria)).OrderBy(p => p.NazwaProduktu);
                iloscProduktow = threeTables.Count();
                if (iloscProduktowWczesniej < iloscProduktow || iloscProduktowWczesniej > iloscProduktow)
                {
                    skip = 0;
                    ktoraStrona = 1;
                }
            }
            if (iloscProduktowWczesniej < iloscProduktow || iloscProduktowWczesniej > iloscProduktow)
            {
                skip = 0;
                ktoraStrona = 1;
            }

            threeTables = threeTables.OrderBy(p => p.Kategoria).ThenBy(n => n.Podkategoria);
            threeTables = threeTables.Skip(skip).Take(take);

            try
            {

                foreach (var item in threeTables) // iteracja wynikow zapytania do listy bindingu QuerryResults
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
            catch (ArgumentException) // jesli nastapie wyjatek zwracajacy ze nie ma produktow po wrzyceniu za duzego "skipa"
            {
                WarningTextBlock.Text = "Brak produktów";
            }
            iloscProduktowWczesniej = iloscProduktow;
            NumberOfProductsBox.Text = iloscProduktow.ToString();
            ileStron = iloscProduktow / 10;
            if (iloscProduktow % 10 != 0)
            {
                ileStron += 1;
            }
            NumberOfPagesBox.Text = ileStron.ToString();

            
            Resources["QuerryResults"] = QuerryResults;



            foreach (var item in TheList)
            {
                string nazwa = item.NazwaKategorii;
                Console.WriteLine(nazwa);
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e) // du usniecia dziala teraz dynamicznie
        {
            ShowDB(skip, take);
        }


        private void dg1_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) // podczas auto generowania kolumn i wrzucaniu wierszy konwertuje path do zdjecia na zdjecie
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
            //textbox1.Text = czytaj;
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
            //textbox1.Text = czytaj;
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
            if (skip < 10)
            {
                skip = 0;
                ShowDB(skip, take);
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
                szukaj = "";
                ShowDB(skip, take);
            }
        }

        private string isDigit()
        {
            //Regex regex = new Regex("^[0-9A-Za-z ]+$");
            Regex regex1 = new Regex("^[\\w ]+$");

            if (regex1.IsMatch(WyszukajTextBox.Text))
            {
                return WyszukajTextBox.Text;
            }
            else
            {
                WarningTextBlock.Text = "Akcpetowalne są tylko liczby i litery";
                int cos1 = WyszukajTextBox.Text.Length;
                string cos = "";
                try
                {
                    cos = WyszukajTextBox.Text.Remove(cos1 - 1, 1);
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                
                return WyszukajTextBox.Text = cos;
            }
        }

        private void AddWidnow_Click(object sender, RoutedEventArgs e)
        {
            AddEditProduct add = new AddEditProduct();
            add.Show();
        }

        //private void LoginBTN_Click(object sender, RoutedEventArgs e)
        //{
        //    var login = LoginTB.Text;
        //    var password = PswdTB.Text;

        //    var czyzalogowano = xkom.sprawdzanie_konta(login, password);

        //    DaneKlientaTB.Text = czyzalogowano.FirstOrDefault().ToString(); // logowanie 

        //}


    }
}

