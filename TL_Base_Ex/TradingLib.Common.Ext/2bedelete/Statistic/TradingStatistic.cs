using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /*
    public class TradingStatistic
    {
        IClearCentreBase _clearcentre;
        public TradingStatistic(IClearCentreBase c)
        {
            _clearcentre = c;
        }

        //返回所有账户数
        public int NumTotalAccount { get { return _clearcentre.Accounts.Length; } }
        //返回所有实盘数
        public int NumTotalAccountLive { get {

            int r = 0;
            foreach (IAccount a in _clearcentre.Accounts)
            {
                if (a.OrderRouteType == QSEnumOrderTransferType.LIVE)
                    r++;
            }
            return r;
        } }
        /// <summary>
        /// 返回委托数量
        /// </summary>
        public int NumOrders {
            get
            {
                return (_clearcentre.DefaultOrderTracker as OrderTracker).Count;
            }
        }
        /// <summary>
        /// 返回成交数量
        /// </summary>
        public int NumTrades
        {
            get
            {
                return _clearcentre.DefaultTradeList.Count;
            }
        }
        /// <summary>
        /// 当日取消数目
        /// </summary>
        public int NumCancels
        {
            get
            { 
                int r=0;
                foreach (Order o in (_clearcentre.DefaultOrderTracker as OrderTracker))
                {
                    if((_clearcentre.DefaultOrderTracker  as OrderTracker).isCanceled(o.id))
                        r++;
                }
                return r;
            }
        }
        /// <summary>
        /// 计算某个账户组合的所有权益
        /// </summary>
        /// <param name="accs"></param>
        /// <returns></returns>
        public decimal SumEquity(IAccount[] accs)
        { 
            decimal v = 0;
                foreach (IAccount a in accs)
                {
                        v = v + a.NowEquity;
                }
                return v;
        }
        /// <summary>
        /// 计算某组账户的平仓盈亏
        /// </summary>
        /// <param name="accs"></param>
        /// <returns></returns>
        public decimal SumRealizedPL(IAccount[] accs)
        {
            decimal v = 0;
            foreach (IAccount a in accs)
            {
                v = v + a.RealizedPL;
            }
            return v;
        }
        /// <summary>
        /// 计算某足账户的未平仓盈亏
        /// </summary>
        /// <param name="accs"></param>
        /// <returns></returns>
        public decimal SumUnRealizedPL(IAccount[] accs)
        {
            decimal v = 0;
            foreach (IAccount a in accs)
            {
                v = v + a.UnRealizedPL;
            }
            return v;
        }
        /// <summary>
        /// 计算某组账户的所有手续费
        /// </summary>
        /// <param name="accs"></param>
        /// <returns></returns>
        public decimal SumCommission(IAccount[] accs)
        {
            decimal v = 0;
            foreach (IAccount a in accs)
            {
                v = v + a.Commission;
            }
            return v;
        }
        /// <summary>
        /// 计算某组账户的净利
        /// </summary>
        /// <param name="accs"></param>
        /// <returns></returns>
        public decimal SumNetProfit(IAccount[] accs)
        {
            decimal v = 0;
            foreach (IAccount a in accs)
            {
                v = v + a.Profit;
            }
            return v;
        }
        /// <summary>
        /// 计算某组账户的保证金
        /// </summary>
        /// <param name="accs"></param>
        /// <returns></returns>
        public decimal SumMargin(IAccount[] accs)
        {
            decimal v = 0;
            foreach (IAccount a in accs)
            {
                v = v + a.Margin;
            }
            return v;
        }
        /// <summary>
        /// 计算某组账户的冻结保证金
        /// </summary>
        /// <param name="accs"></param>
        /// <returns></returns>
        public decimal SumFrozenMargin(IAccount[] accs)
        {
            decimal v = 0;
            foreach (IAccount a in accs)
            {
                v = v + a.ForzenMargin;
            }
            return v;
        }
        /// <summary>
        /// 返回所有实盘账户
        /// </summary>
        public IAccount[] LiveAccounts
        { 
            get
            {
                List<IAccount> l = new List<IAccount>();
                foreach (IAccount a in _clearcentre.Accounts)
                {
                    if (a.OrderRouteType == QSEnumOrderTransferType.LIVE)
                        l.Add(a);
                }
                return l.ToArray();
            }
        }
        /// <summary>
        /// 返回所有模拟账户
        /// </summary>
        public IAccount[] SimAccounts
        {
            get
            {
                List<IAccount> l = new List<IAccount>();
                foreach (IAccount a in _clearcentre.Accounts)
                {
                    if (a.OrderRouteType == QSEnumOrderTransferType.SIM)
                        l.Add(a);
                }
                return l.ToArray();
            }
        }

        //需要建立一定的逻辑利用方便的脚本语言来将交易账户分类,从而有效的区分盈利账户与亏损账户,然后利用相关指标
        //组合交易信号


        
        
        
        
        

    }
     * ***/
}
