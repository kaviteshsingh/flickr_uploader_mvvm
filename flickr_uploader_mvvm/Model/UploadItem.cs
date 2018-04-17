using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Common.MVVM;



namespace flickr_uploader_mvvm.Model
{
    class UploadItem :INotifyBase
    {
        private FileInfo _fileDetail;

        public FileInfo FileDetail
        {
            get { return _fileDetail; }
            set { _fileDetail = value; base.OnPropertyChanged("FileDetail"); }
        }

        private string _photoid;

        public string PhotoID
        {
            get { return _photoid; }
            set { _photoid = value; base.OnPropertyChanged("PhotoID"); }
        }

        private string _result ;

        public string Result
        {
            get { return _result; }
            set { _result = value; base.OnPropertyChanged("Result"); }
        }

        private UploadStatus _status;

        public UploadStatus Status
        {
            get { return _status; }
            set { _status = value; base.OnPropertyChanged("Status"); }
        }
        
    }
}
