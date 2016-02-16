//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using ZeroMQ;
//using TradingLib.Mixins.TNetStrings;
//using TradingLib.Mixins.Json;

//namespace TradingLib.Mixins.m2net
//{
//    public delegate void HttpRequestDelegate(M2NetConnection conn,Request request);
//    public delegate void HttpDebugDelegate(string msg);

//    public class M2NetConnection : MarshalByRefObject, IDisposable
//    {
//        public event HttpRequestDelegate SendHttpRequestEvent;
//        public event HttpDebugDelegate SendDebugEvent;

//        void debug(string msg)
//        {
//            if (SendDebugEvent != null)
//                SendDebugEvent(msg);
//        }
//        void HandleRequest(M2NetConnection conn,Request request)
//        {
//            if (SendHttpRequestEvent != null)
//                SendHttpRequestEvent(conn,request);
//        }
//        private Encoding Enc = Encoding.UTF8;

//        private string sub_addr;
//        private string pub_addr;
//        public string SenderId { get; private set; }
        
//        public M2NetConnection(string sender_id, string sub_addr, string pub_addr)
//        {

//            this.SenderId = sender_id;

//            this.sub_addr = sub_addr;//mongrel2 任务发布端口
//            this.pub_addr = pub_addr;//mongrel2 结果接受端口
//        }

//        bool handlergo = false;

//        ZmqSocket pubchannel = null;

//        /// <summary>
//        /// 启动链接处理线程
//        /// </summary>
//        public void Start()
//        {
//            debug("start connection....");
//            if (handlergo) return;
//            handlergo = true;
//            handlerthread = new Thread(process);
//            handlerthread.IsBackground = true;
//            handlerthread.Start();

//        }

//        Thread handlerthread = null;

//        public void Join()
//        {
//            if (handlergo) return;
//            handlergo = true;
//            process();
//            int mainwait = 0;
//            while (!running && mainwait < 10)
//            {
//                Thread.Sleep(1000);
//                mainwait++;
//            }

//        }
//        bool running = false;
//        void process()
//        {
//            byte[] data = new byte[0];
//            int size = 0;
//            string rtn = string.Empty;

//            using (var ctx = ZmqContext.Create())
//            {
//                using (ZmqSocket pub = ctx.CreateSocket(SocketType.PUB), req = ctx.CreateSocket(SocketType.PULL))
//                {

//                    debug("SenderID:" + this.SenderId);
//                    debug("Pub Socket Connect To:" + pub_addr);
//                    pub.Connect(pub_addr);
//                    //pub.Subscribe(Encoding.UTF8.GetBytes(SenderId));
//                    pubchannel = pub;

//                    debug("Req Socket Connect To:" + sub_addr);
//                    req.Connect(sub_addr);

//                    //有任务进来时候进行调用
//                    req.ReceiveReady += (s, e) =>
//                    {
//                        data = req.Receive(data, out size);
//                        //debug("Got request:"+data.ToAsciiString());
//                        Request request = Request.Parse(data);
//                        //this.Reply(request, "");
                        
//                        //如果web端request发送过来的请求是disconnect  则不用在返回客户端，则本地逻辑处理客户端断开即可，如果服务端需要主动让mongrel2断开某个链接，则我们主动向该Request 发送 "" 则表示断开该链接
//                        if (request.IsDisconnect)
//                        {
//                            debug(string.Format("ConnId:{0} Disconnected",request.ConnId));
//                        }
//                        else
//                        {
//                            HandleRequest(this, request);
//                        }
//                    };

//                    var poller = new Poller(new List<ZmqSocket> { req });
//                    running = true;
//                    while (handlergo)
//                    {
//                        try
//                        {
//                            poller.Poll();
//                            if (!handlergo)
//                            {
//                                req.Close();
//                                pub.Close();
//                            }


//                        }
//                        catch (Exception ex)
//                        {

//                        }
//                    }
//                    running = false; 
//                }
//            }
//        }

//        public void Release()
//        {
//            if (!handlergo) return;
//            handlergo = false;
//            int mainwait = 0;
//            while (handlerthread.IsAlive && mainwait < 10)
//            {
//                Thread.Sleep(1000);
//                mainwait++;
//            }
//        }
//        /// <summary>
//        /// 停止链接处理线程
//        /// </summary>
//        public void Stop()
//        {
//            if (!handlergo) return;
//            handlergo = false;

