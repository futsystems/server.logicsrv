﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Core
{
    /// <summary>
    /// 出入金请求 辅助类
    /// 
    /// </summary>
    public static class JsonWrapperCashOperationUtils
    {

        /// <summary>
        /// 如果出入金请求的 主域ID 则该出入金请求来自代理帐户
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool IsForAgent(this JsonWrapperCashOperation op)
        {
            return op.mgr_fk > 0;
        }

        /// <summary>
        /// 如果有交易帐号并且交易 则该出入金请求是来自交易帐户
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static bool IsForAccount(this JsonWrapperCashOperation op)
        {
            return !string.IsNullOrEmpty(op.Account);
        }


        
        /// <summary>
        /// 获得出入金操作的通知对象判断诸词
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Predicate<Manager> GetNotifyPredicate(this JsonWrapperCashOperation op)
        {
            Predicate<Manager> func=null;

            //如果是交易帐户的出入金记录
            if (op.IsForAccount())
            {   
                //交易帐户出入金 通知代理和Root
                func = (mgr)=>
                {
                    if (mgr == null) return false;
                    IAccount account = TLCtxHelper.CmdAccount[op.Account];

                    //如果有Root域的管理端登入 则需要通知
                    if (mgr.RightRootDomain())
                        return true;
                    //如果该交易帐户的代理客户端登入 则需要通知
                    if (mgr.GetBaseMGR() == account.Mgr_fk)
                        return true;
                    return false;
                };
            }
            //如果是代理的出入金记录
            if (op.IsForAgent())
            {
                func = (mgr) => 
                {
                    if (mgr == null) return false;
                    if (mgr.RightRootDomain())
                        return true;
                    if (mgr.GetBaseMGR() == op.mgr_fk)//如果该出入金记录属于该代理客户端 则需要通知
                        return true;
                    return false;
                };
            }
            return func;
        }
    }
}