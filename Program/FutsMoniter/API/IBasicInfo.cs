using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.API
{
    public interface IBasicInfo
    {
        /// <summary>
        /// 通过全局ID获得市场时间对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        MarketTime GetMarketTime(int id);
        /// <summary>
        /// 通过全局ID获得交易所对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Exchange GetExchange(int id);
        /// <summary>
        /// 通过全局ID获得品种对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SecurityFamilyImpl GetSecurity(int id);

        /// <summary>
        /// 通过品种字头获得品种对象
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        SecurityFamilyImpl GetSecurity(string code);
        /// <summary>
        /// 通过全局ID获得合约对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        SymbolImpl GetSymbol(int id);
        /// <summary>
        /// 通过合约字头获得合约对象
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        SymbolImpl GetSymbol(string symbol);
        /// <summary>
        /// 获得所有品种数组
        /// </summary>
        SecurityFamilyImpl[] Securities { get; }

        /// <summary>
        /// 获得所有合约数组
        /// </summary>
        SymbolImpl[] Symbols { get; }

        /// <summary>
        ///  获得委托风控规则数组
        /// </summary>
        /// <returns></returns>
        //RuleClassItem[] GetOrderRuleClass();

        /// <summary>
        /// 获得某个风控规则RuleClass
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        RuleClassItem GetOrderRuleClass(string classname);

        /// <summary>
        /// 获得某个帐户风控项
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        RuleClassItem GetAccountRuleClass(string classname);


        /// <summary>
        /// 获得某个风控项的RuleClass
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        RuleClassItem GetRuleItemClass(RuleItem item);
        /// <summary>
        /// 获得帐户风控规则数组
        /// </summary>
        /// <returns></returns>
        RuleClassItem[] GetAccountRuleClass();

        ArrayList GetOrderRuleClassListItems();
        ArrayList GetAccountRuleClassListItems();

        ArrayList GetExchangeCombList(bool isany=false);
        ArrayList GetMarketTimeCombList(bool isany = false);
        ArrayList GetSecurityCombList(bool isany = false);
        ArrayList GetSecurityCombListViaExchange(int id);
        ArrayList GetExpireMonth();

        
    }
}
