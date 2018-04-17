using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;


namespace flickr_uploader_mvvm.Flickr
{

    //////////////////////////////////////////////////////////////


    [DataContract]
    public class Username
    {

        [DataMember(Name = "_content")]
        public string _content { get; set;  }
    }

    [DataContract]
    public class User
    {

        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "username")]
        public Username username { get; set; }
    }

    [DataContract]
    public class TestLoginResponse
    {

        [DataMember(Name = "user")]
        public User user { get; set; }

        [DataMember(Name = "stat")]
        public string stat { get; set; }
    }


    //////////////////////////////////////////////////////////////
    [DataContract]
    public class Title
    {
        [DataMember(Name = "_content")]
        public string _content { get; set; }
    }

    [DataContract]
    public class Description
    {
        [DataMember(Name = "_content")]
        public string _content { get; set; }
    }


    //////////////////////////////////////////////////////////////


    [DataContract]
    public class Photoset
    {
        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "primary")]
        public string primary { get; set; }

        [DataMember(Name = "secret")]
        public string secret { get; set; }

        [DataMember(Name = "server")]
        public string server { get; set; }

        [DataMember(Name = "farm")]
        public int farm { get; set; }

        [DataMember(Name = "photos")]
        public int photos { get; set; }

        [DataMember(Name = "videos")]
        public string videos { get; set; }

        [DataMember(Name = "title")]
        public Title title { get; set; }

        [DataMember(Name = "description")]
        public Description description { get; set; }

        [DataMember(Name = "needs_interstitial")]
        public int needs_interstitial { get; set; }

        [DataMember(Name = "visibility_can_see_set")]
        public int visibility_can_see_set { get; set; }

        [DataMember(Name = "count_views")]
        public string count_views { get; set; }

        [DataMember(Name = "count_comments")]
        public string count_comments { get; set; }

        [DataMember(Name = "can_comment")]
        public int can_comment { get; set; }

        [DataMember(Name = "date_create")]
        public string date_create { get; set; }

        [DataMember(Name = "date_update")]
        public string date_update { get; set; }
    }

    [DataContract]
    public class Photosets
    {
        [DataMember(Name = "cancreate")]
        public int cancreate { get; set; }

        [DataMember(Name = "page")]
        public int page { get; set; }

        [DataMember(Name = "pages")]
        public int pages { get; set; }

        [DataMember(Name = "perpage")]
        public int perpage { get; set; }

        [DataMember(Name = "total")]
        public int total { get; set; }

        [DataMember(Name = "photoset")]
        public IList<Photoset> photoset { get; set; }
    }

    [DataContract]
    public class PhotosetsGetListResponse
    {
        [DataMember(Name = "photosets")]
        public Photosets photosets { get; set; }

        [DataMember(Name = "stat")]
        public string stat { get; set; }
    }


    //////////////////////////////////////////////////////////////

    [DataContract]
    public class Gallery
    {
        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "url")]
        public string url { get; set; }

        [DataMember(Name = "owner")]
        public string owner { get; set; }

        [DataMember(Name = "username")]
        public string username { get; set; }

        [DataMember(Name = "iconserver")]
        public string iconserver { get; set; }

        [DataMember(Name = "iconfarm")]
        public int iconfarm { get; set; }

        [DataMember(Name = "primary_photo_id")]
        public string primary_photo_id { get; set; }

        [DataMember(Name = "date_create")]
        public string date_create { get; set; }

        [DataMember(Name = "date_update")]
        public string date_update { get; set; }

        [DataMember(Name = "count_photos")]
        public string count_photos { get; set; }

        [DataMember(Name = "count_videos")]
        public string count_videos { get; set; }

        [DataMember(Name = "count_views")]
        public string count_views { get; set; }

        [DataMember(Name = "count_comments")]
        public string count_comments { get; set; }

        [DataMember(Name = "title")]
        public Title title { get; set; }

        [DataMember(Name = "description")]
        public Description description { get; set; }

        [DataMember(Name = "primary_photo_server")]
        public string primary_photo_server { get; set; }

        [DataMember(Name = "primary_photo_farm")]
        public int primary_photo_farm { get; set; }

        [DataMember(Name = "primary_photo_secret")]
        public string primary_photo_secret { get; set; }
    }

    [DataContract]
    public class Galleries
    {
        [DataMember(Name = "total")]
        public string total { get; set; }

        [DataMember(Name = "page")]
        public int page { get; set; }

        [DataMember(Name = "pages")]
        public int pages { get; set; }

        [DataMember(Name = "per_page")]
        public int per_page { get; set; }

        [DataMember(Name = "user_id")]
        public string user_id { get; set; }

        [DataMember(Name = "gallery")]
        public IList<Gallery> gallery { get; set; }
    }

    [DataContract]
    public class GalleriesGetListResponse
    {
        [DataMember(Name = "galleries")]
        public Galleries galleries { get; set; }

        [DataMember(Name = "stat")]
        public string stat { get; set; }
    }




    //////////////////////////////////////////////////////////////

    [DataContract]
    public class Photo
    {

        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "secret")]
        public string secret { get; set; }

        [DataMember(Name = "server")]
        public string server { get; set; }

        [DataMember(Name = "farm")]
        public int farm { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }

        [DataMember(Name = "isprimary")]
        public string isprimary { get; set; }

        [DataMember(Name = "ispublic")]
        public int ispublic { get; set; }

        [DataMember(Name = "isfriend")]
        public int isfriend { get; set; }

        [DataMember(Name = "isfamily")]
        public int isfamily { get; set; }
    }

    [DataContract]
    public class PhotosetGetPhotos
    {

        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "primary")]
        public string primary { get; set; }

        [DataMember(Name = "owner")]
        public string owner { get; set; }

        [DataMember(Name = "ownername")]
        public string ownername { get; set; }

        [DataMember(Name = "photo")]
        public IList<Photo> photo { get; set; }

        [DataMember(Name = "page")]
        public int page { get; set; }

        [DataMember(Name = "per_page")]
        public int per_page { get; set; }

        [DataMember(Name = "perpage")]
        public int perpage { get; set; }

        [DataMember(Name = "pages")]
        public int pages { get; set; }

        [DataMember(Name = "total")]
        public string total { get; set; }

        [DataMember(Name = "title")]
        public string title { get; set; }
    }

    [DataContract]
    public class PhotosetsGetPhotosResponse
    {

        [DataMember(Name = "photoset")]
        public PhotosetGetPhotos photoset { get; set; }

        [DataMember(Name = "stat")]
        public string stat { get; set; }
    }



    //////////////////////////////////////////////////////////////



    //////////////////////////////////////////////////////////////



    //////////////////////////////////////////////////////////////

    [DataContract]
    public class PhotosetsAddPhotoResponse
    {

        [DataMember(Name = "stat")]
        public string stat { get; set; }

        [DataMember(Name = "code")]
        public int code { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }
    }



    //////////////////////////////////////////////////////////////



    [DataContract]
    public class PhotosetResponse
    {

        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "url")]
        public string url { get; set; }
    }

    [DataContract]
    public class PhotosetsCreateResponse
    {

        [DataMember(Name = "photoset")]
        public PhotosetResponse photoset { get; set; }

        [DataMember(Name = "stat")]
        public string stat { get; set; }
    }




    ////////////////////////////////////////////

    [XmlRoot(ElementName = "err")]
    public class Err
    {
        [XmlAttribute(AttributeName = "code")]
        public string Code { get; set; }
        [XmlAttribute(AttributeName = "msg")]
        public string Msg { get; set; }
    }


    [XmlRoot(ElementName = "rsp")]
    public class PhotoUploadResponse
    {
        [XmlElement(ElementName = "photoid")]
        public string Photoid { get; set; }
        [XmlElement(ElementName = "err")]
        public Err Err { get; set; }
        [XmlAttribute(AttributeName = "stat")]
        public string Stat { get; set; }
    }




    /// //////////////////////////////////////////////////////


    [DataContract]
    public class SetPhotoPermissionId
    {

        [DataMember(Name = "_content")]
        public string _content { get; set; }

        [DataMember(Name = "secret")]
        public string secret { get; set; }

        [DataMember(Name = "originalsecret")]
        public string originalsecret { get; set; }
    }


    [DataContract]
    public class SetPhotoPermissionResponse
    {

        [DataMember(Name = "stat")]
        public string stat { get; set; }

        [DataMember(Name = "photoid")]
        public SetPhotoPermissionId photoid { get; set; }

        [DataMember(Name = "code")]
        public int code { get; set; }

        [DataMember(Name = "message")]
        public string message { get; set; }
    }




}
