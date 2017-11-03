using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HttpFileUploader
{
    public class MyHttp : HttpBase
    {
        public const string WEB_HOST = "127.0.0.1:8000";
        protected override void SetHttpHeader(HttpWebRequest request)
        {
            base.SetHttpHeader(request);
            //request.ContentType = "application/json";
        }
    }
}
