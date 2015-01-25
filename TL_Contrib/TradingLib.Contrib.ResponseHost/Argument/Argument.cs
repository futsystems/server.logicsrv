using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.ResponseHost
{
    public class Argument
    {
        /// <summary>
        /// 策略参数
        /// 支持整数，小数，字符串，布尔 类型
        /// 用于将参数序列化到数据库 或者从数据库加载对应的参数
        /// </summary>
        public Argument()
            : this(string.Empty, string.Empty, EnumArgumentType.STRING)
        {


        }


        public Argument(string name, string value, EnumArgumentType type)
        {
            this.Name = name;
            this.Value = value;
            this.Type = type;
        }
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

        public override string ToString()
        {
            return " Name:" + this.Name + " Value:" + this.Value + " Type:" + this.Type.ToString();
        }
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

        public bool AsBool()
        {
            if (this.Type == EnumArgumentType.BOOLEAN)
                return bool.Parse(this.Value);
            else
                throw new Exception("参数类型不正确");
        }
    }
}
