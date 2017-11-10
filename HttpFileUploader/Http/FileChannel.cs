

using System.Diagnostics;
using WebSocket4Net;
using System;
using Newtonsoft.Json.Linq;

namespace HttpFileUploader
{
    public class FileChannel : IDisposable
    {
        private static FileChannel instance;
        public static FileChannel Instance
        {
            get
            {
                instance = instance ?? new FileChannel();
                return instance;
            }
        }


        WebSocket client;
        public FileChannel()
        {
            client = HttpFactory.Instance.GetWSClient();
            client.Opened += Client_Opened;
            client.DataReceived += Client_DataReceived;
            client.MessageReceived += Client_MessageReceived;
            client.Error += Client_Error;
            client.Closed += Client_Closed;
        }

        public void Open()
        {
            if (client.State == WebSocketState.Open ||
                client.State == WebSocketState.Connecting)
            {
                return;
            }
            client.Open();
        }

        public void Send(string msg)
        {
            this.client.Send(msg);

        }
        public void Close()
        {
            this.client.Close();
            this.client = null;
        }


        private void Client_Closed(object sender, System.EventArgs e)
        {
            Debug.WriteLine("WebSocket_Closed");
        }

        private void Client_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Debug.WriteLine("WebSocket_Error");
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            JObject jObj = JObject.Parse(e.Message);
            Debug.WriteLine(e.Message);
        }

        private void Client_DataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            Debug.WriteLine("WebSocket_DataReceived");
        }

        private void Client_Opened(object sender, System.EventArgs e)
        {
            Debug.WriteLine("WebSocket_Opened");
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}
