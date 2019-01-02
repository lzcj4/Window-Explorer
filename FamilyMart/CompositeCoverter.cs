using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace WpfApplication3
{
    public class CompositeCoverter : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object result = value;
            foreach (var item in this)
            {
                result = item.Convert(result, targetType, parameter, culture);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
