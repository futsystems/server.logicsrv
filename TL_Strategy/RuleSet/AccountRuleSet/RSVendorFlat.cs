using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins;

namespace AccountRuleSet
{
    public class RSVendorFlat : RuleBase, IAccountCheck
    {
        /// <summary>
        /// 参数集【json格式】
        /// </summary>
        private string _args = string.Empty;

        /// <summary>
        /// 期初权益
        /// </summary>
        decimal lastEquity = 0;
        /// <summary>
        /// 强平比例
        /// </summary>
        int flatLevel = 10;
        /// <summary>
        /// 报警比例
        /// </summary>
        int warnLevel = 10;
        /// <summary>
        /// 过夜倍数
        /// </summary>
        decimal nightHold = 1;

        /// <summary>
        /// 报警金额
        /// </summary>
        decimal WarnEquity
        {
            get { return lastEquity * this.warnLevel / 100; }
        }

        /// <summary>
        /// 强平金额
        /// </summary>
        decimal FlatEquity
        {
            get { return lastEquity * this.flatLevel / 100; }
        }


        public override string Value
        {
            get { return _args.ToString(); }
            set
            {
                try
                {
                    _args = value;
                    //解析json参数
                    var args = TradingLib.Mixins.Json.JsonMapper.ToObject(_args);
                    lastEquity = decimal.Parse(args["equity"].ToString());//初始权益
                    warnLevel = int.Parse(args["warn_level"].ToString());//报警线
                    flatLevel = int.Parse(args["flat_level"].ToString());//强平线
                    nightHold = decimal.Parse(args["night_hold"].ToString());//过夜倍数



                }
                catch (Exception ex)
                {
                    Util.Error(string.Format("帐户{0}的风控规则{1}参数出错:{2}", this.Account.ID, Title, _args));
                }
            }
        }



        bool flatStart = false;//强平开始
        bool warnStart = false;//报警开始
        public bool CheckAccount(out string msg)
        {
            msg = string.Empty;
            decimal equity = this.Account.NowEquity;//获得该账户的当前权益
            
            //当前权益小于强平金额 执行强平操作
            if (equity < this.FlatEquity)
            {
                if (!flatStart)
                {
                    if (this.Account.Execute)
                        this.Account.InactiveAccount();//冻结账户
                    if (this.Account.AnyPosition)
                    {
                        msg = string.Format("{0}风控规则{1}[{2}]",this.Account.ID,Title,this.RuleDescription) + ":全平所有仓位并冻结账户";
                        this.Account.FlatPosition(QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);
                    }
                    flatStart = true;//开始平仓
                    return false;
                }
            }

            if (equity < this.WarnEquity)
            {
                //
                if (!warnStart)
                {
                    return false;
                }
            }
            return true;
        }

        public override string RuleDescription
        {
            get
            {
                return string.Format("强平:{0} 过夜:{1}",this.FlatEquity,this.nightHold);
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "强平[主帐户监控]"; }
        }
        public static new string Description
        {
            get { return "帐户权益满足相关条件进行强平或报警"; }
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public static new string ValueName { get { return "参数(Json)"; } }

        /// <summary>
        /// 不用设置比较关系
        /// </summary>
        public static new bool CanSetCompare { get { return false; } }

        /// <summary>
        /// 默认比较关系大于等于
        /// </summary>
        public static new QSEnumCompareType DefaultCompare { get { return QSEnumCompareType.Less; } }

        /// <summary>
        /// 不用设置品种集合
        /// </summary>
        public static new bool CanSetSymbols { get { return false; } }

        //用于验证客户端的输入值是否正确
        public static new bool ValidSetting(RuleItem item, out string msg)
        {
            try
            {
                decimal v = decimal.Parse(item.Value);
                if (v < 0)
                {
                    msg = "请去掉负号";
                    return false;
                }
                msg = "";
                return true;
            }
            catch (Exception ex)
            {
                msg = "请设定有效数值";
                return false;
            }

        }

        #endregion
    }
}
