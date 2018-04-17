using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace flickr_uploader_mvvm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App:Application
    {

        public  string consumer_key = "";

        public App() : base()
        {
            // obtain the keys from flickr
            string consumer_key = null;
            string consumer_secret = null;

            Application.Current.Properties["consumer_key"] = consumer_key;
            Application.Current.Properties["consumer_secret"] = consumer_secret;

            if(String.IsNullOrEmpty(consumer_key) || String.IsNullOrEmpty(consumer_secret))
            {
                MessageBox.Show("Please fill consumer_key and consumer_secret in App.xaml.cs", "ERROR", MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current.Shutdown();
            }

        }

    }
}
