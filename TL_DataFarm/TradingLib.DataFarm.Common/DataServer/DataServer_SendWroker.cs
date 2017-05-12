using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;
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
            /* 数据发送服务 
             * 历史数据部分通过SendTLData SendXLData对外发送，内部数据处理后 最后通过_SendData(IConnection connection, byte[] data)发送
             * 实时行情部分优化后直接通过_SendData(IConnection connection, byte[] data)进行发送
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * 
             * */

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

            /// <summary>
            /// 发送IPacket类型的数据包
            /// </summary>
            /// <param name="connection"></param>
            /// <param name="packet"></param>
            void SendTLData(IConnection connection, IPacket packet)
            {
                
                byte[] data = packet.Data;
                this._SendData(connection, data);

                //统计业务数据包发送次数与大小
                switch (packet.Type)
                {
                        //Bar
                    case MessageTypes.BIN_BARRESPONSE:
                        {
                            dfStatistic.BarDataSendCnt++;
                            dfStatistic.BarDataSendSize += data.Length;
                            break;
                        }
                        //分笔
                    case MessageTypes.XQRYTRADSPLITRESPONSE:
                        {
                            dfStatistic.TradeSplitSendCnt++;
                            dfStatistic.TradeSplitSendSize += data.Length;
                            break;
                        }
                        //PriceVol
                    case MessageTypes.XQRYPRICEVOLRESPONSE:
                        {
                            dfStatistic.PriceVolSendCnt++;
                            dfStatistic.PriceVolSendSize += data.Length;
                            break;
                        }
                    case MessageTypes.XQRYMINUTEDATARESPONSE:
                        {
                            dfStatistic.MinuteDataSendCnt++;
                            dfStatistic.MinuteDataSendSize += data.Length;
                            break;
                        }
                    default:
                        {
                            dfStatistic.OtherPktSendCnt++;
                            dfStatistic.OtherPktSendSize += data.Length;
                            break;
                        }
                }
            }

            /// <summary>
            /// 发送XL数据包
            /// </summary>
            /// <param name="conn"></param>
            /// <param name="pkt"></param>
            /// <param name="seqType"></param>
            /// <param name="seqNo"></param>
            /// <param name="requestId"></param>
            /// <param name="islast"></param>
            void SendXLData(IConnection conn, XLPacketData pkt,XLEnumSeqType seqType,uint seqNo,uint requestId, bool islast)
            {
                if (conn.FrontType == EnumFrontType.XLTinny)
                {
                    byte[] ret = XLPacketData.PackToBytes(pkt, seqType, seqNo, requestId, islast);
                    _SendData(conn, ret);
                }
                if (conn.FrontType == EnumFrontType.WebSocket)
                {
                    string json = XLPacketData.PackJsonResponse(pkt, (int)requestId, islast);
                    _SendData(conn, json);
                }
                //增加数据统计
            }

            void _SendData(IConnection connection, byte[] data)
            {
                sendbuffer.Write(new SendStruct(connection, data));
                NewSend();
            }

            void _SendData(IConnection connection, string json)
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
