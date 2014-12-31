using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 合约接口
    /// 演示更新
    /// </summary>
    public interface Symbol
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// 计价合约,异化合约不存在对应的合约行情
        /// 需要通过底层合约获得对应的行情合约
        /// </summary>
        string TickSymbol { get; }

        /// <summary>
        /// 底层合约用于异化合约的生成
        /// </summary>
        Symbol ULSymbol { get; set; }

        /// <summary>
        /// 合约隶属品种
        /// </summary>
        SecurityFamily SecurityFamily { get; set; }

        /// <summary>
        /// 开仓手续费
        /// </summary>
        decimal EntryCommission { get; set; }

        /// <summary>
        /// 平仓手续费
        /// </summary>
        decimal ExitCommission { get; set; }


        /// <summary>
        /// 保证金比例/保证金数额
        /// </summary> 
        decimal Margin { get; set; }

        /// <summary>
        /// 额外保证金字段
        /// 用于在基本保证金外提供额外质押
        /// </summary>
        decimal ExtraMargin { get; set; }

        /// <summary>
        /// 过夜保证金,如果需要过夜则需要提供Maintance保证金
        /// </summary>
        decimal MaintanceMargin { get; set; }

        /// <summary>
        /// 乘数
        /// </summary>
        int Multiple { get; }

       
        /// <summary>
        /// 全称
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// 品种类别
        /// </summary>
        SecurityType SecurityType { get; }

        /// <summary>
        /// 货币类别
        /// </summary>
        CurrencyType Currency { get; }
       

        string Exchange { get; }

        /// <summary>
        /// 是否可交易
        /// </summary>
        bool Tradeable { get; }

        /// <summary>
        /// 综合判断获得是否可交易标识
        /// </summary>
        bool IsTradeable{get;}

        /// <summary>
        /// 检查是否是市场开市时间
        /// </summary>
        bool IsMarketTime { get; }


        /// <summary>
        /// 是否在强平时间段内
        /// </summary>
        bool IsFlatTime { get; }

        /// <summary>
        /// 合约到期月
        /// </summary>
        //int ExpireMonth { get; set; }

        /// <summary>
        /// 合约到期日
        /// </summary>
        int ExpireDate { get; set; }


        /// <summary>
        /// 期权 方向
        /// </summary>
        QSEnumOptionSide OptionSide { get; set; }

        /// <summary>
        /// 期权中的行权价
        /// </summary>
        decimal Strike { get; set; }

        /// <summary>
        /// 底层依赖合约 比如股指期权依赖于股指期货
        /// </summary>
        Symbol UnderlayingSymbol { get; set; }

        


    }
}
