using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using TradingLib.API;
using System.Data;

namespace TradingLib.Common
{
    
    //定义了交易所,交易所由ExchangeTracker管理 并从本地文件负责加载
    public class ExchangeTracker
    {
        const string EXIndex = "编码";
        const string EXCode = "代码";
        const string EXName = "名称";
        const string EXCountry = "国家";

        public event DebugDelegate SendDebugEvent;
        public ExchangeTracker()
        {
       
        }

        public static DataTable getExchTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(EXIndex);
            table.Columns.Add(EXCode);
            table.Columns.Add(EXName);
            table.Columns.Add(EXCountry);
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Exchange");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                table.Rows.Add(new object[] { xe.GetAttribute("Index"), xe.SelectSingleNode("EXCode").InnerText, xe.SelectSingleNode("Name").InnerText, xe.SelectSingleNode("Country").InnerText });

            }
            return table;            
        }


        public static List<Exchange> getExchList()
        {
            List<Exchange> tmp = new List<Exchange>();

            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Exchange");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                tmp.Add(new Exchange(xe.SelectSingleNode("EXCode").InnerText, xe.SelectSingleNode("Name").InnerText, (Country)Enum.Parse(typeof(Country), xe.SelectSingleNode("Country").InnerText, true), xe.SelectSingleNode("Session").InnerText));
            }
            return tmp;
            
        }


        private static XmlDocument getXMLDoc()
        {
            XmlDocument xmlDoc = new XmlDocument();
            //XmlReaderSettings settings = new XmlReaderSettings();
            //settings.IgnoreComments = true;//忽略文档里面的注释
           // XmlReader reader = XmlReader.Create(@"config\exchange.xml", settings);
            xmlDoc.Load(@"config\exchange.xml");
            return xmlDoc;
            
        }

        public static Exchange getExchagne(string exindex)
        {
            return Exchange(exindex);
        }
        public static Exchange Exchange(string exindex)
        {
             //debug("更新数据");
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Exchange");
            XmlNodeList exlist = xn.ChildNodes;
            Exchange ex = null;
            foreach (XmlNode x in exlist)
            {
                
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("Index") == exindex)
                {
                    ex = new Exchange(xe.SelectSingleNode("EXCode").InnerText, xe.SelectSingleNode("Name").InnerText, (Country)Enum.Parse(typeof(Country), xe.SelectSingleNode("Country").InnerText, true), xe.SelectSingleNode("Session").InnerText);
                    return ex;
                }
            }
            return ex;
           
    
        }
        private void debug(string s)
        {
            if (SendDebugEvent != null)
            {
                SendDebugEvent(s);
            }
        }
        
        //增加一个节点
        public static void addExchange(Exchange ex)
        {
            if (HaveExchange(ex))
            {
                updateExchange(ex);
                return;
            }
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Exchange");

            XmlElement e = xmlDoc.CreateElement("EXCH");
            e.SetAttribute("Index", ex.Index);

            XmlElement excode = xmlDoc.CreateElement("EXCode");
            excode.InnerText = ex.EXCode;
            XmlElement name = xmlDoc.CreateElement("Name");
            name.InnerText = ex.Name;
            XmlElement country = xmlDoc.CreateElement("Country");
            country.InnerText = ex.Country.ToString();
            XmlElement session = xmlDoc.CreateElement("Session");
            session.InnerText = ex.SessionString;
            e.AppendChild(excode);
            e.AppendChild(name);
            e.AppendChild(country);
            e.AppendChild(session);
            xn.AppendChild(e);
            xmlDoc.Save(@"config\exchange.xml");
        
        }

        //删除一个节点
        public static void delExchange(Exchange ex)
        {
            delExchange(ex.Index);
        }

        public static void delExchange(string index)
        {
            //debug("更新数据");
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Exchange");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("Index") == index)
                {
                    xn.RemoveChild(x);//删除该节点下所有数据包含子节点本身
                }

            }
            xmlDoc.Save(@"config\exchange.xml");


        }

        //更新一个节点
        public static void updateExchange(Exchange ex)
        {
            if (!HaveExchange(ex))
            {
                addExchange(ex);
                return;
            }
            //debug("更新数据");
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Exchange");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("Index") == ex.Index)
                {
                    //debug("it is here");
                    xe.SelectSingleNode("EXCode").InnerText = ex.EXCode;
                    xe.SelectSingleNode("Name").InnerText = ex.Name;
                    xe.SelectSingleNode("Country").InnerText = ex.Country.ToString();
                    xe.SelectSingleNode("Session").InnerText = ex.SessionString;
                }
            }
            xmlDoc.Save(@"config\exchange.xml");



            
        }

        //查询一个结点
        public static bool HaveExchange(Exchange ex)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Exchange");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("Index") == ex.Index)
                    return true;
            }

            return false;
            
        }
    }
    
    

    
}
