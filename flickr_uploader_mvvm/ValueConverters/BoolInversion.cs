using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Data;



namespace flickr_uploader_mvvm.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolInversion:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ToConvert = (bool)value;

            return !ToConvert;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ToConvert = (bool)value;

            return !ToConvert;
        }
    }
}
