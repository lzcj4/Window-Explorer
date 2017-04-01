using System;
using System.Globalization;
using System.Windows.Data;

namespace FileExplorer.Converter
{
    class BoolToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int result = 0;
            bool isTrue = false;
            if (value == null)
            {
                return result;
            }
            Boolean.TryParse(value.ToString(), out isTrue);
            if (!isTrue && parameter != null)
            {
                int.TryParse(parameter.ToString(), out result);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
