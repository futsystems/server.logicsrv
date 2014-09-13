using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using TradingLib.API;
//using System.Windows.Forms;

namespace TradingLib.Common
{
    public class DefaultSymSettingTracker
    {
        
        public static XmlDocument getXMLDoc()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"config\DefaultSymSetting.xml");
            return xmlDoc;
        }
        const string CODE = "品种代码";
        const string LOSS = "止损点数";
        const string PROFIT = "止盈点数";
        const string SIZE = "手数";

        public static DataTable getTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(CODE);
            table.Columns.Add(LOSS);
            table.Columns.Add(PROFIT);
            table.Columns.Add(SIZE);
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("DefaultStrategy");

            XmlNodeList exlist = xn.ChildNodes;
            
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;

                table.Rows.Add(new object[] { xe.GetAttribute("Code"), xe.GetAttribute("Stop"), xe.GetAttribute("Profit"), xe.GetAttribute("Size") });
            }
            return table;
        }
        public static bool HaveSymbolCode(string code)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("DefaultStrategy");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("Code") == code)
                    return true;
            }
            return false;
        }

        public static void delDefaultSymSetting(string symbolcode)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("DefaultStrategy");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("Code") == symbolcode)
                    xn.RemoveChild(x);
            }

            xmlDoc.Save(@"config\DefaultSymSetting.xml");
            
        }

        public static List<DefaultSymSetting> getAllSymSettings()
        {
            try
            {
                List<DefaultSymSetting> list = new List<DefaultSymSetting>();
                XmlDocument xmlDoc = getXMLDoc();
                XmlNode xn = xmlDoc.SelectSingleNode("DefaultStrategy");

                XmlNodeList exlist = xn.ChildNodes;
                foreach (XmlNode x in exlist)
                {
                    XmlElement xe = (XmlElement)x;
                   list.Add( new DefaultSymSetting( xe.GetAttribute("Code"),  xe.GetAttribute("Stop"), xe.GetAttribute("Profit"),  xe.GetAttribute("Size")));
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static DefaultSymSetting getDefaultSymSetting(string symbolcode)
        {
            try
            {
                if (!HaveSymbolCode(symbolcode))
                    return null;

                XmlDocument xmlDoc = getXMLDoc();
                XmlNode xn = xmlDoc.SelectSingleNode("DefaultStrategy");

                XmlElement cfgnode = null;
                XmlNodeList exlist = xn.ChildNodes;
                foreach (XmlNode x in exlist)
                {
                    XmlElement xe = (XmlElement)x;
                    if (xe.GetAttribute("Code") == symbolcode)
                        cfgnode = xe;
                }
                return new DefaultSymSetting(cfgnode.GetAttribute("Code"), cfgnode.GetAttribute("Stop"), cfgnode.GetAttribute("Profit"), cfgnode.GetAttribute("Size"));
            }
            catch (Exception ex)
            {
                return null;
            }

            

        }
        public static void addDefaultSymSetting(string symbolcode,decimal stop,decimal profit,int size)
        {
            if (HaveSymbolCode(symbolcode))
            {
                //删除数据
                updateDefaultSymSetting(symbolcode, stop, profit, size);
                return;
            }

            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("DefaultStrategy");
            XmlElement e = xmlDoc.CreateElement("Setting");
            e.SetAttribute("Code", symbolcode);
            e.SetAttribute("Stop", stop.ToString());//增加Strategy的时候我们不需要设定其运行模式，在策略控制的时候我们选择运行模式然后进行回测或者实盘
            e.SetAttribute("Profit", profit.ToString());
            e.SetAttribute("Size",size.ToString());
            
            xn.AppendChild(e);
            xmlDoc.Save(@"config\DefaultSymSetting.xml");

        }


        public static void updateDefaultSymSetting(string symbolcode, decimal stop, decimal profit, int size)
        {
            if (!HaveSymbolCode(symbolcode))
            {
                addDefaultSymSetting(symbolcode, stop, profit, size);
                return;
            }

            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("DefaultStrategy");

            XmlElement cfgnode = null;
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("Code") == symbolcode)
                    cfgnode = xe;
            }
            cfgnode.SetAttribute("Stop", stop.ToString());
            cfgnode.SetAttribute("Profit", profit.ToString());
            cfgnode.SetAttribute("Size", size.ToString());
            xmlDoc.Save(@"config\DefaultSymSetting.xml");

        }
    }
}
