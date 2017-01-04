using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace RuleSet2.Account
{
    public class RSEquity :RuleBase, IAccountCheck
    {
        /// <summary>
        /// 参数【json格式】
        /// </summary>
        private string _args = string.Empty;

        /// <summary>
        /// 权益 强平线
        /// </summary>
        decimal equity_flat = 0;

        /// <summary>
        /// 权益 报警线
        /// </summary>
        decimal equity_warn = 0;


        public override string Value
        {
            get { return _args; }
            set
            {

                try
                {
                    _args = value;
                    //解析json参数
                    var args = TradingLib.Mixins.Json.JsonMapper.ToObject(_args);
                    equity_flat = decimal.Parse(args["equity_flat"].ToString());//强平线
                    equity_warn = decimal.Parse(args["equity_warn"].ToString());//报警线

                    
                }
                catch (Exception ex)
                { }
            } 
        }



        bool flatStart = false;//强平触发
        bool iswarnning = false;//是否处于报警状态

        public bool CheckAccount(out string msg)
        {
            msg = string.Empty;
            decimal equity = this.Account.NowEquity;//获得该账户的当前权益

            if (equity_warn > 0)
            {
                iswarnning = this.Account.IsWarn;
                if (equity < equity_warn)
                {
                    if (!iswarnning)
                    {
                        iswarnning = true;
                        this.Account.Warn(true, "达到警告线");
                        Util.Debug("帐户警告开启~~~~~~~~~~~~~~~~~~~");
                    }
                }
                else
                {
                    if (iswarnning)
                    {
                        iswarnning = false;
                        this.Account.Warn(false, "");
                        Util.Debug("帐户警告关闭~~~~~~~~~~~~~~~~~~~");
                    }
                }
            }


            if (equity <= equity_flat)
            {
                if (!flatStart)
                {
                    if (this.Account.Execute)
                        this.Account.InactiveAccount();//冻结账户
                    if (this.Account.AnyPosition)
                    {
                        msg = RuleDescription + ":全平所有仓位并冻结账户";
                        this.Account.FlatAllPositions(QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);
                    }
                    flatStart = true;//开始平仓
                    return false;
                }
            }
            return true;
        }

        public override string RuleDescription
        {
            get
            {
                return "帐户权益小于"+ equity_flat.ToString("N2")+"强平持仓并禁止交易";
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "帐户权益小于X时,强平并冻结帐户"; }
        }

        public static new string Description
        {
            get { return "监控帐户权益,当帐户权益小于设定值时,触发强平并冻结交易帐户"; }
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public static new string ValueName { get { return "当前权益"; } }

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
