using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{
        internal class SendStruct
        {
            public SendStruct(IConnection conn, byte[] packet)
            {
                this.Connection = conn;
                this.Packet = packet;
                this.JsonPacket = null;
            }

            public SendStruct(IConnection conn,string jsonPacket)
            {
                this.Connection = conn;
                this.Packet = null;
                this.JsonPacket = jsonPacket;
            }



            public IConnection Connection { get; set; }

            public byte[] Packet { get; set; }

            public string JsonPacket { get; set; }
        }
        public partial class DataServer
        {


            /// <summary>
            /// 启动数据储存服务
            /// </summary>
            protected void StartSendService()
            {
                logger.Info("[Start SendWorker Service]");
                if (_sendgo) return;
                _sendgo = true;
                _sendthread = new Thread(ProcessSend);
                _sendthread.IsBackground = false;
                _sendthread.Start();
            }



            RingBuffer<SendStruct> sendbuffer = new RingBuffer<SendStruct>(50000);
            //const int SLEEPDEFAULTMS = 100;
            static ManualResetEvent _sendwaiting = new ManualResetEvent(false);
            Thread _sendthread = null;
            bool _sendgo = false;
            void NewSend()
            {
                if ((_sendthread != null) && (_sendthread.ThreadState == System.Threading.ThreadState.WaitSleepJoin))
                {
                    _sendwaiting.Set();
                }
            }

            void SendData(IConnection connection, IPacket packet)
            {
                this.SendData(connection, packet.Data);
            }

            void SendData(IConnection connection, byte[] data)
            {
                sendbuffer.Write(new SendStruct(connection, data));
                NewSend();
            }

            void SendData(IConnection connection, string json)
            {
                sendbuffer.Write(new SendStruct(connection, json));
                NewSend();
            }

            int getRequestId(IPacket packet)
            {
                if (packet is RspResponsePacket)
                {
                    return (packet as RspResponsePacket).RequestID;
                }
                return 0;
            }
            void ProcessSend()
            {
                SendStruct st = null;
                while (_sendgo)
                {
                    try
                    {
                        while (sendbuffer.hasItems)
                        {
                            try
                            {
                                st = sendbuffer.Read();
                                //在某个特定情况下 会出现 待发送数据结构为null的情况 多个线程对sendbuffer的访问 形成竞争
                                if (st == null)
                                {
                                    logger.Error("XXXX Send Buffer Got Null Struct");
                                    continue;
                                }
                                if (IsConnectionRegisted(st.Connection.SessionID))
                                {
                                    if(st.Packet != null)
                                        st.Connection.Send(st.Packet);
                                    if (st.JsonPacket != null)
                                        st.Connection.Send(st.JsonPacket);
                                }
                            }
                            catch (Exception ex)
                            {

                                logger.Error(string.Format("Conn:{0} Send Data:{1} Error:{2}", st.Connection.SessionID, st.Packet.ToString(), ex.ToString()));

                                logger.Error(string.Format("RequestID:{0} BufferSize:{1}", 0, sendbuffer.Count));

                                //数据发送异常后 关闭该Socket
                                if (st != null && st.Connection != null)
                                {
                                    CloseConnection(st.Connection);
                                }
                            }
                        }
                        // clear current flag signal
                        _sendwaiting.Reset();
                        //logger.Info("process send");
                        // wait for a new signal to continue reading
                        _sendwaiting.WaitOne(SLEEPDEFAULTMS);

                    }
                    catch (Exception ex)
                    {
                        logger.Error("SendWorker Process  error:" + ex.ToString());
                    }
                }
                
            }
        }
    
}
