using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;

using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace DataClient
{
    public class TLSocket_TCP:TLSocketBase
    {

        protected ILog logger = LogManager.GetLogger("TLSocket_TCP");

        Socket _socket;

        bool _recvgo = false;
        Thread _recvThread = null;


        

        bool _connected = false;
        public override bool IsConnected { get { return _connected; } }

        


        public override RspQryServiceResponse QryService(QSEnumAPIType apiType, string version)
        {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(this.Server);
            QryServiceRequest request = RequestTemplate<QryServiceRequest>.CliSendRequest(0);
            request.APIType = apiType;
            request.APIVersion = version;

            byte[] nrequest = Message.sendmessage(request.Type,request.Content);
            s.Send(nrequest);

            byte[] tmp = new byte[s.ReceiveBufferSize];
            int len = s.Receive(tmp);
            byte[] data = new byte[len];
            Array.Copy(tmp, 0, data, 0, len);
            Message message = Message.gotmessage(data);

            RspQryServiceResponse response = null;
            if (message.isValid && message.Type == MessageTypes.SERVICERESPONSE)
            {
                response = ResponseTemplate<RspQryServiceResponse>.CliRecvResponse(message.Content);

            }
            return response;
        }


        public override void Send(byte[] msg)
        {
            if (_socket == null)
                throw new InvalidOperationException("Socket is null");
            if (_socket.Connected)
            {
                _socket.Send(msg);
            }
        }


        public override  void Connect()
        {
            _socket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Connect(this.Server);

            if (_socket.Connected)
            {
                buffer = new byte[_socket.ReceiveBufferSize];
                bufferoffset = 0;

                StartRecv();

                _connected = true;
            }

            
        }

        public override void Disconnect()
        {
            if (_socket != null && _socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Disconnect(true);
                _connected = false;
            }

            StopRecv();
        }


        void StartRecv()
        {
            if (_recvgo) return;
            logger.Info("Start recv thread");
            _recvgo = true;
            _recvThread = new Thread(RecvProcess);
            _recvThread.IsBackground = true;
            _recvThread.Start();

        }

        void StopRecv()
        {
            if (!_recvgo) return;
            logger.Info("stop recv thread");
            _recvgo = false;
            Util.WaitThreadStop(_recvThread);

        }

        const int BUFFERSIZE = 1024;
        byte[] buffer;
        int bufferoffset = 0;

        void RecvProcess()
        {
            while (_recvgo)
            {
                int ret = _socket.Receive(buffer, bufferoffset, buffer.Length - bufferoffset, SocketFlags.None);
                if (ret > 0)
                {
                    //logger.Debug("socket recv bytes:" + ret + "raw data:" + HexToString(buffer, ret));
                    Message[] messagelist = Message.gotmessages(ref buffer, ref ret);
                    foreach (var msg in messagelist)
                    {
                        HandleMessage(msg);
                    }
                }
                else if (ret == 0) // socket was shutdown
                {
                    _connected = IsSocketConnected(_socket);
                    if (!_connected)
                    {
                        StopRecv();
                    }
                }               
            }
        }

        bool IsSocketConnected(Socket client) 
        { 
            int err;
            return IsSocketConnected(client, out err); 
        }

        bool IsSocketConnected(Socket client, out int errorcode)
        {
            bool blockingState = client.Blocking;
            errorcode = 0;
            try
            {
                byte[] tmp = new byte[1];

                client.Blocking = false;
                client.Send(tmp, 0, 0);
            }
            catch (SocketException e)
            {
                // 10035 == WSAEWOULDBLOCK
                if (e.NativeErrorCode.Equals(10035))
                {
                    logger.Info("connected but send blocked.");
                    return true;
                }
                else
                {
                    errorcode = e.NativeErrorCode;
                    logger.Info("disconnected, error: " + errorcode);
                    return false;
                }
            }
            finally
            {
                client.Blocking = blockingState;
            }
            return client.Connected;
        }


        public static string HexToString(byte[] buf, int len)
        {
            string Data1 = "";
            string sData = "";
            int i = 0;
            while (i < len)
            {
                Data1 = buf[i++].ToString("X").PadLeft(2, '0');
                sData += Data1;
            }
            return sData;
        }





        
           

    }
    
}
