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
    public abstract partial class ClearCentreBase : BaseSrvObject, IClearCentreBase
    {
        

        #region 获得某个合约的当前价格信息
        public event GetSymbolTickDel newSymbolTickRequest;
        protected Tick getSymbolTick(string symbol)
        {
            if (newSymbolTickRequest != null)
                return newSymbolTickRequest(symbol);
            else
                return null;
        }
        /// <summary>
        /// 获得某个合约的有效价格
        /// 如果返回-1则价格无效
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetAvabilePrice(string symbol)
        {
            Tick k = getSymbolTick(symbol);//获得当前合约的最新数据
            if (k == null) return -1;

            decimal price = somePrice(k);

            //如果价格有效则返回价格 否则返回-1无效价格
            return price > 0 ? price : -1;
        }

        /// <summary>
        /// 从Tick数据采获当前可用的价格
        /// 优先序列 最新价/ ask / bid 如果均不可用则返回价格0
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private decimal somePrice(Tick k)
        {
            if (k.isTrade)
                return k.trade;
            if (k.hasAsk)
                return k.ask;
            if (k.hasBid)
                return k.bid;
            else
                return -1;
        }
        #endregion


        #region 数据结构

        protected AccountTracker acctk = new AccountTracker();

        protected TotalTracker totaltk = new TotalTracker();
        #endregion


        public ClearCentreBase(string name = "ClearCentreBase")
            : base(name)
        {
            acctk.NewPositionEvent += new Action<Position>(acctk_NewPositionEvent);
        }

        //当帐户交易对象维护器产生持仓时，我们将持仓加入total维护其列表用于快速反问
        void acctk_NewPositionEvent(Position obj)
        {
            Util.Debug("new postion created " + obj.GetPositionKey(), QSEnumDebugLevel.MUST);
            totaltk.NewPosition(obj);
        }


        #region 【IAccountOperation】修改账户相应设置,查询可开,修改密码,验证交易账户,出入金,激活禁止交易账户等
        
        /// <summary>
        /// 激活某个交易账户 允许其进行交易
        /// </summary>
        /// <param name="id"></param>
        public abstract void ActiveAccount(string id);
        /// <summary>
        /// 禁止某个账户进行交易
        /// </summary>
        /// <param name="id"></param>
        public abstract void InactiveAccount(string id);
        /// <summary>
        /// 修改账户类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ca"></param>
        public abstract void UpdateAccountCategory(string id, QSEnumAccountCategory ca);

        /// <summary>
        /// 更改账户密码
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public abstract void ChangeAccountPass(string acc, string pass);

        /// <summary>
        /// 更新账户购买乘数
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="buymup"></param>
        //public abstract void UpdateAccountBuyMultiplier(string acc, int buymultiplier);

        /// <summary>
        /// 修改账户交易转发类别
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="type"></param>
        public abstract void UpdateAccountRouterTransferType(string id, QSEnumOrderTransferType type);

        /// <summary>
        /// 平调某个账户所有持仓
        /// </summary>
        /// <param name="accid"></param>
        //public abstract void FlatPosition(string accid,QSEnumOrderSource source, string comment);


        /// <summary>
        /// 平掉某个仓为
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        //public abstract void  FlatPosition(Position pos, QSEnumOrderSource source, string comment);
        /// <summary>
        /// 交易账户的资金操作
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        public abstract void CashOperation(string acc, decimal amount,string transref, string comment);

        /// <summary>
        /// 复位某个账户的资金到多少数值
        /// </summary>
        /// <param name="account"></param>
        /// <param name="value"></param>
        public abstract void ResetEquity(string account, decimal value);

        /// <summary>
        /// 更新账户日内交易设置
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="intraday"></param>
        public abstract void UpdateAccountIntradyType(string acc, bool intraday);
        
        #endregion


        #region 【IClearCentreBase】交易账户 操作
        /// <summary>
        /// 获得Account数组
        /// </summary>
        /// <returns></returns>
        public IAccount[] Accounts
        {
            get { return acctk.Accounts; }//AcctList.Values.ToArray(); }
        }
        /// <summary>
        /// 通过AccountID获得某个账户
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        public IAccount this[string accid]
        {
            get
            {
                return acctk[accid];
            }
        }

        /// <summary>
        /// 查找某个userid的交易帐户
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public IAccount QryAccount(int uid,QSEnumAccountCategory category)
        {
            return acctk.QryAccount(uid, category);
        }


        /// <summary>
        /// 查询是否有某个ID的账户
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool HaveAccount(string a)
        {
            return acctk.HaveAccount(a);
        }
        /// <summary>
        /// 查询是否有某个ID的账户并返回该账户
        /// </summary>
        /// <param name="a"></param>
        /// <param name="acc"></param>
        /// <returns></returns>
        public bool HaveAccount(string a, out IAccount acc)
        {
            acc = null;
            return acctk.HaveAccount(a, out acc);
        }

        /// <summary>
        /// 将某个账户缓存到服务器内存，注意检查是否已经存在该账户
        /// 生成该账户所对应的数据对象用于实时储存交易信息与合约信息
        /// </summary>
        /// <param name="a"></param>
        protected void CacheAccount(IAccount a)
        {
            acctk.CacheAccount(a);
        }

        /// <summary>
        /// 将某个帐户从内存中删除
        /// </summary>
        /// <param name="a"></param>
        protected void DropAccount(IAccount a)
        {
            acctk.DropAccount(a);
        }
        #endregion



    }
}
