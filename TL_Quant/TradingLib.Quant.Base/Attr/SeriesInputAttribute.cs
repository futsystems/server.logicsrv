using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// Serires序列值输入属性
    /// </summary>
    public class SeriesInputAttribute : InputAttribute
    {
        // Fields
        private object _value;

        // Methods
        public SeriesInputAttribute()
        {
        }

        public SeriesInputAttribute(string name, int order)
        {
            base.Name = name;
            base.Order = order;
        }

        public SeriesInputAttribute Clone()
        {
            return (SeriesInputAttribute)base.MemberwiseClone();
        }

        public static List<SeriesInputAttribute> GetSeriesInputs(object obj)
        {
            List<SeriesInputAttribute> rightEdgeAttributeList = QuantShopObjectAttribute.GetQuantShopAttributeList<SeriesInputAttribute>(obj);
            rightEdgeAttributeList.Sort(delegate(SeriesInputAttribute att1, SeriesInputAttribute att2)
            {
                if (att1.Order < att2.Order)
                {
                    return -1;
                }
                if (att1.Order > att2.Order)
                {
                    return 1;
                }
                return 0;
            });
            return rightEdgeAttributeList;
        }

        // Properties
        public object Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }
    }

}
