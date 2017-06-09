using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MAgentSettlement : MBase
    {
        /// <summary>
        /// 插入代理结算账户结算记录
        /// 同时更新代理结算账户表的相关状态字段
        /// </summary>
        /// <param name="settle"></param>
        public static void InsertAgentSettlement(AccountSettlement settle)
        {
            using (DBMySql db = new DBMySql())
            {
                using (var transaction = db.Connection.BeginTransaction())
                {
                    bool istransok = true;
                    string query = string.Format("INSERT INTO log_agent_settlement (`account`,`settleday`,`closeprofitbydate`,`positionprofitbydate`,`commission`,`cashin`,`cashout`,`lastequity`,`equitysettled`,`lastcredit`,`creditsettled`,`creditcashin`,`creditcashout`,`assetbuyamount`,`assetsellamount`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", settle.Account, settle.Settleday, settle.CloseProfitByDate, settle.PositionProfitByDate, settle.Commission, settle.CashIn, settle.CashOut, settle.LastEquity, settle.EquitySettled, settle.LastCredit, settle.CreditSettled, settle.CreditCashIn, settle.CreditCashOut, settle.AssetBuyAmount, settle.AssetSellAmount);
                    istransok = istransok && (db.Connection.Execute(query) > 0);

                    query = string.Format("UPDATE agents SET lastequity = '{0}',lastcredit='{1}' ,settledtime= '{2}' WHERE account = '{3}'", settle.EquitySettled, settle.CreditSettled,Util.ToTLDateTime(), settle.Account);
                    istransok = istransok && (db.Connection.Execute(query) >= 0);
                    //如果所有操作均正确,则提交数据库transactoin
                    if (istransok)
                        transaction.Commit();

                }
            }


        }

    }
}
