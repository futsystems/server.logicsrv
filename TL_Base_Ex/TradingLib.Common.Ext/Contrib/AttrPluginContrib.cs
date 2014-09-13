using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /*关于扩展模块的抽象处理
     * 每个扩展模块用ContribAttr特性进行标注
     * 在反射过程中,将ContribAttr封装成支持IContribPlugin的对象暴露到系统,系统通过IContribPlugin对模块进行识别
     * 
     * 同理其他插件对象都是通过继承插件接口IPlugin来提供特殊的一些属性和内容用于暴露给系统
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * 
     * */
    public class AttrPluginContrib:IContribPlugin
    {

        ContribAttr _attr = null;
        public AttrPluginContrib(ContribAttr attr)
        {
            _attr = attr;
        }

        public string ContribClassName { get { return _attr.Id; } }
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get { return _attr.Name; } }

        /// <summary>
        /// 插件描述
        /// </summary>
        public string Description { get { return _attr.Description; } }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get { return _attr.Version; } }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Author { get { return _attr.Author; } }

        /// <summary>
        /// 公司
        /// </summary>
        public string Compnay { get { return _attr.CompanyName; } }

        /// <summary>
        /// 
        /// </summary>
        public string Id { get { return _attr.Id; } }

        /// <summary>
        /// 扩展模块ID
        /// </summary>
        public string ContribID { get { return _attr.ContribID; } }

    }
}
