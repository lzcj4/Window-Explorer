using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace HttpFileUploader
{
    public abstract class HttpBase
    {
        CookieContainer Cookie { get; set; }
        public HttpBase()
        {
            this.Cookie = new CookieContainer();
        }

        protected virtual void SetHttpHeader(HttpWebRequest request)
        {
            if (request == null)
            {
                return;
            }

            request.KeepAlive = true;
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2914.3 Safari/537.36";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Upgrade-Insecure-Requests", "1");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8,en;q=0.6");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, sdch");
            request.CookieContainer = this.Cookie;
        }

        public HttpWebRequest GetRequest(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            SetHttpHeader(request);
            return request;
        }

        public HttpWebRequest PostRequest(string url, string content)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            SetHttpHeader(request);
            using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
            {
                sw.Write(content);
            }

            return request;
        }

        public bool Merge(string url, string str)
        {
            var request = PostRequest(url, str);
            using (var resposne = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(resposne.GetResponseStream()))
                {
                    var uploadResult = sr.ReadToEnd();
                    Debug.WriteLine(uploadResult);
                    JObject jObj = JObject.Parse(uploadResult);

                    return jObj["upload"].ToString() == "succeed";
                }
            }

        }
        /// <summary>
        /// Upload a file by path
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool UploadEx(string url, string filePath)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string fileName = Path.GetFileName(filePath);
                return this.UploadEx(url, fileName, stream);
            }
        }

        public bool UploadEx(string url, string fileName, Stream fileStream)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            Guid guid = new Guid();
            string boundary = "----{0}".StrFormat(guid);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "------{0}\r\n".StrFormat(guid);
            string headerTemplate = boundary + "Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\n" +
                                               "Content-Type: application/octet-stream\r\n\r\n";
            string header = string.Format(headerTemplate, fileName);
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            var footerBytes = Encoding.UTF8.GetBytes("\r\n{0}".StrFormat(boundary));
            request.ContentLength = headerbytes.Length + fileStream.Length + footerBytes.Length;

            byte[] buffer = new byte[4000];
            request.Proxy = WebProxy.GetDefaultProxy();
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(headerbytes, 0, headerbytes.Length);
                using (fileStream)
                {
                    int len = 0;
                    while ((len = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, len);
                    }
                }
                stream.Write(footerBytes, 0, footerBytes.Length);
            }

            using (var resposne = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(resposne.GetResponseStream()))
                {
                    var uploadResult = sr.ReadToEnd();
                    Debug.WriteLine(uploadResult);
                    JObject jObj = JObject.Parse(uploadResult);

                    return jObj["upload"].ToString() == "succeed";
                }
            }
        }

        /// <summary>
        /// Upload a file by path
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool Upload(string url, string filePath)
        {
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                string fileName = Path.GetFileName(filePath);
                return this.Upload(url, fileName, stream);
            }
        }


        /// <summary>
        /// Upload a file by stream 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public bool Upload(string url, string fileName, Stream stream)
        {
            HttpContent stringContent = new StringContent(fileName);
            using (HttpContent streamContent = new StreamContent(stream, 65535))
            {
                using (MultipartFormDataContent multiContent = new MultipartFormDataContent())
                {
                    multiContent.Add(stringContent, "file", fileName);
                    var handler = new HttpClientHandler
                    {
                        CookieContainer = new CookieContainer(),
                        UseCookies = true,
                        UseDefaultCredentials = false,
                        Proxy = WebProxy.GetDefaultProxy(),
                        UseProxy = true,
                    };

                    HttpClient client = new HttpClient();
                    var response = client.PostAsync(url, multiContent).Result;
                    JObject jObj = JObject.Parse(response.Content.ReadAsStringAsync().Result);
                    return response.StatusCode == HttpStatusCode.OK && jObj["upload"].ToString() == "succeed";
                }
            }
        }

        protected string GetHtml(HttpWebRequest request)
        {
            using (WebResponse response = request.GetResponse())
            {
                //using (GZipStream strem = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress))
                using (Stream strem = response.GetResponseStream())
                {
                    Encoding gb2312Encoding = Encoding.GetEncoding("utf-8");
                    using (StreamReader sr = new StreamReader(strem, gb2312Encoding))
                    {
                        string result = sr.ReadToEnd();
                        return result;
                    }
                }
            }
        }

        protected Stream GetStream(HttpWebRequest request)
        {
            using (WebResponse response = request.GetResponse())
            {
                MemoryStream ms = new MemoryStream();
                using (Stream stream = response.GetResponseStream())
                {
                    int len = 1024;
                    int pos = 0;
                    int readLen = 1;
                    byte[] buffer = new byte[len];
                    while (readLen > 0)
                    {
                        readLen = stream.Read(buffer, pos, len);
                        if (readLen > 0)
                        {
                            ms.Write(buffer, 0, readLen);
                            Array.Clear(buffer, 0, len);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return ms;
            }
        }
    }
}
