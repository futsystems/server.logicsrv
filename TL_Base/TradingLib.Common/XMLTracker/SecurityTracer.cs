//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.Common;
//using System.Collections;
//using System.Data;
//using System.Xml;
//using TradingLib.API;
//using System.IO;


//namespace TradingLib.Common
//{
    
//    public class SecurityTracker
//    {
//        const string FULLNAME = "全称";
//        const string SYMBOL = "代码";
//        const string EXCHANGE = "交易所";
//        const string DESCRIPTION = "名称";
//        const string TYPE = "类型";
//        const string MULTIPLE = "乘数";
//        const string PRICETICK = "跳";
//        const string MARGIN = "保证金";
//        const string ENTRYCOMM = "开仓手续费";
//        const string EXITCOMM = "平仓手续费";
//        public event DebugDelegate SendDebugEvent;


//        static string FN{get{return Platform.SecurityFN;}}
//        public static DataTable getSecuityTable()
//        {
//            return getSecuityTable(null);
//        }
//        public static DataTable getSecuityTable(string accountID)
//        {
//            return getSecuityTable(accountID,string.Empty, string.Empty);
//        }

//        public static DataTable getSecuityTable(string secType, string exchange)
//        {
//            return getSecuityTable(null, secType, exchange);
//        }
//        public static DataTable getSecuityTable(string accountID,string secType,string exchange)

//        {
//            DataTable table = new DataTable();
//            table.Columns.Add(FULLNAME);
//            table.Columns.Add(SYMBOL);
//            table.Columns.Add(EXCHANGE);
//            table.Columns.Add(DESCRIPTION);
//            table.Columns.Add(TYPE);
//            table.Columns.Add(MULTIPLE);
//            table.Columns.Add(PRICETICK);
//            table.Columns.Add(MARGIN);
//            table.Columns.Add(ENTRYCOMM);
//            table.Columns.Add(EXITCOMM);
//            XmlDocument xmlDoc = getXMLDoc(accountID);
//            XmlNode xn = xmlDoc.SelectSingleNode("Security");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                string e = x.SelectSingleNode("Exchange").InnerText.ToString();
//                string s = x.SelectSingleNode("SecType").InnerText.ToString();
//                if (((secType == string.Empty) || (secType == s)) && ((exchange == string.Empty) || (exchange == e)))
//                {
//                    table.Rows.Add(new object[] { xe.GetAttribute("FN"), xe.SelectSingleNode("Symbol").InnerText, xe.SelectSingleNode("Exchange").InnerText, xe.SelectSingleNode("Description").InnerText, xe.SelectSingleNode("SecType").InnerText, xe.SelectSingleNode("Multiple").InnerText, xe.SelectSingleNode("PriceTick").InnerText, xe.SelectSingleNode("Margin").InnerText, xe.SelectSingleNode("Entry").InnerText, xe.SelectSingleNode("Exit").InnerText });

//                }
//            }
//            return table;
//        }
//        public static XmlDocument getXMLDoc()
//        {
//            XmlDocument xmlDoc = new XmlDocument();
//            xmlDoc.Load(FN);
//            return xmlDoc;
//        }
       
//        获得某个账户的合约配置信息
//        public static XmlDocument getXMLDoc(string accountID)
//        {
//            if (accountID == null)
//                return getXMLDoc();
//            string filename = LibGlobal.CONFIGPATH + accountID + @"\security.xml";
//            if (File.Exists(filename))
//            {
//                XmlDocument xmlDoc = new XmlDocument();
//                xmlDoc.Load(filename);
//                return xmlDoc;
//            }
//            else
//            {
//                if (!Directory.Exists(LibGlobal.CONFIGPATH + accountID))
//                    Directory.CreateDirectory(LibGlobal.CONFIGPATH + accountID);
//                File.Copy(FN, filename);
//                XmlDocument xmlDoc = new XmlDocument();
//                xmlDoc.Load(filename);
//                return xmlDoc;
//            }
//        }
//        删除一个品种 
//        public static void delSecurity(SecurityBase sec)
//        {
//            delSecurity(sec, null);
//        }
//        public static void delSecurity(SecurityBase sec,string accountID)
//        {
//            delSecurity(sec.FullName,accountID);//品种全称 品种代码 交易所 类型 就对品种做了一个唯一标识
//        }

//        public static void delSecurity(string fn)
//        {
//            delSecurity(fn, null);
//        }

//        public static void delSecurity(string fn,string accountID)
//        {
//            XmlDocument xmlDoc = getXMLDoc(accountID);
//            XmlNode xn = xmlDoc.SelectSingleNode("Security");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("FN") == fn)
//                {
//                    xn.RemoveChild(x);//删除该节点下所有数据包含子节点本身
//                }
//            }
//            string filename = accountID == null ? FN : @"config\Account\" + accountID + @"\security.xml";
//            xmlDoc.Save(filename);
//        }



