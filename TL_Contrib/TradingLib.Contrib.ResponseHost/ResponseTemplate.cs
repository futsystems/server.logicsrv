using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.ResponseHost
{
    /// <summary>
    /// 策略模板
    /// </summary>
    public class ResponseTemplate
    {
        /// <summary>
        /// 服务外键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        public bool Active { get; set; }
    }
}
