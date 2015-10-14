using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using TradingLib.API;


namespace TradingLib.Common
{

    public class TimeZoneIDTracker
    {
        Dictionary<int, TimeZoneIDPair> timezonemap = new Dictionary<int, TimeZoneIDPair>();


        public IEnumerable<TimeZoneIDPair> TimeZoneIDPiars
        {
            get
            {
                return timezonemap.Values;
            }
        }
        /// <summary>
        /// 获得TimeZoneIDPairt
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TimeZoneIDPair this[int id]
        {
            get
            {
                if (timezonemap.Keys.Contains(id))
                {
                    return timezonemap[id];
                }
                return null;
            }
        }

        public TimeZoneIDTracker()
        {
            foreach (var tz in LoadXMLTimeZoneIDPairs())
            {
                timezonemap.Add(tz.ID, tz);
            }
        }

        /// <summary>
        /// 加载xml rspinfo
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TimeZoneIDPair> LoadXMLTimeZoneIDPairs()
        {
            string xmlfile = Util.GetConfigFile("timezone.xml");
            XmlDocument xmlDoc = null;
            if (File.Exists(xmlfile))
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(xmlfile);
            }

            List<TimeZoneIDPair> rsplist = new List<TimeZoneIDPair>();
            XmlNode xn = xmlDoc.SelectSingleNode("timezones");
            XmlNodeList timezones = xn.ChildNodes;

            foreach (XmlNode node in timezones)
            {
                try
                {
                    XmlElement timezone = (XmlElement)node;
                    string winid = timezone.GetAttribute("windowsid");
                    string unixid = timezone.GetAttribute("unixid");
                    string tz = timezone.GetAttribute("timezone");
                    int id = int.Parse(timezone.GetAttribute("id"));

                    rsplist.Add(new TimeZoneIDPair() { TimeZone = tz, Windows_TimeZoneID = winid, Unix_TimeZoneID = unixid,ID=id });
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
