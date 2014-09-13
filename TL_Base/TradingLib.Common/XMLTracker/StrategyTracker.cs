using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using TradingLib.API;
//using System.Windows.Forms;

namespace TradingLib.Common
{
    public class StrategyTracker
    {
        private static XmlDocument getXMLDoc()
        {
            XmlDocument xmlDoc = new XmlDocument();
            //XmlReaderSettings settings = new XmlReaderSettings();
            //settings.IgnoreComments = true;//忽略文档里面的注释
            // XmlReader reader = XmlReader.Create(@"config\exchange.xml", settings);
            xmlDoc.Load(@"config\Strategy.xml");
            return xmlDoc;
        }

        //检查是否有某个Strategy配置节点 某个Strategy可以有多个配置项目
        public static bool HaveStrategy(string FullName)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FullName") == FullName)
                    return true;
            }
            return false;
        }

        //增加一个策略节点
        public static void addStrategy(string FullName)
        {
            if (HaveStrategy(FullName))
                return;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");

            XmlElement e = xmlDoc.CreateElement("Strategy");
            e.SetAttribute("FullName",FullName);
            xn.AppendChild(e);
            xmlDoc.Save(@"config\Strategy.xml");
        }
        public static void addStrategyConfig(int rid, string fullname,string symbols, string datadriver, string strategyconfig)
        {
            addStrategyConfig(rid, fullname, QSEnumStrategyMode.HIST.ToString(), symbols, datadriver, strategyconfig);
        }
        //将某个Strategy配置加入到配置文件中
        public static void addStrategyConfig(int rid,string fullname,string mode,string symbols,string datadriver,string strategyconfig)
        {
            //策略参数
            //1.策略所监听的symbol数据
            //2.策略驱动模式周期模式
            //3.策略参数配置
            string rname = fullname;//获得策略类型全名
            if (!HaveStrategy(rname))
                addStrategy(rname);
            //如果已存在该策略配置则返回
            if (haveStrategyConfig(rid,fullname))
                return;

            
            XmlNode bnode = null;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FullName") == rname)
                    bnode = x;
            }

            XmlElement e = xmlDoc.CreateElement("StrategyConfig");
            e.SetAttribute("ID", rid.ToString());
            e.SetAttribute("Mode", mode);//增加Strategy的时候我们不需要设定其运行模式，在策略控制的时候我们选择运行模式然后进行回测或者实盘
            e.SetAttribute("Symbols", symbols);
            e.SetAttribute("DataDriver", datadriver);
            e.SetAttribute("Text", strategyconfig);
            bnode.AppendChild(e);
            xmlDoc.Save(@"config\Strategy.xml");
        }

        public static void updateStrategyConfig(int rid, string fullname,string mode, string symbols, string datadriver, string strategyconfig)
        {
            //策略参数
            //1.策略所监听的symbol数据
            //2.策略驱动模式周期模式
            //3.策略参数配置
            string rname = fullname;//获得策略类型全名
            if (!HaveStrategy(rname))
                addStrategy(rname);
            //如果已存在该策略配置则返回
            if (!haveStrategyConfig(rid, fullname))//如果没有该策略则添加该策略
            {
                addStrategyConfig(rid, fullname, mode,symbols, datadriver, strategyconfig);
                return;
            }
            XmlNode bnode = null;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FullName") == fullname)
                    bnode = x;
            }
            XmlElement cfgnode = null;
            foreach (XmlNode x in bnode.ChildNodes)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("ID") == rid.ToString())
                    cfgnode = xe;
            }
            //MessageBox.Show(rid.ToString() + "|" + fullname + "|" + datadriver);
            if (mode != null)
                cfgnode.SetAttribute("Mode", mode);
            if (symbols != null)
                cfgnode.SetAttribute("Symbols", symbols);
            if (datadriver != null)
                cfgnode.SetAttribute("DataDriver", datadriver);
            if (strategyconfig != null)
                cfgnode.SetAttribute("Text", strategyconfig);
            
            xmlDoc.Save(@"config\Strategy.xml");
            


            /*
            e.SetAttribute("ID", rid.ToString());
            e.SetAttribute("Symbols", symbols);
            e.SetAttribute("DataDriver", datadriver);
            e.SetAttribute("Text", strategyconfig);
            bnode.AppendChild(e);
            xmlDoc.Save(@"config\Strategy.xml");
             * */
        }

        public static int MaxResponseID()
        {
            int res = 0;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                foreach (XmlNode x1 in x.ChildNodes)
                {
                    XmlElement xe = (XmlElement)x1;
                    res = Math.Max(res, int.Parse(xe.GetAttribute("ID")));
                    
                }
            }
            return res > 1000 ? res : 1000; 
        }
        public static List<string> getStrategyConfigs(string fullname)
        {
            List<string> l = new List<string>();
            //if (!HaveSymbol(sym))
            //    return l;
            //获得basket的node
            XmlNode bnode = null;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FullName") == fullname)
                    bnode = x;
            }
            if (bnode == null)
                return l;

            foreach (XmlNode x in bnode.ChildNodes)
            {
                XmlElement xe = (XmlElement)x;
                l.Add(xe.GetAttribute("ID")+":"+xe.GetAttribute("Mode")+":"+xe.GetAttribute("Symbols") + ":" + xe.GetAttribute("DataDriver")+":"+xe.GetAttribute("Text"));
            }
            return l;
        }

        public static string getStrategyRunMode(int rid,string fullname)
        {
            XmlNode bnode = null;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FullName") == fullname)
                    bnode = x;
            }
            if (bnode == null)
                return string.Empty;
            foreach (XmlNode x in bnode.ChildNodes)
            {
                XmlElement xe = (XmlElement)x;
                if ((xe.GetAttribute("ID") == rid.ToString()))
                    return xe.GetAttribute("Mode");
            }
            return string.Empty;
        }
        public static bool haveStrategyConfig(int rid,string fullname)
        {
            XmlNode bnode = null;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FullName") == fullname)
                    bnode = x;
            }
            if (bnode == null)
                return false;
            foreach (XmlNode x in bnode.ChildNodes)
            {
                XmlElement xe = (XmlElement)x;
                if ((xe.GetAttribute("ID") == rid.ToString()))
                    return true;
            }
            return false;
        }

        /*
        public static bool haveStrategyConfig(string fullname, string symbols, string datadriver, string strategyconfig)
        {
            XmlNode bnode = null;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FullName") == fullname)
                    bnode = x;
            }
            if (bnode == null)
                return false;
            foreach (XmlNode x in bnode.ChildNodes)
            {
                XmlElement xe = (XmlElement)x;
                if ((xe.GetAttribute("Symbols") == symbols) && (xe.GetAttribute("DataDriver") == datadriver)  && (xe.GetAttribute("Text") == strategyconfig))
                    return true;
            }
            return false;  
        }**/


        public static void delStrategyConfig(int rid, string fullname)
        {
            XmlNode bnode = null;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FullName") == fullname)
                    bnode = x;
            }
            if (bnode == null)
                return;
            foreach (XmlNode x in bnode.ChildNodes)
            {
                XmlElement xe = (XmlElement)x;
                if ((xe.GetAttribute("ID") == rid.ToString()))
                    bnode.RemoveChild(x);
            }
            xmlDoc.Save(@"config\Strategy.xml");
        }

        /*
        public static void delStrategyConfig(string fullname, string symbols, string datadriver, string config)
        {
            XmlNode bnode = null;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("StrategyList");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("FullName") == fullname)
                    bnode = x;
            }
            if (bnode == null)
                return;
            foreach (XmlNode x in bnode.ChildNodes)
            {
                XmlElement xe = (XmlElement)x;
                if ((xe.GetAttribute("Symbols") == symbols) && (xe.GetAttribute("DataDriver") == datadriver) && (xe.GetAttribute("Text") ==config))
                    bnode.RemoveChild(x);
            }
            xmlDoc.Save(@"config\Strategy.xml");
            
        }
         * 
         * */
    }

    
}
