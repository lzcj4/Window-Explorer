using MongoDB.Messaging;
using MongoDB.Messaging.Service;
using MongoDB.Messaging.Subscription;
using MongoMQTest.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MongoMQTest
{
    public class MsgEventArgs : EventArgs
    {
        public MsgBase Msg { get; private set; }
        public MsgEventArgs(MsgBase msg)
        {
            if (msg.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.Msg = msg;
        }
    }

    public class MsgQueue : IDisposable
    {
        private const string db_connstr = AppSettings.Message_ConnectionString;
        public const string quenue_name = "MessageQueue";
        private const int retry_count = 5;
        private const int publish_retry_count = 2;
        private const int queue_worker = 1;

        private static object lockObj = new object();
        private static MsgQueue instance;
        public static MsgQueue Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        instance = new MsgQueue();
                    }
                }
                return instance;
            }
        }

        public event EventHandler<MsgEventArgs> OnMsgReceived;

        MessageService msgService;

        public MsgQueue()
        {
            this.InitalQueue();
        }

        private void InitalQueue()
        {
            MessageQueue.Default.Configure(c => c
               .Connection(db_connstr)
               .Queue(s => s
                   .Name(quenue_name)
                   .Priority(MessagePriority.Normal)
                   //.ResponseQueue("ReplyQueueName")
                   .Retry(retry_count)));
        }


        public void Subscribe()
        {
            MessageQueue.Default.Configure(c => c
                               .Connection(db_connstr)
                               .Subscribe(s =>
                               {
                                   s.Queue(quenue_name)
                                   .Handler<MsgHandler>()
                                   .Retry<MessageRetry>()
                                   .Workers(queue_worker);
                               }));

            msgService = new MessageService();
            msgService.Start();
        }

        public void Publish(IEnumerable<MsgBase> list)
        {
            if (list.IsNullOrEmpty())
            {
                return;
            }

            foreach (var item in list)
            {
                Publish(item);
            }
        }

        IList<string> publistGroupIdList = new List<string>();

        public void Publish(MsgBase msg)
        {
            if (!publistGroupIdList.Contains(msg.GroupId))
            {
                publistGroupIdList.Add(msg.GroupId);
            }

            MessageQueue.Default.Publish(m => m
                 .Queue(quenue_name)
                 .Name(msg.Name)
                 .Data(msg)
                 .Correlation(msg.GroupId)
                 .Description(msg.Description)
                 .Priority(MessagePriority.Normal)
                 .Retry(publish_retry_count)
             );
        }


        public void Stop()
        {
            if (!msgService.IsNull())
            {
                msgService.Stop();
                msgService = null;
            }
        }

        private void RaiseOnMsgReceive(MsgBase msg)
        {
            var msgEvent = this.OnMsgReceived;
            if (!msgEvent.IsNull())
            {
                msgEvent(this, new MsgEventArgs(msg));
            }
        }

        public void Dispose()
        {
            this.Stop();
        }

        private class MsgHandler : IMessageSubscriber
        {
            public MessageResult Process(ProcessContext processContext)
            {
                bool isHandled = Handler<AnalysisMsg>(processContext);
                if (!isHandled)
                {
                    isHandled = Handler<FileMergeMsg>(processContext);
                }
                return isHandled ? MessageResult.Successful : MessageResult.None;
            }

            private bool Handler<T>(ProcessContext processContext) where T : MsgBase
            {
                bool result = false;
                try
                {
                    T msg = processContext.Data<T>();
                    if (!msg.IsNull())
                    {
                        result = true;
                        MsgQueue.instance.RaiseOnMsgReceive(msg);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("MsgHandler.Handle excepiton:{0}", ex.Message));
                }

                return result;
            }

            public void Dispose()
            {
            }
        }

        public class MessageRetry : IMessageRetry
        {
            public virtual bool ShouldRetry(ProcessContext processContext, Exception exception)
            {
                // get current message 
                var message = processContext.Message;

                // true to retry message
                return message.ErrorCount < message.RetryCount;
            }

            public virtual DateTime NextAttempt(ProcessContext processContext)
            {
                var message = processContext.Message;

                // retry weight, 1 = 1 min, 2 = 30 min, 3 = 2 hrs, 4+ = 8 hrs
                if (message.ErrorCount > 3)
                    return DateTime.Now.AddHours(8);

                if (message.ErrorCount == 3)
                    return DateTime.Now.AddHours(2);

                if (message.ErrorCount == 2)
                    return DateTime.Now.AddMinutes(30);

                // default
                return DateTime.Now.AddMinutes(1);
            }
        }
    }
}
