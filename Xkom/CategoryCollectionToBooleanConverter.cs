using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Xkom
{
    class CategoryCollectionToBooleanConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<Category> categories = values[0] as IEnumerable<Category>;
            string categoryName = values[1] as string;
            if (categories != null && categoryName != null)
            {
                IEnumerable<Category> categoriesOnTheCurrentContinent
                    = categories.Where(c => c.Kategoria.Equals(categoryName));

                int selectedCategories = categoriesOnTheCurrentContinent
                    .Where(c => c.IsSelected)
                    .Count();

                if (selectedCategories.Equals(categoriesOnTheCurrentContinent.Count()))
                    return true;

                if (selectedCategories > 0)
                    return null;
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
