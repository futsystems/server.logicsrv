using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 帐户操作接口
    /// 包含激活 冻结 强迫等操作
    /// 
    /// </summary>
    public interface IAccOperation
    {
        /// <summary>
        /// 冻结帐户
        /// </summary>
        void InactiveAccount();

        /// <summary>
        /// 激活帐户
        /// </summary>
        void ActiveAccount();

        /// <summary>
        /// 强平帐户所有持仓
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        void FlatPosition(QSEnumOrderSource source, string forcereason);//平掉所有持仓

        /// <summary>
        /// 平掉某个特定持仓
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comment"></param>
        /// <param name="pos"></param>
        void FlatPosition(Position pos, QSEnumOrderSource source, string forcereason);//平掉某个持仓
    }
}
