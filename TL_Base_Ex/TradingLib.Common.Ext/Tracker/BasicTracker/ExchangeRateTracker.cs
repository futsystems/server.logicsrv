﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 某个结算日的汇率数据结构
    /// </summary>
    public class ExchangeRateStruct
    {
        public ExchangeRateStruct(int settleday)
        {
            this.Settleday = settleday;
        }

        public int Settleday { get; set; }

        Dictionary<CurrencyType, ExchangeRate> exchangeratemap = new Dictionary<CurrencyType, ExchangeRate>();

        public IEnumerable<ExchangeRate> ExchangeRates
        {
            get
            {
                return exchangeratemap.Values;
            }
        }

        public ExchangeRate this[CurrencyType currency]
        {
            get
            {
                ExchangeRate target = null;
                if (exchangeratemap.TryGetValue(currency, out target))
                {
                    return target;
                }
                return null;
            }
        }

        /// <summary>
        /// 插入汇率数据
        /// </summary>
        /// <param name="rate"></param>
        public void AppendExchangeRate(ExchangeRate rate)
        {
            if (rate.Settleday != this.Settleday) return;
            exchangeratemap[rate.Currency] = rate;
        }
    }

    public class ExchangeRateTracker
    {
        Dictionary<int, DBExchangeRateTracker> domainExchangeRateMap = new Dictionary<int, DBExchangeRateTracker>();

        public ExchangeRateTracker()
        {
            //加载所有Domain的汇率数据
            foreach (Domain domain in BasicTracker.DomainTracker.Domains)
            {
                if (!domainExchangeRateMap.Keys.Contains(domain.ID))
                {
                    domainExchangeRateMap.Add(domain.ID, new DBExchangeRateTracker(domain));
                }
            }
        }

        /// <summary>
        /// 获得某个分区汇率
        /// </summary>
        /// <param name="domain_id"></param>
        /// <returns></returns>
        public DBExchangeRateTracker this[int domain_id]
        {
            get
            {
                DBExchangeRateTracker target = null;
                if (domainExchangeRateMap.TryGetValue(domain_id, out target))
                {
                    return target;
                }
                Domain domain = BasicTracker.DomainTracker[domain_id];
                if (domain != null)
                { 
                    target = new DBExchangeRateTracker(domain);
                    domainExchangeRateMap.Add(domain.ID, target);
                }
                return target;
            }
        }

        /// <summary>
        /// 创建某个交易的所有汇率数据
        /// </summary>
        /// <param name="settleday"></param>
        public void CreateExchangeRates(int settleday)
        {
            foreach (var t in domainExchangeRateMap.Values)
            {
                t.CreateExchangeRates(settleday);
            }
        }
    }
    

    public class DBExchangeRateTracker
    {

        /// <summary>
        /// 分区编号
        /// </summary>
        public Domain Domain{ get; private set; }

        Dictionary<int, ExchangeRateStruct> exchangeratestructmap = new Dictionary<int, ExchangeRateStruct>();

        Dictionary<int, ExchangeRate> exchangerateidmap = new Dictionary<int, ExchangeRate>();


        public DBExchangeRateTracker(Domain domain)
        {
            this.Domain = domain;
            //汇率数据初始化时 按交易日生成一组数据 用于索引到某个交易日的汇率数据
            foreach (var rate in ORM.MExchangeRate.SelectExchangeRates(domain.ID))
            {
                exchangerateidmap.Add(rate.ID, rate);
                if (!exchangeratestructmap.Keys.Contains(rate.Settleday))
                {
                    exchangeratestructmap.Add(rate.Settleday, new ExchangeRateStruct(rate.Settleday));
                }
                //将汇率数据记录到对应的交易日汇率结构体中
                exchangeratestructmap[rate.Settleday].AppendExchangeRate(rate);
            }
        }

        /// <summary>
        /// 获得某个ID对应的汇率数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ExchangeRate this[int id]
        {
            get
            {
                ExchangeRate target = null;
                if (exchangerateidmap.TryGetValue(id, out target))
                {
                    return target;
                }
                return null;
            }
        }

        /// <summary>
        /// 获得某个交易日某个货币的汇率数据
        /// 
        /// </summary>
        /// <param name="settleday"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public ExchangeRate this[int settleday, CurrencyType currency]
        {
            get
            {
                ExchangeRateStruct target = null;
                //查找对应交易日的汇率结构体
                if (exchangeratestructmap.TryGetValue(settleday, out target))
                {
                    return target[currency];//通过currency返回汇率对象
                }
                return null;
            }
        }

        /// <summary>
        /// 获得某个交易日的所有汇率数据
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public IEnumerable<ExchangeRate> GetExchangeRates(int settleday)
        {
            ExchangeRateStruct target = null;
            //查找对应交易日的汇率结构体
            if (exchangeratestructmap.TryGetValue(settleday, out target))
            {
                return target.ExchangeRates;
            }
            return null;
        }

        /// <summary>
        /// 生成某日汇率表
        /// </summary>
        public void CreateExchangeRates(int settleday)
        {

            if (!exchangeratestructmap.Keys.Contains(settleday))//如果不存在对应交易日的汇率数据
            {
                exchangeratestructmap.Add(settleday, new ExchangeRateStruct(settleday));//添加交易日汇率数据结构体

                DateTime dt = Util.ToDateTime(settleday, 0);
                DateTime lastday = dt.AddDays(-1);//上一日
                //查询上日汇率信息
                IEnumerable<ExchangeRate> rates = ORM.MExchangeRate.SelectExchangeRates(this.Domain.ID,Util.ToTLDate(lastday));
                //如果上日汇率存在 则复制对应的汇率信息
                if (rates.Count() > 0)
                {
                    foreach (var rate in rates)
                    {
                        ExchangeRate target = new ExchangeRate();
                        target.Domain_ID = this.Domain.ID;
                        target.Settleday = settleday;
                        target.Currency = rate.Currency;
                        target.AskRate = rate.AskRate;
                        target.BidRate = rate.BidRate;
                        target.IntermediateRate = rate.IntermediateRate;
                        target.UpdateTime = Util.ToTLDateTime();

                        //更新汇率
                        this.UpdateExchangeRate(target);
                    }
                }

                //遍历所有货币类别 如果没有对应货币类别的汇率则插入该汇率信息 默认为1
                foreach (CurrencyType c in Enum.GetValues(typeof(CurrencyType)))
                {
                    var rate = this[settleday, c];
                    if (rate == null)
                    {
                        ExchangeRate target = new ExchangeRate();
                        target.Domain_ID = this.Domain.ID;
                        target.Settleday = settleday;
                        target.Currency = c;
                        target.AskRate = 1;
                        target.BidRate = 1;
                        target.IntermediateRate = 1;
                        target.UpdateTime = Util.ToTLDateTime();

                        //更新汇率
                        this.UpdateExchangeRate(target);
                    }
                }
            }
        }

        /// <summary>
        /// 更新汇率数据
        /// </summary>
        /// <param name="rate"></param>
        public void UpdateExchangeRate(ExchangeRate rate)
        {

            ExchangeRate target = null;
            if (exchangerateidmap.TryGetValue(rate.ID, out target))//如果存在则更新数据
            {
                target.AskRate = rate.AskRate;
                target.BidRate = rate.BidRate;
                target.IntermediateRate = rate.IntermediateRate;
                target.UpdateTime = Util.ToTLDateTime();
                rate.UpdateTime = target.UpdateTime;

                ORM.MExchangeRate.UpdateExchangeRate(target);
            }
            else
            {
                target = new ExchangeRate();
                target.Domain_ID = rate.Domain_ID;
                target.Settleday = rate.Settleday;
                target.Currency = rate.Currency;
                target.AskRate = rate.AskRate;
                target.BidRate = rate.BidRate;
                target.IntermediateRate = rate.IntermediateRate;
                target.UpdateTime = Util.ToTLDateTime();
                rate.UpdateTime = target.UpdateTime;

                ORM.MExchangeRate.InsertExchangeRate(target);

                rate.ID = target.ID;
                exchangerateidmap.Add(target.ID, target);

                //记录到对应的结算日汇率结构体中
                if (!exchangeratestructmap.Keys.Contains(target.Settleday))
                {
                    exchangeratestructmap.Add(target.Settleday, new ExchangeRateStruct(target.Settleday));
                }
                //将汇率数据记录到对应的交易日汇率结构体中
                exchangeratestructmap[target.Settleday].AppendExchangeRate(target);
            }
        }
    }
}