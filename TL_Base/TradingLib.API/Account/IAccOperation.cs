using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 帐户操作接口
    /// </summary>
    public interface IAccOperation
    {
        /// <summary>
        /// 强平帐户所有持仓
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        void FlatPosition(QSEnumOrderSource source, string comment);//平掉所有持仓

        /// <summary>
        /// 冻结帐户
        /// </summary>
        void InactiveAccount();//冻结账户

        /// <summary>
        /// 平掉某个特定持仓
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        /// <param name="pos"></param>
        void FlatPosition(Position pos,QSEnumOrderSource source, string comment);//平掉某个持仓
    }
}
