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

namespace TradingLib.Common.DataFarm
{
        internal class SendStruct
        {
            public SendStruct(IConnection conn, IPacket packet)
            {
                this.Connection = conn;
                this.Packet = packet;
            }
            public IConnection Connection { get; set; }

            public IPacket Packet { get; set; }
        }
        public partial class DataServerBase
        {


            /// <summary>
            /// 启动数据储存服务
            /// </summary>
            protected void StartSendService()
            {
                if (_sendgo) return;
                _sendgo = true;
                _sendthread = new Thread(ProcessSend);
                _datathread.IsBackground = false;
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
                sendbuffer.Write(new SendStruct(connection, packet));
                NewSend();
            }

            
            void ProcessSend()
            {
                SendStruct st = null; 
                while (_sendgo)
                {
                    while (sendbuffer.hasItems)
                    {
                        try
                        {
                            st = sendbuffer.Read();
                            st.Connection.Send(st.Packet);
                        }
                        catch (Exception ex)
                        {

                            logger.Error(string.Format("Conn:{0} Send Data:{1} Error:{2}", st.Connection.SessionID, st.Packet.ToString(), ex.ToString()));

                        }
                    }
                    // clear current flag signal
                    _sendwaiting.Reset();
                    //logger.Info("process send");
                    // wait for a new signal to continue reading
                    _sendwaiting.WaitOne(SLEEPDEFAULTMS);

                }
            }
        }
    
}
