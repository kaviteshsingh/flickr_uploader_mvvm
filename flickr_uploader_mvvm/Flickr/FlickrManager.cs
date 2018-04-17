using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;


namespace flickr_uploader_mvvm.Flickr
{
    class FlickrManager:RestOauthManager
    {
        string _reqUri = "https://api.flickr.com/services/rest";
        string _uploadUri = "https://up.flickr.com/services/upload";

        Dictionary<string,string> _methods = new Dictionary<string, string>()
        {
            {"TestLogin","flickr.test.login" },
            { "GetAlbumList","flickr.photosets.getList"},
            {"AddPhotoToAlbum","flickr.photosets.addPhoto" },
            {"CreateAlbum", "flickr.photosets.create" },
            {"SetPhotoPermission","flickr.photos.setPerms" }
        };

        public FlickrManager() : base()
        {

        }

        public FlickrManager(string consumerKey, string consumerSecret) : base(consumerKey, consumerSecret)
        {

        }

        public FlickrManager(string consumerKey,
               string consumerSecret,
               string token,
               string tokenSecret) : base(consumerKey, consumerSecret, token, tokenSecret)
        {

        }


        public bool FlickrRequestToken()
        {
            bool requestToken = AcquireRequestToken("https://www.flickr.com/services/oauth/request_token", "POST");

            if(requestToken)
            {
                //var url = "https://www.flickr.com/services/oauth/authorize?oauth_token=" + AccessToken +"&perms=delete";
                var url = "https://www.flickr.com/services/oauth/authorize?oauth_token=" + AccessToken ;
                System.Diagnostics.Process.Start(url);
                return true;
            }
            else
                return false;
        }

        public bool FlickrAccessToken(string Pin)
        {
            if(!(String.IsNullOrEmpty(Pin) || String.IsNullOrWhiteSpace(Pin)))
            {
                //return AcquireAccessToken("https://www.flickr.com/services/oauth/access_token"+"&perms=write", "POST", Pin);

                return AcquireAccessToken("https://www.flickr.com/services/oauth/access_token", "POST", Pin);
            }
            else
                return false;
        }


