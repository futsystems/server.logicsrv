using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
//using System.Windows.Forms;

namespace TradingLib.Common
{

    public class MktTime
    {
        public MktTime(int starttime, int stoptime)
        {
            this.StartTime = starttime;
            this.EndTime = stoptime;
        }
        public MktTime()
        {
            this.StartTime = 0;
            this.EndTime = 0;
        }
        /// <summary>
        /// 距离该时间段开始还有多少秒
        /// </summary>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public int StartDiff
        {
            get
            {
                int diff = Util.FTDIFF(Util.ToTLTime(), this.StartTime);
                if (diff < 0)
                    diff += 86400;
                return diff;
            }
        }
        public bool CrossSpan(MktTime t)
        {
            if (IsInSpan(t.StartTime) || IsInSpan(t.EndTime))
                return true;
            if (t.IsInSpan(this.StartTime) || t.IsInSpan(this.EndTime))
                return true;
            return false;
        }

        public bool IsInSpan(MktTime t)
        {
            if (this.IsInSpan(t.StartTime) && this.IsInSpan(t.EndTime))
                return true;
            return false;
        }

        public static MktTime UnionSpan(MktTime t1, MktTime t2)
        {
            return new MktTime(Math.Min(t1.StartTime, t2.StartTime), Math.Max(t1.EndTime, t2.EndTime));
        }
        public bool IsInSpan(int time)
        {
            if (time >= this.StartTime && time < this.EndTime)
                return true;
            return false;
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public int EndTime { get; set; }

        public override string ToString()
        {
            return "TimeSpan Start:" + this.StartTime.ToString() + " - End:" + this.EndTime.ToString();
        }
    }

    /// <summary>
    /// 时段
    /// </summary>
    public class MktTimeEntry : MktTime
    {

        public bool SameTimeSpan(MktTimeEntry obj)
        {
            MktTimeEntry other = obj;
            if (other.StartTime == this.StartTime && other.EndTime == this.EndTime)
                return true;
            return false;
        }

        /// <summary>
        /// 开始强平时间
        /// </summary>
        public int FlatStartTime { get; set; }

        /// <summary>
        /// 是否是强平时间点
        /// 关于强平时间点与尾盘时间点的逻辑分析
        /// 需要强平那么就需要强平时间点禁止开仓,否则强平后还会出现开仓
        /// </summary>
        public bool NeedFlat { get; set; }


        /// <summary>
        /// 检查是否处于强平状态
        /// </summary>
        public bool IsFlatTime()
        {
            int now = Util.ToTLTime();
            //LibUtil.Debug("NeedFlatCheck, now:" + now.ToString() +" NeedFlat:"+NeedFlat.ToString() +" FlatStart:" + FlatStartTime.ToString() + " End:" + EndTime.ToString());
            if (!this.NeedFlat)
                return false;
            //2.在结束前5分钟
            return now >= FlatStartTime && now <= EndTime;

            
        }
        public MktTimeEntry(int start, int end,bool needflat)
        {
            StartTime = start;
            EndTime = end;
            NeedFlat = needflat;
            if (NeedFlat)
            {
                //计算开始强平时间
                DateTime t = Util.ToDateTime(Util.ToTLDate(), end).Subtract(new TimeSpan(0,GlobalConfig.FlatTimeAheadOfMarketClose, 0));
                FlatStartTime = Util.ToTLTime(t);
            }
            else
            {
                FlatStartTime = 0;
            }
        }

        /// <summary>
        /// 判断当前时间是否在时间段内
        /// </summary>
        /// <returns></returns>
        public bool IsOpenTime()
        {
            
            int now = Util.ToTLTime();
            //LibUtil.Debug("OpenTimeCheck, now:" + now.ToString() + " Start:" + StartTime.ToString() + " End:" + EndTime.ToString());
            return now >= StartTime && now <= EndTime;
        }

        public bool IsInTimeEntry(int time)
        {
            return time >= StartTime && time <= EndTime;
        }

        public  string Serialize()
        {
            return "#" + StartTime.ToString() + "-" + EndTime.ToString() + (NeedFlat ? "*" : "");
        }

