using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public partial class ClearCentreBase
    {

        #region 【IAccountTradingInfo】从内存获得相关数据

        /// <summary>
        /// 检查某个账户是否有暴露的仓位
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        public bool AnyPosition(string accid)
        {
            return acctk.AnyPosition(accid);
        }

        #region 获得交易账号对应的交易信息

        //委托 持仓 成交 取消信息
        /// <summary>
        /// 获得昨日持仓数据
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Position[] getPositionHold(string account)
        {
            if (string.IsNullOrEmpty(account))
            {
                return totaltk.PositionHoldTracker.ToArray();
            }
            return acctk.GetPositionHold(account);
        }

        /// <summary>
        /// 获得某个账户的所有持仓
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public Position[] getPositions(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
            {
                return totaltk.PositionTracker.ToArray();
            }
            return acctk.GetPositions(accountID);
        }

        /// <summary>
        /// 返回净持仓
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Position[] getNetPositions(string account)
        {
            return acctk.GetNetPositions(account);
        }


        /// <summary>
        /// 获得某个交易账户当天所有的委托
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public Order[] getOrders(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
            {
                return totaltk.OrderTracker.ToArray();
            }
            return acctk.GetOrders(accountID);
        }
        /// <summary>
        /// 获得某个交易账户的成交数据
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public Trade[] getTrades(string accountID)
        {
            if (string.IsNullOrEmpty(accountID))
            {
                return totaltk.TradeTracker.ToArray();
            }
            return acctk.GetTrades(accountID);
        }
        /// <summary>
        /// 获得某个交易账户的取消委托数据
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public long[] getCancels(string accountID)
        {
            return acctk.GetCancels(accountID);
        }



        #endregion


        /// <summary>
        /// 获得交易帐号上次结算持仓数量
        /// </summary>
        public int getPositionHoldSize(string account, string symbol)
        {
            foreach (Position pos in totaltk.PositionHoldTracker)
            {
                if (pos.Account == account && pos.Symbol == symbol)
                    return pos.Size;
            }
            return 0;
        }

        /// <summary>
        /// 通过AccountID,symbol返回Position
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position getPosition(string account, string symbol, bool side)
        {
            return acctk.GetPosition(account, symbol, side);
        }

        /// <summary>
        /// 净持仓
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position getPosition(string account, string symbol)
        {
            return acctk.GetPosition(account, symbol);

        }
        /// <summary>
        /// 获得某个交易帐户 某个合约 某个持仓方向的待成交数量
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="positionside"></param>
        /// <returns></returns>
        public int GetPendingEntrySize(string account,string symbol,bool positionside)
        {
            return acctk.GetOrderBook(account).GetPendingEntrySize(symbol, positionside);    
        }
        public int GetPendingExitSize(string account, string symbol, bool positionside)
        {
            return acctk.GetOrderBook(account).GetPendingExitSize(symbol, positionside);
        }

        public bool HaveLongPosition(string account)
        {
            return acctk.HaveLongPosition(account);
        }

        public bool HaveShortPosition(string account)
        {
            return acctk.HaveShortPosition(account);
        }
        /// <summary>
        /// 获得某个委托对应的Account symbol下 所持仓位的反向为成交委托,用于CTP发送限价委托时检查仓位情况
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        //public int getUnfilledSizeExceptStop(Order o)
        //{
        //    //多头仓位 查询卖委托 空头仓位 查询买委托 (买 卖方向与仓位方向相反,则为未成交的 平仓委托)
        //    //OrdBook[o.Account].getUnfilledSizeExceptStop(o.symbol, !this.getPosition(o).isLong);
        //    return 0;
        //    //return (this.getOrderTracker(o.Account) as OrderTracker).getUnfilledSizeExceptStop(o.symbol, !this.getPosition(o.Account,o.symbol).isLong);

        //}

        ///// <summary>
        ///// 获得某个持仓的未成交平仓委托数量
        ///// 
        ///// </summary>
        ///// <param name="o"></param>
        ///// <returns></returns>
        //public int getUnfilledPositionFlatOrderSize(Position pos)
        //{
        //    return acctk.GetOrderBook(pos.Account).getUnfilledSize(pos.Symbol, !pos.isLong);
        //}

        /// <summary>
        /// 获得某个账户的所有待成交合约
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public long[] getPendingOrders(string account)
        {
            return acctk.GetOrderBook(account).getPending();
        }
        /// <summary>
        /// 返回某个委托对应账户与合约下所有与该委托方向相同其他委托,用于提交委托前取消同向委托
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public long[] getPendingOrders(Order o)
        {
            long[] olist = this.getPendingOrders(o.Account, o.symbol, o.side);//(this.getOrderTracker(o.Account) as OrderTracker).getPending(o.symbol, o.side);
            List<long> nlist = new List<long>();
            foreach (long oid in olist)
            {
                if (oid != o.id)
                    nlist.Add(oid);
            }
            return nlist.ToArray();
        }
        /// <summary>
        /// 返回某个账户 某个合约 某个方向的待成交委托
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <returns></returns>
        public long[] getPendingOrders(string account, string symbol, bool side)
        {
            return acctk.GetOrderBook(account).getPending(symbol, side);
        }
        /// <summary>
        /// 获得某个账户 某个合约的所有待成交委托
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public long[] getPendingOrders(string account, string symbol)
        {
            return acctk.GetOrderBook(account).getPending(symbol);
        }
        #endregion

    }
}
