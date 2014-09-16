using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.FinService
{
    
    public class Argument
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public EnumArgumentType Type { get; set; }


        /// <summary>
        /// 服务外键
        /// 表面该参数是为某个服务设定的
        /// 每个帐户只允许有一个配资服务，该配资服务从数据库加载参数列表 没有参数则从Agent获得基本数据 如果 Agent没有数据则从Base获得数据
        /// </summary>
        public int service_fk { get; set; }

        /// <summary>
        /// 转换成字符串
        /// </summary>
        /// <returns></returns>
        public string AsString()
        {
            if (this.Type == EnumArgumentType.STRING)
                return this.Value;
            else
                throw new Exception("参数类型不正确");
        }

        /// <summary>
        /// 转换成数字
        /// </summary>
        /// <returns></returns>
        public int AsInt()
        {
            if (this.Type == EnumArgumentType.INT)
                return int.Parse(this.Value);
            else
                throw new Exception("参数类型不正确");
        }

        /// <summary>
        /// 转换成decimal
        /// </summary>
        /// <returns></returns>
        public decimal AsDecimal()
        {
            if (this.Type == EnumArgumentType.DECIMAL)
                return decimal.Parse(this.Value);
            else
                throw new Exception("参数类型不正确");
        }
    }
}
