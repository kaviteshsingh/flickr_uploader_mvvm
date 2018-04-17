
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

using flickr_uploader_mvvm.Core;

/*
 * http://www.wackylabs.net/2011/12/oauth-and-flickr-part-2/
 * http://stackoverflow.com/questions/4002847/oauth-with-verification-in-net
 * http://stackoverflow.com/questions/3330667/how-do-i-get-a-all-flickr-photosets-of-a-user-using-yql
 * http://deanhume.com/home/blogpost/a-simple-guide-to-using-oauth-with-c-/49
 * https://snakeycode.wordpress.com/2013/05/01/troubles-with-the-flickr-api/ 
 * https://www.flickr.com/services/api/
 * https://www.flickr.com/services/api/upload.api.html
 * https://www.flickr.com/services/api/upload.example.html
 * https://www.techcoil.com/blog/sending-a-file-and-some-form-data-via-http-post-in-c/
 * https://www.techcoil.com/blog/uploading-large-http-multipart-request-with-system-net-httpwebrequest-in-c/
 * https://gist.github.com/bgrins/1789787
 * http://www.briangrinstead.com/blog/multipart-form-post-in-c
 * http://stackoverflow.com/questions/19954287/how-to-upload-file-to-server-with-http-post-multipart-form-data
 * http://stackoverflow.com/questions/3274968/send-multipart-form-data-content-type-request
 * https://msdn.microsoft.com/en-us/library/system.net.httpwebrequest.begingetrequeststream.aspx
 */


namespace flickr_uploader_mvvm.Flickr
{
    public class RestOauthManager
    {
        Dictionary<String, String> oauth_params = new Dictionary<String, String>();

        public string ConsumerKey
        {
            get { return oauth_params["oauth_consumer_key"]; }
            set { oauth_params["oauth_consumer_key"] = value; }
        }

        private string _consumerSecret;

        public string ConsumerSecret
        {
            get { return _consumerSecret; }
            set { _consumerSecret = value; }
        }

        public string AccessToken
        {
            get { return oauth_params["oauth_token"]; }
            set { oauth_params["oauth_token"] = value; }
        }

        private string _accessSecret;

        public string AccessSecret
        {
            get { return _accessSecret; }
            set { _accessSecret = value; }
        }
        
        public RestOauthManager()
        {
            oauth_params["oauth_callback"] = "oob"; // presume "desktop" consumer
            oauth_params["oauth_signature_method"] = "HMAC-SHA1";
            oauth_params["oauth_version"] = "1.0";
            oauth_params["oauth_consumer_key"] = "";
            //oauth_params["oauth_token"] = "";
            _consumerSecret = "";
            _accessSecret = "";
            //_accessToken = "";
        }
        
        public RestOauthManager(string consumerKey, string consumerSecret) : this()
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }

        public RestOauthManager(string consumerKey,
               string consumerSecret,
               string token,
               string tokenSecret) : this()
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;


            // only add these params if valid value, because empty or "" or can break
            // authentication when creating signature. 
            if(! (String.IsNullOrEmpty(token)|| (String.IsNullOrWhiteSpace(token))) )
                AccessToken = token;

