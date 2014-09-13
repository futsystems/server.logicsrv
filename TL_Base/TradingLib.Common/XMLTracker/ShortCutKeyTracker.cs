using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using TradingLib.API;

namespace TradingLib.Common
{
    public class ShortCutKeyTracker
    {
        public static XmlDocument getXMLDoc()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"config\ShortCutKeySetting.xml");
            return xmlDoc;
        }
        /// <summary>
        /// 查询是否有某个功能操作的编号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool HaveFunCode(string code)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("ShortCutKey");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FunCode") == code)
                    return true;
            }
            return false;
        }

        public static void delFunCode(string code)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("ShortCutKey");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FunCode") == code)
                    xn.RemoveChild(x);
            }

            xmlDoc.Save(@"config\ShortCutKeySetting.xml");

        }
        const string FUNCODE = "功能代码";
        const string FUNNAME = "功能名称";
        const string KEYCODE = "快捷键";
        const string DEFAULTKEYCODE = "默认快捷键";

        public static DataTable getTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add(FUNCODE);
            table.Columns.Add(FUNNAME);
            table.Columns.Add(KEYCODE);
            table.Columns.Add(DEFAULTKEYCODE);
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("ShortCutKey");

            XmlNodeList exlist = xn.ChildNodes;

            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;

                table.Rows.Add(new object[] { xe.GetAttribute("FunCode"), xe.GetAttribute("FunName"), xe.GetAttribute("KeyCode"), xe.GetAttribute("DefaultKeyCode") });
            }
            return table;
        }

        public static List<ShortCutKey> getAllShortCutKey()
        {
            try
            {
                List<ShortCutKey> list = new List<ShortCutKey>();
                XmlDocument xmlDoc = getXMLDoc();
                XmlNode xn = xmlDoc.SelectSingleNode("ShortCutKey");

                XmlNodeList exlist = xn.ChildNodes;
                foreach (XmlNode x in exlist)
                {
                    XmlElement xe = (XmlElement)x;
                    ShortCutKey ck = new ShortCutKey(xe.GetAttribute("FunCode"), xe.GetAttribute("FunName"), xe.GetAttribute("KeyCode"), xe.GetAttribute("DefaultKeyCode"));

                    list.Add(ck);
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        
        public static ShortCutKey getShortCutKey(string funcode)
        {
            try
            {
                if (!HaveFunCode(funcode))
                    return null;

                XmlDocument xmlDoc = getXMLDoc();
                XmlNode xn = xmlDoc.SelectSingleNode("ShortCutKey");

                XmlElement cfgnode = null;
                XmlNodeList exlist = xn.ChildNodes;
                foreach (XmlNode x in exlist)
                {
                    XmlElement xe = (XmlElement)x;
                    if (xe.GetAttribute("FunCode") == funcode)
                        cfgnode = xe;
                }

                return new ShortCutKey(cfgnode.GetAttribute("FunCode"), cfgnode.GetAttribute("FunName"), cfgnode.GetAttribute("KeyCode"), cfgnode.GetAttribute("DefaultKeyCode"));
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       
        public static void addShortCutKey(string funcode, string funname,string keycode,string defaultkeycode)
        {
            if (HaveFunCode(funcode))
            {
                //删除数据
                updateShortCutKey(funcode,funname,keycode,defaultkeycode);
                return;
            }

            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("ShortCutKey");
            XmlElement e = xmlDoc.CreateElement("Key");
            e.SetAttribute("FunCode", funcode);
            e.SetAttribute("FunName", funname);//增加Strategy的时候我们不需要设定其运行模式，在策略控制的时候我们选择运行模式然后进行回测或者实盘
            e.SetAttribute("KeyCode",keycode);
            e.SetAttribute("DefaultKeyCode", defaultkeycode);
            

            xn.AppendChild(e);
            xmlDoc.Save(@"config\ShortCutKeySetting.xml");

        }

        public static void updateShortCutKey(ShortCutKey sk, string keycode)
        {
            updateShortCutKey(sk.FunctionCode, sk.FunctionName, keycode, sk.DefaultKeyCode);
        }
        public static void updateShortCutKey(string funcode, string funname, string keycode,string defaultkeycode)
        {
            if (!HaveFunCode(funcode))
            {
                addShortCutKey(funcode, funname, keycode, defaultkeycode);
                return;
            }

            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("ShortCutKey");

            XmlElement cfgnode = null;
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FunCode") == funcode)
                    cfgnode = xe;
            }

            cfgnode.SetAttribute("FunName", funname);
            cfgnode.SetAttribute("KeyCode", keycode);
            cfgnode.SetAttribute("DefaultKeyCode",defaultkeycode);
            xmlDoc.Save(@"config\ShortCutKeySetting.xml");

        }

    }
}
