using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace TradingLib.Quant.Base
{
    [Serializable, XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        // Methods
        public SerializableDictionary()
        {
        }

        public SerializableDictionary(Dictionary<TKey, TValue> other)
            : base(other)
        {
        }

        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TKey));
            XmlSerializer serializer2 = new XmlSerializer(typeof(TValue));
            bool isEmptyElement = reader.IsEmptyElement;
            reader.Read();
            if (!isEmptyElement)
            {
                while ((reader.NodeType != XmlNodeType.EndElement) && (reader.NodeType != XmlNodeType.None))
                {
                    reader.ReadStartElement("item");
                    reader.ReadStartElement("key");
                    TKey key = (TKey)serializer.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadStartElement("value");
                    TValue local2 = (TValue)serializer2.Deserialize(reader);
                    reader.ReadEndElement();
                    base.Add(key, local2);
                    reader.ReadEndElement();
                    reader.MoveToContent();
                }
                if (reader.NodeType != XmlNodeType.None)
                {
                    reader.ReadEndElement();
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TKey));
            XmlSerializer serializer2 = new XmlSerializer(typeof(TValue));
            foreach (TKey local in base.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                serializer.Serialize(writer, local);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue o = base[local];
                serializer2.Serialize(writer, o);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }

 

}
