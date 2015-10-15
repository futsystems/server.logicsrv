using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{


    public class Exchange : IExchange
    {

        private int _id=0;//数据库编号
        private string _ex;//交易所代码
        private string _name;//交易所名称
        private Country _country;//交易所所处国家
        private string _title;//简称
        private string _calendar;//假日
        private string _timezone;//时区信息
        private int _closetime;//收盘时间

        /// <summary>
        /// 交易所数据库编号
        /// </summary>
        public int ID { get { return _id; } set { _id = value; } }

        /// <summary>
        /// 交易所编码
        /// </summary>
        public string EXCode { get { return _ex; } set { _ex = value; } }

        /// <summary>
        /// 交易所名称
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// 国家
        /// </summary>
        public Country Country { get { return _country; } set { _country = value; } }

        /// <summary>
        /// 交易所简称
        /// </summary>
        public string Title { get { return _title; } set { _title = value; } }

        /// <summary>
        /// 假日
        /// </summary>
        public string Calendar { get { return _calendar; } set { _calendar=value; } }

        /// <summary>
        /// 时区
        /// </summary>
        public string TimeZone { get { return _timezone; } set { _genTimeZone = false; _timezone = value; } }

        /// <summary>
        /// 收盘时间
        /// </summary>
        public int CloseTime { get { return _closetime; } set { _closetime = value; } }


        public Exchange()
        {
            this.ID = 0;
            this.EXCode = "";
            this.Name = "";
            this.Country = Country.CN;
            this.TimeZone = "";
            this.Title = "";
        }


        TimeZoneInfo _targetTimeZone = null;
        bool _genTimeZone = false;

        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (!_genTimeZone)//延迟生成时区对象
                {
                    _genTimeZone = true;
                    if (string.IsNullOrEmpty(this.TimeZone))
                    {
                        _targetTimeZone = null;//没有提供具体市区信息则与本地系统时间一致
                    }
                    else
                    {
                        _targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById(this.TimeZone);
                        //DateTime t = new DateTime(2015,1,1,1,1,1,DateTimeKind.
                    }
                }
                return _targetTimeZone;
            }
        }


        public override string ToString()
        {
            return "ID:" + ID.ToString() + " Code:" + EXCode.ToString() + " Name:" + Name.ToString() + " Country:" + Country.ToString(); 
        }


        public static string Serialize(Exchange ex)
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';

            sb.Append(ex.ID.ToString());
            sb.Append(d);
            sb.Append(ex.EXCode.ToString());
            sb.Append(d);
            sb.Append(ex.Name);
            sb.Append(d);
            sb.Append(ex.Country.ToString());
            sb.Append(d);
            sb.Append(ex.Title);
            sb.Append(d);
            sb.Append(ex.Calendar);
            sb.Append(d);
            sb.Append(ex.TimeZone);
            sb.Append(d);
            sb.Append(ex.CloseTime);
            return sb.ToString();
        }

        public  static Exchange Deserialize(string content)
        {
            Exchange ex = new Exchange();

            string[] rec = content.Split(',');
            ex.ID = int.Parse(rec[0]);
            ex.EXCode = rec[1];
            ex.Name = rec[2];
            ex.Country = (Country)Enum.Parse(typeof(Country), rec[3]);
            ex.Title = rec[4];
            ex.Calendar = rec[5];
            ex.TimeZone = rec[6];
            ex.CloseTime = int.Parse(rec[7]);
            return ex;

        }
    }
}
