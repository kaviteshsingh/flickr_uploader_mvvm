using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using System.Threading;
using System.Windows;


using Common.MVVM;
using FileEnumeration;
using flickr_uploader_mvvm.Model;
using flickr_uploader_mvvm.Flickr;
using flickr_uploader_mvvm.Core;


namespace flickr_uploader_mvvm.ViewModel
{
    class MainViewModel:ViewModelBase
    {

        #region privates


        private FlickrManager _flickrManager;

        #endregion        


        #region Properties


        private bool _IsPinControlVisible;
        public bool IsPinControlVisible
        {
            get { return _IsPinControlVisible; }
            set { _IsPinControlVisible = value; OnPropertyChanged("IsPinControlVisible"); }
        }

        private bool _IsUserLogged;
        public bool IsUserLogged
        {
            get { return _IsUserLogged; }
            set { _IsUserLogged = value; OnPropertyChanged("IsUserLogged"); }
        }

        private bool _enableUpload;
        public bool EnableUpload
        {
            get { return _enableUpload; }
            set { _enableUpload = value; OnPropertyChanged("EnableUpload"); }
        }

        private bool _enableRetry;
        public bool EnableRetry
        {
            get { return _enableRetry; }
            set { _enableRetry = value; OnPropertyChanged("EnableRetry"); }
        }

        private User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set { _currentUser = value; OnPropertyChanged("CurrentUser"); }
        }
        
        private UploadItem _currentItem;
        public UploadItem CurrentItem
        {
            get { return _currentItem; }
            set { _currentItem = value; OnPropertyChanged("CurrentItem"); }
        }
        
        private ObservableCollection<UploadItem> _uploadList;
        public ObservableCollection<UploadItem> UploadList
        {
            get { return _uploadList; }
            set { _uploadList = value; OnPropertyChanged("UploadList"); }
        }

        private DirFileEnumeration _fileEnumerator;
        public DirFileEnumeration FileEnumerator
        {
            get { return _fileEnumerator; }
            set { _fileEnumerator = value; }
        }

        private ObservableCollection<Photoset> _albums;
        public ObservableCollection<Photoset> Albums
        {
            get { return _albums; }
            set { _albums = value; OnPropertyChanged("Albums"); }
        }

        private int _AlbumsSelectedIndex;
        public int AlbumsSelectedIndex
        {
            get { return _AlbumsSelectedIndex; }
            set { _AlbumsSelectedIndex = value; OnPropertyChanged("AlbumsSelectedIndex"); }
        }


        // this is the album where photos are uploaded.
        private Photoset _UploadAlbum;
        public Photoset UploadAlbum
        {
            get { return _UploadAlbum; }
            set { _UploadAlbum = value; OnPropertyChanged("UploadAlbum"); }
        }

        private string _newAlbumName;
        public string NewAlbumName
        {
            get { return _newAlbumName; }
            set { _newAlbumName = value; OnPropertyChanged("NewAlbumName"); }
        }

        private bool _isCreateAlbum;
        public bool IsCreateAlbum
        {
            get { return _isCreateAlbum; }
            set { _isCreateAlbum = value; OnPropertyChanged("IsCreateAlbum"); Console.WriteLine("_isCreateAlbum {0}.", _isCreateAlbum); }
        }

        private bool _isExistingAlbum;
        public bool IsExistingAlbum
        {
            get { return _isExistingAlbum; }
            set { _isExistingAlbum = value; OnPropertyChanged("IsExistingAlbum"); Console.WriteLine("_isExistingAlbum {0}.", _isExistingAlbum); }
        }

        private string _FolderPath;
        public string FolderPath
        {
            get { return _FolderPath; }
            set { _FolderPath = value; OnPropertyChanged("FolderPath"); }
        }

        private string _pin;
        public string Pin
        {
            get { return _pin; }
            set { _pin = value; OnPropertyChanged("Pin"); }
        }