        public static MktTimeEntry Deserialize(string msg)
        {
            try
            {
                string[] p = msg.Split('-');
                if (p.Length < 2) return null;
                bool needflat = false;
                if (p[1].EndsWith("*"))
                {
                    needflat = true;
                    p[1] = p[1].TrimEnd(new char[] { '*' });
                }
                MktTimeEntry se = new MktTimeEntry(int.Parse(p[0]), int.Parse(p[1]),needflat);
                return se;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
    }

    /// <summary>
    /// 交易所交易时段
    /// </summary>
    public class MarketTime : IMarketTime
    {
        /// <summary>
        /// 数据库全局编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 时间段名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 时间段描述
        /// </summary>
        public string Description { get; set; }


        List<MktTimeEntry> sessionlist = new List<MktTimeEntry>();
        /// <summary>
        /// 是否是开市时,检查所有的session配对,当前是否是有效交易时间
        /// </summary>
        /// <returns></returns>
        public bool IsOpenTime
        {
            get
            {
                //判断每个时间段 如果某个时间段满足则返回true,全部不满足 则返回false
                foreach (MktTimeEntry s in sessionlist)
                {
                    if (s.IsOpenTime()) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 判断某个时间是否在交易时间段内
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsInMarketTime(int time)
        {
            //判断每个时间段 如果某个时间段满足则返回true,全部不满足 则返回false
            foreach (MktTimeEntry s in sessionlist)
            {
                if (s.IsInTimeEntry(time)) return true;
            }
            return false;
        }


        /// <summary>
        /// 是否是强平时间段
        /// </summary>
        public bool IsFlatTime
        {
            get
            {
                //判断每个时间段 如果某个时间段满足则返回true,全部不满足 则返回false
                foreach (MktTimeEntry s in sessionlist)
                {
                    if (s.IsFlatTime()) return true;
                }
                return false;
            }
        }

        public MktTimeEntry[] MktTimeEntries
        {
            get
            {
                return sessionlist.ToArray();
            }
        }

        public override string ToString()
        {
            string r = string.Empty;
            foreach (MktTimeEntry s in sessionlist)
            {
                r += s.ToString();
            }
            return "ID:" + ID.ToString() +" Name:" + Name + " Desp:" + Description + " MktTime:" + this.SerializeMktTimeString() +" "+r;
        }

        internal void AddMktTimeEntry(MktTimeEntry s)
        {
            this.sessionlist.Add(s);
        }

        /// <summary>
        /// 将时间段设置序列化成字符串
        /// </summary>
        /// <returns></returns>
        public string SerializeMktTimeString()
        {
            string str = string.Empty;
            foreach (MktTimeEntry s in sessionlist)
            {
                str += s.Serialize();
            }
            return str;
        }

        /// <summary>
        /// 反序列化到字市场开市时间对象
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void DeserializeMktTimeString(string msg)
        {
            string[] p = msg.Split('#');
            foreach (string s in p)
            {
                MktTimeEntry se = MktTimeEntry.Deserialize(s);
                if(se == null) continue;
                this.AddMktTimeEntry(se);
            }
            
        }
       


        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';

            sb.Append(this.ID.ToString());
            sb.Append(d);
            sb.Append(this.Name);
            sb.Append(d);
            sb.Append(this.Description);
            sb.Append(d);
            sb.Append(this.SerializeMktTimeString());
            return sb.ToString();
        }

        public void Deserialize(string content)
        {
            string[] rec = content.Split(',');
            this.ID = int.Parse(rec[0]);
            this.Name = rec[1];
            this.Description = rec[2];
            this.DeserializeMktTimeString(rec[3]);

        }

        public override bool Equals(object obj)
        {
            if (obj is MarketTime)
            { 
                MarketTime mtobj = obj as MarketTime;
                if (this.ID.Equals(mtobj.ID))
                    return true;
            }
            return false;
        }
    }

    public class Exchange : IExchange
    {

        private int _id=0;//数据库编号
        private string _ex;//交易所代码
        private string _name;//交易所名称
        private Country _country;//交易所所处国家
        private string _title;//简称

        public int ID { get { return _id; } set { _id = value; } }
        public string EXCode { get { return _ex; } set { _ex = value; } }//从数据库加载
        public string Name { get { return _name; } set { _name = value; } }//从数据库加载
        public Country Country { get { return _country; } set { _country = value; } }//从数据库加载

        public string Title { get { return _title; } set { _title = value; } }

        private MarketTime _session;//交易所交易时间


        
        public string SessionString
        {
            get
            {
                return _session.ToString();
            }
        }

        /// <summary>
        /// 交易所编号 国家_交易所代号
        /// </summary>
        public string Index { get { return _country.ToString() + "_" + _ex.ToString(); } }

        public Exchange()
        { 
            
        }

        /// <summary>
        /// 初始化交易所 编码,名称,国家,时段字符(从xml文件获取)
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="name"></param>
        /// <param name="country"></param>
        /// <param name="sessionstr"></param>
        public Exchange(string ex, string name, Country country,string sessionstr="")
        {
            _ex = ex;
            _name = name;
            _country = country;
            _session = null;

        }

        public override string ToString()
        {
            return "ID:" + ID.ToString() + " Code:" + EXCode.ToString() + " Name:" + Name.ToString() + " Country:"+ Country.ToString()+" ExIndex:" + Index.ToString(); 
        }

        public string Serialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';

            sb.Append(this.ID.ToString());
            sb.Append(d);
            sb.Append(this.EXCode.ToString());
            sb.Append(d);
            sb.Append(this.Name);
            sb.Append(d);
            sb.Append(this.Country.ToString());
            sb.Append(d);
            sb.Append(this.Title);
            return sb.ToString();
        }

        public void Deserialize(string content)
        {
            string[] rec = content.Split(',');
            this.ID = int.Parse(rec[0]);
            this.EXCode = rec[1];
            this.Name = rec[2];
            this.Country = (Country)Enum.Parse(typeof(Country), rec[3]);
            if (rec.Length > 4)
            {
                this.Title = rec[4];
            }

        }
    }
}
