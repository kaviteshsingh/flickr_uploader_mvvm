using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using flickr_uploader_mvvm.Model;

namespace flickr_uploader_mvvm.ValueConverters
{
    [ValueConversion(typeof(UploadStatus), typeof(string))]
    public class UploadStatusToString:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            UploadStatus ToConvert = (UploadStatus)value;

            switch(ToConvert)
            {
                case UploadStatus.Pending:
                    return "Pending";

                case UploadStatus.Uploading:
                    return "Uploading";

                case UploadStatus.Uploaded:
                    return "Uploaded";

                case UploadStatus.Failed:
                    return "Failed";

                case UploadStatus.Cancelled:
                    return "Cancelled";

                default:
                    return "No Mapping";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ToConvert = (string)value;

            if(ToConvert.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                return UploadStatus.Pending;


            if(ToConvert.Equals("Uploading", StringComparison.OrdinalIgnoreCase))
                return UploadStatus.Uploading;

            if(ToConvert.Equals("Uploaded", StringComparison.OrdinalIgnoreCase))
                return UploadStatus.Uploaded;

            if(ToConvert.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                return UploadStatus.Failed;

            if(ToConvert.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
                return UploadStatus.Cancelled;

            return UploadStatus.Pending;
        }
    }

}
