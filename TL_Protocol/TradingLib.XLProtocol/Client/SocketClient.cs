using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
#if DEV
using Common.Logging;
#endif

namespace TradingLib.XLProtocol.Client
{

    

    public class SocketClient
    {
        #if DEV
        ILog logger = LogManager.GetLogger("SocketClient");
        #endif


        Socket _socket = null;
        Thread _thread = null;
        int _bufferSize = 65535;


        public event Action<XLProtocolHeader, byte[], int> DataReceived = delegate { };
        public event Action Connected = delegate { };
        public event Action Disconnected = delegate { };

        ManualResetEvent manualReset = new ManualResetEvent(false);

        /// <summary>
        /// 当前Socket是否处于连接状态
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return _socket != null;
            }
        }


        #region 后台维护线程
        DateTime _lastHeartbeatSent = DateTime.MinValue;

        int _watchWait = 1000;//心跳检测线程检测频率
        int _sendheartbeat =5000;//发送心跳请求间隔 距离上次心跳回应时间大于设定值 则向服务端请求心跳
        int _hbDeadTimeSpan = 15000;//心跳死亡间隔
        long _lastheartbeat = 0;//最后心跳时间

        bool _started = false;//后台检测连接状态线程是否启动
        bool _connect = false;//客户端是否连接到服务端
        bool _requestheartbeat = false;//请求心跳回复
        bool _recvheartbeat = false;//收到心跳回复
        bool _reconnectreq = false;//请求重新连接


        Thread _bwthread = null;
        void StartWatchDog()
        {
            if (_started) return;
            _bwthread = new Thread(_bw_DoWork);
            _bwthread.IsBackground = true;
            _bwthread.Start();
#if DEV
            logger.Info("Watcher backend thread started");
#endif
            Console.WriteLine("Watcher backend thread started");
        }

        void StopWatchDog()
        {
            if (!_started) return;
            _started = false;
            _bwthread.Join();
            _bwthread = null;
#if DEV
            logger.Info("Watcher backend thread stopped");
#endif
            Console.WriteLine("Watcher backend thread stopped");
        }

        bool IsHeartbeatOk { get { return _connect && (_requestheartbeat == _recvheartbeat); } }
        /// <summary>
        /// 心跳维护线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _bw_DoWork()
        {
            _started = true;
            while (_started)
            {
                // 计算上次heartbeat以来的时间间隔
                long now = DateTime.Now.Ticks;
                long diff = (now - _lastheartbeat) / 10000;//
#if DEV
                //logger.Info("连接:" + _connect.ToString() + " 请求重新连接:" + (_reconnectreq).ToString() + "心跳间隔"+(diff < _sendheartbeat).ToString()+" 上次心跳时间:" + _lastheartbeat.ToString() + " Diff:" + diff.ToString() + " 发送心跳间隔:" + _sendheartbeat.ToString());
#endif
                //服务端处于连接状态 服务端不处于重连状态 服务端心跳间隔小于设定间隔
                if (!(_connect && (!_reconnectreq) && (diff < _sendheartbeat)))//任何一个条件不满足将进行下面的操作
                {
                    try
                    {
                        //如果心跳当前状态正常,则请求一个心跳 请求后心跳状态处于非正常状态 不会再重复发送请求
                        if (IsHeartbeatOk)
                        {
#if DEV
                            //logger.Info("heartbeat request at: " + DateTime.Now.ToString() + " _heartbeatdeadat:" + _hbDeadTimeSpan.ToString() + " _diff:" + diff.ToString());
                            //当得到响应请求后,_recvheartbeat = !_recvheartbeat; 因此在发送了一个hearbeatrequest后 在没有得到服务器反馈前不会再次重新发送
#endif
                            RequestHeartBeat();
                        }
                        else if (diff > _hbDeadTimeSpan)//心跳间隔超时后,我们请求服务端的心跳回报,如果服务端的心跳响应超过心跳死亡时间,则我们尝试 重新建立连接
                        {
#if DEV
                            //logger.Info("xxxxxxxxxxxxxxx diff:" + diff.ToString() + " dead:" + _hbDeadTimeSpan.ToString());
#endif
                            StartReconnect();
                        }
                    }
                    catch (Exception ex)
                    {
#if DEV
                        logger.Error("Watch Dog Error:"+ex.ToString());
#endif
                    }
                }

                Thread.Sleep(_watchWait);//每隔多少秒检查心跳时间MS
            }
        }

