//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;
//using TradingLib.Common;

//namespace TradingLib.Common
//{
//    public class ClearCentreMgr:ClearCentreBase
//    {
//        //将管理client传递个clearcentre则远程的clearcentre可以通过调用client的通讯机制来操控交易服务器进行相应操作
//        //private MgrClient _mgrcli;
//        //public MgrClient Mgrcli { get { return _mgrcli; } set { _mgrcli = value; } }


//        public ClearCentreMgr()
//            : base()
//        {
//            //this.DefaultOrderTracker.SendDebugEvent +=new TradeLink.API.DebugDelegate(debug);
//            PROGRAME = "ClearCentreMgr";
//        }


//        public IAccount this[int uid]
//        {
//            get
//            {
//                return null;
//            }
//        }
//        /// <summary>
//        /// 将某个Account加载到该清算中心
//        /// </summary>
//        /// <param name="a"></param>
//        //public void LoadAccount(IAccount a)
//        //{
//        //    if (!HaveAccount(a.ID))
//        //        CacheAccount(a);
//        //}

//        //public string AddRaceAccount(string user_id, string pass) { return ""; }
//        /// <summary>
//        /// 更新账户状态
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="type"></param>
//        public override void UpdateAccountRouterTransferType(string id, QSEnumOrderTransferType type)
//        {
//            _mgrcli.UpdateAccountRouterTransferType(id, type);
//            this[id].OrderRouteType = type;
//        }
//        /// <summary>
//        /// 更新账户类别
//        /// </summary>
//        /// <param name="id"></param>
//        /// <param name="ca"></param>
//        public override void UpdateAccountCategory(string id, QSEnumAccountCategory ca)
//        {
//            _mgrcli.UpdateAccountCategory(id, ca);
//            this[id].Category = ca;
//        }

//        /// <summary>
//        /// 激活某个交易账户
//        /// </summary>
//        /// <param name="id"></param>
//        public override void ActiveAccount(string id)
//        {
//            _mgrcli.ActiveAccount(id);
//            this[id].Execute = true;
//        }

//        /// <summary>
//        /// 禁止某个交易账户
//        /// </summary>
//        /// <param name="id"></param>
//        public override void InactiveAccount(string id)
//        {
//            _mgrcli.InactiveAccount(id);
//            this[id].Execute = false;
//        }

//        /// <summary>
//        /// 对某个账户进行出入金
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="ammount"></param>
//        /// <param name="comment"></param>
//        public override void CashOperation(string id, decimal ammount, string comment)
//        {
//            IAccount acc = this[id];
//            _mgrcli.CashOperation(acc.ID, ammount, comment);
//            if (ammount > 0)
//                acc.Deposit(ammount);
//            else
//                acc.Withdraw(ammount);
//        }
//        public override void ChangeAccountPass(string acc, string pass)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 更新账户购买乘数
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="buymultiplier"></param>
//        //public override void UpdateAccountBuyMultiplier(string acc, int buymultiplier)
//        //{
//        //    _mgrcli.UpdateAccountBuyMultiplier(acc, buymultiplier);
//        //    this[acc].BuyMultiplier = buymultiplier;
//        //}

//        //public override void FlatPosition(string accid,QSEnumOrderSource source, string comment)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public override void FlatPosition(Position pos, QSEnumOrderSource source, string comment)
//        //{
//        //    throw new NotImplementedException();
//        //}
//        public override void onCacheAccount(IAccount a)
//        {
//            //a.ClearCentre = new ClearCentreAdapterToAccountMgr(a, this);
//        }
//        #region 实现IManager接口

//        /// <summary>
//        /// 清空某个账户下所有的委托检查
//        /// </summary>
//        /// <param name="accid"></param>
//        public void ClearOrderCheck(string accid)
//        {
//            _mgrcli.ClearOrderCheck(accid);

//        }

//        /// <summary>
//        /// 为某个账户增加一条委托检查
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        public void AddOrderCheck(string accid, IOrderCheck rc)
//        {
//            _mgrcli.AddOrderCheck(accid, rc);
//        }

