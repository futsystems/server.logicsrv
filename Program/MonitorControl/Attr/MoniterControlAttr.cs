using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.MoniterControl
{
    /// <summary>
    /// 用于标注管理端控件
    /// 管理端控件用于在界面的不同区域进行展示
    /// 默认区域有WorkSpace,RightPanel,LeftPanel用于将控件显示在主区域，左侧面板，右侧面板
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MoniterControlAttr:Attribute
    {
        public MoniterControlAttr(string name, string title, QSEnumControlPlace cp = QSEnumControlPlace.WorkSpace)
        {
            this.Name = name;
            this.Title = title;
            this.ControlPlace = cp;
        }
        /// <summary>
        /// 用于标注控件位置
        /// </summary>
        public QSEnumControlPlace ControlPlace { get; set; }

        /// <summary>
        /// 控件显示的时候tab标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 名称 在控件生成对应的page时 作为key使用，需要保持唯一
        /// </summary>
        public string Name { get; set; }
    }
}
