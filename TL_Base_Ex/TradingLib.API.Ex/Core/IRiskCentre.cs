using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IRiskCentre
    {
        /// <summary>
        /// 查找某个交易帐号的配资服务
        /// </summary>
        //event GetFinServiceDel GetFinServiceDelEvent;
        /// <summary>
        /// 当帐户进入某个阶段的比赛时,触发风控中心调整风控规则
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="type"></param>
        //void OnAccountEntryRace(IAccount acc, QSEnumRaceType type);

        /// <summary>
        /// 执行某个帐户的帐户风控规则检查,比如损失超过多少执行强平等
        /// </summary>
        /// <param name="a"></param>
        void CheckAccount(IAccount a);

        /// <summary>
        /// 开始时间检查
        /// 
        /// </summary>
        bool MarketOpenTimeCheck { get; }
    }
}
