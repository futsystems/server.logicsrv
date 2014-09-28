using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 储存于xml文件中的Rsp消息
    /// 
    /// </summary>
    public class XMLRspInfo
    {
        public XMLRspInfo(string key, int code, string message)
        {
            Key = key;
            Code = code;
            Message = message;
        }
        /// <summary>
        /// xml消息的key用于建立key索引
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// xml消息的code 用于消息处理的地方进行逻辑判断
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// xml消息的具体内容
        /// </summary>
        public string Message { get; set; }
    }


    public class XMLRspInfoHelper
    {
        static XMLRspInfoTracker defaultinstance = null;
        
        static XMLRspInfoHelper()
        {
            defaultinstance = new XMLRspInfoTracker();
        }

        private XMLRspInfoHelper()
        { 
            
        }

        public static XMLRspInfoTracker Tracker
        {
            get
            {
                if (defaultinstance != null)
                    return defaultinstance;
                defaultinstance = new XMLRspInfoTracker();
                return defaultinstance;
            }
        }

    }
    public class XMLRspInfoTracker
    {
        ConcurrentDictionary<string, XMLRspInfo> xmlkeymap = new ConcurrentDictionary<string, XMLRspInfo>();
        ConcurrentDictionary<int, XMLRspInfo> xmlcodemap = new ConcurrentDictionary<int, XMLRspInfo>();

        public XMLRspInfoTracker()
        {
            List<XMLRspInfo> list = LoadXMLRspInfo();
            foreach (XMLRspInfo rsp in list)
            {
                xmlkeymap[rsp.Key] = rsp;
                xmlcodemap[rsp.Code] = rsp;
            }
        }

        static XMLRspInfo defaulterror = new XMLRspInfo("DEFAULT", 9999, "未指定错误");
        /// <summary>
        /// 通过code获得对应的消息对象
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public  XMLRspInfo this[int code]
        {
            get
            {
                XMLRspInfo info=null;
                if (xmlcodemap.TryGetValue(code, out info))
                {
                    return info;
                }
                return defaulterror;
            }
        }

        /// <summary>
        /// 通过key获得对应的消息对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public  XMLRspInfo this[string key]
        {
            get
            {
                XMLRspInfo info = null;
                if (xmlkeymap.TryGetValue(key, out info))
                {
                    return info;
                }
                return defaulterror;
            }
        }

        public XMLRspInfo[] RspInfos
        {
            get
            {
                return xmlkeymap.Values.ToArray();
            }
        }


        /// <summary>
        /// 加载xml rspinfo
        /// </summary>
        /// <returns></returns>
        private static List<XMLRspInfo> LoadXMLRspInfo()
        {
            string xmlfile = Util.GetConfigFile("error.xml");
            XmlDocument xmlDoc = null;
            //LibUtil.Debug(">>>> xml error filename:" + xmlfile);
            if (File.Exists(xmlfile))
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlfile);
            }

            List<XMLRspInfo> rsplist  = new List<XMLRspInfo>();
            XmlNode xn = xmlDoc.SelectSingleNode("errors");
            XmlNodeList errors = xn.ChildNodes;
            Util.Debug("total errors:" + errors.Count.ToString());
            foreach (XmlNode node in errors)
            {
                try
                {
                    XmlElement error = (XmlElement)node;
                    string key = error.GetAttribute("id");
                    int code = int.Parse(error.GetAttribute("value"));
                    string prompt = error.GetAttribute("prompt");
                    rsplist.Add(new XMLRspInfo(key, code, prompt));
                }
                catch (Exception ex)
                {
                    Util.Debug("error:" + ex.ToString());
                }

            }
            return rsplist;
        }
    }
}