//        通过账户来查询该账户对应的品种列表,如果没有配置文件则返回默认的品种列表
//        public static List<Security> getMastSecurities(string accountID)
//        {
//            return getMastSecurities(getXMLDoc(accountID));
//        }
//        获得所有默认品种列表
//        public static List<Security> getMastSecurities()
//        {
//            return getMastSecurities(getXMLDoc());
//        }
//        从某个xmldoc中获得品种列表
//        private static List<Security> getMastSecurities(XmlDocument doc)
//        {
//            List<Security> seclist = new List<Security>();
//            XmlNode xn = doc.SelectSingleNode("Security");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
                
//                XmlElement xe = (XmlElement)x;
//                SecurityImpl.Parse(fn)
//                string sym = xe.SelectSingleNode("Symbol").InnerText.ToString();
//                string ex = xe.SelectSingleNode("Exchange").InnerText.ToString();
//                string desp = xe.SelectSingleNode("Description").InnerText.ToString();
//                MessageBox.Show(xe.SelectSingleNode("Currency").InnerText);
//                CurrencyType currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), xe.SelectSingleNode("Currency").InnerText.ToString(), true);
//                MessageBox.Show(sym + "|" + desp);
//                SecurityType type = (SecurityType)Enum.Parse(typeof(SecurityType), xe.SelectSingleNode("SecType").InnerText.ToString(), true);
//                int multiple = Convert.ToInt32(xe.SelectSingleNode("Multiple").InnerText.ToString());
//                decimal pricetick = Convert.ToDecimal(xe.SelectSingleNode("PriceTick").InnerText.ToString());
//                decimal margin = Convert.ToDecimal(xe.SelectSingleNode("Margin").InnerText.ToString());
//                decimal entry = Convert.ToDecimal(xe.SelectSingleNode("Entry").InnerText.ToString());
//                decimal exit = Convert.ToDecimal(xe.SelectSingleNode("Exit").InnerText.ToString());
//                Security sec = new SecurityBase(sym, ex, type, desp, multiple, pricetick, margin);
//                sec.EntryCommission = entry;
//                sec.ExitCommission = exit;
//                sec.Currency = currency;
//                seclist.Add(sec);
//            }
//            return seclist;
//        }


//        public static Security getSecurity(string fn)
//        {
//            return getSecurity(fn, null);
//        }
//        从xml还原出secueitybase
//        public static Security getSecurity(string fn,string accountID)
//        {
//            XmlDocument xmlDoc = getXMLDoc(accountID);
//            XmlNode xn = xmlDoc.SelectSingleNode("Security");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("FN") == fn)
//                {   
//                    SecurityImpl.Parse(fn)
//                    string sym = xe.SelectSingleNode("Symbol").InnerText.ToString();
//                    string ex = xe.SelectSingleNode("Exchange").InnerText.ToString();
//                    string desp = xe.SelectSingleNode("Description").InnerText.ToString();
//                    CurrencyType currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), xe.SelectSingleNode("Currency").InnerText.ToString(), true);
//                    SecurityType type = (SecurityType)Enum.Parse(typeof(SecurityType), xe.SelectSingleNode("SecType").InnerText.ToString(), true);
//                    int multiple = Convert.ToInt32(xe.SelectSingleNode("Multiple").InnerText.ToString());
//                    decimal pricetick = Convert.ToDecimal(xe.SelectSingleNode("PriceTick").InnerText.ToString());
//                    decimal margin = Convert.ToDecimal(xe.SelectSingleNode("Margin").InnerText.ToString());
//                    decimal entry = Convert.ToDecimal(xe.SelectSingleNode("Entry").InnerText.ToString());
//                    decimal exit = Convert.ToDecimal(xe.SelectSingleNode("Exit").InnerText.ToString());
//                    Security sec= new SecurityBase(sym, ex, type,desp,multiple, pricetick, margin);
//                    sec.EntryCommission = entry;
//                    sec.ExitCommission = exit;
//                    sec.Currency = currency;
//                    return sec;
//                }  
//            }
//            return null;
//        }

//        增加一个交易品种
//        public static void addSecurity(SecurityBase sec)
//        {
//            addSecurity(sec, null);
//        }

//        public static void addSecurity(SecurityBase sec,string accountID)
//        {
//            if (HaveSecurity(sec,accountID))
//            {
//                updateSecurity(sec);
//                return;
//            }
//            XmlDocument xmlDoc = getXMLDoc(accountID);
//            XmlNode xn = xmlDoc.SelectSingleNode("Security");

//            XmlElement e = xmlDoc.CreateElement("Sec");
//            e.SetAttribute("FN", sec.FullName);