#endregion


        /// <summary>
        /// 关闭Socket
        /// </summary>
        public void Close()
        {
#if DEV
            logger.Info("Socket Close");
#endif
            Console.WriteLine("Socket Close");
            //停止WatchDog
            StopWatchDog();

            //防止后台重连线程一直执行重连操作
            _reconnectreq = false;
            //关闭Socket
            SafeCloseSocket();
        }

        string SockErrorStr = string.Empty;

        ManualResetEvent TimeoutObject = new ManualResetEvent(false);


        List<IPEndPoint> remoteServer = new List<IPEndPoint>();

        Random random = new Random();

        public void RegisterServer(string server, int port)
        {
            remoteServer.Add(new IPEndPoint(IPAddress.Parse(server), port));
        }

        /// <summary>
        /// 连接到服务端并启动数据接受线程
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect()
        {


            //从服务端列表中随机选择一个服务器进行连接
            IPEndPoint server = remoteServer[random.Next(0, remoteServer.Count - 1)];
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Blocking = false;
            //socket.SendTimeout = 1000;
            //byte[] inValue = new byte[] { 1, 0, 0, 0, 0x88, 0x13, 0, 0, 0xd0, 0x07, 0, 0 };// 首次探测时间5 秒, 间隔侦测时间2 秒  
            //socket.IOControl(IOControlCode.KeepAliveValues, inValue, null);

#if DEV
            logger.Info("Socket Created Remote EndPoint:"+server.ToString());
#endif

                Console.WriteLine(string.Format("Socket Created Remote EndPoint:{0}", server.ToString()));

                TimeoutObject.Reset();
                try
                {
                    socket.BeginConnect(server, ConnectCallback, socket);
                }
                catch (Exception err)
                {
                    SockErrorStr = err.ToString();
                    return false;
                }
                if (TimeoutObject.WaitOne(5000, false))//直到timeout，或者TimeoutObject.set()  
                {
                    return _connect;
                }
                else
                {
#if DEV
                logger.Info("Socket Connect Time Out");
#endif
                    Console.WriteLine("Socket Connect Time Out");
                    //socket.Close();
                    /* mono环境
                     * BeginConnect会一直挂起 不会像windows中返回异常，
                     * 1.此时如果socket.close()就会触发BeginConnect完毕事件，且 ConnectCallback中 EndConnect异常 socket已释放
                     * 2.不执行操作 则等待服务端可连后，就会出现多个Socket连接到服务端的情况
                     * 后来发现 需要设置Socket.Blocking为false就可以解决上面的问题
                     * **/
                    _socket = null;
                    SockErrorStr = "Time Out";
                    return false;
                }
           
          
        }


        object lockObj_IsConnectSuccess = new object();

        void ConnectCallback(IAsyncResult iar)
        {
            lock (lockObj_IsConnectSuccess)
            {
                Socket client = (Socket)iar.AsyncState;
                try
                {
                    //mono环境下 Timeout 之后还会执行该调用
                    client.EndConnect(iar);
                   
#if DEV
                    logger.Info("Socket Connected Local EndPoint:" + client.LocalEndPoint.ToString() + " connected:"+client.Connected.ToString());
#endif
                    Console.WriteLine("Socket Connected Local EndPoint:" + client.LocalEndPoint.ToString() + " connected:"+client.Connected.ToString());

                    _socket = client;
                    UpdateServerHeartBeat();
                    _reconnectreq = false;//注通过Mod重新建立连接的过程中,连接线程会停止在 TLFound过程中，会一直等待服务器返回服务名
                    _recvheartbeat = true;
                    _requestheartbeat = true;
                    _connect = true;//连接建立标识

                    //对外触发连接建立事件
                    Connected();

                    //启动后台WatchDog
                    StartWatchDog();

                    BeginReceive(); //开始接收数据
                }
                catch (Exception e)
                {
#if DEV
                    logger.Error("ConnectCallBack Error:" + e.ToString());
#endif
                    Console.WriteLine("ConnectCallBack Error:" + e.ToString());
                    _socket = null; 
                    SockErrorStr = e.ToString();
                    _connect = false;
                }
                finally
                {
                    TimeoutObject.Set();
                }
            }
        }

        int bufferOffset = 0;
        byte[] buffer = null;
        void BeginReceive()
        {
            //重置缓存与便宜 重新建立Socket后执行BeginReceive需要重置 否则无法获取数据
            buffer = new byte[_bufferSize];
            bufferOffset = 0;
            _socket.BeginReceive(buffer, bufferOffset, buffer.Length - bufferOffset, SocketFlags.None, new AsyncCallback(OnReceiveCallback), _socket);
        }



        void OnReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket peerSock = (Socket)ar.AsyncState;
                int ret = peerSock.EndReceive(ar);
                //Console.WriteLine("************* Receive data cnt:" + ret.ToString());
                if (ret > 0)
                {
                    int dataLen = ret + bufferOffset;
                    int offset = 0;
                    bool parseFlag = true;
                    int pktLen = 0;
                    while (parseFlag)
                    {
                        if (dataLen - offset >= XLConstants.PROTO_HEADER_LEN)
                        {
                            XLProtocolHeader header = XLStructHelp.BytesToStruct<XLProtocolHeader>(buffer, offset);
                            //buffer包含了一个完整的协议数据包 
                            pktLen = XLConstants.PROTO_HEADER_LEN + header.XLMessageLength;
                            if (dataLen - offset >= pktLen)
                            {
                                //心跳包
                                if (header.XLMessageType == (int)XLMessageType.T_HEARTBEEAT)
                                {
                                    UpdateServerHeartBeat(); //只用心跳包 来更新Heartbeat时间
                                    _recvheartbeat = !_recvheartbeat;
#if DEV
                                    logger.Info("HeartBeat from server:" + DateTime.Now.ToString("HH:mm:ss"));
#endif
                                    Console.WriteLine("HeartBeat from server:" + DateTime.Now.ToString("HH:mm:ss"));
                                }
                                else
                                {
                                    //UpdateServerHeartBeat();
                                    DataReceived(header, buffer, offset + XLConstants.PROTO_HEADER_LEN);

                                }
                                offset += pktLen;
                            }
                            else //当前数据没有完整的包含一个协议数据包 则不进行解析
                            {
                                parseFlag = false;
                            }
                        }
                        else
                        {
                            //如果当前可用数据小于协议头长度 则不进行解析
                            parseFlag = false;
                        }

                        //将剩余数据复制到缓存中
                        if (!parseFlag)
                        {
                            byte[] pdata = new byte[dataLen - offset];
                            Array.Copy(buffer, offset, pdata, 0, dataLen - offset);
                            Array.Copy(pdata, 0, buffer, 0, dataLen - offset);
                            bufferOffset = dataLen - offset;
                        }
                    }

                    
                }
                else//对端gracefully关闭一个连接  
                {
                    if (_socket.Connected)//上次socket的状态  
                    {
                        //if (socketDisconnected != null)
                        //{
                        //    //1-重连  
                        //    socketDisconnected();
                        //    //2-退出，不再执行BeginReceive  
                        //    return;
                        //}
                        //if (working)
                        //{
                        //    Reconnect();
                        //}
                        return;
                    }
                }

                _socket.BeginReceive(buffer, bufferOffset, buffer.Length - bufferOffset, SocketFlags.None, new AsyncCallback(OnReceiveCallback), _socket);
            }
            catch (Exception ex)
            {
                
#if DEV
                logger.Error("Receive Callback Error:" + ex.ToString());
#endif
                Console.WriteLine("Receive Callback Error:" + ex.ToString());
                //WatchDog处于运行状态 则直接启动重连操作
                if (_started)
                {
                    StartReconnect();
                }
            }
        }


        void UpdateServerHeartBeat()
        {
            _lastheartbeat = DateTime.Now.Ticks;
        }


        /// <summary>
        /// 请求服务端心跳回报
        /// </summary>
        public void RequestHeartBeat()
        {
            //设置请求状态与接收状态相反 当收到心跳回报后将请求状态设置成接收状态
            _requestheartbeat = !_recvheartbeat;
            //发送请求心跳响应
            XLPacketData pktData = new XLPacketData(XLMessageType.T_HEARTBEEAT);
            byte[] data = XLPacketData.PackToBytes(pktData, XLEnumSeqType.SeqReq, 0, 0, true);
            Send(data, data.Length);
        }


        int retrynum = 10;
        /// <summary>
        /// 在后台线程中执行重连操作
        /// </summary>
        void StartReconnect()
        {

            if (_reconnectreq) return;
#if DEV
            logger.Info("Reconnect to server in background");
#endif
            Console.WriteLine("Reconnect to server in background");
            _reconnectreq = true;

            System.Threading.ThreadPool.QueueUserWorkItem((o) =>
            {
                bool ret = false;
                int cnt = 0;
                while (cnt < retrynum && !ret && _reconnectreq)
                {
                    if (cnt != 0)
                    {
                        Thread.Sleep(10000);
                    }
                    cnt ++;
#if DEV
                    logger.Info(string.Format("Connect to server #:{0}", cnt));
#endif
                    
                    //关闭Socket
                    SafeCloseSocket();
                    //连接Socket
                    ret = Connect();
                    Console.WriteLine(string.Format("Connect to server #:{0} ret:{1}", cnt,ret));
                }
            }
            );
        }

        /// <summary>
        /// 关闭Socket连接
        /// </summary>
        void SafeCloseSocket()
        {
            if (_socket != null)
            {
                //关闭socket  
                //_socket.Shutdown(SocketShutdown.Both);
                _socket.Disconnect(true);
                _connect = false;
                _socket.Close();
                _socket = null;
#if DEV
                logger.Info("Socket Closed");
#endif
                Console.WriteLine("Socket Closed");
                Disconnected();
            }
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Send(byte[] data, int count)
        {
            if (!IsOpen) return -1;
            if (_connect)
            {
                int ret = _socket.Send(data, 0, count, SocketFlags.None);
                return ret == count ? ret : -1;
            }
            return -1;
        }
    }




}
