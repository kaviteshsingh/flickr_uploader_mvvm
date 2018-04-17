using System;
using System.Windows.Data;
using System.Globalization;



namespace flickr_uploader_mvvm.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(System.Windows.Media.Brush))]
    class UserLoggedToTextColor:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ToConvert = (bool)value;

            if(ToConvert)
                return System.Windows.Media.Brushes.Blue;
            else
                return System.Windows.Media.Brushes.Red;


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            System.Windows.Media.SolidColorBrush ToConvert = (System.Windows.Media.SolidColorBrush)value;

            if(ToConvert == System.Windows.Media.Brushes.Red)
                return false;
            else
                return true;

        }
    }
}