using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TradingLib.API;

namespace TradingLib.Common
{
    public class AccountCheckTracker
    {

        const string FN = @"config\AccountCheck.xml";
        public static XmlDocument getXMLDoc()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FN);

            return xmlDoc;
        }

        public static void delRuleFromAccount(string acc)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("RuleSet");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("ID") == acc)
                {
                    //xe.RemoveAll();
                    
                    foreach (XmlNode rsnode in xe.ChildNodes)
                    {
                        XmlElement rs = (XmlElement)rsnode;
                        x.RemoveChild(rs);
                    }
                }
            }
            xmlDoc.Save(FN);

        }
        public static void delRuleFromAccount(string acc, IAccountCheck rc)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("RuleSet");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("ID") == acc)
                {
                    foreach (XmlNode rsnode in xe.ChildNodes)
                    {
                        XmlElement rs = (XmlElement)rsnode;
                        if (rs.GetAttribute("RuleName") == rc.GetType().FullName && rs.GetAttribute("Compare") == rc.Compare.ToString() && rs.GetAttribute("Value") == rc.Value.ToString() && rs.GetAttribute("Symbols") == rc.SymbolSet.ToString())
                            x.RemoveChild(rs);
                    }
                }
            }
            xmlDoc.Save(FN);

        }
        public static List<string> getRuleTextFromAccount(string acc)
        {
            List<string> r = new List<string>();
            if (!HaveAccount(acc))
                return  r;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("RuleSet");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("ID") == acc)
                {
                    foreach (XmlNode rsnode in xe.ChildNodes)
                    {
                        XmlElement rs = (XmlElement)rsnode;
                        
                        r.Add(rs.GetAttribute("RuleName")+","+rs.GetAttribute("Enable")+","+rs.GetAttribute("Compare")+","+rs.GetAttribute("Value")+","+rs.GetAttribute("Symbols"));
                    }

                }
            }
            return r;
        }
        public static void addRuleIntoAccount(string acc, IAccountCheck rc)
        {
            if (!HaveAccount(acc))
                addAccount(acc);
            if (AccountHaveRule(acc, rc))
            {
                //return;
                delRuleFromAccount(acc, rc);
            }
            XmlNode bnode = null;
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("RuleSet");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("ID") == acc)
                {
                    bnode = x;
                }

            }

            XmlElement e = xmlDoc.CreateElement("RuleCheck");
            e.SetAttribute("RuleName", rc.GetType().FullName);
            e.SetAttribute("Enable", rc.Enable.ToString());
            e.SetAttribute("Compare", rc.Compare.ToString());
            e.SetAttribute("Value", rc.Value.ToString());
            e.SetAttribute("Symbols", rc.SymbolSet.ToString());
            bnode.AppendChild(e);

            xmlDoc.Save(FN);
            
        }

        public static void addAccount(string acc)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("RuleSet");

            XmlElement e = xmlDoc.CreateElement("Account");
            e.SetAttribute("ID",acc);
            xn.AppendChild(e);
            xmlDoc.Save(FN);
        
        
        }

        public static bool HaveAccount(string acc)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("RuleSet");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("ID") == acc)
                    return true;
            }
            return false;
        }

        public static bool AccountHaveRule(string acc, IAccountCheck rc)
        {
            XmlDocument xmlDoc = getXMLDoc();
            XmlNode xn = xmlDoc.SelectSingleNode("RuleSet");
            XmlNodeList exlist = xn.ChildNodes;
            foreach (XmlNode x in exlist)
            {
                XmlElement xe = (XmlElement)x;
                if (xe.GetAttribute("ID") == acc)
                {
                    foreach (XmlNode rsnode in xe.ChildNodes)
                    {
                        XmlElement rs = (XmlElement)rsnode;
                        if (rs.GetAttribute("RuleName") == rc.GetType().FullName && rs.GetAttribute("Compare") == rc.Compare.ToString() && rs.GetAttribute("Value") == rc.Value.ToString() && rs.GetAttribute("Symbols") == rc.SymbolSet.ToString())
                            return true;
                    }
                    return false;
                }
            }
            return false;
            
        }
    }
}
