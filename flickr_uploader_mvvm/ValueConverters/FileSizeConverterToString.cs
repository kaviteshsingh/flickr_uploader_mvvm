using System;
using System.Windows.Data;
using System.Globalization;



namespace flickr_uploader_mvvm.ValueConverters
{
    [ValueConversion(typeof(UInt64), typeof(string))]
    class FileSizeConverterToString: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UInt64 val = System.Convert.ToUInt64(value);

            //if(val < 1024)
            //    return String.Format("{0:0000} B", val);

            if(val < (1024 * 1024))
                return String.Format("{0:0.00} KB", ((double)val) / 1024);


            return String.Format("{0:0.00} MB", ((double)val) / 1024 / 1024);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //UInt64 val = System.Convert.ToUInt64(value);

            return 0;
        }
    }
}

