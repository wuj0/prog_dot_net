using System;
using System.Collections.Generic;
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

namespace Xkom
{
    /// <summary>
    /// Logika interakcji dla klasy ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private Xkom_ProjektEntities xkom = new Xkom_ProjektEntities();
        string name = MainWindow.NAME_OF_PRODUCT;
        int Id = 0;
        public static Dictionary<int, int> KOSZYK = new Dictionary<int, int>();
        int iloscProduktu = 0;
        int iloscKupowana = 1;
        public ProductWindow()
        {
            InitializeComponent();
            ReadProduct();
        }

        private void btnBuy_Click(object sender, RoutedEventArgs e)
        {
            if (iloscProduktu > 0 && iloscProduktu >= iloscKupowana)
            {
                try
                {
                    KOSZYK.Add(Id, iloscKupowana);
                    MainWindow.koszyk.Refresh();
                    foreach (var item in KOSZYK)
                    {
                        Console.WriteLine(item.ToString());
                    }
                }
                catch (ArgumentException)
                {
                    KOSZYK[Id] += iloscKupowana;
                    MainWindow.koszyk.Refresh();
                    
                }
            }

        }


        private void ReadProduct()
        {
            var threeTables = from p in xkom.produkt
                              join k in xkom.kategorie on p.kategorie_id equals k.id
                              join zdj in xkom.zdj_produktu on p.id equals zdj.produkt_id
                              join opis in xkom.produkt_opis on p.id equals opis.produkt_id
                              join ilosc in xkom.ilosc on p.id equals ilosc.produkt_id
                              join pk in xkom.podkategorie on k.id equals pk.kategorie_id
                              where p.Nazwa_produktu == name
                              select new
                              {
                                  Id = p.id,
                                  NazwaProduktu = p.Nazwa_produktu,
                                  Zdjecie = zdj.path_do_zdj,
                                  Opis = opis.opis,
                                  Kategoria = k.nazwa_kategorii,
                                  Ilosc = ilosc.ilosc1,
                                  Podkategoria = pk.nazwa_podkategorii
                              };
            Id = threeTables.FirstOrDefault().Id;
            iloscProduktu = threeTables.FirstOrDefault().Ilosc;
            txtBoxName.Text = threeTables.FirstOrDefault().NazwaProduktu;
            txtBoxDesc.Text = threeTables.FirstOrDefault().Opis;
            Uri url = new Uri(threeTables.FirstOrDefault().Zdjecie);
            if (url.IsFile)
            {
                try
                {
                    imageProduct.Source = new BitmapImage(url);
                }
                catch (NotSupportedException)
                {
                    MessageBox.Show("Brak zdjecia");
                }
            }
        }

        private void dodajButtton_Click(object sender, RoutedEventArgs e)
        {
            iloscKupowana++;
            numberOfPcs.Text = iloscKupowana.ToString();
            if (iloscKupowana > 1)
            {
                odejmijButton.Visibility = Visibility.Visible;
            }
        }

        private void odejmijButton_Click(object sender, RoutedEventArgs e)
        {
            iloscKupowana--;
            numberOfPcs.Text = iloscKupowana.ToString();
            if (iloscKupowana == 1)
            {
                odejmijButton.Visibility = Visibility.Hidden;
            }
        }
    }
}