        private bool _IsUploadInProgress;
        public bool IsUploadInProgress
        {
            get { return _IsUploadInProgress; }
            set { _IsUploadInProgress = value; OnPropertyChanged("IsUploadInProgress"); }
        }

        private int _TotalFilesUploaded;
        public int TotalFilesUploaded
        {
            get { return _TotalFilesUploaded; }
            set { _TotalFilesUploaded = value; OnPropertyChanged("TotalFilesUploaded"); }
        }




        #endregion


        #region Commands

        private ICommand _CmdLoginLogout;
        public ICommand CmdLoginLogout
        {
            get
            {
                if(_CmdLoginLogout == null)
                {
                    _CmdLoginLogout = new RelayCommand(
                        param => this.CmdLoginLogoutHandler(param)
                        );
                }
                System.Diagnostics.Debug.WriteLine("_CmdLoginLogout..");
                return _CmdLoginLogout;
            }
        }

        void CmdLoginLogoutHandler(object param)
        {
            if(IsUserLogged)
            {
                // perform logout
                RegistryOperation.DeleteRegistryKeyName(RegistryOperation.szAppRegPathOauth, RegistryOperation.szKeyNameAccessToken);
                RegistryOperation.DeleteRegistryKeyName(RegistryOperation.szAppRegPathOauth, RegistryOperation.szKeyNameAccessTokenSecret);

                _flickrManager.AccessSecret = null;
                _flickrManager.AccessToken = null;

                TestLogin();
                Albums.Clear();
            }
            else
            {
                //perform login
                _flickrManager.AccessSecret = null;
                _flickrManager.AccessToken = null;
                IsPinControlVisible = _flickrManager.FlickrRequestToken();

            }
        }


        private ICommand _CmdPinInput;
        public ICommand CmdPinInput
        {
            get
            {
                if(_CmdPinInput == null)
                {
                    _CmdPinInput = new RelayCommand(
                        param => this.CmdPinInputHandler(param)
                        );
                }
                System.Diagnostics.Debug.WriteLine("_CmdPinInput..");
                return _CmdPinInput;
            }
        }

        void CmdPinInputHandler(object param)
        {
            bool response = _flickrManager.FlickrAccessToken(Pin);

            if(response)
            {
                RegistryOperation.CreateBinaryKeyValue(RegistryOperation.szAppRegPathOauth, RegistryOperation.szKeyNameAccessToken, _flickrManager.AccessToken);
                RegistryOperation.CreateBinaryKeyValue(RegistryOperation.szAppRegPathOauth, RegistryOperation.szKeyNameAccessTokenSecret, _flickrManager.AccessSecret);

                IsPinControlVisible = false;

                TestLogin();
                GetAlbumList();
            }

            Pin = string.Empty;
        }


        private ICommand _CmdRefreshAlbumList;
        public ICommand CmdRefreshAlbumList
        {
            get
            {
                if(_CmdRefreshAlbumList == null)
                {
                    _CmdRefreshAlbumList = new RelayCommand(
                        param => this.CmdRefreshAlbumListHandler(param)
                        );
                }
                System.Diagnostics.Debug.WriteLine("CmdRefreshAlbumListHandler..");
                return _CmdRefreshAlbumList;
            }
        }

        void CmdRefreshAlbumListHandler(object param)
        {
            GetAlbumList();
        }


        private ICommand _CmdUploadAlbumList;
        public ICommand CmdUploadAlbumList
        {
            get
            {
                if(_CmdUploadAlbumList == null)
                {
                    _CmdUploadAlbumList = new RelayCommand(
                        param => this.CmdUploadAlbumListHandler(param)
                        );
                }
                System.Diagnostics.Debug.WriteLine("CmdUploadAlbumListHandler..");
                return _CmdUploadAlbumList;
            }
        }

