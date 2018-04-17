using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using flickr_uploader_mvvm.ViewModel;

namespace flickr_uploader_mvvm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                MainViewModel mvm = this.DataContext as MainViewModel;

                if(mvm != null && mvm.CmdSetFolderPath != null && mvm.CmdSetFolderPath.CanExecute(null))
                {
                    mvm.CmdSetFolderPath.Execute(dialog.SelectedPath);
                }

            }
        }

    }
}
