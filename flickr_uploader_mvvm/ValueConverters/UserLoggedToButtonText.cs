using System;
using System.Windows.Data;
using System.Globalization;



namespace flickr_uploader_mvvm.ValueConverters
{
    [ValueConversion(typeof(bool), typeof(string))]
    class UserLoggedToButtonText:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool ToConvert = (bool)value;

            if(ToConvert)
                return "Logout";
            else
                return "Login";


        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ToConvert = (string)value;

            if(ToConvert.Equals("Login", StringComparison.OrdinalIgnoreCase))
                return false;
            else
                return true;

        }
    }
}



