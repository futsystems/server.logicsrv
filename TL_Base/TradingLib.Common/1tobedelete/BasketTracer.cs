//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.Common;
//using System.Collections;
//using System.Data;
//using System.Xml;
//using TradingLib.API;
////using System.Windows.Forms;

//namespace TradingLib.Common
//{

//    //保存具体的合约代码并组成symbol 列表 合约列表使用basketimpl来实现
//    /// <summary>
//    /// symbol:symbol全称
//    /// MasterFN:对应证券全名用于索引security
//    /// 其他相关信息
//    /// </summary>
//    /// 
//    //用于读取 保存 basket列表
//    public class BasketTracker
//    {
//        static string FN{ get { return Platform.SecListFN; } }
//        private static XmlDocument getXMLDoc()
//        {
//            XmlDocument xmlDoc = new XmlDocument();
//            xmlDoc.Load(FN);
//            return xmlDoc;

//        }

//        //更新一个代码列表
//        public static void updateBasket(string  b)
//        {
//            if (!HaveBasket(b))
//            {   //如果没有该sec记录则直接插入
//                addBasket(b);
//                return;
//            }
//            //若有该sec记录，则先删除然后再插入
//            delBasket(b);
//            addBasket(b);
//        }

//        /// <summary>
//        /// 删除代码列表
//        /// </summary>
//        /// <param name="fn"></param>
//        public static void delBasket(string fn)
//        {
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("Name") == fn)
//                {
//                    xn.RemoveChild(x);//删除该节点下所有数据包含子节点本身
//                }
//            }
//            xmlDoc.Save(FN);


//        }

//        /// <summary>
//        /// 增加一个代码列表
//        /// </summary>
//        /// <param name="b"></param>
//        public static void addBasket(string b)
//        {
//            if (HaveBasket(b))
//                return;
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");

//            XmlElement e = xmlDoc.CreateElement("Basket");
//            e.SetAttribute("Name",b);
//            xn.AppendChild(e);
//            xmlDoc.Save(FN);

//        }

//        /// <summary>
//        /// 将某个合约从basket中删除
//        /// </summary>
//        /// <param name="sym"></param>
//        /// <param name="basket"></param>
//        public static void delSecFromBasket(string sym, string basket)
//        {
//            //如果不存在某个basket则我们添加该basket
//            if (!HaveBasket(basket))
//                return;
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("Name") == basket)
//                {
//                    foreach (XmlNode s in x.ChildNodes)
//                    {
//                        XmlElement xs = (XmlElement)s;
//                        if (xs.GetAttribute("Symbol") == sym)
//                        {
//                            x.RemoveChild(xs);//删除该节点下所有数据包含子节点本身
//                        }
//                    }
//                }
//            }
//            xmlDoc.Save(FN);
//        }
//        /// <summary>
//        /// 将某个合约插入到某个Basket中
//        /// </summary>
//        /// <param name="sec"></param>
//        /// <param name="basket"></param>
//        /// <param name="expire"></param>
//        /// <returns></returns>
//        public static string addSecIntoBasket(Symbol sec,string basket,string expire)
//        {
//            //如果不存在某个basket则我们添加该basket
//            string s = string.Empty;
//            if (!HaveBasket(basket))
//                addBasket(basket);
//            //获得basket的node
//            XmlNode bnode = null;
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("Name") == basket)
//                    bnode = x;
//            }

//            //增加一个Security结点
//            switch (sec.Type)
//            {
//                //Fut类型的security需要制定到期日期以及保证金
//                case SecurityType.FUT:
//                    {
//                        XmlElement e = xmlDoc.CreateElement("Sec");
//                        string sym = sec.Symbol + expire;
//                        s = sym;
//                        e.SetAttribute("Symbol", sym);
//                        //检查是否有该symbol如果有则我们返回
//                        if (HaveSecInBasket(sym, basket))
//                            break ;
//                        XmlElement symbol = xmlDoc.CreateElement("MasterSec");//代码从该代码可以从security数据中获取主Security
//                        symbol.InnerText = sec.FullName;
//                        XmlElement secType = xmlDoc.CreateElement("SecType");//类别
//                        secType.InnerText = sec.Type.ToString();
//                        XmlElement margin = xmlDoc.CreateElement("Margin");//保证金
//                        margin.InnerText = sec.Margin.ToString();
//                        XmlElement exdate = xmlDoc.CreateElement("Expire");//保证金
//                        exdate.InnerText = expire;
//                        e.AppendChild(symbol);
//                        e.AppendChild(secType);
//                        e.AppendChild(margin);
//                        e.AppendChild(exdate);
//                        bnode.AppendChild(e);
//                        //return sym;
//                        break;
//                    }
//                case SecurityType.STK:
//                    {
//                        XmlElement e = xmlDoc.CreateElement("Sec");
//                        e.SetAttribute("Symbol", sec.Symbol);
//                        XmlElement symbol = xmlDoc.CreateElement("MasterSec");//代码从该代码可以从security数据中获取主Security
//                        symbol.InnerText = sec.FullName;
//                        XmlElement secType = xmlDoc.CreateElement("SecType");//类别
//                        secType.InnerText = sec.Type.ToString();
//                        e.AppendChild(symbol);
//                        e.AppendChild(secType);
//                        bnode.AppendChild(e);
//                        s = sec.Symbol;
//                        //return sec.Symbol;
//                        break;
//                    }
//                case SecurityType.CASH:
//                    {
//                        XmlElement e = xmlDoc.CreateElement("Sec");
//                        e.SetAttribute("Symbol", sec.Symbol);
//                        XmlElement symbol = xmlDoc.CreateElement("MasterSec");//代码从该代码可以从security数据中获取主Security
//                        symbol.InnerText = sec.FullName;
//                        XmlElement secType = xmlDoc.CreateElement("SecType");//类别
//                        secType.InnerText = sec.Type.ToString();
//                        e.AppendChild(symbol);
//                        e.AppendChild(secType);
//                        bnode.AppendChild(e);
//                        s = sec.Symbol;
//                        //return sec.Symbol;
//                        break;
//                    }
//                default :
                    
