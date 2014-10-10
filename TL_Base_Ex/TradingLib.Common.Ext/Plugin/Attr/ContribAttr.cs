using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /*
     * TLObjectAttribute 是内部插件基础特性
     * 不同插件或者功能模块的特性集成该特性
     * 同时给出不同种类某块的自定义特性
     * 
     * AttrPluginContrib 是实现IXXXPlugin的类
     * 用于在反射过程中获得对应的特新并将其封装成符合IXXXPlugin接口的对象,从而实现模块信息的对外暴露
     * 
     * 上层业务通过PluginHelper获得IXXXPlugin列表，变可以从中获得模块信息并生成对应实例
     * 
     * 
     * 
     * 
     * 
     * */
    [AttributeUsage(AttributeTargets.Class)]//该特性只能用于类
    public class ContribAttr:TLObjectAttribute
    {
        /// <summary>
        /// 扩展模块ID,在系统内部路由中,通过该ID实现消息的路由
        /// </summary>
        public string ContribID { get { return _contribID; } }
        string _contribID;

        public ContribAttr(string contribid,string name, string description)
            :base(name,"",description,"TLF","1.0.0","Qianbo")
        {
            _contribID = contribid;
            
        }

    }
}