//            int mainwait = 0;
//            while (running && mainwait < 10)
//            {
//                //debug(string.Format("#{0} wait mainthread stopping....", mainwait), QSEnumDebugLevel.INFO);
//                Thread.Sleep(1000);
//                mainwait++;
//            }
//        }


//        /*
//        private void sendProc()
//        {
//            byte[] buffer = new byte[0];
//            int size = 0;

//            using (var ctx = ZmqContext.Create())
//            {
//                using (var rep = ctx.CreateSocket(SocketType.REP))
//                {
//                    buffer = rep.Receive(buffer, out size);
//                }
//            }

//            ZmqSocket resp = CTX.CreateSocket(SocketType.PUB);
//            resp.Connect(pub_addr);
//            resp.Subscribe(Encoding.UTF8.GetBytes(SenderId));

//            while (isRunning)
//            {
//                itemsReadyToSend.WaitOne();
//                lock (sendQ)
//                {
//                    while (sendQ.Count != 0)
//                    {
//                        byte[] stuffToSend = sendQ.Dequeue();

//                        bool sentOk = false;
//                        while (!sentOk)
//                        {
//                            try
//                            {
//                                resp.Send(stuffToSend);
//                                sentOk = true;
//                            }
//                            catch (ZmqException ex)
//                            {
//                                if (ex.Errno == (int)ZMQ.ERRNOS.EAGAIN)
//                                {
//                                    sentOk = false;
//                                }
//                                else
//                                {
//                                    throw ex;
//                                }
//                            }
//                        }
//                    }
//                }
//            }

//            resp.Dispose();
//            itemsReadyToSend.Close();
//            Interlocked.Decrement(ref threadStillRunning);
//        }

//        private void recvProc()
//        {
            

//            if (!isRunning)
//                throw new ObjectDisposedException("Connection");

//            ZmqSocket reqs = CTX.CreateSocket(SocketType.PULL);
//            reqs.Connect(sub_addr);

//            while (isRunning)
//            {
//                foreach (byte[] data in reqs.rece)
//                {
//                    recvQ.Enqueue(data);
//                    itemsReadyToRecv.Set();
//                }
//                Thread.Sleep(1);
//            }

//            reqs.Dispose();
//            itemsReadyToRecv.Close();
//            Interlocked.Decrement(ref threadStillRunning);
//        }

//        public Request Receive()
//        {

//            byte[] data = null;

//            while (data == null)
//            {
//                if (!isRunning)
//                    throw new ObjectDisposedException("Connection");

//                if (recvQ.Count != 0)
//                {
//                    lock (recvQ)
//                    {
//                        if (recvQ.Count != 0)
//                            data = recvQ.Dequeue();
//                    }
//                }
//                else
//                {
//                    itemsReadyToRecv.WaitOne(1);
//                }
//            }

//            return Request.Parse(data);
//        }
//        **/

//        #region 底层数据发送
//        /// <summary>
//        /// 底层数据发送
//        /// </summary>
//        /// <param name="uuid"></param>
//        /// <param name="conn_id"></param>
//        /// <param name="msg"></param>
//        /// <param name="offset"></param>
//        /// <param name="length"></param>
//        public void Send(string uuid, string conn_id, byte[] msg, int offset, int length)
//        {
//            lock (pubchannel)
//            {
//                if (!handlergo)
//                    throw new ObjectDisposedException("Connection");

//                string header = string.Format("{0} {1}:{2}, ", uuid, conn_id.Length, conn_id);
//                byte[] headerBytes = Enc.GetBytes(header);
//                byte[] data = new byte[headerBytes.Length + length];
//                Array.Copy(headerBytes, data, headerBytes.Length);
//                Array.Copy(msg, offset, data, headerBytes.Length, length);


//                this.pubchannel.Send(data);
//                //debug("Send Reply:" + data.ToUTF8String());
//            }
//        }

//        public void Reply(Request req, byte[] msg, int offset, int length)
//        {
//            this.Send(req.Sender, req.ConnId, msg, offset, length);
//        }

//        public void Reply(Request req, ArraySegment<byte> msg)
//        {
//            this.Send(req.Sender, req.ConnId, msg.Array, msg.Offset, msg.Count);
//        }



