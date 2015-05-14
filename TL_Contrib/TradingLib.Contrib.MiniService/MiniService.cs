using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.MiniService
{
    public class MiniService : IAccountService
    {
        /// <summary>
        /// 数据库底层编号
        /// </summary>
        public int ID { get; set; }

        IAccount _account = null;
        public IAccount Account { get { return _account; } set { _account = value; } }


        MiniServiceSetting _setting = null;
        public MiniService(MiniServiceSetting ms)
        {
            this.ID = ms.ID;
            _setting = ms;
            this.Account = TLCtxHelper.CmdAccount[ms.Account];
            this.Account.BindService(this);
        }


        /// <summary>
        /// 是否允许交易某个合约 
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CanTradeSymbol(Symbol symbol, out string msg)
        {
            msg = string.Empty;

            Util.Debug("check mini symbol:" + symbol.Symbol);

            //通过所有检查
            return true;
        }


        /// <summary>
        /// 是否可以接受某个合约 保证金检查
        /// </summary>
        /// <param name="order"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CanTakeOrder(Order order, out string msg)
        {
            msg = string.Empty;

            //如果是平仓委托 则直接返回
            if (!order.IsEntryPosition) return true;

            //获得某个帐户交易某个合约的可用资金
            decimal avabile = GetFundAvabile(order.oSymbol);

            //可用资金大于需求资金则可以接受该委托
            decimal required = this.Account.CalOrderFundRequired(order, 0);

            Util.Debug("MinService Fundavabile:" + avabile.ToString() + " Required:" + required + " account avabile fund:" + this.Account.AvabileFunds.ToString());
            if (required > avabile)
            {
                msg = "资金不足";
                return false;
            }

            return true;
        }

        public int CanOpenSize(Symbol symbol,bool side,QSEnumOffsetFlag flag)
        {
            return 0;
        }

        public CommissionConfig GetCommissionConfig(Symbol symbol)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获得某个合约的可用资金
        /// 秘籍服务不对可用资金进行调整返回-1
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetFundAvabile(Symbol symbol)
        {
            //帐户当前可用资金即为所有额度， 在帐户可用额度计算时 加上了配资扩展的额度 见GetFinAmountAvabile()
            return this.Account.AvabileFunds;
        }


        public string SN { get { return "MiniService"; } }

        /// <summary>
        /// 服务是否可用
        /// 
        /// </summary>
        public bool IsAvabile
        {
            get
            {
                return this.IsValid && _setting.Active;
            }
        }

        /// <summary>
        /// 服务是否有效
        /// 无效服务不会被系统加载
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this.Account != null;
            }
        }

        public IEnumerable<string> GetNotice()
        {
            return new List<string>();
        }

    }
}