//            XmlElement symbol = xmlDoc.CreateElement("Symbol");//代码
//            symbol.InnerText = sec.Symbol;
//            XmlElement exchange = xmlDoc.CreateElement("Exchange");//交易所
//            exchange.InnerText = sec.DestEx;
//            XmlElement currency = xmlDoc.CreateElement("Currency");//交易所
//            currency.InnerText = sec.Currency.ToString();
//            XmlElement des = xmlDoc.CreateElement("Description");//交易所
//            des.InnerText = sec.Description;
//            XmlElement secType = xmlDoc.CreateElement("SecType");//类别
//            secType.InnerText = sec.Type.ToString();
//            XmlElement multiple = xmlDoc.CreateElement("Multiple");//乘数
//            multiple.InnerText = sec.Multiple.ToString();
//            XmlElement priceTick = xmlDoc.CreateElement("PriceTick");//最小价格变动
//            priceTick.InnerText = sec.PriceTick.ToString();
//            XmlElement marginRation = xmlDoc.CreateElement("Margin");//保证金
//            marginRation.InnerText = sec.Margin.ToString();
//            XmlElement entry = xmlDoc.CreateElement("Entry");//保证金
//            entry.InnerText = sec.EntryCommission.ToString();
//            XmlElement exit = xmlDoc.CreateElement("Exit");//保证金
//            exit.InnerText = sec.ExitCommission.ToString();



//            e.AppendChild(symbol);
//            e.AppendChild(exchange);
//            e.AppendChild(des);
//            e.AppendChild(currency);
//            e.AppendChild(secType);
//            e.AppendChild(multiple);
//            e.AppendChild(priceTick);
//            e.AppendChild(marginRation);
//            e.AppendChild(entry);
//            e.AppendChild(exit);
//            xn.AppendChild(e);

//            string filename =accountID==null? FN :@"config\Account\" + accountID + @"\security.xml";
//            xmlDoc.Save(filename);

//        }

//        public static void updateSecurity(SecurityBase sec)
//        {
//            updateSecurity(sec,null);
//        }
//        更新一个品种
//        public static void updateSecurity(SecurityBase sec,string accountID)
//        {
//            if (!HaveSecurity(sec,accountID))
//            {   //如果没有该sec记录则直接插入
//                addSecurity(sec,accountID);
//                return;
//            }
//            /*
//            若有该sec记录，则先删除然后再插入
//            delSecurity(sec.FullName);
//            addSecurity(sec);
//             * **/
//            XmlDocument xmlDoc = getXMLDoc(accountID);
//            XmlNode xn = xmlDoc.SelectSingleNode("Security");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("FN") == sec.FullName)
//                {
//                    xe.SelectSingleNode("Symbol").InnerText = sec.Symbol;
//                    xe.SelectSingleNode("Exchange").InnerText = sec.DestEx;
//                    xe.SelectSingleNode("Description").InnerText = sec.Description;
//                    xe.SelectSingleNode("Currency").InnerText = sec.Currency.ToString();
//                    xe.SelectSingleNode("SecType").InnerText = sec.Type.ToString();
//                    xe.SelectSingleNode("Multiple").InnerText = sec.Multiple.ToString();
//                    xe.SelectSingleNode("PriceTick").InnerText = sec.PriceTick.ToString();
//                    xe.SelectSingleNode("Margin").InnerText = sec.Margin.ToString();
//                    xe.SelectSingleNode("Entry").InnerText = sec.EntryCommission.ToString();
//                    xe.SelectSingleNode("Exit").InnerText = sec.ExitCommission.ToString();

//                    string filename = accountID == null ? FN : @"config\Account\" + accountID + @"\security.xml";
//                    xmlDoc.Save(filename);
//                }
//            }
//        }

//        检查是否已经存在该交易品种
//        public static bool HaveSecurity(Security sec)
//        {
//            return HaveSecurity(sec, null);
//        }
//        public static bool HaveSecurity(Security sec,string accountID)
//        {
//            return HaveSecurity(sec.FullName,accountID);
//        }
//        public static bool HaveSecurity(string fn)
//        {
//            return HaveSecurity(fn,null);
//        }
//        public static bool HaveSecurity(string fn,string accountID)
//        {
//            XmlDocument xmlDoc = getXMLDoc(accountID);
//            XmlNode xn = xmlDoc.SelectSingleNode("Security");
//            XmlNodeList exlist = xn.ChildNodes;
//            foreach (XmlNode x in exlist)
//            {
//                XmlElement xe = (XmlElement)x;
//                if (xe.GetAttribute("FN") == fn)
//                    return true;
//            }
//            return false;
//        }




//    }
//    public class SecurityBase : SecurityImpl,Security
//    {
//        public SecurityBase(string sym, string exchange, int mutiple, decimal priceTick, decimal margin)
//            : base(sym, exchange, SecurityType.FUT)
//        {
//            Margin = margin;
//            PriceTick = priceTick;
//            Multiple = mutiple;

//        }
      

//        public SecurityBase(string sym, string exchange,SecurityType type,string des, int mutiple, decimal priceTick, decimal margin)
//            : base(sym, exchange, type)
//        {
//            Description = des;
//            Margin = margin;
//            PriceTick = priceTick;
//            Multiple = mutiple;
//        }

//    }
//}