//        /// <summary>
//        /// byte[] 发送
//        /// </summary>
//        /// <param name="uuid"></param>
//        /// <param name="conn_id"></param>
//        /// <param name="msg"></param>
//        public void Send(string uuid, string conn_id, byte[] msg)
//        {
//            Send(uuid, conn_id, msg, 0, msg.Length);
//        }

//        /// <summary>
//        /// 向对应的一批idents发送数据
//        /// </summary>
//        /// <param name="uuid"></param>
//        /// <param name="idents"></param>
//        /// <param name="data"></param>
//        public void Deliver(string uuid, string[] idents, byte[] data)
//        {
//            Send(uuid, string.Join(" ", idents), data);
//        }


//        /// <summary>
//        /// string 发送
//        /// </summary>
//        /// <param name="uuid"></param>
//        /// <param name="conn_id"></param>
//        /// <param name="msg"></param>
//        public void Send(string uuid, string conn_id, string msg)
//        {
//            Send(uuid, conn_id, Enc.GetBytes(msg));
//        }

//        public void Reply(Request req, byte[] msg)
//        {
//            this.Send(req.Sender, req.ConnId, msg);
//        }

//        public void Reply(Request req, string msg)
//        {
//            this.Send(req.Sender, req.ConnId, msg);
//        }
//        #endregion


//        #region HttpResponse 拼接
//        private const string HTTP_FORMAT = "HTTP/1.1 {0} {1}\r\n{2}\r\n";
//        /// <summary>
//        /// 将数据拼接成httpresponse并进行返回
//        /// </summary>
//        /// <param name="body"></param>
//        /// <param name="code"></param>
//        /// <param name="status"></param>
//        /// <param name="headers"></param>
//        /// <returns></returns>
//        private byte[] httpResponse(ArraySegment<byte> body, int code, string status, Dictionary<string, string> headers)
//        {
//            try
//            {
//                var bodyBytes = body;
//                if (headers == null)
//                    headers = new Dictionary<string, string>();
//                headers["Content-Length"] = bodyBytes.Count.ToString();
//                var formattedHeaders = new StringBuilder();

//                if (headers != null)
//                {
//                    foreach (var kvp in headers)
//                    {
//                        formattedHeaders.AppendFormat("{0}: {1}\r\n", kvp.Key, kvp.Value);
//                    }
//                }

//                var header = string.Format(HTTP_FORMAT, code, status, formattedHeaders);
//                var headerBytes = Enc.GetBytes(header);
//                var ret = new byte[bodyBytes.Count + headerBytes.Length];
//                Array.Copy(headerBytes, ret, headerBytes.Length);
//                Array.Copy(bodyBytes.Array, bodyBytes.Offset, ret, headerBytes.Length, bodyBytes.Count);
//                return ret;
//            }
//            catch (Exception ex)
//            {
//                debug("httpresponse error:" + ex.ToString());
//                return new byte[0];
//            }
//        }


//        private byte[] httpResponse(byte[] body, int code, string status,Dictionary<string, string> headers)
//        {
//            return httpResponse(new ArraySegment<byte>(body), code, status, headers);
//        }

//        private byte[] httpResponse(string body, int code, string status,Dictionary<string, string> headers)
//        {
//            return httpResponse(Enc.GetBytes(body), code, status, headers);
//        }
//        #endregion


//        #region Http发送
//        public void ReplyHttp(Request req, string body, int code = 200, string status = "OK", Dictionary<string, string> headers = null)
//        {
//            var thingToSend = httpResponse(body, code, status, headers);
//            this.Reply(req, thingToSend);
//        }

//        public void ReplyHttp(Request req, byte[] body, int code=200, string status="OK",Dictionary<string, string> headers=null)
//        {
//            var thingToSend = httpResponse(body, code, status, headers);
//            this.Reply(req, thingToSend);
//        }

//        public void ReplyHttp(Request req, ArraySegment<byte> body, int code=200, string status="OK",Dictionary<string, string> headers=null)
//        {
//            var thingToSend = httpResponse(body, code, status, headers);
//            this.Reply(req, thingToSend);
//        }
//        #endregion


//        public void Dispose()
//        {

//        }
//    }
//}
