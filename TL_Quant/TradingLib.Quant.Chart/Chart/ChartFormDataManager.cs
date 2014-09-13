using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Easychart.Finance;
using Easychart.Finance.DataProvider;
using TradingLib.API;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Chart
{
    /// <summary>
    /// 继承了datamanagerBase用于向FinanceChart提供数据
    /// </summary>
    public class ChartFormDataManager : DataManagerBase
    {
        protected ChartPriceSeries series;

        public ChartFormDataManager(ChartPriceSeries _series)
        {
            series = _series;
        }
        /// <summary>
        /// symbol count 获得对应数量的数据 该函数由绘图控件进行调用
        /// 函数内封装了逻辑调用QSCommonDataProvider GetData(QList<bardata> bars) 最终得到实现接口IDataProvider的CDP数据
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        public override IDataProvider GetData(string Code, int Count)
        {
            //MessageBox.Show("code:"+Code + " count:"+Count.ToString());
            try
            {
                QSCommonDataProvider cdp = new QSCommonDataProvider(this);
                //cdp.DataCycle = new DataCycle(DataCycleBase.MINUTE, 1);//设定dataprovider的周期
                //cdp.SetStringData("Code", Code);
                //cdp.LoadBinary(new double[][] { ChartData.Open, ChartData.High, ChartData.Low, ChartData.Close, ChartData.Volume, ChartData.Date });
                
                cdp.LoadQSChartData(ChartData);//将ChartData填充到IDataProvider中
                //cdp.SetStringData()
                //MessageBox.Show("raw data length:" + BarData[BarDataType.Open].Count.ToString());
                //MessageBox.Show("cdp lenght:" + cdp.Count.ToString());(调用了this[date] 这个调用调用了母函数)
                //MessageBox.Show("cdp datacycle:" + cdp.DataCycle.ToString());
                return cdp;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
            return null;
        }


        public void NewBar(Bar bar)
        { 
            
        }
        public void NewPartialBar(Bar partialBar)
        { 
        
        
        }

        public void NewTick(Tick tick)
        {
            this.series.NewTick(tick);
        }

        /// <summary>
        /// 获得ChartData数据
        /// </summary>
        public QSChartData ChartData
        {
            get
            {
               return  new QSChartData(BarData);
            }
        }
        /// <summary>
        /// 获得Bars
        /// </summary>
        public IBarData BarData
        {
            get
            {
                return series.PriceData;
            }
        }

        public DateTime BarStartTime
        {
            get
            {
                return series.PriceData.First().BarStartTime;
            }
        }
        /// <summary>
        /// 获得ChartPriceSeries
        /// </summary>
        public ChartPriceSeries Series {
            get
            {
                return series;
            }
        }

    


    }
}
