using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
//using System.Windows.Forms;

namespace TradingLib.Common
{


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