//                    break;
//           }
//            xmlDoc.Save(FN);
//            return s;
//        }

//        /// <summary>
//        /// 获得所有basket name列表
//        /// </summary>
//        /// <returns></returns>
//        public static string[] getBaskets()
//        {
//            ArrayList blist = new ArrayList();
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                blist.Add(xe.GetAttribute("Name"));
//            }
//            string[] ret = new string[blist.Count];
//            for (int i = 0; i < blist.Count; i++)
//            {
//                ret[i] = (string)blist[i];
//            }
//            return ret;     
//        }

//        /// <summary>
//        /// 获得所有交易所的security集合
//        /// </summary>
//        /// <returns></returns>
//        //public static Basket getBasket()
//        //{
//        //    Basket b = new BasketImpl();
//        //    List<Exchange> exlist = ExchangeTracker.getExchList();
//        //    foreach (Exchange ex in exlist)
//        //    {
//        //        string exname = ex.Index;
//        //        b.Add(getBasket(exname));
//        //    }
//        //    return b;
        
//        //}

//        /// <summary>
//        /// 获得所有合约实例字典
//        /// </summary>
//        /// <returns></returns>
//        public static Dictionary<string,Security> getSymbolInstanceMap()
//        {
//            Dictionary<string, Security> symbolSecMap = new Dictionary<string, Security>();
//            List<Exchange> exlist = ExchangeTracker.getExchList();
//            foreach (Exchange ex in exlist)
//            {
//                string exname = ex.Index;
//                //得到所有交易所得Symbol
//                foreach (Security s in getBasket(exname).ToArray())
//                {
//                    //MessageBox.Show(s.Symbol.ToString() + "|" + s.MasterSec.ToString());
//                    symbolSecMap.Add(s.Symbol, s);

//                }
//            }
//            return symbolSecMap;

//        }
//        /// <summary>
//        /// 获得symbol-->品种全名 映射字典
//        /// </summary>
//        /// <returns></returns>
//        public static Dictionary<string, string>  getSymbolSecurityMap()
//        {
//            Dictionary<string, string> symbolSecMap = new Dictionary<string, string>();
//            List<Exchange> exlist = ExchangeTracker.getExchList();
//            foreach (Exchange ex in exlist)
//            {
//                string exname = ex.Index;
//                //得到所有交易所得Symbol
//                foreach (Security s in getBasket(exname).ToArray())
//                {
//                    //MessageBox.Show(s.Symbol.ToString() + "|" + s.MasterSec.ToString());
//                    //symbolSecMap.Add(s.Symbol, s.MasterSec.FullName);
                
//                }
//            }
//            return symbolSecMap;
        
//        }
        
//        /// <summary>
//        /// 通过列表名获得security的basket
//        /// </summary>
//        /// <param name="basket"></param>
//        /// <returns></returns>
//        public static Basket getBasket(string basket)
//        {
//            Basket b = new BasketImpl();
//            ArrayList seclist = new ArrayList();
//            b.Name = basket;
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("Name") == basket)
//                {
//                    foreach (XmlNode secnode in xe.ChildNodes)
//                    {
//                        //bool _good = false;
//                        XmlElement sec = (XmlElement)secnode;
//                        string sym = sec.GetAttribute("Symbol");//获得合约symbol
//                        string mastSecname = sec.SelectSingleNode("MasterSec").InnerText.ToString();//获得Security
//                        Security s = SecurityImpl.Parse(mastSecname);//从mastSec可以获得品种,市场,类型等信息
                        
//                        Security mastersec = SecurityTracker.getSecurity(mastSecname);//获得品种实例
//                        if (mastersec == null)
//                        {
//                            x.RemoveChild(sec);
//                            continue;//没有对应的mastersec,则忽略该security
//                        }
//                        /*
//                        if (mastersec == null)
//                        {
//                            x.RemoveChild(sec);
//                            _good = false;
//                        }
//                        else
//                        {
//                            _good = true;
//                        }**/
                            
