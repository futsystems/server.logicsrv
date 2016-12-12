﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class RiskCentre
    {
        /// <summary>
        /// 执行账户的 账户规则检查
        /// </summary>
        /// <param name="a"></param>
        public void CheckAccount(IAccount a)
        {
            try
            {
                if (!a.RuleItemLoaded)
                    LoadRuleItem(a);
                string msg = string.Empty;
                if (!a.CheckAccountRule(out msg) && msg != string.Empty)//账户检查不通过并且有返回消息则我们打印消息
                {
                    logger.Warn(msg);
                }
            }
            catch (Exception ex)
            {
                string s = PROGRAME + ":checkAccount error:" + ex.ToString();
                logger.Error(s);
            }
        }
        
    }
}
