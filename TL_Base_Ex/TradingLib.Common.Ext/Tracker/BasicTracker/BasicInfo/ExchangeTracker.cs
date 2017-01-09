using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 交易所信息维护
    /// 用于维护系统全局交易所信息
    /// </summary>
    public  class DBExchangeTracker
    {
        Dictionary<string, ExchangeImpl> exchagneCodeMap = new Dictionary<string, ExchangeImpl>();
        Dictionary<int, ExchangeImpl> exchangeIdMap = new Dictionary<int, ExchangeImpl>();

        public DBExchangeTracker()
        { 
            //从数据库加载交易所信息 将其缓存到内存
            foreach (ExchangeImpl ex in ORM.MBasicInfo.SelectExchange())
            {
                exchangeIdMap.Add(ex.ID, ex);
                exchagneCodeMap.Add(ex.EXCode, ex);
            }
        }

        /// <summary>
        /// 通过交易所代码获得交易简称
        /// </summary>
        /// <param name="excode"></param>
        /// <returns></returns>
        public string GetExchangeTitle(string excode)
        {
            ExchangeImpl ex = this[excode];
            return ex != null ? ex.Title : excode;
        }

        /// <summary>
        /// 通过数据库ID获得交易所对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExchangeImpl this[int id]
        {
            get {
                ExchangeImpl ex = null;
                if (exchangeIdMap.TryGetValue(id, out ex))
                {
                    return ex;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 通过交易所编号获得交易所对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ExchangeImpl this[string excode]
        {
            get
            {
                ExchangeImpl ex = null;
                if (exchagneCodeMap.TryGetValue(excode, out ex))
                {
                    return ex;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 返回所有交易所列表
        /// </summary>
        public ExchangeImpl[] Exchanges
        {
            get
            {
                return exchagneCodeMap.Values.ToArray();
            }
        }


        public void UpdateExchange(ExchangeImpl ex)
        {
            ExchangeImpl target = null;
            if (exchangeIdMap.TryGetValue(ex.ID, out target))
            {
                target.Name = ex.Name;
                target.Title = ex.Title;
                target.Country = ex.Country;
                target.Calendar = ex.Calendar;
                target.TimeZoneID = ex.TimeZoneID;
                target.CloseTime = ex.CloseTime;
                target.SettleType = ex.SettleType;
                target.DataFeed = ex.DataFeed;
                ORM.MBasicInfo.UpdateExchange(target);
            }
            else
            {
                target = new ExchangeImpl();
                target.EXCode = ex.EXCode;
                target.Name = ex.Name;
                target.Title = ex.Title;
                target.Country = ex.Country;
                target.Calendar = ex.Calendar;
                target.TimeZoneID = ex.TimeZoneID;
                target.CloseTime = ex.CloseTime;
                target.SettleType = ex.SettleType;
                target.DataFeed = ex.DataFeed;

                ORM.MBasicInfo.InsertExchange(target);
                ex.ID = target.ID;

                exchangeIdMap.Add(target.ID, ex);
                exchagneCodeMap.Add(target.EXCode, ex);
            }
        }
    }
}
