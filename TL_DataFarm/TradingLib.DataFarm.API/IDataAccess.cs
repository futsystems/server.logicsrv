//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace TradingLib.DataFarm.API
//{
//    public interface IDataAccessor<T>
//    {
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="start">The date of the first data item to load.</param>
//        /// <param name="end">The date of the last data item to load.</param>
//        /// <param name="maxCount">The maximum number of items to load from the store.</param>
//        /// <param name="loadFromEnd">Controls whether the items returned should come from the beginning or the end,if countof the specified date range (if there are more items in the date range than specified by the <paramref name="maxItems" /> parameter).</param>
//        /// <returns></returns>
//        List<T> Load(DateTime start, DateTime end, long maxCount, bool loadFromEnd);


//        /// <summary>
//        /// 获得某个时间段的数据个数
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        long GetCount(DateTime start, DateTime end);

//        /// <summary>
//        /// 保存数据
//        /// </summary>
//        /// <param name="items"></param>
//        /// <returns></returns>
//        long Save(List<T> items);


//        /// <summary>
//        /// 在数据库末尾增加一条记录
//        /// </summary>
//        /// <param name="item"></param>
//        void Append(T item);
//        /// <summary>
//        /// 删除数据
//        /// </summary>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        long Delete(DateTime start, DateTime end);

//        /// <summary>
//        /// 获得某个index对应的时间
//        /// </summary>
//        /// <param name="index"></param>
//        /// <param name="numSameDatePreceding"></param>
//        /// <returns></returns>
//        DateTime GetDateTimeAtIndex(long index, out long numSameDatePreceding);
//    }
//}
