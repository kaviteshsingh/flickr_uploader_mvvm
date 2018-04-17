using System;
using System.Windows.Data;
using System.Globalization;





namespace flickr_uploader_mvvm.Model
{    
    public enum UploadStatus
    {
        Pending = 0,
        Uploading,
        Uploaded,
        Failed,
        Cancelled
    }
}