//        /// <summary>
//        /// 为某个账户删除一条委托检查
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        public void DelOrderCheck(string accid, IOrderCheck rc)
//        {
//            _mgrcli.DelOrderCheck(accid, rc);
            
//        }
//        /// <summary>
//        /// 清空账户检查规则
//        /// </summary>
//        /// <param name="accid"></param>
//        public void ClearAccountCheck(string accid)
//        {
//            _mgrcli.ClearAccountCheck(accid);
//        }

//        /// <summary>
//        /// 增加账户检查规则
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        public void AddAccountCheck(string accid, IAccountCheck rc)
//        {
//            _mgrcli.AddAccountCheck(accid, rc);
//        }

//        /// <summary>
//        /// 删除账户检查规则
//        /// </summary>
//        /// <param name="accid"></param>
//        /// <param name="rc"></param>
//        public void DelAccountCheck(string accid, IAccountCheck rc)
//        {
//            _mgrcli.DelAccountCheck(accid, rc);
//        }

//        #endregion
//        /// <summary>
//        /// 资金复位
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="value"></param>
//        public override void ResetEquity(string account, decimal value)
//        {
//            _mgrcli.ResetEquity(account, value);
//            IAccount acc = this[account];
//            decimal ammount = value - acc.NowEquity;
//            if (ammount > 0)
//                acc.Deposit(ammount);
//            else
//                acc.Withdraw(ammount);
//        }
//        /// <summary>
//        /// 修改账户日内交易属性
//        /// </summary>
//        /// <param name="acc"></param>
//        /// <param name="intraday"></param>
//        public override void UpdateAccountIntradyType(string acc, bool intraday)
//        {
//            _mgrcli.UpdateAccountIntradyType(acc, intraday);
//            this[acc].IntraDay = intraday;
//        }

//        /// <summary>
//        /// 上传某个账户的合约列表
//        /// </summary>
//        /// <param name="account"></param>
//        public void UploadAccountSecurityTable(string account)
//        {
//            _mgrcli.UploadAccountSecurityTable(account);
//            //this[account].UpdateMasterSecurity();
//        }
//        /// <summary>
//        /// 现在账户合约列表
//        /// </summary>
//        /// <param name="account"></param>
//        public void DownloadAccountSecurityTable(string account)
//        {
//            _mgrcli.DownloadAccountSecurityTable(account);
//        }


//        public void DownloadDefaultSecurity()
//        {
//            _mgrcli.DownloadDefaultSecurity();
//        }

//        public void UploadDefaultSecurity()
//        {
//            _mgrcli.UploadDefaultSecurity();
//        }
//        //查询账户注册信息
//        public void QryAccountProfile(string account)
//        {
//            _mgrcli.QryAccountProfile(account);
//        }
//        //查询账户比赛信息
//        public void QryAccountRaceInfo(string account)
//        {
//            _mgrcli.QryAccountRaceInfo(account);
            
//        }
//        //回补账户交易记录
//        public void ResuemAccount(string account)
//        {
//            IAccount acc = this[account];
//            if(acc ==null) return;
//            //清空记录
//            //DefaultPosTracker.Clear();
//            //DefaultOrdTracker.Clear();
//            //DefaultTradeTracker.Clear();
//            //PositionsHold.Clear();

//            acctk.GetOrderBook(acc.ID).Clear();
//            acctk.GetTradeBook(acc.ID).Clear();
//            acctk.GetPositionBook(acc.ID).Clear();
//            acctk.GetPositionHoldBook(acc.ID).Clear();
//            //请求回补记录
//            _mgrcli.ResuemAccount(account);
//        }
//        //设定账户观察列表
//        public void SetWatchList(string acclist)
//        {
//            _mgrcli.SetWatchList(acclist);
//        }

//        public void QryAccountInfo(string account)
//        {
//            _mgrcli.QryAccountInfo(account);
//        }
//        #region IReport 接口

//        public void QryDailySummary(string acc)
//        {
//            //_mgrcli.QryDailySummary(acc);
//        }


//        public void RequestRaceStatistic()
//        {
//            _mgrcli.RequestRaceStatistic();
//        }
//        #endregion



//    }
//}
