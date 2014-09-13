using System;
using System.Collections.Generic;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 所有Bar数据集合,通过合约访问对应的BarData数据
    /// </summary>
    public interface ITBars
    {
        IBarData this[string symbol] { get; }
        IBarData this[Security security] {get;}
    
    }
    /// <summary>
    /// 定义了BarData接口
    /// 通过该接口是得到访问Open/High/Low/Close/Volume/OpenInterest数据序列,以及通过时间和Index对Bar数据进行访问
    /// 
    /// </summary>
    public interface IBarData :IEnumerable<Bar>
    {
        /// <summary>
        /// BarData对应的Security
        /// </summary>
        Security Security { get; }//获得对应的合约
        /// <summary>
        /// 通过字符串"OPEN" "HIGH"等对数据序列进行访问
        /// </summary>
        /// <param name="seriesname"></param>
        /// <returns></returns>
        ISeries this[string seriesname] { get; }//绘图时进行的访问
        /// <summary>
        /// 通过BarDataType 对数据序列进行访问
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        ISeries this[BarDataType type] { get; }//其他指标计算时进行的访问
        /// <summary>
        /// 获得BarData数据中所含Bar的数目
        /// </summary>
        int Count { get; }//获得数据总数
        //IList<Bar> BarList { get; }
        /// <summary>
        /// 回溯多少个周期得到对应的Bar
        /// </summary>
        /// <param name="nBars"></param>
        /// <returns></returns>
        Bar LookBack(int nBars);
        /// <summary>
        /// 通过Bar的开始时间索引得到对应的Bar数据
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        Bar this[DateTime time] { get; }
        /// <summary>
        /// 通过index索引得到对应的Bar数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Bar this[int index]{get;}


        
    }
}
