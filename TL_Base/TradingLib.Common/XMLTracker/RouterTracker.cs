using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace TradingLib.Common
{
    public class RouterTracker
    {
        private static XmlDocument getXMLDoc()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"config\Router.xml");
            return xmlDoc;
        }

        #region Broker交易通道
        //获得某个交易所得交易路由
        public static string getBrockerRoute(string exidx)
        {
            if (!haveBrokerRoute(exidx))
            {
                return null;
            }
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Router");
            //XmlNodeList exlist = xn.ChildNodes;
            XmlNode brokernode = xn.SelectSingleNode("BrokerRouter");

            foreach (XmlNode br in brokernode.ChildNodes)
            {
                XmlElement bre = (XmlElement)br;
                if (bre.GetAttribute("Exchange") == exidx)
                {
                    return bre.GetAttribute("Connect");
                }
            }
            return null;
        
        }
        //更新某个交易所得交易路由
        public static void updateBrockerRoute(string exidx, string connect)
        {
            if (!haveBrokerRoute(exidx))
            {
                addBrockerRoute(exidx, connect);
                return;
            }
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Router");
            //XmlNodeList exlist = xn.ChildNodes;
            XmlNode brokernode  =  xn.SelectSingleNode("BrokerRouter");

            foreach (XmlNode br in brokernode.ChildNodes)
            {
                XmlElement bre = (XmlElement)br;
                if (bre.GetAttribute("Exchange") == exidx)
                {
                    bre.SetAttribute("Connect",connect);
                }
            }
            xmlDoc.Save(@"config\Router.xml");
        }

       

        //增加一个交易所的交易路由
        public static void addBrockerRoute(string exidx, string connect)
        {
            if (haveBrokerRoute(exidx))
            {
                updateBrockerRoute(exidx,connect);
                return;
            }
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Router");
            //XmlNodeList exlist = xn.ChildNodes;
            XmlNode brokernode = xn.SelectSingleNode("BrokerRouter");

            XmlElement e = xmlDoc.CreateElement("BR");
            e.SetAttribute("Exchange", exidx);
            e.SetAttribute("Connect", connect);//增加Strategy的时候我们不需要设定其运行模式，在策略控制的时候我们选择运行模式然后进行回测或者实盘

            brokernode.AppendChild(e);
            xmlDoc.Save(@"config\Router.xml");
        }

        //检查是否有某个交易所的交易路由记录
        private static bool haveBrokerRoute(string exchange)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Router");
            //XmlNodeList exlist = xn.ChildNodes;
            XmlNode brokernode = xn.SelectSingleNode("BrokerRouter");
            foreach (XmlNode br in brokernode.ChildNodes)
            {
                XmlElement bre = (XmlElement)br;
                if (bre.GetAttribute("Exchange") == exchange)
                    return true;
            }
            return false;
        }
        #endregion


        #region DataFeed数据通道
        //获得某个交易所得交易路由
        public static string getDataFeedRoute(string exidx)
        {
            if (!haveDataFeedRoute(exidx))
            {
                return null;
            }
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Router");
            //XmlNodeList exlist = xn.ChildNodes;
            XmlNode nodes = xn.SelectSingleNode("DataFeedRouter");

            foreach (XmlNode br in nodes.ChildNodes)
            {
                XmlElement bre = (XmlElement)br;
                if (bre.GetAttribute("Exchange") == exidx)
                {
                    return bre.GetAttribute("Connect");
                }
            }
            return null;

        }

        //更新某个交易所得交易路由
        public static void updateDataFeedRoute(string exidx, string connect)
        {
            if (!haveDataFeedRoute(exidx))
            {
                addDataFeedRoute(exidx, connect);
                return;
            }
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Router");
            //XmlNodeList exlist = xn.ChildNodes;
            XmlNode brokernode = xn.SelectSingleNode("DataFeedRouter");

            foreach (XmlNode br in brokernode.ChildNodes)
            {
                XmlElement bre = (XmlElement)br;
                if (bre.GetAttribute("Exchange") == exidx)
                {
                    bre.SetAttribute("Connect", connect);
                }
            }
            xmlDoc.Save(@"config\Router.xml");
        }

        //增加一个交易所的数据路由
        public static void addDataFeedRoute(string exidx, string connect)
        {
            if (haveDataFeedRoute(exidx))
            {
                updateDataFeedRoute(exidx, connect);
                return;
            }
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Router");
            //XmlNodeList exlist = xn.ChildNodes;
            XmlNode nodes = xn.SelectSingleNode("DataFeedRouter");

            XmlElement e = xmlDoc.CreateElement("DR");
            e.SetAttribute("Exchange", exidx);
            e.SetAttribute("Connect", connect);//增加Strategy的时候我们不需要设定其运行模式，在策略控制的时候我们选择运行模式然后进行回测或者实盘

            nodes.AppendChild(e);
            xmlDoc.Save(@"config\Router.xml");
        }




        //检查是否有某个交易所的数据路由记录
        private static bool haveDataFeedRoute(string exchange)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("Router");
            //XmlNodeList exlist = xn.ChildNodes;
            XmlNode nodes = xn.SelectSingleNode("DataFeedRouter");
            foreach (XmlNode br in nodes.ChildNodes)
            {
                XmlElement bre = (XmlElement)br;
                if (bre.GetAttribute("Exchange") == exchange)
                    return true;
            }
            return false;
        }
        #endregion 

    }
}
