using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace TradingLib.Quant.Base
{
   [Serializable]
    public class StrategyParameterInfo //: IOwnedDataSerializableAndRecreatable, IOwnedDataSerializable, ISerializable
    {
        // Methods
        public StrategyParameterInfo()
        {
        }
       /*
        protected StrategyParameterInfo(SerializationInfo info, StreamingContext context)
        {
            SerializationReader r = new SerializationReader((byte[])info.GetValue("data", typeof(byte[])));
            this.DeserializeOwnedData(r, context);
        }**/

        public StrategyParameterInfo(string name, double value)
        {
            this.Name = name;
            this.Value = value;
            this.Description = this.Name;
            this.High = double.MaxValue;
            this.Low = double.MinValue;
            this.NumSteps = 10;
        }

        public StrategyParameterInfo Clone()
        {
            return (StrategyParameterInfo)base.MemberwiseClone();
        }
       /*
        public virtual void DeserializeOwnedData(SerializationReader r, object context)
        {
            this.Name = r.ReadString();
            this.Value = r.ReadDouble();
            this.Low = r.ReadDouble();
            this.High = r.ReadDouble();
            this.NumSteps = r.ReadInt32();
            this.Description = r.ReadString();
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter w = new SerializationWriter();
            this.SerializeOwnedData(w, context);
            info.AddValue("data", w.ToArray());
        }


        public virtual void SerializeOwnedData(SerializationWriter w, object context)
        {
            w.Write(this.Name);
            w.Write(this.Value);
            w.Write(this.Low);
            w.Write(this.High);
            w.Write(this.NumSteps);
            w.Write(this.Description);
        }
       **/

        // Properties
        //[Description("参数名称(用英文字母表示,用于策略中的变量字段)"), Category("基本设置"), DisplayName("名称")]
        public string Name { get { return _name; } set { _name = value; } }
        string _name="";
        //[Description("参数默认值"), Category("基本设置"), DisplayName("默认值")]
        public double Value { get { return _value; } set { _value = value; } }
        double _value = 0;
        //[Description("参数描述"), Category("基本设置"), DisplayName("参数描述")]
        public string Description { get { return _descriptoin; } set { _descriptoin = value; } }
        string _descriptoin = "";
        //[Description("最大值"), Category("参数优化设置"), DisplayName("最大值")]
        public double High { get { return _high; } set { _high = value; } }
        double _high=0;

        //[Description("最小值"), Category("参数优化设置"), DisplayName("最小值")]
        public double Low { get { return _low; } set { _low = value; } }
        double _low = 0;
        //[Description("优化时,每次递增多少数字进行多次计算"), Category("参数优化设置"), DisplayName("优化步长")]
        public int NumSteps { get { return _step; } set { _step = value; } }
        int _step = 1;
       /*

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        string str2;
                        if (((str2 = reader.LocalName) != null) && (str2 == "Name"))
                        {
                            this.Name = reader.ReadElementContentAsString();
                        }
                        else if (((str2 = reader.LocalName) != null) && (str2 == "Value"))
                        {
                            this.Value = double.Parse(reader.ReadElementContentAsString());
                        }
                        else if (((str2 = reader.LocalName) != null) && (str2 == "Description"))
                        {
                            this.Description = reader.ReadElementContentAsString();
                        }
                        else if (((str2 = reader.LocalName) != null) && (str2 == "High"))
                        {
                            this.High = double.Parse(reader.ReadElementContentAsString());
                        }
                        else if (((str2 = reader.LocalName) != null) && (str2 == "High"))
                        {
                            this.Low = double.Parse(reader.ReadElementContentAsString());
                        }
                        else if (((str2 = reader.LocalName) != null) && (str2 == "NumSteps"))
                        {
                            this.NumSteps = int.Parse(reader.ReadElementContentAsString());
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        reader.Read();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Name", this.Name.ToString());
            writer.WriteElementString("Value", this.Value.ToString());
            writer.WriteElementString("Description", this.Description.ToString());
            writer.WriteElementString("High", this.High.ToString());
            writer.WriteElementString("Low", this.Low.ToString());
            writer.WriteElementString("NumSteps", this.NumSteps.ToString());
        }**/
    }

    
}
