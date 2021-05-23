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
using Xkom.DBXkom;

namespace Xkom
{
    /// <summary>
    /// Logika interakcji dla klasy ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private Xkom_ProjektEntities xkom = new Xkom_ProjektEntities();
        //string name = MainWindow.NAME_OF_PRODUCT?.ToString();
        int Id = 0;
        public static Dictionary<int, int> KOSZYK = new Dictionary<int, int>();
        
        int iloscProduktu = 0;
        int iloscKupowana = 1;
        public ProductWindow()
        {
            InitializeComponent();
            //ReadProduct();
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
            else
            {
                MessageBox.Show("");
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
                              join cc in xkom.cena on p.id equals cc.produkt_id
                              where p.Nazwa_produktu == MainWindow.NAME_OF_PRODUCT
                              select new
                              {
                                  Id = p.id,
                                  NazwaProduktu = p.Nazwa_produktu,
                                  Zdjecie = zdj.path_do_zdj,
                                  Opis = opis.opis,
                                  Kategoria = k.nazwa_kategorii,
                                  Ilosc = ilosc.ilosc1,
                                  Podkategoria = pk.nazwa_podkategorii,
                                  Cena = cc.cena_brutto
                              };
            Id = threeTables.FirstOrDefault().Id;
            CenaTxtBox.Text = threeTables.FirstOrDefault().Cena.ToString();
            iloscProduktu = threeTables.FirstOrDefault().Ilosc;
            IloscTxtBox.Text = iloscProduktu.ToString();
            NameTB.Text = threeTables.FirstOrDefault().NazwaProduktu;
            DescTB.Text = threeTables.FirstOrDefault().Opis;
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

        private void AddButtton_Click(object sender, RoutedEventArgs e)
        {
            iloscKupowana++;
            numberOfPcs.Text = iloscKupowana.ToString();
            if (iloscKupowana > 1)
            {
                SubstractButton.Visibility = Visibility.Visible;
            }
        }

        private void SubstractButton_Click(object sender, RoutedEventArgs e)
        {
            iloscKupowana--;
            numberOfPcs.Text = iloscKupowana.ToString();
            if (iloscKupowana == 1)
            {
                SubstractButton.Visibility = Visibility.Hidden;
            }
        }

        private void ProductWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!MainWindow.isClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        public new void Show()
        {
            iloscKupowana = 1;

            ReadProduct();
            
            base.Show();
        }

        
    }
}