            if(!(String.IsNullOrEmpty(tokenSecret) || (String.IsNullOrWhiteSpace(tokenSecret))))
                AccessSecret = tokenSecret;
        }
        
        public ResponseType GETOperation<ResponseType>(string requestUri, bool bXMLResponse = false)
        {

            Dictionary<string, string> AdditionalParams = new Dictionary<string, string>();

            //AdditionalParams.Add("oauth_token", AccessToken);

            var authzHeaders = GetAuthorizationHeader(requestUri, "GET", AdditionalParams, null );

            HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;
            request.ContentLength = 0;
            request.Method = "GET";
            request.ServicePoint.Expect100Continue = false;
            request.ContentType = "x-www-form-urlencoded";

            request.Headers["Authorization"] = authzHeaders;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using(Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadToEnd()));

                string respStr = Encoding.UTF8.GetString(ms.ToArray());
                Console.WriteLine(respStr);
                ms.Position = 0;


                if(bXMLResponse)
                {
                    XmlSerializer x = new XmlSerializer(typeof(ResponseType));
                    ResponseType xmlResponse = (ResponseType)x.Deserialize(ms);
                    return xmlResponse;
                }
                else
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseType));
                    ResponseType jsonResponse = (ResponseType)ser.ReadObject(ms);
                    return jsonResponse;
                }
            }

        }

        /// <summary>
        /// URI should be of similar type:  "https://api.flickr.com/services/rest?";
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="QueryParams"></param>
        /// <returns></returns>
        public ResponseType GETOperation<ResponseType>(string requestUri, Dictionary<string, string> QueryParams, bool bXMLResponse = false)
        {

            // form full query
            StringBuilder FullQueryUri = new StringBuilder();
            FullQueryUri.Append(requestUri);

            if(QueryParams != null && QueryParams.Count > 0)
            {
                if(FullQueryUri[FullQueryUri.Length - 1] != '?')
                {
                    FullQueryUri.Append("?");
                }

                foreach(var item in QueryParams)
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


            var authzHeaders = GetAuthorizationHeader(FullQueryUri.ToString(), "GET", null, null );

            HttpWebRequest request = WebRequest.Create(FullQueryUri.ToString()) as HttpWebRequest;
            request.ContentLength = 0;
            request.Method = "GET";
            request.ServicePoint.Expect100Continue = false;
            request.ContentType = "x-www-form-urlencoded";

            request.Headers["Authorization"] = authzHeaders;

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using(Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadToEnd()));

                string respStr = Encoding.UTF8.GetString(ms.ToArray());
                Console.WriteLine(respStr);
                ms.Position = 0;

                if(bXMLResponse)
                {
                    XmlSerializer x = new XmlSerializer(typeof(ResponseType));
                    ResponseType xmlResponse = (ResponseType)x.Deserialize(ms);
                    return xmlResponse;
                }
                else
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseType));
                    ResponseType jsonResponse = (ResponseType)ser.ReadObject(ms);
                    return jsonResponse;
                }
            }

        }

        public ResponseType POSTOperation<ResponseType>(string requestUri, Dictionary<string, string> PostParams, bool bXMLResponse = false)
        {

            try
            {
                // form full query
                StringBuilder FullQueryUri = new StringBuilder();
                FullQueryUri.Append(requestUri);

                if(PostParams != null && PostParams.Count > 0)
                {
                    if(FullQueryUri[FullQueryUri.Length - 1] != '?')
                    {
                        FullQueryUri.Append("?");
                    }

                    foreach(var item in PostParams)
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

                var authzHeader = GetAuthorizationHeader(FullQueryUri.ToString(), "POST", null, null);


                // prepare the token request
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(FullQueryUri.ToString());
                request.ContentLength = 0;
                request.Headers.Add("Authorization", authzHeader);
                request.Method = "POST";
                //request.ContentType = "image/jpeg";
                request.Timeout = 1000 * 60;
                request.KeepAlive = false;



                using(var response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    using(Stream stream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadToEnd()));

                        string respStr = Encoding.UTF8.GetString(ms.ToArray());
                        Console.WriteLine(respStr);
                        ms.Position = 0;
                     
                        if(bXMLResponse)
                        {
                            XmlSerializer x = new XmlSerializer(typeof(ResponseType));
                            ResponseType xmlResponse = (ResponseType)x.Deserialize(ms);
                            return xmlResponse;
                        }
                        else
                        {
                            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResponseType));
                            ResponseType jsonResponse = (ResponseType)ser.ReadObject(ms);
                            return jsonResponse;
                        }
                    }
                }
            }
            catch(Exception ex1)
            {
                Console.WriteLine("Exception: {0}", ex1.ToString());
                return default(ResponseType);
            }

        }

        protected bool AcquireAccessToken(string uri, string method, string pin)
        {
            Dictionary<string, string> AdditionalParams = new Dictionary<string, string>();

            AdditionalParams.Add("oauth_verifier", pin);

            var authzHeader = GetAuthorizationHeader(uri, method, AdditionalParams, null);

            try
            {
                // prepare the token request
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
                request.ContentLength = 0;
                request.Headers.Add("Authorization", authzHeader);
                request.Method = method;

                using(var response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    using(var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        var s = reader.ReadToEnd();
                        Console.WriteLine("resp: {0}", s);
                        var r = new OAuthResponse(s);

                        AccessToken = r["oauth_token"];
                        AccessSecret = r["oauth_token_secret"];
                        return true;
                    }
                }
            }
            catch(Exception ex1)
            {
                Console.WriteLine("Exception: {0}", ex1.ToString());
                return false;
            }
        }

        protected bool AcquireRequestToken(string uri, string method)
        {
            var authzHeader = GetAuthorizationHeader(uri, method, null, null);

            // prepare the token request
            var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(uri);
            request.ContentLength = 0;
            request.Headers.Add("Authorization", authzHeader);
            request.Method = method;

            try
            {
                using(var response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    using(var reader = new System.IO.StreamReader(response.GetResponseStream()))
                    {
                        var r = new OAuthResponse(reader.ReadToEnd());
                        AccessToken = r["oauth_token"];

                        // Sometimes the request_token URL gives us an access token,
                        // with no user interaction required. Eg, when prior approval
                        // has already been granted.
                        try
                        {
                            if(r["oauth_token_secret"] != null)
                            {
                                AccessSecret = r["oauth_token_secret"];
                            }
                        }
                        catch { }
                        return true;
                    }
                }
            }
            catch(Exception Ex)
            {
                IErrorInfo me = new MessageBoxErrorInfo();
                me.ShowErrorMessage(Ex.Message);
                return false;
            }
        }

        public string GetAuthorizationHeader(string uri, string method, Dictionary<string, string> AdditionalParams, string realm)
        {

            if(string.IsNullOrEmpty(this.ConsumerKey))
                throw new ArgumentNullException("ConsumerKey");

            if(string.IsNullOrEmpty(this.oauth_params["oauth_signature_method"]))
                throw new ArgumentNullException("oauth_signature_method");



            var FinalRequestParams = Sign(uri, method, AdditionalParams);

            var erp = EncodeRequestParameters(FinalRequestParams);

            return (String.IsNullOrEmpty(realm))
                ? "OAuth " + erp
                : String.Format("OAuth realm=\"{0}\", ", realm) + erp;

        }

        private static string EncodeRequestParameters(ICollection<KeyValuePair<String, String>> p)
        {
            var sb = new System.Text.StringBuilder();
            foreach(KeyValuePair<String, String> item in p.OrderBy(x => x.Key))
            {
                if(!String.IsNullOrEmpty(item.Value))
                {
                    sb.AppendFormat("{0}=\"{1}\", ", item.Key, UrlEncode(item.Value));
                }
            }

            return sb.ToString().TrimEnd(' ').TrimEnd(',');
        }

        private Dictionary<string, string> Sign(string uri, string method, Dictionary<string, string> AdditionalParams)
        {
            var signatureBase = GetSignatureBase(uri, method, AdditionalParams);
            var hash = GetHash();

            byte[] dataBuffer = Encoding.ASCII.GetBytes(signatureBase.Item2);
            byte[] hashBytes = hash.ComputeHash(dataBuffer);
            var signature = Convert.ToBase64String(hashBytes);

            signatureBase.Item1.Add("oauth_signature", signature);

            return signatureBase.Item1;
        }

        private HashAlgorithm GetHash()
        {
            if(oauth_params["oauth_signature_method"] != "HMAC-SHA1")
                throw new NotImplementedException();

            var keystring = string.Format("{0}&{1}",
                                   UrlEncode(ConsumerSecret), UrlEncode(AccessSecret));
            //Console.WriteLine("signing keystring: {0}", keystring);
            return new HMACSHA1
            {
                Key = Encoding.ASCII.GetBytes(keystring)
            };
        }

        // return string and dictionary of params
        private Tuple<Dictionary<string, string>, string> GetSignatureBase(string url, string method, Dictionary<string, string> AdditionalParams)
        {
            // normalize the URI
            var uri = new Uri(url);
            var normUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);
            if(!((uri.Scheme == "http" && uri.Port == 80) ||
                  (uri.Scheme == "https" && uri.Port == 443)))
                normUrl += ":" + uri.Port;

            normUrl += uri.AbsolutePath;

            // the sigbase starts with the method and the encoded URI
            var sb = new System.Text.StringBuilder();
            sb.Append(method)
                .Append('&')
                .Append(UrlEncode(normUrl))
                .Append('&');

            // The parameters follow. This must include all oauth params
            // plus any query params on the uri.  Also, each uri may
            // have a distinct set of query params.

            // first, get the query params
            var p = ExtractQueryParameters(uri.Query);

            if(AdditionalParams != null && AdditionalParams.Count > 0)
            {
                foreach(var item in AdditionalParams)
                {
                    if(!String.IsNullOrEmpty(item.Key))
                    {
                        p.Add(item.Key, item.Value);
                    }
                }
            }

            foreach(var item in oauth_params)
            {
                // this is to ensure only params with valid value go in
                if(!String.IsNullOrEmpty(item.Value))
                {
                    p.Add(item.Key, (item.Key == "oauth_callback") ? UrlEncode(item.Value) : item.Value); 
                }
            }

            p.Add("oauth_timestamp", GenerateTimeStamp());
            p.Add("oauth_nonce", GenerateNonce());

            // concat+format the sorted list of all those params
            var sb1 = new System.Text.StringBuilder();
            foreach(KeyValuePair<String, String> item in p.OrderBy(x => x.Key))
            {
                // even "empty" params need to be encoded this way.
                sb1.AppendFormat("{0}={1}&", item.Key, item.Value);
            }

            //Console.WriteLine("oauth piece: {0}", sb1.ToString());

            // append the UrlEncoded version of that string to the sigbase
            sb.Append(UrlEncode(sb1.ToString().TrimEnd('&')));
            var result = sb.ToString();
            //Console.WriteLine("Sigbase final: {0}", result);

            return Tuple.Create(p, result);
        }

        private Dictionary<String, String> ExtractQueryParameters(string queryString)
        {
            if(queryString.StartsWith("?"))
                queryString = queryString.Remove(0, 1);

            var result = new Dictionary<String, String>();

            if(string.IsNullOrEmpty(queryString))
                return result;

            foreach(string s in queryString.Split('&'))
            {
                if(!string.IsNullOrEmpty(s) && !s.StartsWith("oauth_"))
                {
                    if(s.IndexOf('=') > -1)
                    {
                        string[] temp = s.Split('=');
                        result.Add(temp[0], temp[1]);
                    }
                    else
                        result.Add(s, string.Empty);
                }
            }

            return result;
        }

        private static string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        private static string UrlEncode(string value)
        {
            var result = new StringBuilder();

            if(!String.IsNullOrEmpty(value))
            {
                foreach(char symbol in value)
                {
                    if(unreservedChars.IndexOf(symbol) != -1)
                        result.Append(symbol);
                    else
                    {
                        foreach(byte b in Encoding.UTF8.GetBytes(symbol.ToString()))
                        {
                            result.Append('%' + String.Format("{0:X2}", b));
                        }
                    }
                } 
            }

            return result.ToString();
        }

        private string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        private string GenerateNonce()
        {
            return String.Format("{0:N}", Guid.NewGuid());
        }
        
    }


    /// <summary>
    ///   A class to hold an OAuth response message.
    /// </summary>
    public class OAuthResponse
    {
        /// <summary>
        ///   All of the text in the response. This is useful if the app wants
        ///   to do its own parsing.
        /// </summary>
        public string AllText { get; set; }
        private Dictionary<String, String> _params;

        /// <summary>
        ///   a Dictionary of response parameters.
        /// </summary>
        public string this[string ix]
        {
            get
            {
                return _params[ix];
            }
        }

        /// <summary>
        ///   Constructor for the response to one transmission in an oauth dialogue.
        ///   An application or may not not want direct access to this response.
        /// </summary>
        internal OAuthResponse(string alltext)
        {
            AllText = alltext;
            Console.WriteLine("OAuthResponse.ctor: {0}", alltext);
            _params = new Dictionary<String, String>();
            var kvpairs = alltext.Split('&');
            foreach(var pair in kvpairs)
            {
                Console.WriteLine("pair: {0}", pair);
                var kv = pair.Split('=');
                _params.Add(kv[0], kv[1]);
            }
            // expected keys:
            //   oauth_token, oauth_token_secret, user_id, screen_name, etc
        }
    }


}

