using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// Bar数据序列管理器,用于策略系统管理所有合约对应的主频率Bar数据
    /// </summary>
    [Serializable]
    public class BarDataSeriesManager
    {
        // Fields
        private Dictionary<BDSMKey, BarElementSeries> dict = new Dictionary<BDSMKey, BarElementSeries>();
        private IStrategyData system;

        // Methods
        public BarDataSeriesManager(IStrategyData system)
        {
            this.system = system;
        }

        // Properties
        public BarElementSeries this[Security symbol,BarDataType element, bool useSymbolBars]
        {
            get
            {
                BDSMKey key = new BDSMKey(symbol, element, useSymbolBars);
                if (!this.dict.ContainsKey(key))
                {
                    this.dict[key] = new BarElementSeries(this.system, symbol, element, useSymbolBars);
                }
                return this.dict[key];
            }
        }

        // Nested Types
        [Serializable, StructLayout(LayoutKind.Sequential)]
        protected struct BDSMKey
        {
            public Security symbol;
            public BarDataType element;
            public bool _useSymbolBars;
            public BDSMKey(Security symbol, BarDataType element, bool useSymbolBars)
            {
                this.symbol = symbol;
                this.element = element;
                this._useSymbolBars = useSymbolBars;
            }

            public override int GetHashCode()
            {
                return ((this.element.GetHashCode() ^ this.symbol.GetHashCode()) ^ this._useSymbolBars.GetHashCode());
            }

            public override bool Equals(object obj)
            {
                if (!(obj is BarDataSeriesManager.BDSMKey))
                {
                    return false;
                }
                BarDataSeriesManager.BDSMKey key = (BarDataSeriesManager.BDSMKey)obj;
                return (((this.symbol == key.symbol) && (this.element == key.element)) && (this._useSymbolBars == key._useSymbolBars));
            }
        }
    }

 

}