        void CmdUploadAlbumListHandler(object param)
        {
            ThreadPool.QueueUserWorkItem(ThreadPoolWorkerUploadAlbumList, this);
        }

        private ICommand _CmdRetryAlbumList;
        public ICommand CmdRetryAlbumList
        {
            get
            {
                if(_CmdRetryAlbumList == null)
                {
                    _CmdRetryAlbumList = new RelayCommand(
                        param => this.CmdRetryAlbumListHandler(param)
                        );
                }
                System.Diagnostics.Debug.WriteLine("CmdRetryAlbumListHandler..");
                return _CmdRetryAlbumList;
            }
        }

        void CmdRetryAlbumListHandler(object param)
        {
            ThreadPool.QueueUserWorkItem(ThreadPoolWorkerRetryAlbumList, this);
        }


        private ICommand _CmdSetFolderPath;
        public ICommand CmdSetFolderPath
        {
            get
            {
                if(_CmdSetFolderPath == null)
                {
                    _CmdSetFolderPath = new RelayCommand(
                        param => this.CmdSetFolderPathHandler(param)
                        );
                }
                System.Diagnostics.Debug.WriteLine("_CmdSetFolderPath..");
                return _CmdSetFolderPath;
            }
        }

        void CmdSetFolderPathHandler(object param)
        {
            System.Diagnostics.Debug.WriteLine("CmdBeginScanHandler..");

            if(Directory.Exists(param as string))
            {
                FolderPath = param as string;
            }
            else
            {
                FolderPath = "Invalid folder path";
            }
            IsUploadInProgress = false;
            ThreadPool.QueueUserWorkItem(ThreadPoolWorkerFileEnumeration, this);
        }

        #endregion


        #region ThreadPoolHandlers

