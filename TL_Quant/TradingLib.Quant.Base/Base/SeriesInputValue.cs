using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    public class SeriesInputValue
    {
        // Fields
        private string _name;
        private bool _repeatable;
        private InputType _type;
        private object _value;

        // Methods
        public SeriesInputValue()
        {
        }

        /// <summary>
        /// seriresInputValue由SeriresInputAttribute来生成
        /// </summary>
        /// <param name="att"></param>
        public SeriesInputValue(SeriesInputAttribute att)
        {
            this.Name = att.Name;//名称
            this.Value = att.Value;//数值
            this.Repeatable = att.Repeatable;
            if ((this.Value is double) || (this.Value is int))//单数据常数,
            {
                this.Type = InputType.Constant;
            }
            else if (this.Value is BarDataType)//Bar类型
            {
                this.Type = InputType.BarElement;
            }
            else
            {
                this.Type = InputType.Series;//其他ISeries
            }
        }

        public SeriesInputValue Clone()
        {
            return (SeriesInputValue)base.MemberwiseClone();
        }

        // Properties
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public bool Repeatable
        {
            get
            {
                return this._repeatable;
            }
            set
            {
                this._repeatable = value;
            }
        }

        public InputType Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }

        public object Value
        {
            get
            {
                if (((this.Type != InputType.BarElement) || (this._value == null)) || (!(this._value is BarDataType) && !(this._value is int)))
                {
                    return this._value;
                }
                return (BarDataType)this._value;
            }
            set
            {
                if (value is BarDataType)
                {
                    this.Type = InputType.BarElement;
                }
                else if ((this.Type == InputType.BarElement) && !(value is int))
                {
                    if (value is double)
                    {
                        this.Type = InputType.Constant;
                    }
                    else
                    {
                        this.Type = InputType.Series;
                    }
                }
                this._value = value;
            }
        }

        // Nested Types
        public enum InputType
        {
            Series,//另外一个ISeries序列
            BarElement,//BarElement BarData
            Constant//常数
        }
    }


}
