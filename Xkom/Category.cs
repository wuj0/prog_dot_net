using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Xkom
{
    class Category : INotifyPropertyChanged
    {
        private string _kategoria;
    public string Kategoria
    {
        get { return _kategoria; }
        set { _kategoria = value; OnPropertyChanged(); }
    }
  
    private string _podkategoria;
    public string Podkategoria
    {
        get { return _podkategoria; }
        set { _podkategoria = value; OnPropertyChanged(); }
    }
  
    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        set { _isSelected = value; OnPropertyChanged(); }
    }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