        public TestLoginResponse TestLogin()
        {
            string method = _methods["TestLogin"];
            var queryParam = new Dictionary<string, string>();
            queryParam.Add("nojsoncallback", "1");
            queryParam.Add("method", method);
            queryParam.Add("format", "json");

            try
            {
                TestLoginResponse response = GETOperation<TestLoginResponse>(_reqUri, queryParam);
                return response;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public PhotosetsGetListResponse GetAlbumList()
        {
            string method = _methods["GetAlbumList"];
            var queryParam = new Dictionary<string, string>();
            queryParam.Add("nojsoncallback", "1");
            queryParam.Add("method", method);
            queryParam.Add("format", "json");

            try
            {
                PhotosetsGetListResponse response = GETOperation<PhotosetsGetListResponse>(_reqUri, queryParam);
                return response;
            }
            catch(Exception)
            {
                return null;
            }
        }


        //public ResponseType POSTUploadOperation<ResponseType>(string requestUri, string filePath, Dictionary<string, string> AdditionalParams)
        //{
        //    if(AdditionalParams == null)
        //    {
        //        AdditionalParams = new Dictionary<string, string>();
        //    }

        //    //AdditionalParams.Add("oauth_token", AccessToken);
        //    //AdditionalParams.Add("title", "kavitesh_title");
        //    //AdditionalParams.Add("description", "test_pic_description");
        //    //AdditionalParams.Add("is_public", "0");
        //    //AdditionalParams.Add("is_friend", "0");
        //    //AdditionalParams.Add("is_family", "0");
        //    //AdditionalParams.Add("content_type", "1");
        //    //AdditionalParams.Add("hidden", "2");


        //    var authzHeader = GetAuthorizationHeader(requestUri, "POST", AdditionalParams, null);

        //    try
        //    {
        //        // prepare the token request
        //        var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(requestUri);
        //        request.ContentLength = 0;
        //        request.Headers.Add("Authorization", authzHeader);
        //        request.Method = "POST";
        //        //request.ContentType = "image/jpeg";
        //        request.Timeout = 1000 * 60;
        //        request.KeepAlive = false;

        //        string boundaryString = String.Format("FLICKR_MIME_{0:N}", Guid.NewGuid());
        //        string fileUrl = @"C:\Users\kavitesh\Downloads\Test_flickr.jpg";
        //        request.ContentType = "multipart/form-data; boundary=" + boundaryString;
        //        request.KeepAlive = false;
        //        request.SendChunked = true;
        //        //request.AllowWriteStreamBuffering = false;


        //        MemoryStream postDataStream = new MemoryStream();
        //        StreamWriter postDataWriter = new StreamWriter(postDataStream);



        //        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
        //        postDataWriter.Write("Content-Disposition: form-data; name=\"" + "title" + "\"\r\n");
        //        postDataWriter.Write("\r\n");
        //        postDataWriter.Write("kavitesh_title" + "\r\n");

        //        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
        //        postDataWriter.Write("Content-Disposition: form-data; name=\"" + "description" + "\"\r\n");
        //        postDataWriter.Write("\r\n");
        //        postDataWriter.Write("this_is_some_description" + "\r\n");

        //        // Include value from the myFileDescription text area in the post data
        //        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");

        //        postDataWriter.Write("Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\n",
        //                "photo",
        //                @"singh_singh.jpg");

        //        // the double end is necessary else throws "5: Filetype was not recognised" 
        //        postDataWriter.Write("Content-Type: image/jpg\r\n\r\n");


        //        postDataWriter.Flush();

        //        // Read the file
        //        FileStream fileStream = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
        //        byte[] buffer = new byte[409600];
        //        int bytesRead = 0;
        //        while((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        //        {
        //            //Console.WriteLine("REQ:: Read {0} bytes.", bytesRead);
        //            postDataStream.Write(buffer, 0, bytesRead);
        //        }
        //        fileStream.Close();

        //        postDataWriter.Write("\r\n--" + boundaryString + "--\r\n");
        //        postDataWriter.Flush();

        //        // Set the http request body content length
        //        request.ContentLength = postDataStream.Length;

        //        // Dump the post data from the memory stream to the request stream
        //        using(Stream s = request.GetRequestStream())
        //        {
        //            postDataStream.WriteTo(s);
        //        }
        //        postDataStream.Close();


        //        using(var response = (System.Net.HttpWebResponse)request.GetResponse())
        //        {
        //            using(Stream stream = response.GetResponseStream())
        //            {
        //                StreamReader reader = new StreamReader(stream, Encoding.UTF8);


        //                XmlSerializer x = new XmlSerializer(typeof(ResponseType));
        //                ResponseType uploadResponse = (ResponseType)x.Deserialize(reader);

        //                return uploadResponse;
        //            }
        //        }
        //    }
        //    catch(Exception ex1)
        //    {
        //        Console.WriteLine("Exception: {0}", ex1.ToString());
        //        return default(ResponseType);
        //    }

        //}

        //public ResponseType POSTUploadOperationVideo<ResponseType>(string requestUri, string filePath, Dictionary<string, string> AdditionalParams)
        //{
        //    if(AdditionalParams == null)
        //    {
        //        AdditionalParams = new Dictionary<string, string>();
        //    }

        //    //AdditionalParams.Add("oauth_token", AccessToken);
        //    //AdditionalParams.Add("title", "kavitesh_title");
        //    //AdditionalParams.Add("description", "test_pic_description");
        //    //AdditionalParams.Add("is_public", "0");
        //    //AdditionalParams.Add("is_friend", "0");
        //    //AdditionalParams.Add("is_family", "0");
        //    //AdditionalParams.Add("content_type", "1");
        //    //AdditionalParams.Add("hidden", "2");


        //    var authzHeader = GetAuthorizationHeader(requestUri, "POST", AdditionalParams, null);

        //    try
        //    {
        //        // prepare the token request
        //        var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(requestUri);
        //        request.ContentLength = 0;
        //        request.Headers.Add("Authorization", authzHeader);
        //        request.Method = "POST";
        //        //request.ContentType = "image/jpeg";
        //        request.Timeout = 1000 * 60;
        //        request.KeepAlive = false;

        //        string boundaryString = String.Format("FLICKR_MIME_{0:N}", Guid.NewGuid());
        //        string fileUrl = @"C:\Users\kavitesh\Downloads\CreateACSharpConsoleApp\CreateACSharpConsoleApp.wmv";
        //        request.ContentType = "multipart/form-data; boundary=" + boundaryString;
        //        request.KeepAlive = false;
        //        request.SendChunked = true;
        //        //request.AllowWriteStreamBuffering = false;


        //        MemoryStream postDataStream = new MemoryStream();
        //        StreamWriter postDataWriter = new StreamWriter(postDataStream);


        //        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
        //        postDataWriter.Write("Content-Disposition: form-data; name=\"" + "title" + "\"\r\n");
        //        postDataWriter.Write("\r\n");
        //        postDataWriter.Write("kavitesh_title" + "\r\n");

        //        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
        //        postDataWriter.Write("Content-Disposition: form-data; name=\"" + "description" + "\"\r\n");
        //        postDataWriter.Write("\r\n");
        //        postDataWriter.Write("this_is_some_description" + "\r\n");

        //        // Include value from the myFileDescription text area in the post data
        //        postDataWriter.Write("\r\n--" + boundaryString + "\r\n");

        //        postDataWriter.Write("Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\n",
        //                "photo",
        //                @"CreateACSharpConsoleApp.wmv");

        //        // the double end is necessary else throws "5: Filetype was not recognised" 
        //        postDataWriter.Write("Content-Type: video/wmv\r\n\r\n");


        //        postDataWriter.Flush();

        //        // Read the file
        //        FileStream fileStream = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
        //        byte[] buffer = new byte[409600];
        //        int bytesRead = 0;
        //        while((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
        //        {
        //            //Console.WriteLine("REQ:: Read {0} bytes.", bytesRead);
        //            postDataStream.Write(buffer, 0, bytesRead);
        //        }
        //        fileStream.Close();

        //        postDataWriter.Write("\r\n--" + boundaryString + "--\r\n");
        //        postDataWriter.Flush();

        //        // Set the http request body content length
        //        request.ContentLength = postDataStream.Length;

        //        // Dump the post data from the memory stream to the request stream
        //        using(Stream s = request.GetRequestStream())
        //        {
        //            postDataStream.WriteTo(s);
        //        }
        //        postDataStream.Close();


        //        using(var response = (System.Net.HttpWebResponse)request.GetResponse())
        //        {
        //            using(Stream stream = response.GetResponseStream())
        //            {
        //                StreamReader reader = new StreamReader(stream, Encoding.UTF8);


        //                XmlSerializer x = new XmlSerializer(typeof(ResponseType));
        //                ResponseType uploadResponse = (ResponseType)x.Deserialize(reader);

        //                return uploadResponse;
        //            }
        //        }
        //    }
        //    catch(Exception ex1)
        //    {
        //        Console.WriteLine("Exception: {0}", ex1.ToString());
        //        return default(ResponseType);
        //    }

        //}


        public PhotosetsAddPhotoResponse AssignPhotoToAlbum(string PhotoId, string AlbumId)
        {
            // PhotosetsAddPhotoResponse
            string method = _methods["AddPhotoToAlbum"];
            var queryParam = new Dictionary<string, string>();
            queryParam.Add("nojsoncallback", "1");
            queryParam.Add("method", method);
            queryParam.Add("format", "json");
            queryParam.Add("photo_id", PhotoId);
            queryParam.Add("photoset_id", AlbumId);

            try
            {
                PhotosetsAddPhotoResponse response = POSTOperation<PhotosetsAddPhotoResponse>(_reqUri, queryParam);
                return response;
            }
            catch(Exception)
            {
                return null;
            }

        }

        public PhotosetsCreateResponse CreateAlbumFromPhotoId(string PhotoId, string AlbumName)
        {
            string method = _methods["CreateAlbum"];
            var queryParam = new Dictionary<string, string>();
            queryParam.Add("nojsoncallback", "1");
            queryParam.Add("method", method);
            queryParam.Add("format", "json");
            queryParam.Add("primary_photo_id", PhotoId);
            queryParam.Add("title", AlbumName);

            try
            {
                PhotosetsCreateResponse response = POSTOperation<PhotosetsCreateResponse>(_reqUri, queryParam);
                return response;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public SetPhotoPermissionResponse SetPhotoPermissions(string PhotoId)
        {

            string method = _methods["SetPhotoPermission"];
            var queryParam = new Dictionary<string, string>();
            queryParam.Add("nojsoncallback", "1");
            queryParam.Add("method", method);
            queryParam.Add("format", "json");
            queryParam.Add("photo_id", PhotoId);
            queryParam.Add("is_public", "0");
            queryParam.Add("is_friend", "0");
            queryParam.Add("is_family", "0");

            try
            {

                SetPhotoPermissionResponse response = POSTOperation<SetPhotoPermissionResponse>(_reqUri, queryParam);
                return response;
            }
            catch(Exception)
            {
                return null;
            }



        }

        public ResponseType POSTUploadOperation<ResponseType>(FileInfo fileInfo, Dictionary<string, string> AdditionalParams)
        {
            Dictionary<string, string> urlParams = new Dictionary<string, string>();

            String[] PhotoExt = new String[]
            {".jpg", ".jpeg", ".jpe", ".bmp",".png", ".gif",".tif",".tiff" };


            String[] VideoExt = new String[]
            {".avi", ".wmv", ".mov", ".m2ts", ".ogg", ".ogv", ".mp4", ".m4p", ".m4v", ".mpg", ".mp2", ".mpeg", ".mpe", ".mpv", ".m2v", ".m4v" };

            if(fileInfo == null)
                return default(ResponseType);

            bool IsPhoto = PhotoExt.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);
            bool IsVideo = VideoExt.Contains(fileInfo.Extension, StringComparer.OrdinalIgnoreCase);

            if(IsPhoto == false && IsVideo == false)
                return default(ResponseType);

            urlParams.Add("title", fileInfo.Name);
            //urlParams.Add("description", "test_pic_description");
            urlParams.Add("is_public", "0");
            urlParams.Add("is_friend", "0");
            urlParams.Add("is_family", "0");
            //urlParams.Add("content_type", "1");
            //urlParams.Add("hidden", "2");
                                 

            try
            {
                // form full query
                StringBuilder FullQueryUri = new StringBuilder();
                FullQueryUri.Append(_uploadUri);

                if(urlParams != null && urlParams.Count > 0)
                {
                    if(FullQueryUri[FullQueryUri.Length - 1] != '?')
                    {
                        FullQueryUri.Append("?");
                    }

                    foreach(var item in urlParams)
                    {
                        if(FullQueryUri[FullQueryUri.Length - 1] == '?')
                        {
                            FullQueryUri.Append(item.Key + "=" + item.Value);
                        }
                        else
                        {
                            FullQueryUri.Append("&" + item.Key + "=" + item.Value);
                        }
                    }
                }

                var authzHeader = GetAuthorizationHeader(FullQueryUri.ToString(), "POST", AdditionalParams, null);

                // prepare the token request
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(FullQueryUri.ToString());
                request.ContentLength = 0;
                request.Headers.Add("Authorization", authzHeader);
                request.Method = "POST";
                //request.ContentType = "image/jpeg";
                request.Timeout = 1000 * 60;
                request.KeepAlive = false;

                string boundaryString = String.Format("FLICKR_MIME_{0:N}", Guid.NewGuid());
                string fileUrl = fileInfo.FullName;
                request.ContentType = "multipart/form-data; boundary=" + boundaryString;
                request.KeepAlive = false;
                request.SendChunked = true;
                //request.AllowWriteStreamBuffering = false;


                MemoryStream postDataStream = new MemoryStream();
                StreamWriter postDataWriter = new StreamWriter(postDataStream);



                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"" + "title" + "\"\r\n");
                postDataWriter.Write("\r\n");
                postDataWriter.Write(fileInfo.Name + "\r\n");


                //postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                //postDataWriter.Write("Content-Disposition: form-data; name=\"" + "description" + "\"\r\n");
                //postDataWriter.Write("\r\n");
                //postDataWriter.Write("this_is_some_description" + "\r\n");


                // Include value from the myFileDescription text area in the post data
                postDataWriter.Write("\r\n--" + boundaryString + "\r\n");
                postDataWriter.Write("Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\n", "photo", fileInfo.Name + fileInfo.Extension);

                if(IsVideo)
                {
                    // the double end is necessary else throws "5: Filetype was not recognised" 
                    postDataWriter.Write("Content-Type: video/" + fileInfo.Extension.Trim(new Char[] { ' ', '.' }) + "\r\n\r\n");
                }
                else
                {
                    // the double end is necessary else throws "5: Filetype was not recognised" 
                    postDataWriter.Write("Content-Type: image/" + fileInfo.Extension.Trim(new Char[] { ' ', '.' }) + "\r\n\r\n");
                }


                //postDataWriter.Write("Content-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\n",
                //        "photo",
                //        @"singh_singh.jpg");

                //// the double end is necessary else throws "5: Filetype was not recognised" 
                //postDataWriter.Write("Content-Type: image/jpg\r\n\r\n");


                postDataWriter.Flush();

                // Read the file
                FileStream fileStream = new FileStream(fileUrl, FileMode.Open, FileAccess.Read);
                // 1MB buffer
                byte[] buffer = new byte[1024*1024];
                int bytesRead = 0;
                while((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    //Console.WriteLine("REQ:: Read {0} bytes.", bytesRead);
                    postDataStream.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                postDataWriter.Write("\r\n--" + boundaryString + "--\r\n");
                postDataWriter.Flush();

                // Set the http request body content length
                request.ContentLength = postDataStream.Length;

                // Dump the post data from the memory stream to the request stream
                using(Stream s = request.GetRequestStream())
                {
                    postDataStream.WriteTo(s);
                }
                postDataStream.Close();


                using(var response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    using(Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);


                        XmlSerializer x = new XmlSerializer(typeof(ResponseType));
                        ResponseType uploadResponse = (ResponseType)x.Deserialize(reader);

                        return uploadResponse;
                    }
                }
            }
            catch(Exception ex1)
            {
                Console.WriteLine("Exception: {0}", ex1.ToString());
                return default(ResponseType);
            }

        }
    }
}