        void ThreadPoolWorkerFileEnumeration(object state)
        {
            System.Diagnostics.Debug.WriteLine("ThreadPoolWorkerFileEnumeration:: Start");

            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                UploadList.Clear();
            }));

            MainViewModel mvm = state as MainViewModel;
            _fileEnumerator.EnumerateFiles(mvm.FolderPath);

        }


        void ThreadPoolWorkerUploadAlbumList(object state)
        {
            System.Diagnostics.Debug.WriteLine("ThreadPoolWorkerUploadAlbumList:: Start");

            IsUploadInProgress = true;
            EnableUpload = false;

            if(IsCreateAlbum)
            {
                if(!String.IsNullOrWhiteSpace(NewAlbumName))
                {
                    UploadItemsAndCreateAlbum(NewAlbumName);
                }
                else
                {
                    EnableUpload = true;
                }
            }
            else
            {
                UploadAlbum = Albums[AlbumsSelectedIndex];
                UploadItemsToAlbum(UploadAlbum.id);
            }
            
            EnableRetry = false;
            foreach(var item in UploadList)
            {
                if(item.Status == UploadStatus.Failed)
                {
                    EnableRetry = true;
                    break;
                }
            }

            //if(EnableRetry)
            //    IsUploadInProgress = true;
            //else
                IsUploadInProgress = false;

        }


        void ThreadPoolWorkerRetryAlbumList(object state)
        {
            System.Diagnostics.Debug.WriteLine("ThreadPoolWorkerRetryUploadAlbumList:: Start");

            EnableUpload = false;
            EnableRetry = false;
            IsUploadInProgress = true;

            foreach(var item in UploadList)
            {
                if(item.Status == UploadStatus.Failed)
                {
                    item.Status = UploadStatus.Pending;
                    item.Result = "";
                }
            }

            if(UploadAlbum != null)
            {
                UploadItemsToAlbum(UploadAlbum.id);
            }
            else
            {
                ThreadPoolWorkerUploadAlbumList(this);
            }


            foreach(var item in UploadList)
            {
                if(item.Status == UploadStatus.Failed)
                {
                    EnableRetry = true;
                    break;
                }
            }

            //if(EnableRetry)
            //    IsUploadInProgress = true;
            //else
                IsUploadInProgress = false;
        }


        #endregion


        #region Photo/Videos Upload APIs

        void UploadItemsToAlbum(string AlbumId)
        {
            foreach(var item in UploadList)
            {
                if(item.Status == UploadStatus.Pending)
                {
                    item.Status = UploadStatus.Uploading;

                    PhotoUploadResponse uploadRespone = _flickrManager.POSTUploadOperation<PhotoUploadResponse>(item.FileDetail, null);

                    if(uploadRespone != null && uploadRespone.Photoid != null)
                    {
                        PhotosetsAddPhotoResponse addPhotoResponse =_flickrManager.AssignPhotoToAlbum(uploadRespone.Photoid, AlbumId);

                        if(addPhotoResponse.message == null)
                        {
                            // uploaded
                            item.Status = UploadStatus.Uploaded;
                            item.Result = addPhotoResponse.stat;
                            item.PhotoID = uploadRespone.Photoid;
                            SetPhotoPermissionResponse permResponse = _flickrManager.SetPhotoPermissions(item.PhotoID);

                            TotalFilesUploaded++;
                        }
                        else
                        {
                            // failed
                            item.Status = UploadStatus.Failed;
                            item.Result = String.Format("{0}::{1}", addPhotoResponse.code, addPhotoResponse.message);
                        }
                    }
                    else
                    {
                        item.Status = UploadStatus.Failed;
                        if(uploadRespone !=null && uploadRespone.Err != null)
                        {
                            item.Result = String.Format("{0}::{1}", uploadRespone.Err.Code, uploadRespone.Err.Msg);
                        }
                        else
                        {
                            item.Result = "WebRequest failed. Exception occured.";
                        }
                    }
                }
            }
        }


        void UploadItemsAndCreateAlbum(string AlbumName)
        {
            // upload one picture and then use the photoId to create album.
            // then use that albumId to upload remaining photos.

            UploadAlbum = null;

            UploadItem firstItem = null;

            foreach(var item in UploadList)
            {
                if(item.Status == UploadStatus.Pending)
                {
                    firstItem = item;
                    break;
                }
            }

            firstItem.Status = UploadStatus.Uploading;

            PhotoUploadResponse uploadRespone = _flickrManager.POSTUploadOperation<PhotoUploadResponse>(firstItem.FileDetail, null);

            if(uploadRespone != null && uploadRespone.Photoid != null)
            {
                // uploaded
                firstItem.Status = UploadStatus.Uploaded;
                firstItem.Result = uploadRespone.Stat;
                firstItem.PhotoID = uploadRespone.Photoid;
                TotalFilesUploaded++;

                SetPhotoPermissionResponse Permresponse = _flickrManager.SetPhotoPermissions(firstItem.PhotoID);

                PhotosetsCreateResponse createResponse = _flickrManager.CreateAlbumFromPhotoId(uploadRespone.Photoid, AlbumName);

                if(createResponse != null)
                {
                    if(createResponse.photoset != null)
                    {
                        string AlbumId = createResponse.photoset.id;

                        PhotosetsGetListResponse AlbumListResp = _flickrManager.GetAlbumList();

                        if(AlbumListResp.photosets != null)
                        {
                            foreach(var item in AlbumListResp.photosets.photoset)
                            {
                                if(0 == String.Compare(AlbumId, item.id, true))
                                {
                                    UploadAlbum = item;
                                    break;
                                }
                            }
                        }

                        UploadItemsToAlbum(createResponse.photoset.id);
                    }
                    else
                    {
                        firstItem.Result = createResponse.stat;
                    }
                }
                else
                {
                    // failed
                    firstItem.Result = "WebRequest failed. Exception occured.";
                }
            }
            else
            {
                firstItem.Status = UploadStatus.Failed;
                if(uploadRespone != null && uploadRespone.Err != null)
                {
                    firstItem.Result = String.Format("{0}::{1}", uploadRespone.Err.Code, uploadRespone.Err.Msg);
                }
                else
                {
                    firstItem.Result = "WebRequest failed. Exception occured.";
                }
            }
        }



        #endregion




        public MainViewModel()
        {
            DisplayName = "MainViewModel";
            IsUserLogged = false;
            IsPinControlVisible = false;
            IsUploadInProgress = false;
            _uploadList = new ObservableCollection<UploadItem>();
            _fileEnumerator = new DirFileEnumeration(this);
            _fileEnumerator.FileFound += _fileEnumerator_FileFound;
            _fileEnumerator.EnumerationComplete += _fileEnumerator_EnumerationComplete;
            Albums = new ObservableCollection<Photoset>();
            IsCreateAlbum = true;
            IsExistingAlbum = false;
            EnableRetry = false;
            EnableUpload = false;


            string consumerKey = (string)Application.Current.Properties["consumer_key"] ;
            string consumerSecret = (string)Application.Current.Properties["consumer_secret"] ;
            string accessToken = RegistryOperation.GetBinaryKeyValueInString(RegistryOperation.szAppRegPathOauth, RegistryOperation.szKeyNameAccessToken);
            string accessTokenSecret = RegistryOperation.GetBinaryKeyValueInString(RegistryOperation.szAppRegPathOauth, RegistryOperation.szKeyNameAccessTokenSecret);

            _flickrManager = new FlickrManager(consumerKey, consumerSecret, accessToken, accessTokenSecret);

            if(TestLogin())
            {
                GetAlbumList();
            }
        }

        private void _fileEnumerator_EnumerationComplete(object sender, EnumerationCompleteEventArgs e)
        {
            if(UploadList.Count > 0)
            {
                EnableUpload = true;
            }
            else
            {
                EnableUpload = false;
            }

            TotalFilesUploaded = 0;
            
            EnableRetry = false;
        }

        private void _fileEnumerator_FileFound(object sender, FileFoundEventArgs e)
        {

            // valid file extensions for photos and movies
            String[] fileExt = new String[]
            {".jpg", ".jpeg", ".jpe", ".bmp",".png", ".gif",".tif",".tiff", ".avi", ".wmv", ".mov", ".m2ts", ".ogg", ".ogv", ".mp4", ".m4p",
                ".m4v", ".mpg", ".mp2", ".mpeg", ".mpe", ".mpv", ".m2v", ".m4v" };

            bool IsValid = fileExt.Contains(e.fileInfo.Extension, StringComparer.OrdinalIgnoreCase);

            if(IsValid)
            {
                UploadItem item = new UploadItem();
                item.FileDetail = e.fileInfo;
                item.Status = UploadStatus.Pending;


                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    UploadList.Add(item);
                }));
            }

        }



        #region FlickerOperations

        bool TestLogin()
        {
            TestLoginResponse loginResponse = _flickrManager.TestLogin();

            if(loginResponse == null || loginResponse.user == null)
            {
                IsUserLogged = false;
                User BlankUser = new User();
                BlankUser.id = null;
                BlankUser.username = new Username();
                BlankUser.username._content = "DISCONNECTED";

                // instead of directly creating user in currentuser variable because
                // this breaks username update in view because username() doesnt have inotify in FlickrData.cs
                CurrentUser = BlankUser;

                return false;
            }
            else
            {
                IsUserLogged = true;
                CurrentUser = loginResponse.user;
                return true;
            }
        }

        bool GetAlbumList()
        {
            // get album list
            Albums.Clear();
            PhotosetsGetListResponse allist = _flickrManager.GetAlbumList();

            if(allist.photosets != null)
            {
                foreach(var item in allist.photosets.photoset)
                {
                    Albums.Add(item);
                }
                AlbumsSelectedIndex = 0;
            }

            return true;
        }

        #endregion
    }
}