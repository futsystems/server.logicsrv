using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 交易帐户对象
    /// </summary>
    public partial class AccountBase : IAccount
    {
        
        public AccountBase(string AccountID)
        {
            _id = AccountID;
            this.Execute = true;
            this.IntraDay = true;
            this.Category = QSEnumAccountCategory.SUBACCOUNT;
            this.OrderRouteType = QSEnumOrderTransferType.SIM;
            this.MAC = string.Empty;
            this.Name = string.Empty;
            this.Broker = string.Empty;
            this.BankID = 0;
            this.BankAC = string.Empty;
            this.CreatedTime = DateTime.Now;
            this.SettleDateTime = DateTime.Now;
            this.SettlementConfirmTimeStamp = Util.ToTLDateTime();
            //this.PosLock = false;
            //this.SideMargin = true;
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
        /// 硬件地址
        /// </summary>
        public string MAC { get; set; }

        /// <summary>
        /// 交易帐户名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 期货公司名称
        /// </summary>
        public string Broker { get; set; }

        /// <summary>
        /// 银行
        /// </summary>
        public int BankID { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        public string BankAC { get; set; }

        /// <summary>
        /// 记录账户的建立时间
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 上次结算日
        /// </summary>
        public DateTime SettleDateTime { get; set; }

        /// <summary>
        /// 最近结算确认日期
        /// </summary>
        public long SettlementConfirmTimeStamp { get; set; }



        /// <summary>
        /// 代理商ID
        /// </summary>
        public int Mgr_fk { get; set; }

        /// <summary>
        /// 路由组ID 用于将某个帐户绑定到某个路由组上面,然后这组用户下单就会下单路由组内的成交接口上
        /// </summary>
        public int RG_FK { get; set; }

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

        /// <summary>
        /// 域ID
        /// </summary>
        public Domain Domain { get; internal set; }

        /// <summary>
        /// 与交易帐号所绑定的全局UserID
        /// </summary>
        public int UserID { get; set; }


        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool isValid { get { return (ID != null) && (ID != ""); } }
        

        public override string ToString()
        {
            return "帐号:"+ID+" 类型:"+Category.ToString();
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }


        /// <summary>
        /// 判断2个帐户是否相同
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            AccountBase o = (AccountBase)obj;
            return base.Equals(o);
        }
        public bool Equals(AccountBase a)
        {
            return this._id.Equals(a.ID);
        }




        /// <summary>
        /// 重置账户状态,用于每日造成开盘时,重置数据 
        /// </summary>
        public void Reset()
        {
            //出入金归零
            _cashin = 0;
            _cashout = 0;
            _creditcashin = 0;
            _creditcashout = 0;

            //清空账户附加的规则 用于重新加载帐户规则
            ClearAccountCheck();
            ClearOrderCheck();
            _rulitemloaded = false;
        }

        public bool Deleted { get; set; }


        public string DisplayString
        {
            get
            {
                string re = "ID:" + this.ID + " 昨日权益:" + this.LastEquity.ToString() + " 当前权益:" + this.NowEquity.ToString() + " 总委托:" + this.Orders.Count().ToString() + " 总成交:" + this.Trades.Count().ToString();
                return re;

            }
        }
    }

    

}
