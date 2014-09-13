using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 定义了bar生成的频率
    /// 时间/tick/volume/等 以及间隔单位次数
    /// </summary>
    [Serializable]
    public class BarFrequency//: IXmlSerializable
    {
        [XmlIgnore]
        public static BarFrequency OneMin
        {
            get
            {
                return new BarFrequency(BarInterval.Minute);
            }
        }
        [XmlIgnore]
        public static  BarFrequency ThreeMin
        {
            get
            {
                return new  BarFrequency(BarInterval.CustomTime,180);
            }
        }
        [XmlIgnore]
        public static  BarFrequency FiveMin
        {
            get
            {
                return new  BarFrequency(BarInterval.FiveMin);
            }
        }
        [XmlIgnore]
        public static  BarFrequency FifteenMin
        {
            get
            {
                return new  BarFrequency(BarInterval.FifteenMin);
            }
        }
        [XmlIgnore]
        public static  BarFrequency ThirtyMin
        {
            get
            {
                return new  BarFrequency(BarInterval.ThirtyMin);
            }
        }
        [XmlIgnore]
        public static  BarFrequency Hour
        {
            get
            {
                return new  BarFrequency(BarInterval.Hour);
            }
        }
        [XmlIgnore]
        public static  BarFrequency Day
        {
            get
            {
                return new  BarFrequency(BarInterval.Day);
            }
        }

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }

        }
        int _interval;

        public BarInterval Type
        {
            get
            {
                return _type;
            }
            set { _type = value; }
        }
        BarInterval _type;


        

        TimeSpan timespan;
        [XmlIgnore]
        public TimeSpan TimeSpan
        {
            get {
                if(timespan!=null)
                    return timespan;
                else
                    timespan = new TimeSpan(0, 0, _interval);
                return timespan;
            }
        }
        public BarFrequency()
        { 
        
        }
        public  BarFrequency(BarInterval interval)
        {
            switch (interval)
            {
                case BarInterval.Day:
                case BarInterval.FifteenMin:
                case BarInterval.FiveMin:
                case BarInterval.Hour:
                case BarInterval.Minute:
                case BarInterval.ThirtyMin:
                    {
                        _type = BarInterval.CustomTime;
                        _interval = (int)interval;
                    }
                    break;
                default:
                    _type = BarInterval.CustomTime;
                    _interval = 60;
                    break;
            }
            timespan = new TimeSpan(0, 0, _interval);
            return;
        }
        public BarFrequency(BarInterval type, int interval)
        {
            _interval = interval;
            switch (_type)
            {
                case BarInterval.CustomTicks:
                case BarInterval.CustomTime:
                case BarInterval.CustomVol:
                    _type = type;
                    break;
                default:
                    _type = BarInterval.CustomTime;
                    break;
            }
            timespan = new TimeSpan(0, 0, _interval);
            return;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is BarFrequency)) return false;
            BarFrequency f = obj as BarFrequency;
            if (f._interval == this.Interval && f._type == this._type)
                return true;
            return false;
        }
        public override string ToString()
        {
            return _interval.ToString() + "@" + _type.ToString();
        }
        /*
        
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.Read();
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == GetType().ToString())
            {
                this.Type = (BarInterval)Enum.Parse(typeof(BarInterval),reader["Type"]);
                this.Interval = int.Parse(reader["Interveal"]);
                if (this.Type == BarInterval.CustomTime)
                    this.timespan = new TimeSpan(this._interval);
                //reader.Read();
            }
            else if (reader.NodeType == XmlNodeType.EndElement)
            {
                reader.Read();
                return;
            }
    
            
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string str2;
                    if (((str2 = reader.LocalName) != null) && (str2 == "Type"))
                    {
                        BarInterval type = (BarInterval)Enum.Parse(typeof(BarInterval),reader.ReadElementContentAsString());
                        
                        this._type = type;
                    }
                    if (((str2 = reader.LocalName) != null) && (str2 == "Interveal"))
                    {
                        this._interval = int.Parse(reader.ReadElementContentAsString());
                        if(this.Type == BarInterval.CustomTime)
                            this.timespan = new TimeSpan(this._interval);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    //reader.Read();
                    return;
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(GetType().ToString());
            writer.WriteAttributeString("Type", this.Type.ToString());
            writer.WriteAttributeString("Interveal", this.Interval.ToString());
            writer.WriteEndElement();

            //writer.WriteElementString("Type", this.Type.ToString());
            //writer.WriteElementString("Interveal", this.Interval.ToString());
        }
        **/
    }
}
