using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using Common.Logging;

namespace TradingLib.Common
{
    /// <summary>
    /// 交易帐户对象
    /// </summary>
    public partial class AccountBase : IAccount
    {
        protected ILog logger = LogManager.GetLogger("Account");
        public AccountBase(string AccountID)
        {
            _id = AccountID;
            this.Execute = true;
            this.IntraDay = true;
            this.Category = QSEnumAccountCategory.SUBACCOUNT;
            this.OrderRouteType = QSEnumOrderTransferType.SIM;
            this.CreatedTime = DateTime.Now;
            this.SettleDateTime = DateTime.Now;
            this.SettlementConfirmTimeStamp = Util.ToTLDateTime();
            this.Mgr_fk = 0;
            this.UserID = 0;
            this.Deleted = false;
        
        }

        string _id = "";
        /// <summary>
        /// 交易帐户ID 9680001
        /// </summary>
        /// <value>The ID.</value>
        public string ID { get { return _id; } }

        /// <summary>
        /// 密码
        /// </summary>
        public string Pass { get; set; }

        /// <summary>
        /// 是否允许交易
        /// </summary>
        public bool Execute { get; set; }

        /// <summary>
        /// 是否处于警告状态
        /// </summary>
        public bool IsWarn { get; set; }


        /// <summary>
        /// 是否是日内交易
        /// </summary>
        public bool IntraDay { get; set; }

        /// <summary>
        /// 路由类别
        /// </summary>
        public QSEnumOrderTransferType OrderRouteType { get; set; }

        /// <summary>
        /// 交易帐户类比 模拟帐户，实盘帐户，交易员
        /// </summary>
        public QSEnumAccountCategory Category { get; set; }

        /// <summary>
        /// 交易账户货币
        /// </summary>
        public CurrencyType Currency { get; set; }

        /// <summary>
        /// 记录账户的建立时间
        /// </summary>
        public DateTime CreatedTime { get; set; }



        #region 模板编号
        /// <summary>
        /// 手续费模板ID
        /// </summary>
        public int Commission_ID { get; set; }

        /// <summary>
        /// 保证金模板ID
        /// </summary>
        public int Margin_ID { get; set; }


        /// <summary>
        /// 交易参数模板ID
        /// </summary>
        public int ExStrategy_ID { get; set; }
        #endregion

        #region 对象绑定
        /// <summary>
        /// 账户绑定路由组
        /// </summary>
        public RouterGroup RouteGroup { get; internal set; }

        /// <summary>
        /// 账户所在域
        /// </summary>
        public Domain Domain { get; internal set; }


        /// <summary>
        /// 账户User绑定 用于与其他系统用户进行关联
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 账户所属管理员
        /// </summary>
        public int Mgr_fk { get; set; }
        #endregion


        public override string ToString()
        {
            return string.Format("AC:{0} Type:{1}", this.ID, this.Category);
        }


        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            AccountBase target = obj as AccountBase;
            if (target == null) return false;
            if (this.ID == target.ID) return true;
            return false;
        }

        /// <summary>
        /// 重置账户状态,用于每日造成开盘时,重置数据 
        /// </summary>
        public void Reset()
        {
            this.LastEquity = 0;
            this.LastCredit = 0;
            //清空出入金与交易所结算数据
            settlementlist.Clear();
            cashtranslsit.Clear();
            //重置风控规则
            ClearAccountCheck();
            ClearOrderCheck();
            _ruleitemloaded = false;
        }

        public bool Deleted { get; set; }
    }

    

}
