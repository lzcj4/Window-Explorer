using System;
using System.Globalization;
using System.Windows.Data;

namespace FileExplorer.Converter
{
    public class BooleanReserveConverter : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = !((bool)value);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
