using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using CTPService.Struct.V12;

namespace CTPService
{
    public partial class CTPServiceHost
    {
        /// <summary>
        /// 将逻辑服务端的消息进行处理 转换成ServiceHost支持协议内容
        /// 如果不需发送消息到客户端则返回null
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void HandleLogicMessage(FrontServer.IConnection connection, IPacket packet)
        {
            try
            {
                CTPConnection conn = GetConnection(connection.SessionID);
                if (conn == null)
                {
                    logger.Warn(string.Format("Session:{0} CTPConnection do not exist", connection.SessionID));
                    return;
                }
                switch (packet.Type)
                {
                    //登入回报
                    case MessageTypes.LOGINRESPONSE:
                        {
                            LoginResponse response = packet as LoginResponse;
                            //将数据转换成CTP业务结构体
                            LCThostFtdcRspUserLoginField field = new LCThostFtdcRspUserLoginField();
                            field.TradingDay = response.TradingDay.ToString();
                            field.MaxOrderRef = "1";
                            field.UserID = response.LoginID;
                            field.SystemName = "FutsSystems";
                            field.FrontID = response.FrontIDi;
                            field.SessionID = response.SessionIDi;
                            field.BrokerID = "88888";

                            string time = DateTime.Now.ToString("HH:mm:ss");
                            field.LoginTime = time;
                            field.SHFETime = time;
                            field.DCETime = time;
                            field.CZCETime = time;
                            field.FFEXTime = time;
                            field.INETime = time;

                            LCThostFtdcRspInfoField rsp = new LCThostFtdcRspInfoField();
                            rsp.ErrorID = response.RspInfo.ErrorID;
                            rsp.ErrorMsg = string.Format("CTP:{0}", response.RspInfo.ErrorMessage);

                            //打包数据
                            byte[] data = StructHelperV12.FillRsp<LCThostFtdcRspUserLoginField>(ref rsp, ref field, EnumSeqType.SeqReq, EnumTransactionID.T_RSP_LOGIN, 1, response.RequestID);

                            //发送数据
                            conn.Send(data);
                            break;
                        }
                    default:
                        logger.Warn(string.Format("Logic Packet:{0} not handled", packet.ToString()));
                        break;
                }

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Handler Logic Packet:{0} Error:{1}", packet.ToString(), ex.ToString()));
            }
        }
    }
}
