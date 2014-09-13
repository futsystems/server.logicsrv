using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

 
namespace TradingLib.Common
{
    public class XMLTransportHelper
    {

        /// <summary>
        /// 将xml文件转换成字符串,这样可以在整个消息总线上进行传输
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string XML2String(XmlDocument xml)
        {
            MemoryStream stream = new MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, null);
            writer.Formatting = Formatting.Indented;
            xml.Save(writer);

            StreamReader sr = new StreamReader(stream, Encoding.UTF8);
            stream.Position = 0;
            string xmlString = sr.ReadToEnd();
            sr.Close();
            stream.Close();

            return xmlString;
        }

        public static XmlDocument String2XML(string msg)
        {
            System.IO.StringReader xmlSR = new System.IO.StringReader(msg);
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlSR);
            return doc;

        }

    }
}
