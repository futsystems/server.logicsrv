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
        Dictionary<string, Exchange> exchagneCodeMap = new Dictionary<string, Exchange>();
        Dictionary<int, Exchange> exchangeIdMap = new Dictionary<int, Exchange>();

        public DBExchangeTracker()
        { 
            //从数据库加载交易所信息 将其缓存到内存
            foreach (Exchange ex in ORM.MBasicInfo.SelectExchange())
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
            Exchange ex = this[excode];
            return ex != null ? ex.Title : excode;
        }

        /// <summary>
        /// 通过数据库ID获得交易所对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Exchange this[int id]
        {
            get {
                Exchange ex = null;
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
        public Exchange this[string excode]
        {
            get
            {
                Exchange ex = null;
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
        public Exchange[] Exchanges
        {
            get
            {
                return exchagneCodeMap.Values.ToArray();
            }
        }


        public void UpdateExchange(Exchange ex)
        {
            Exchange target = null;
            if (exchangeIdMap.TryGetValue(ex.ID, out target))
            {
                target.Name = ex.Name;
                target.Title = ex.Title;
                target.Country = ex.Country;
                target.Calendar = ex.Calendar;

                ORM.MBasicInfo.UpdateExchange(target);
            }
            else
            {
                target = new Exchange();
                target.EXCode = ex.EXCode;
                target.Name = ex.Name;
                target.Title = ex.Title;
                target.Country = ex.Country;
                target.Calendar = ex.Calendar;

                ORM.MBasicInfo.InsertExchange(target);
                ex.ID = target.ID;

                exchangeIdMap.Add(target.ID, ex);
                exchagneCodeMap.Add(target.EXCode, ex);
            }
        }
    }
}
