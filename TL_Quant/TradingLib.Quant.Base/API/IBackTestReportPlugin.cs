using System;
using System.Windows.Forms;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 系统回测报告插件 用于实现多种不同的方式去呈现系统回测数据
    /// 汇总报表,权益曲线,委托/成交列表,回撤曲线等等......
    /// 通过实现该接口可以实现自定义的回测报表
    /// </summary>
    public interface IBackTestReportPlugin
    {
        Control CreateReport(BackTestData finalResults, SerializableDictionary<string, string> settings);
        bool HasConfig();
        bool ShowConfigForm(ref SerializableDictionary<string, string> settings);
    }
}
