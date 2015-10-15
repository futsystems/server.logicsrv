using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Common
{
    /// <summary>
    /// 清算中心，为服务器维护了一批交易账户,以及每个交易账户的实时Order,trades,position的相关信息。
    /// </summary>
    public abstract partial class ClearCentreBase : BaseSrvObject
    {
        /// <summary>
        /// 分帐户交易数据维护器
        /// </summary>
        protected AccountTracker acctk = new AccountTracker();

        /// <summary>
        /// 总交易数据维护器
        /// </summary>
        protected TotalTracker totaltk = new TotalTracker();



        public ClearCentreBase(string name = "ClearCentreBase")
            : base(name)
        {
            acctk.NewPositionEvent += new Action<Position>(acctk_NewPositionEvent);
            acctk.NewPositionCloseDetailEvent += new Action<Trade,PositionCloseDetail>(acctk_NewPositionCloseDetailEvent);
            acctk.NewPositionDetailEvent += new Action<Trade, PositionDetail>(acctk_NewPositionDetailEvent);
        }


        void acctk_NewPositionDetailEvent(Trade arg1, PositionDetail arg2)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[arg1.Account];
            if (account != null)
            {
                account.FirePositoinDetailEvent(arg1,arg2);
            }
        }

        /// <summary>
        /// 当有持仓关闭时出发持仓关闭时间
        /// </summary>
        /// <param name="obj"></param>
        void acctk_NewPositionCloseDetailEvent(Trade obj1,PositionCloseDetail obj2)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[obj1.Account];
            if (account != null)
            {
                account.FirePositionCloseDetailEvent(obj1, obj2);
            }
        }

        //当帐户交易对象维护器产生持仓时，我们将持仓加入total维护其列表用于快速访问
        void acctk_NewPositionEvent(Position obj)
        {
            logger.Info("new postion created " + obj.GetPositionKey());
            totaltk.NewPosition(obj);

        }

        #region 添加或删除交易帐户到清算服务的内存数据哭
        /// <summary>
        /// 将某个账户缓存到服务器内存，注意检查是否已经存在该账户
        /// 生成该账户所对应的数据对象用于实时储存交易信息与合约信息
        /// </summary>
        /// <param name="a"></param>
        public void CacheAccount(IAccount a)
        {
            acctk.CacheAccount(a);
        }

        /// <summary>
        /// 将某个帐户从内存中删除
        /// </summary>
        /// <param name="a"></param>
        public void DropAccount(IAccount a)
        {
            //将该交易帐户的委托 成交 持仓 从统计维护器中删除
            foreach (Order o in a.Orders)
            {
                totaltk.DropOrder(o);
            }

            foreach (Trade f in a.Trades)
            {
                totaltk.DropFill(f);
            }

            foreach (Position p in a.Positions)
            {
                totaltk.DropPosition(p);
            }
            //将帐户从帐户维护器中删除
            acctk.DropAccount(a);
        }

        /// <summary>
        /// 清空某个交易帐户的交易记录
        /// </summary>
        public void ResetAccount(IAccount a)
        {
            //将该交易帐户的委托 成交 持仓 从统计维护器中删除
            foreach (Order o in a.Orders)
            {
                totaltk.DropOrder(o);
            }

            foreach (Trade f in a.Trades)
            {
                totaltk.DropFill(f);
            }

            foreach (Position p in a.Positions)
            {
                totaltk.DropPosition(p);
            }

            //将交易帐户 交易记录维护器中该帐户的交易记录清空
            acctk.ResetAccount(a);
        }

        /// <summary>
        /// 清空交易帐户某个交易所的交易记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="exchange"></param>
        //public void ResetAccount(IAccount account, IExchange exchange)
        //{
        //    foreach (Order o in account.GetOrders(exchange))
        //    {
        //        totaltk.DropOrder(o);
        //    }

        //    foreach (Trade f in account.GetTrades(exchange))
        //    {
        //        totaltk.DropFill(f);
        //    }

        //    foreach (Position p in account.GetPositions(exchange))
        //    {
        //        totaltk.DropPosition(p);
        //    }

        //}
        #endregion


    }
}
