using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public static partial class MangerUtils
    {

        /// <summary>
        /// 获得管理员更新的判断谓词
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static Predicate<CustInfoEx> GetNotifyPredicate(this Manager op)
        {
            Predicate<CustInfoEx> func = null;

            func = (info) =>
            {
                
                //该custinfoex 绑定了管理端
                if (info.Manager != null)
                {
                    //如果有Root域的管理端登入 则需要通知
                    if (info.Manager.RightRootDomain())
                        return true;
                    //如果是该管理端的子代理 则需要通知
                    if (info.Manager.RightAgentParent(op.GetBaseMGR())) //
                        return true;
                    return false;
                }
                else
                {
                    return false;
                }
            };
            return func;
        }
    }
}
