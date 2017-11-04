using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace HttpFileUploader
{
    public class MyHttp : HttpBase
    {
        public bool Upload(string url, string folderName, string fileName, int fileNum, Stream fileStream)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //request.SendChunked = true;
            request.AllowWriteStreamBuffering = false;
            request.Method = "POST";
            Guid guid = new Guid();
            string boundary = "----{0}".StrFormat(guid);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "------{0}\r\n".StrFormat(guid);
            string folderNameHeader = "\r\n{0}Content-Disposition: form-data; name=\"foldername\"\r\n\r\n{1}".StrFormat(boundary, folderName);
            string fileNameHeader = "\r\n{0}Content-Disposition: form-data; name=\"filenum\"\r\n\r\n{1}".StrFormat(boundary, fileNum);

            string fileHeader = ("\r\n{0}Content-Disposition: form-data; name=\"file\";filename=\"{1}\"\r\n" +
                                  "Content-Type: application/octet-stream\r\n\r\n")
                                  .StrFormat(boundary, fileName);
            var folderNameHeaderBytes = Encoding.UTF8.GetBytes(folderNameHeader);
            var fileNameHeaderBytes = Encoding.UTF8.GetBytes(fileNameHeader);
            byte[] fileHeaderBytes = Encoding.UTF8.GetBytes(fileHeader);
            var footerBytes = Encoding.UTF8.GetBytes("\r\n{0}".StrFormat(boundary));
            request.ContentLength = folderNameHeaderBytes.Length + fileNameHeaderBytes.Length +
                                    fileHeaderBytes.Length + fileStream.Length + footerBytes.Length;

            byte[] buffer = new byte[256 * 1024];
            // request.Proxy = WebProxy.GetDefaultProxy();
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(folderNameHeaderBytes, 0, folderNameHeaderBytes.Length);
                stream.Write(fileNameHeaderBytes, 0, fileNameHeaderBytes.Length);
                stream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                using (fileStream)
                {
                    int len = 0;
                    while ((len = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, len);
                        //stream.Flush();
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

                    return jObj["msg"].ToString() == "succeed";
                }
            }
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

                    return jObj["msg"].ToString() == "succeed";
                }
            }
        }
    }
}
