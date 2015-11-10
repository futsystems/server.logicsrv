using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.DataFarm.API
{
    public interface IDataStore
    {
        /// <summary>
        /// 获得Bar数据储存对象
        /// </summary>
        /// <returns></returns>
        IDataAccessor<Bar> GetBarStorage(SymbolFreq freq);
        /// <summary>
        /// 获得行情数据储存对象
        /// </summary>
        /// <returns></returns>
        IDataAccessor<Tick> GetTickStorage(string symbol);
        /// <summary>
        /// 保存所有数据
        /// </summary>
        void FlushAll();
    }
}
