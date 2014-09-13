using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using TradingLib.API;

namespace TradingLib.Common
{

    /// <summary>
    /// 合约快捷键
    /// </summary>
    public class SymbolShortCutKeyTrakcer
    {

        public static XmlDocument getXMLDoc()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"config\SymbolShortCutKey.xml");
            return xmlDoc;
        }

        /// <summary>
        /// 查询是否有某个功能操作的编号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool HaveSymbolShortCode(string code)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("SymbolShortCutKey");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("SymbolShortCode") == code)
                    return true;
            }
            return false;
        }

        public static void delSymbolShortCode(string code)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("SymbolShortCutKey");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("SymbolShortCode") == code)
                    xn.RemoveChild(x);
            }

            xmlDoc.Save(@"config\SymbolShortCutKey.xml");

        }

        public static void addSymbolShortCutKey(string code, string symbol, string keycode)
        {
            if (HaveSymbolShortCode(code))
            {
                //删除数据
                updateSymbolShortCutKey(code, symbol, keycode);
                return;
            }

            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("SymbolShortCutKey");
            XmlElement e = xmlDoc.CreateElement("SymbolSK");
            e.SetAttribute("SymbolShortCode",code);
            e.SetAttribute("Symbol",symbol);//增加Strategy的时候我们不需要设定其运行模式，在策略控制的时候我们选择运行模式然后进行回测或者实盘
            e.SetAttribute("KeyCode", keycode);

            xn.AppendChild(e);
            xmlDoc.Save(@"config\SymbolShortCutKey.xml");

        }


        public static void updateSymbolShortCutKey(SymbolShortCutKey sk, string symbol, string keycode)
        {
            updateSymbolShortCutKey(sk.SymbolShortCode, symbol, keycode);
        }

        public static void updateSymbolShortCutKey(string code, string symbol, string keycode)
        {
            if (!HaveSymbolShortCode(code))
            {
                addSymbolShortCutKey(code, symbol, keycode);
                return;
            }

            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("SymbolShortCutKey");

            XmlElement cfgnode = null;
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("SymbolShortCode") == code)
                    cfgnode = xe;
            }

            cfgnode.SetAttribute("KeyCode", keycode);
            cfgnode.SetAttribute("Symbol", symbol);
            xmlDoc.Save(@"config\SymbolShortCutKey.xml");

        }

        const string SYMCODE = "合约号";
        const string SYMBOL = "合约代码";
        const string KEYCODE = "快捷键";

        public static DataTable getTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(SYMCODE);
            table.Columns.Add(SYMBOL);
            table.Columns.Add(KEYCODE);

            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("SymbolShortCutKey");

            XmlNodeList exlist = xn.ChildNodes;

            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;

                table.Rows.Add(new object[] { xe.GetAttribute("SymbolShortCode"), xe.GetAttribute("Symbol"), xe.GetAttribute("KeyCode") });
            }
            return table;
        }

        public static List<SymbolShortCutKey> getAllSymbolShortCutKey()
        {
            try
            {
                List<SymbolShortCutKey> list = new List<SymbolShortCutKey>();
                XmlDocument xmlDoc = getXMLDoc();
                XmlNode xn = xmlDoc.SelectSingleNode("SymbolShortCutKey");

                XmlNodeList exlist = xn.ChildNodes;
                foreach (XmlNode x in exlist)
                {
                    XmlElement xe = (XmlElement)x;
                    SymbolShortCutKey ck = new SymbolShortCutKey(xe.GetAttribute("SymbolShortCode"), xe.GetAttribute("Symbol"), xe.GetAttribute("KeyCode"));

                    list.Add(ck);
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }




        public static SymbolShortCutKey getSymbolShortCutKey(string code)
        {
            try
            {
                if (!HaveSymbolShortCode(code))
                    return null;

                XmlDocument xmlDoc = getXMLDoc();
                XmlNode xn = xmlDoc.SelectSingleNode("SymbolShortCutKey");

                XmlElement cfgnode = null;
                XmlNodeList exlist = xn.ChildNodes;
                foreach (XmlNode x in exlist)
                {
                    XmlElement xe = (XmlElement)x;
                    if (xe.GetAttribute("SymbolShortCode") == code)
                        cfgnode = xe;
                }

                return new SymbolShortCutKey(cfgnode.GetAttribute("SymbolShortCode"), cfgnode.GetAttribute("Symbol"), cfgnode.GetAttribute("KeyCode"));
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
