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
    [ValueConversion(typeof(bool), typeof(System.Windows.Visibility))]
    public class BoolToVisibility:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ToConvert = (bool)value;

            if(ToConvert == true)
                return Visibility.Visible;
            else
                return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility ToConvert = (Visibility)value;

            switch(ToConvert)
            {
                case Visibility.Collapsed:
                    return false;

                case Visibility.Hidden:
                    return false;

                case Visibility.Visible:
                    return true;

                default:
                    return true;
            }
        }
    }

}
