using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.MoniterControl
{
    /* 管理端菜单模型
     * 基础数据
     *      市场时间
     *      交易所
     *      品种
     *      合约
     *      手续费模板
     *      保证金模板
     *      交易参数
     *      
     * 柜员管理
     * 
     * 财务管理
     * 
     * 记录和报表
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
    /// <summary>
    /// 管理端菜单特性
    /// 用于标注某个类为菜单显影操作
    /// 在管理端动态加载时可以动态生成界面上的菜单项目
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MoniterCommandAttr:Attribute
    {
        public MoniterCommandAttr(string uuid,string name, string title)
        {
            this.UUID = uuid;
            this.Name = name;
            this.Title = title;
        }

        /// <summary>
        /// 全局UUID 用于唯一标识该菜单资源项目
        /// </summary>
        public string UUID { get; set; }
        /// <summary>
        /// 菜单标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 菜单标识
        /// </summary>
        public string Name { get; set; }
    }
}
