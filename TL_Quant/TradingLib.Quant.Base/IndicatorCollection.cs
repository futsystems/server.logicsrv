using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 用于访问某个symbol的指标
    /// </summary>
    public class IndicatorCollection
    {
        // Fields
        private IndicatorCollections _x91f347c6e97f1846;
        private Security _symbol;

        // Methods
        public IndicatorCollection(IndicatorCollections manager, Security symbol)
        {
            this._x91f347c6e97f1846 = manager;
            this._symbol = symbol;
        }

        // Properties
        public ISeries this[string id]
        {
            get
            {
                return this._x91f347c6e97f1846[id][this._symbol];
            }
        }
    }
}
