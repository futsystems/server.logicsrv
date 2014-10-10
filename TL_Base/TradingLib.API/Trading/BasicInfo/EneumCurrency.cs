using System.ComponentModel;

namespace TradingLib.API
{
    public enum CurrencyType
    {
        [Description("人民币")]
        RMB,
        [Description("美元")]
        USD,
        AUD,
        CAD,
        CHF,
        EUR,
        GBP,
        HKD,
        JPY,
        MXN,
        NZD,
        SEK,
    }
}
