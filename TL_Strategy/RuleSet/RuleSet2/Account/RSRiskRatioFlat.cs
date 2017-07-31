﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace RuleSet2.Account
{
    /// <summary>
    /// 浮动亏损/静态权益 > 设定值 就执行平仓
    /// </summary>
    public class RSLossRatioFlat : RuleBase, IAccountCheck
    {
        /// <summary>
        /// 参数【json格式】
        /// </summary>
        private string _args = string.Empty;

        /// <summary>
        /// 亏损出发值
        /// </summary>
        decimal loss_ratio = 0;

        /// <summary>
        /// 强平后是否冻结账户
        /// </summary>
        bool acc_lock=false;

        public override string Value
        {
            get { return _args; }
            set
            {

                try
                {
                    _args = value;
                    //解析json参数
                    var args = _args.DeserializeObject();

                    loss_ratio = decimal.Parse(args["loss_ratio"].ToString());
                    acc_lock = bool.Parse(args["acc_lock"].ToString());
                }
                catch (Exception ex)
                { }
            }
        }

        List<string> posflatfired = new List<string>();
        public bool CheckAccount(out string msg)
        {
            msg = string.Empty;
            decimal st_equity = this.Account.LastEquity + this.Account.CashIn - this.Account.CashOut;

            if (st_equity <= 0) return true;
            if (this.Account.Profit >= 0) return true;


            decimal ratio = (Math.Abs(this.Account.Profit) / st_equity) * 100;
            if (ratio >= loss_ratio)
            {
                //账户没被冻结 执行强平 避免冻结后在冻结处理线程和风控规则线程出现竞争
                if (this.Account.Execute)
                {
                    if (this.Account.AnyPosition)
                    {
                        msg = RuleDescription + ":全平所有仓位并冻结账户";
                        TLCtxHelper.ModuleRiskCentre.FlatAllPositions(this.Account.ID, QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);
                    }
                }

                if (acc_lock)
                {
                    if (this.Account.Execute)
                        TLCtxHelper.ModuleAccountManager.InactiveAccount(this.Account.ID);
                }
                return false;
                
            }
            return true;
        }

        public override string RuleDescription
        {
            get
            {
                return "账户亏损/静态权益 大于" + loss_ratio.ToFormatStr() + "% 强平持仓";
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "账户亏损/静态权益 大于X% 强平持仓"; }
        }

        public static new string Description
        {
            get { return "账户亏损/(昨日权益+入金-出金) 大于X% 强平持仓"; }
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
