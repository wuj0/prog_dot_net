using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Xkom
{
    class CategoryView : INotifyPropertyChanged
    {
        private const string _agd = "AGD";
        private const string _akcesoria = "Akcesoria";
        private const string _akcesoriaDlaDzieci = "Akcesoria Dla Dzieci";
        private const string _akcesoriaFotograficzne = "Akcesoria Fotograficzne";
        private const string _akcesoriaKomputerowe = "Akcesoria Komputerowe";

        private ICommand _selectCategoriesCommand;
        private ICommand _deSelectCategoriesCommand;

        public CategoryView()
        {
            this.Categories = new ObservableCollection<Category>();
            this.Categories.CollectionChanged += Categories_CollectionChanged;

            _selectCategoriesCommand = new DelegateCommand<string>((categories) =>
            {
                SetIsSelectedProperty(categories, true);
            });
            _deSelectCategoriesCommand = new DelegateCommand<string>((categories) =>
            {
                SetIsSelectedProperty(categories, false);
            });

            //AGD
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Blendery Reczne" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Czajnik Elektryczny" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Dzbanek Filtrujacy" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Ekspres Do Kawy" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Lodówki" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Mikrofale" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Odkurzacz" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Okap" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Opiekacz Do Kanapek" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Piekarniki" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Plyta Ceramiczna" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Plyta Gazowa" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Plyta Indukcyjna" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Pralka" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Robot Planetarny" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Suszarka" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Zbiornik Na Wode" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Zmywarka" });
            this.Categories.Add(new Category { Kategoria = _agd, Podkategoria = "Zmywarki" });

            //Akcesoria
            this.Categories.Add(new Category { Kategoria = _akcesoria, Podkategoria = "Akcesoria" });
            this.Categories.Add(new Category { Kategoria = _akcesoria, Podkategoria = "Glosniki" });
            this.Categories.Add(new Category { Kategoria = _akcesoria, Podkategoria = "Srodki Czystosci" });

            //AkcesoriaDlaDzieci
            this.Categories.Add(new Category { Kategoria = _akcesoriaDlaDzieci, Podkategoria = "Fotelik Dla Dzieci" });
            this.Categories.Add(new Category { Kategoria = _akcesoriaDlaDzieci, Podkategoria = "Wózki" });

            //AkcesoriaKomputerowe
            this.Categories.Add(new Category { Kategoria = _akcesoriaKomputerowe, Podkategoria = "Akcesoria" });
        }


        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (object country in e.NewItems)
                {
                    (country as INotifyPropertyChanged).PropertyChanged
                        += new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }

            if (e.OldItems != null)
            {
                foreach (object country in e.OldItems)
                {
                    (country as INotifyPropertyChanged).PropertyChanged
                        -= new PropertyChangedEventHandler(item_PropertyChanged);
                }
            }
        }

        private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Countries");
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        public ObservableCollection<Category> Categories;

        public event PropertyChangedEventHandler PropertyChanged;

        private void SetIsSelectedProperty(string continentName, bool isSelected)
        {
            IEnumerable<Category> categoriesOnTheCurrentUnderCategories =
                    this.Categories.Where(c => c.Kategoria.Equals(continentName));

            foreach (Category category in categoriesOnTheCurrentUnderCategories)
            {
                INotifyPropertyChanged c = category as INotifyPropertyChanged;
                c.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
                category.IsSelected = isSelected;
                c.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
            }
        }

        public ICommand SelectCategoriesCommand
        {
            get { return _selectCategoriesCommand; }
        }

        public ICommand DeSelectCategoriesCommand
        {
            get { return _deSelectCategoriesCommand; }
        }
    }
}