//                        //s.MasterSec = mastersec;
//                        s.Description = mastersec.Description;
//                        s.MasterSecurity = mastSecname;//主合约名称
//                        s.Currency = mastersec.Currency;
//                        s.PriceTick = mastersec.PriceTick;
//                        s.Symbol = sym;
//                        //利用masterSecurity 来填充 合约priceTick Margin mutiple等参数
//                        switch (s.Type)
//                        {
//                            case SecurityType.FUT:
//                                {
//                                    s.Margin = Convert.ToDecimal(sec.SelectSingleNode("Margin").InnerText.ToString());
//                                    s.Date = Convert.ToInt32(sec.SelectSingleNode("Expire").InnerText.ToString());
//                                    //if (mastersec != null)
//                                        //s.PriceTick = mastersec.PriceTick; s.Multiple = mastersec.Multiple;
//                                    //s.PriceTick = mastersec.PriceTick;
//                                    //s.Multiple = mastersec.Multiple;

//                                    //s.Multiple = 
//                                    break;
//                                }
//                            case SecurityType.STK:
//                                {
//                                    s.Margin = 1;
//                                    break;
//                                }
//                            default:
//                                break;
//                        }
//                        //if(_good)
//                        b.Add(s);//将从xml文档实例化得到的security加入basket
//                    }
//                }
//            }
//            xmlDoc.Save(FN);
//            return b;
//        }

//        /// <summary>
//        /// 从某个列表中获得某个symbol合约
//        /// </summary>
//        /// <param name="symbol"></param>
//        /// <param name="basket"></param>
//        /// <returns></returns>
//        public static Security getSecFromBasket(string symbol,string basket)
//        {
//            Security s = null;
//            //Basket b = new BasketImpl();
//            //ArrayList seclist = new ArrayList();
//            //b.Name = basket;
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("Name") == basket)
//                {
//                    foreach (XmlNode secnode in xe.ChildNodes)
//                    {
//                        XmlElement sec = (XmlElement)secnode;
//                        string sym = sec.GetAttribute("Symbol");//获得合约symbol
//                        if (sym == symbol)
//                        {
//                            //string sym = sec.GetAttribute("Symbol");//获得合约symbol
//                            string mastSecname = sec.SelectSingleNode("MasterSec").InnerText.ToString();//获得Security
//                            s = SecurityImpl.Parse(mastSecname);//从mastSec可以获得品种,市场,类型等信息
//                            Security mastersec = SecurityTracker.getSecurity(mastSecname);//获得品种实例
//                            //s.MasterSec = mastersec;
//                            s.Description = mastersec.Description;
//                            s.MasterSecurity = mastSecname;
//                            s.Symbol = sym;
//                            //利用masterSecurity 来填充 合约priceTick Margin mutiple等参数
//                            switch (s.Type)
//                            {
//                                case SecurityType.FUT:
//                                    {
//                                        s.Margin = Convert.ToDecimal(sec.SelectSingleNode("Margin").InnerText.ToString());
//                                        s.Date = Convert.ToInt32(sec.SelectSingleNode("Expire").InnerText.ToString());
//                                        break;
//                                    }
//                                default:
//                                    break;
//                            }
//                        }

//                        //b.Add(s);//将从xml文档实例化得到的security加入basket
//                    }
//                }
//            }
//            return s;
//        }

//        /// <summary>
//        /// 获得某个basket的symbol列表
//        /// </summary>
//        /// <param name="basket"></param>
//        /// <returns></returns>
//        public static string[] getSymbolList(string basket)
//        {
//            List<string> seclist = new List<string>();
                       
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("Name") == basket)
//                {
//                    foreach (XmlNode secnode in xe.ChildNodes)
//                    {
//                        XmlElement sec = (XmlElement)secnode;
//                        seclist.Add(sec.GetAttribute("Symbol"));
//                    }
//                }
//            }
//            return seclist.ToArray(); 
//        }
//        /// <summary>
//        /// 检查某个列表中是否存在某个symbol
//        /// </summary>
//        /// <param name="symbol"></param>
//        /// <param name="basket"></param>
//        /// <returns></returns>
//        public static bool HaveSecInBasket(string symbol,string basket)
//        { 
//            if(!HaveBasket(basket))
//                return false;
//            //获得basket的node
//            //XmlNode bnode = null;
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("Name") == basket)
//                {
//                    foreach (XmlNode secnode in xe.ChildNodes)
//                    {
//                        XmlElement sec = (XmlElement)secnode;
//                        if (sec.GetAttribute("Symbol") == symbol)
//                            return true;
//                    }
//                }
//            }

//            return false;
//        }
//        //检查是否存在该代码列表
//        public static bool HaveBasket(Basket b)
//        {
//            return HaveBasket(b.Name);
//        }

//        public static bool HaveBasket(string bn)
//        {
//            XmlDocument xmlDoc = getXMLDoc();
//            XmlNode xn = xmlDoc.SelectSingleNode("SecList");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("Name") == bn)
//                    return true;
//            }

//            return false;

//        }
    

    
//    }
//    public class SecurityList
//    {


//    }
//}
