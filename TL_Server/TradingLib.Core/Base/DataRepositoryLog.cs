using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 用于存放异常数据操作 放入队列或序列化到文件
    /// 在储存系统可用时执行数据储存操作
    /// 重要的交易数据通过异步数据储存组件进行存储
    /// 数据包含:委托,成交,委托操作,平仓明细,出入金记录,（结算）持仓明细,交易所结算记录,以及相关记录的结算标识更新
    /// 这些数据需要放到异步队列中储存，当数据储存出现异常时要有第二个备选方案用于储存记录，防止数据丢失
    /// </summary>
    public class DataRepositoryLog
    {
        public DataRepositoryLog()
        {
            this.RepositoryType = EnumDataRepositoryType.Unknown;
            this.RepositoryData = null;
        }
        public DataRepositoryLog(EnumDataRepositoryType type, object data)
        {
            this.RepositoryType = type;
            this.RepositoryData = data;
        }

        /// <summary>
        /// 数据操作类别
        /// </summary>
        public EnumDataRepositoryType RepositoryType { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object RepositoryData { get; set; }

        /// <summary>
        /// 序列化DataRepositoryLog
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public static string Serialize(DataRepositoryLog log)
        {
            StringBuilder sb = new StringBuilder();
            bool nulldata = false;
            sb.Append(log.RepositoryType);
            sb.Append(" ");
            switch (log.RepositoryType)
            { 
                case EnumDataRepositoryType.InsertOrder:
                case EnumDataRepositoryType.UpdateOrder:
                    Order o = log.RepositoryData as Order;
                    if (o != null)
                    {
                        sb.Append(OrderImpl.Serialize(o));
                        break;
                    }
                    nulldata = true;
                    break;
                case EnumDataRepositoryType.InsertTrade:
                    Trade f = log.RepositoryData as Trade;
                    if (f != null)
                    {
                        sb.Append(TradeImpl.Serialize(f));
                        break;
                    }
                    nulldata = true;
                    break;
                case EnumDataRepositoryType.InsertOrderAction:
                    OrderAction a = log.RepositoryData as OrderAction;
                    if (a != null)
                    {
                        sb.Append(OrderActionImpl.Serialize(a));
                        break;
                    }
                    nulldata = true;
                    break;
                case EnumDataRepositoryType.InsertCashTransaction:
                    CashTransaction c = log.RepositoryData as CashTransaction;
                    if (c != null)
                    {
                        sb.Append(CashTransactionImpl.Serialize(c));
                        break;
                    }
                    nulldata = true;
                    break;
                case EnumDataRepositoryType.InsertPositionCloseDetail:
                    PositionCloseDetail pc = log.RepositoryData as PositionCloseDetail;
                    if (pc != null)
                    {
                        sb.Append(PositionCloseDetailImpl.Serialize(pc));
                        break;
                    }
                    nulldata = true;
                    break;
                case EnumDataRepositoryType.InsertPositionDetail:
                    PositionDetail pd = log.RepositoryData as PositionDetail;
                    if(pd!=null)
                    {
                        sb.Append(PositionDetailImpl.Serialize(pd));
                        break;
                    }
                    nulldata = true;
                    break;
                case EnumDataRepositoryType.InsertExchangeSettlement:
                    ExchangeSettlement s = log.RepositoryData as ExchangeSettlement;
                    if(s!=null)
                    {
                        sb.Append(ExchangeSettlementImpl.Serialize(s));
                        break;
                    }
                    nulldata = true;
                    break;
                default:
                    nulldata = true;
                    break;
            }
            if(nulldata) return string.Empty;
            return sb.ToString();
        }

        public DataRepositoryLog Deserialize(string message)
        {
            string[] rec = message.Split(new char[','], 2);
            DataRepositoryLog log = new DataRepositoryLog();

            try
            {
                EnumDataRepositoryType type = (EnumDataRepositoryType)Enum.Parse(typeof(EnumDataRepositoryType), rec[0]);
                log.RepositoryType = type;
                switch (type)
                {
                    case EnumDataRepositoryType.InsertOrder:
                    case EnumDataRepositoryType.UpdateOrder:
                        Order o = OrderImpl.Deserialize(rec[1]);
                        log.RepositoryData = o;
                        break;
                    case EnumDataRepositoryType.InsertTrade:
                        Trade f = TradeImpl.Deserialize(rec[1]);
                        log.RepositoryData = f;
                        break;
                    case EnumDataRepositoryType.InsertOrderAction:
                        OrderAction a = OrderActionImpl.Deserialize(rec[1]);
                        log.RepositoryData = a;
                        break;
                    case EnumDataRepositoryType.InsertCashTransaction:
                        CashTransaction c = CashTransactionImpl.Deserialize(rec[1]);
                        log.RepositoryData = c;
                        break;
                    case EnumDataRepositoryType.InsertPositionCloseDetail:
                        PositionCloseDetail p = PositionCloseDetailImpl.Deserialize(rec[1]);
                        log.RepositoryData = p;
                        break;
                    case EnumDataRepositoryType.InsertPositionDetail:
                        PositionDetail d = PositionDetailImpl.Deserialize(rec[1]);
                        log.RepositoryData = d;
                        break;
                    case EnumDataRepositoryType.InsertExchangeSettlement:
                        ExchangeSettlement s = ExchangeSettlementImpl.Deserialize(rec[1]);
                        log.RepositoryData = s;
                        break;
                    default:
                        log.RepositoryData = null;
                        break;

                }
            }
            catch (Exception ex)
            { 
            
            }
            if (log.RepositoryData == null) return null;
            return log;
        }
    }
}
