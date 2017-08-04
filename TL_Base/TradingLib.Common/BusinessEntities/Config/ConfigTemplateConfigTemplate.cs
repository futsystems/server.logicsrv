using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 配置模板
    /// 账户设定配置模板后即按配置模板的保证金 手续费 交易参数 风控规则进行设置
    /// CFG_TEMPLATE_XX为account 在cfg_rule中添加风控，添加的风控即为配置模板中的风控
    /// 加载账户时如果设置了配置模板则根据模板对应的Account来统一加载预设风控数据这样可以实现自动进行账户参数配置
    /// 
    /// 
    /// </summary>
    public class ConfigTemplate
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 域ID
        /// </summary>
        public int Domain_ID { get; set; }

        /// <summary>
        /// 管理员主域ID
        /// </summary>
        public int Manager_ID { get; set; }

        /// <summary>
        /// 手续费模板ID
        /// </summary>
        public int Commission_ID { get; set; }

        /// <summary>
        /// 保证金模板ID
        /// </summary>
        public int Margin_ID { get; set; }

        /// <summary>
        /// 交易参数模板ID
        /// </summary>
        public int ExStrategy_ID { get; set; }



    }
}
