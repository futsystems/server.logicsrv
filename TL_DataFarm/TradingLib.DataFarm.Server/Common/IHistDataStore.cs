using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Common.DataFarm;

namespace TradingLib.API
{
    /// <summary>
    /// 历史数据接口
    /// </summary>
    public  interface IHistDataStore
    {
        /// <summary>
        /// 注册合约频率参数
        /// HistDataStore只对注册的频率进行响应,未注册Bar数据不进行保存和提供查询
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        //void RegisterSymbolFreq(string symbol,BarInterval type,int interval);

        /// <summary>
        /// 判断某个合约的频率数据是否注册
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        //bool IsRegisted(string symbol, BarInterval type, int interval);

        /// <summary>
        /// 判断某个合约的频率数据是否缓存
        /// 本地数据库直接返回true,从本地文件加载
        /// 内存数据库没有缓存则从DataCore后端加载历史数据 数据加载完毕后再处理对应的客户端查询请求
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        //bool IsCached(string symbol, BarInterval type, int interval);

        /// <summary>
        /// 保存
        /// </summary>
        //void Commit();


        /// <summary>
        /// 设置某个合约频率数据为已缓存
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        //void SetCached(string symbol, BarInterval type, int interval,bool cached);

        /// <summary>
        /// 恢复某个合约 某个频率的历史数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        bool RestoreIntradayBar(Symbol symbol, BarInterval type, int interval, out DateTime lastBarTime);

        /// <summary>
        /// 恢复日线数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="lastBarTime"></param>
        void RestoreEodBar(Symbol symbol, out DateTime lastBarTime);


        /// <summary>
        /// 查询历史数据
        /// </summary>
        /// <param name="symbol">合约</param>
        /// <param name="type">间隔列别</param>
        /// <param name="interval">间隔数</param>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <param name="maxcount">最大返回Bar数</param>
        /// <param name="fromEnd">是否数据结尾开始</param>
        /// <returns></returns>
        List<BarImpl> QryBar(Symbol symbol, BarInterval type, int interval, DateTime start, DateTime end,int startIndex, int maxcount, bool fromEnd,bool havePartail);

        /// <summary>
        /// 更新Bar数据
        /// </summary>
        /// <param name="bar"></param>
        void UpdateBar(Symbol symbol, BarImpl source, out BarImpl dest, out bool isInsert);


        void UpdateBar(string key, BarImpl source, out BarImpl dest, out bool isInsert);

        /// <summary>
        /// 上传Bar数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bars"></param>
        //void UploadBar(string key, IEnumerable<BarImpl> bars);

        /// <summary>
        /// 删除Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="ids"></param>
        void DeleteBar(Symbol symbol, BarInterval type, int interval, int[] ids);

        /// <summary>
        /// 更新实时Bar系统的第一个Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="firstRealBar"></param>
        void UpdateFirstRealBar(Symbol symbol, BarImpl firstRealBar);


        /// <summary>
        /// 更新某个Symbol对应的PartialBar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="partail"></param>
        void UpdateRealPartialBar(Symbol symbol, BarImpl partail);

        /// <summary>
        /// 更新某个合约 数据恢复完毕后 历史Bar系统的PartialBar
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="histPartial"></param>
        void UpdateHistPartialBar(Symbol symbol, BarImpl histPartial);


        /// <summary>
        /// 插入Bar数据
        /// </summary>
        /// <param name="bar"></param>
        //void InsertBar(Symbol symbol,BarImpl bar);

        /// <summary>
        /// 表信息
        /// </summary>
        //IEnumerable<HistTableInfo> HistTableInfo { get; }
    }
}
