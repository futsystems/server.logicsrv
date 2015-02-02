using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace Lottoqq.Account
{
    public class AccountImpl:AccountBase
    {
        public AccountImpl(string accountid)
            : base(accountid)
        { 
            
        }

        /// <summary>
        /// 如果有配资服务，则调用服务的手续费参数，如果没有给定参数则通过返回默认的手续费参数
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public override CommissionConfig GetCommissionConfig(Symbol symbol)
        {
            CommissionConfig cfg = null;
            IAccountService service = null;
            //如果有配资服务 则调用配资服务的手续费设置
            if (GetService("FinService", out service))
            {
                cfg= service.GetCommissionConfig(symbol);
            }
            return cfg!=null?cfg:base.GetCommissionConfig(symbol);
        }
        /// <summary>
        /// 判断帐户是否有权交易某个合约
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool CanTakeSymbol(Symbol symbol, out string msg)
        {
            IAccountService service = null;
            msg = string.Empty;
            bool re = true;
            //异化证券
            if (symbol.SecurityType == SecurityType.INNOV)
            {
                //如果是LOTTO类型的证券,则需要检查是否具备秘籍服务,并且调用秘籍服务的CanTradeSymbol来判定是否有资格交易该合约
                if (symbol.SecurityFamily.Code.Equals("LOTTO"))
                {
                    //检查秘籍服务是否可以交易该合约
                    service = null;
                    if (GetService("MJService", out service))
                    {
                        re = re && service.CanTradeSymbol(symbol, out msg);
                        return re;
                    }
                    else
                    {
                        msg ="帐户无乐透服务,无法交易乐透期权!";
                        return false;
                    }
                }
            }
            if (GetService("FinService", out service))
            {
                return service.CanTradeSymbol(symbol, out msg);
            }

            //其他未判断的情况通过AccountBase进行判断输出
            return base.CanTakeSymbol(symbol, out msg);
        }

        /// <summary>
        /// 判断资金是否允许提交某个委托
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override bool CanFundTakeOrder(Order o, out string msg)
        {
            IAccountService service = null;

            //如果有配资服务 则调用配资服务的保证金检查机制进行处理
            if (GetService("FinService", out service))
            {
                return service.CanTakeOrder(o, out msg);
            }
            return base.CanFundTakeOrder(o, out msg);
        }

        /// <summary>
        /// 返回可以开仓手数
        /// 乐透合约保证金是按合约设定的Margin和ExtraMargin进行计算
        /// 冻结保证金也是按照固定金额进行冻结
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public override int  CanOpenSize(Symbol symbol, bool side, QSEnumOffsetFlag flag)
        {
            IAccountService service = null;
            //如果是异化合约,则我们按照异化合约的保证金直接进行计算可开手数
            if (symbol.SecurityType == SecurityType.INNOV)
            {
                decimal fundavabile = GetFundAvabile(symbol);

                decimal fundperlot = decimal.MaxValue;

                if (symbol.Margin > 0)
                {
                    fundperlot = symbol.Margin + (symbol.ExtraMargin > 0 ? symbol.ExtraMargin : 0);
                }

                Util.Debug("Fundavabile:" + fundavabile.ToString() + " fundperlot:" + fundperlot.ToString());
                return (int)(fundavabile / fundperlot);
            }
            //配资服务通过配资服务查询可开手数
            if (GetService("FinService", out service))
            {
                return service.CanOpenSize(symbol,side,flag);
            }

            return base.CanOpenSize(symbol,side,flag);
        }

        /// <summary>
        /// 计算某个委托需要的资金 用于计算资金占用 或 保证金检查中的资金检查
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override decimal CalOrderFundRequired(Order o,decimal defaultvalue=0)
        {
            return base.CalOrderFundRequired(o,defaultvalue);
        }

        public override decimal GetFundAvabile(Symbol symbol)
        {
            IAccountService service = null;
            if (GetService("RaceService", out service))
            {
                //如果比赛服务有效 则受到比赛资金的限制
                if(service.IsAvabile)
                    return service.GetFundAvabile(symbol);
            }
            

            return base.GetFundAvabile(symbol);
        }
    }
}
