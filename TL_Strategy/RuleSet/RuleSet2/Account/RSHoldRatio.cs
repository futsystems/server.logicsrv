﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace RuleSet2.Account
{
    /// <summary>
    /// 
    /// </summary>
    public class RSHoldRatio : RuleBase, IAccountCheck
    {
        /// <summary>
        /// 参数【json格式】
        /// </summary>
        private string _args = string.Empty;

        decimal _holdRatio = 1m;

        int _checkTime = 0;

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
                    _checkTime = int.Parse(args["check_time"].ToString());//检查时间
                    _holdRatio = decimal.Parse(args["hold_ratio"].ToString());//持仓比例


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
            int value = Util.ToTLTime() - this._checkTime;
            bool flag = Math.Abs(value) < 5;//是否道道设定时间

            if (flag)
            {
                if (!flatStart)
                {
                    if (this.Account.AnyPosition)
                    {
                        decimal margin = this.Account.Margin;
                        decimal targetMargin = this.Account.NowEquity * this._holdRatio;

                        IAccount acc = TLCtxHelper.ModuleAccountManager[this.Account.ID];
                        if (acc == null)
                            return true;
                        //如果当前保证金大于设定过夜保证金 则执行强平操作
                        if (margin > targetMargin)
                        {
                            msg = "过夜减仓";
                            //遍历所有持仓
                            foreach (Position pos in this.Account.Positions.Where(pos => !pos.isFlat))
                            {
                                if (margin <= targetMargin)
                                    continue;
                                var posmargin = acc.CalPositionMargin(pos) * acc.GetExchangeRate(pos.oSymbol.SecurityFamily);
                                if ((margin - targetMargin) > posmargin)
                                {
                                    TLCtxHelper.ModuleRiskCentre.FlatPosition(pos, pos.UnsignedSize, QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);
                                    margin -= posmargin;
                                }
                                else
                                {
                                    int flatNum = (int)(pos.UnsignedSize * (margin - targetMargin) / posmargin);
                                    flatNum = flatNum < pos.UnsignedSize ? flatNum + 1 : flatNum;

                                    margin -= posmargin * (flatNum / pos.UnsignedSize);//计算强平后的
                                    //强平部分持仓
                                    TLCtxHelper.ModuleRiskCentre.FlatPosition(pos, flatNum, QSEnumOrderSource.RISKCENTREACCOUNTRULE, msg);
                                }
                            }
                        }

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
                return string.Format("{0} 检查保证金在 {1}倍自有资金之内", this._checkTime, this._holdRatio);
            }
        }

        #region 覆写静态对象
        public static new string Title
        {
            get { return "隔夜强平(X倍自有资金)"; }
        }

        public static new string Description
        {
            get { return "指定时间检查持仓,将保证金控制在自有资金倍几倍以内"; }
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