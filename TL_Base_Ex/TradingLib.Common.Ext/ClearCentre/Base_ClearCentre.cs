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


        #region 【IGotTradingInfo】昨日持仓 委托 成交 取消 Tick数据处理
        //注为了记录隔夜尺长 分账户与总账户的隔夜持仓要单独放置即要体现在当前持仓总又要体现在隔夜持仓中
        /// <summary>
        /// 清算中心获得持仓数据
        /// </summary>
        /// <param name="p"></param>
        public void GotPosition(Position p)
        {
            debug("got postioin:" + p.ToString(), QSEnumDebugLevel.INFO);
            if (!HaveAccount(p.Account)) return;
            Symbol symbol = p.oSymbol;
            if (symbol == null)
            {
                debug("symbol:" + p.Symbol + " not exist in basictracker, dropit",QSEnumDebugLevel.ERROR);
                return;
            }
            debug("account tracker got position", QSEnumDebugLevel.INFO);
            acctk.GotPosition(p);
            totaltk.GotPosition(p);
            onGotPosition(p);
        }

        public virtual void onGotPosition(Position p)
        {

        }
        
        /// <summary>
        /// 清算中心获得委托数据
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(Order o)
        {
            try
            {
                if (!HaveAccount(o.Account)) return;
                Symbol symbol = o.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + o.symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }

                bool neworder = !totaltk.IsTracked(o.id);
                acctk.GotOrder(o);
                totaltk.GotOrder(new OrderImpl(o));

                //o.Filled = OrdBook[o.Account].Filled(o.id);//获得委托成交
                onGotOrder(o, neworder);
            }
            catch (Exception ex)
            {
                debug("处理委托异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        public virtual void onGotOrder(Order o, bool neworder)
        {
        }
        
        /// <summary>
        /// 清算中心获得取消
        /// </summary>
        /// <param name="oid"></param>
        public void GotCancel(long oid)
        {
            try
            {
                string account = SentOrder(oid).Account;
                if (!HaveAccount(account)) return;
                acctk.GotCancel(account, oid);
                totaltk.GotCancel(oid);

                onGotCancel(oid);
            }
            catch (Exception ex)
            {
                debug("处理取消异常:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        public virtual void onGotCancel(long oid)
        {

        }

        /// <summary>
        /// 清算中心获得成交
        /// </summary>
        /// <param name="f"></param>
        public void GotFill(Trade f)
        {
            try
            {
                if (!HaveAccount(f.Account)) return;
                Symbol symbol = f.oSymbol;
                if (symbol == null)
                {
                    debug("symbol:" + f.symbol + " not exist in basictracker, dropit", QSEnumDebugLevel.ERROR);
                    return;
                }
                bool positionside = f.PositionSide;
                    
                PositionTransaction postrans = null;
                Position pos = acctk.GetPosition(f.Account, f.symbol, positionside);
                int beforesize = pos.UnsignedSize;
                decimal highest = pos.Highest;
                decimal lowest = pos.Lowest;

                acctk.GotFill(f);

                int aftersize = acctk.GetPosition(f.Account, f.symbol, positionside).UnsignedSize;//查询该成交后数量
                decimal c = -1;

                //debug("got fill beforesize:" + beforesize.ToString() + " aftersize:" + aftersize.ToString(), QSEnumDebugLevel.INFO);
                //当成交数据中f.commission<0表明清算中心没有计算手续费,若>=0表明已经计算过手续费 则不需要计算了
                if (f.Commission < 0)
                {
                    decimal commissionrate = 0;
                    //成交后持仓数量大于成交前数量 开仓或者加仓
                    if (aftersize > beforesize)
                    {
                        commissionrate = symbol.EntryCommission;
                    }
                    //成交后持仓数量小于成交后数量 平仓或者减仓
                    if (aftersize < beforesize)
                    {
                       
                        //如果对应的合约是单边计费的或者有特殊计费方式的合约，则我们单独计算该部分费用,注这里还需要加入一个日内交易的判断,暂时不做(当前交易均为日内)
                        //获得平仓手续费特例
                        if (CommissionHelper.AnyCommissionSetting(SymbolHelper.genSecurityCode(f.symbol), out commissionrate))
                        {
                            //debug("合约:" + SymbolHelper.genSecurityCode(f.symbol) + "日内手续费费差异", QSEnumDebugLevel.MUST);
                        }
                        else//没有特殊费率参数,则为标准的出场费率
                        {
                            commissionrate = symbol.ExitCommission;
                        }
                    }

                    f.Commission = Calc.CalCommission(commissionrate, f);
                }

                //生成持仓操作记录 同时结合beforeszie aftersize 设置fill PositionOperation,需要知道帐户的持仓信息才可以知道是开 加 减 平等信息
                f.PositionOperation = PositionTransaction.GenPositionOperation(beforesize, aftersize);
                postrans = new PositionTransaction(f, symbol, beforesize, aftersize, highest, lowest);

                totaltk.GotFill(f);

                //子类函数的onGotFill用于执行数据记录以及其他相关业务逻辑
                onGotFill(f, postrans);
                
            }
            catch (Exception ex)
            {
                debug("Got Fill error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }
        public virtual void onGotFill(Trade fill, PositionTransaction postrans)
        {
        }

        //得到新的Tick数据
        public void GotTick(Tick k)
        {
            try
            {
                acctk.GotTick(k);
                totaltk.GotTick(k);
            }
            catch (Exception ex)
            {
                debug("Got Tick error:" + ex.ToString());
            }
        }
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
            //6.账户反向引用clearcentre用于计算财务信息
            onCacheAccount(a);
            //a.ClearCentre = new ClearCentreAdapterToAccount(a as IAccount,this);//this;// as IAccountClearCentre);
        }

        public virtual void onCacheAccount(IAccount a)
        { 
        
        }
        #endregion



    }
}
