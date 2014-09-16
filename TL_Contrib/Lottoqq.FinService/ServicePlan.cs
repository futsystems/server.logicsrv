using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Contrib.FinService
{


    public class ServicePlanBase
    {
        //Dictionary<string,>
        /// <summary>
        /// 调整收费项目
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="positionround"></param>
        /// <returns></returns>
        public virtual decimal AdjustCommission(Trade fill, IPositionRound positionround)
        {
            return fill.Commission;
        }

        /// <summary>
        /// 获得可用配资额度，用于在标准资金上加入可用资金额度 实现配资逻辑
        /// </summary>
        /// <param name="ammount"></param>
        /// <returns></returns>
        public virtual decimal AdjustFinAmmount(decimal ammount)
        {
            return ammount;
        }


    }
}
