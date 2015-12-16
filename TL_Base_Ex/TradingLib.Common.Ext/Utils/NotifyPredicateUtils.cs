using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Common
{
    /// <summary>
    /// 通知谓词帮助类
    /// 当某个对象发生添加或更新时,判断需要通知某个管理员
    /// 在执行通知操作时,需要判断当前连接的管理员列表中哪些需要进行通知
    /// </summary>
    public static class NotifyPredicateUtils
    {
        /// <summary>
        /// 获得管理员更新的判断谓词
        /// 用于判断当manager发生更新时 需要通知哪些管理员
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Predicate<Manager> GetNotifyPredicate(this Manager manager)
        {
            Predicate<Manager> func = null;
            func = (mgr) =>
            {
                //该custinfoex 绑定了管理端
                if (mgr == null) return false;
                if (mgr.domain_id != manager.domain_id) return false;//不属于同一分区 则直接返回
                //如果有Root域的管理端登入 则需要通知
                if (mgr.IsInRoot())
                    return true;
                //如果是该管理端的子代理 则需要通知
                if (mgr.IsParentOf(manager))
                    return true;
                return false;
            };
            return func;
        }



        /// <summary>
        /// 如果出入金请求的 主域ID 则该出入金请求来自代理帐户
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        static bool IsForAgent(this JsonWrapperCashOperation op)
        {
            return op.mgr_fk > 0;
        }

        /// <summary>
        /// 如果有交易帐号并且交易 则该出入金请求是来自交易帐户
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        static bool IsForAccount(this JsonWrapperCashOperation op)
        {
            return !string.IsNullOrEmpty(op.Account);
        }


        /// <summary>
        /// 获得某个帐户的通知对象
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Predicate<Manager> GetNotifyPredicate(this IAccount account)
        {
            Predicate<Manager> func = null;
            func = (mgr) =>
            {
                if (mgr == null) return false;
                //如果有Root域的管理端登入 则需要通知
                if (mgr.IsInRoot())
                    return true;
                //如果该交易帐户的代理客户端登入 则需要通知
                if (mgr.RightAccessAccount(account))
                    return true;
                return false;
            };
            return func;
        }
        /// <summary>
        /// 获得出入金操作的通知对象判断诸词
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Predicate<Manager> GetNotifyPredicate(this JsonWrapperCashOperation op)
        {
            Predicate<Manager> func = null;
            //如果是交易帐户的出入金记录
            if (op.IsForAccount())
            {
                //交易帐户出入金 通知代理和Root
                func = (mgr) =>
                {
                    if (mgr == null) return false;
                    IAccount account = TLCtxHelper.ModuleAccountManager[op.Account];

                    //如果有Root域的管理端登入 则需要通知
                    if (mgr.IsInRoot())
                        return true;
                    //如果该交易帐户的代理客户端登入 则需要通知
                    if (mgr.BaseMgrID == account.Mgr_fk)
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
                    if (mgr.IsInRoot())
                        return true;
                    if (mgr.BaseMgrID == op.mgr_fk)//如果该出入金记录属于该代理客户端 则需要通知
                        return true;
                    return false;
                };
            }
            return func;
        }
    }
}
