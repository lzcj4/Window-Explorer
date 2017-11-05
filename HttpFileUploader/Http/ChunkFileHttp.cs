using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace HttpFileUploader
{
    public class ChunkFileHttp : HttpBase
    {
        private const int timeout = 10 * 60 * 1000;
        public bool Upload(string url, string folderName,FileChunkItem fileItem, Stream fileStream)
        {
            HttpWebRequest request = CreateRequest(url, HttpMethod.Post);
            //request.SendChunked = true;
            request.AllowWriteStreamBuffering = false;
            request.Timeout = timeout;
            request.ReadWriteTimeout = timeout;
            request.Proxy = WebProxy.GetDefaultProxy();
            Guid guid = new Guid();
            string boundary = "----{0}".StrFormat(guid);
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            boundary = "------{0}\r\n".StrFormat(guid);
            string folderNameHeader = "\r\n{0}Content-Disposition: form-data; name=\"foldername\"\r\n\r\n{1}".StrFormat(boundary, folderName);
            string fileNameHeader = "\r\n{0}Content-Disposition: form-data; name=\"filenum\"\r\n\r\n{1}".StrFormat(boundary, fileItem.Num);
            string fileLenHeader = "\r\n{0}Content-Disposition: form-data; name=\"filelen\"\r\n\r\n{1}".StrFormat(boundary, fileItem.Len);

            string fileHeader = ("\r\n{0}Content-Disposition: form-data; name=\"file\";filename=\"{1}\"\r\n" +
                                  "Content-Type: application/octet-stream\r\n\r\n")
                                  .StrFormat(boundary, fileItem.Name);
            var folderNameHeaderBytes = Encoding.UTF8.GetBytes(folderNameHeader);
            var fileNameHeaderBytes = Encoding.UTF8.GetBytes(fileNameHeader);
            var fileNameLenBytes = Encoding.UTF8.GetBytes(fileLenHeader);
            byte[] fileHeaderBytes = Encoding.UTF8.GetBytes(fileHeader);
            var footerBytes = Encoding.UTF8.GetBytes("\r\n{0}".StrFormat(boundary));
            request.ContentLength = folderNameHeaderBytes.Length + fileNameHeaderBytes.Length + fileNameLenBytes.Length+
                                    fileHeaderBytes.Length + fileStream.Length + footerBytes.Length;
        
            byte[] buffer = new byte[512 * 1024];
            // request.Proxy = WebProxy.GetDefaultProxy();
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(folderNameHeaderBytes, 0, folderNameHeaderBytes.Length);
                stream.Write(fileNameHeaderBytes, 0, fileNameHeaderBytes.Length);
                stream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                stream.Write(fileNameLenBytes, 0, fileNameLenBytes.Length);
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

                    return jObj["code"].ToString() == "200";
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
