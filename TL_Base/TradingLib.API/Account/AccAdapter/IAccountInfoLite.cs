using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    
    public interface IAccountInfoLite
    {
        string Account { get; set; }
        decimal NowEquity { get; set; }//当前动态权益
        decimal Margin { get; set; }//占用保证金
        decimal ForzenMargin { get; set; }//冻结保证金
        decimal BuyPower { get; set; }//购买能力
        decimal RealizedPL { get; set; }//平仓盈亏
        decimal UnRealizedPL { get; set; }//浮动盈亏
        decimal Commission { get; set; }//手续费
        decimal Profit { get; set; }//净利
    }
}
