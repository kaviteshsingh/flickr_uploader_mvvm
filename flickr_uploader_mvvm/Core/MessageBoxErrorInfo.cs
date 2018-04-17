using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace flickr_uploader_mvvm.Core
{
    class MessageBoxErrorInfo :IErrorInfo
    {

        void IErrorInfo.ShowErrorMessage(string messsage)
        {
            MessageBox.Show(messsage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }


    }
}
