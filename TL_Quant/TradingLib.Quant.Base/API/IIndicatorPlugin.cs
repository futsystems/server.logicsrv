using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TradingLib.Quant.Base
{
    public delegate void IndicatorPluginDel(IIndicatorPlugin indicator);

    /// <summary>
    /// 指标插件接口,实现该接口即可以在指标插件列表中进行显示
    /// </summary>
    public interface IIndicatorPlugin
    {
        // Methods
        string DefaultDrawingPane();//获得默认绘制区域
        Color DefaultLineColor();//获得默认绘制颜色
        string GetAuthor();//作者姓名
        string GetCompanyName();//公司名称
        string GetDescription();//描述
        string GetGroupName();//分组名
        string GetHelp();//获得帮助信息
        string GetIndicatorClassName();//获得指标类名
        string GetName();//获得其描述名称
        string GetVersion();//获得版本信息
        string id();//获得GUID
    }


}
