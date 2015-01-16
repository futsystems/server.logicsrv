using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 基础交易帐户信息
    /// 用于传递交易帐户
    /// </summary>
    public class AccountLite
    {
        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 交易帐户类别
        /// </summary>
        public QSEnumAccountCategory Category { get; set; }

        /// <summary>
        /// 委托路由类别
        /// </summary>
        public QSEnumOrderTransferType OrderRouteType { get; set; }

        /// <summary>
        /// 当前交易状态
        /// </summary>
        public bool Execute { get; set; }

        /// <summary>
        /// 日内交易
        /// </summary>
        public bool IntraDay { get; set; }

        /// <summary>
        /// 上期权益
        /// </summary>
        public decimal LastEquity { get; set; }

        /// <summary>
        /// 当前权益
        /// </summary>
        public decimal NowEquity { get; set; }

        /// <summary>
        /// 平仓利润
        /// </summary>
        public decimal RealizedPL { get; set; }

        /// <summary>
        /// 未平仓利润
        /// </summary>
        public decimal UnRealizedPL { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 净利
        /// </summary>
        public decimal Profit { get; set; }

        /// <summary>
        /// 入金
        /// </summary>
        public decimal CashIn { get; set; }

        /// <summary>
        /// 出金
        /// </summary>
        public decimal CashOut { get; set; }

        /// <summary>
        /// 总占用资金 = 个品种占用资金之和
        /// </summary>
        public decimal MoneyUsed { get; set; }

        /// <summary>
        /// 帐户标识
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 交易帐号
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
        /// 锁仓权限
        /// </summary>
        public bool PosLock { get; set; }

        /// <summary>
        /// 单向大边
        /// </summary>
        public bool SideMargin { get; set; }

        /// <summary>
        /// 帐户所属管理员全局ID
        /// </summary>
        public int MGRID { get; set; }

        /// <summary>
        /// 是否已经删除
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// 路由组
        /// </summary>
        public int RG_ID { get; set; }

        /// <summary>
        /// 手续费模板ID
        /// </summary>
        public int Commissin_ID { get; set; }

        /// <summary>
        /// 是否处于登入状态
        /// </summary>
        public bool IsLogin { get; set; }

        /// <summary>
        /// 登入地址
        /// </summary>
        public string IPAddress { get; set; }



        public static string Serialize(AccountLite account)
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(account.Account);
            sb.Append(d);
            sb.Append(account.Category.ToString());
            sb.Append(d);
            sb.Append(account.OrderRouteType.ToString());
            sb.Append(d);
            sb.Append(account.Execute.ToString());
            sb.Append(d);
            sb.Append(account.IntraDay.ToString());
            sb.Append(d);
            sb.Append(account.LastEquity.ToString());
            sb.Append(d);
            sb.Append(account.NowEquity.ToString());
            sb.Append(d);
            sb.Append(account.RealizedPL.ToString());
            sb.Append(d);
            sb.Append(account.UnRealizedPL.ToString());
            sb.Append(d);
            sb.Append(account.Commission.ToString());
            sb.Append(d);
            sb.Append(account.Profit.ToString());
            sb.Append(d);
            sb.Append(account.CashIn.ToString());
            sb.Append(d);
            sb.Append(account.CashOut.ToString());
            sb.Append(d);
            sb.Append(account.MoneyUsed.ToString());
            sb.Append(d);
            sb.Append(account.Name);
            sb.Append(d);
            sb.Append(account.Broker);
            sb.Append(d);
            sb.Append(account.BankID);
            sb.Append(d);
            sb.Append(account.BankAC);
            sb.Append(d);
            sb.Append(account.PosLock.ToString());
            sb.Append(d);
            sb.Append(account.MGRID.ToString());
            sb.Append(d);
            sb.Append(account.Deleted.ToString());
            sb.Append(d);
            sb.Append(account.RG_ID);
            sb.Append(d);
            sb.Append(account.IsLogin);
            sb.Append(d);
            sb.Append(account.IPAddress);
            sb.Append(d);
            sb.Append(account.SideMargin);
            sb.Append(d);
            sb.Append(account.Commissin_ID);
            return sb.ToString();
        }

        public static AccountLite Deserialize(string msg)
        {
            string[] rec = msg.Split(',');
            AccountLite account = new AccountLite();
            account.Account = rec[0];
            account.Category = (QSEnumAccountCategory)Enum.Parse(typeof(QSEnumAccountCategory), rec[1]);
            account.OrderRouteType = (QSEnumOrderTransferType)Enum.Parse(typeof(QSEnumOrderTransferType), rec[2]);
            account.Execute = bool.Parse(rec[3]);
            account.IntraDay = bool.Parse(rec[4]);
            account.LastEquity = decimal.Parse(rec[5]);
            account.NowEquity = decimal.Parse(rec[6]);
            account.RealizedPL = decimal.Parse(rec[7]);
            account.UnRealizedPL = decimal.Parse(rec[8]);
            account.Commission = decimal.Parse(rec[9]);
            account.Profit = decimal.Parse(rec[10]);
            account.CashIn = decimal.Parse(rec[11]);
            account.CashOut = decimal.Parse(rec[12]);
            account.MoneyUsed = decimal.Parse(rec[13]);
            account.Name = rec[14];
            account.Broker = rec[15];
            account.BankID = int.Parse(rec[16]);
            account.BankAC = rec[17];
            account.PosLock = bool.Parse(rec[18]);
            account.MGRID = int.Parse(rec[19]);
            account.Deleted = bool.Parse(rec[20]);
            account.RG_ID = int.Parse(rec[21]);
            account.IsLogin = bool.Parse(rec[22]);
            account.IPAddress = rec[23];
            account.SideMargin = bool.Parse(rec[24]);
            account.Commissin_ID = int.Parse(rec[25]);
            return account;
        }
    }
}
