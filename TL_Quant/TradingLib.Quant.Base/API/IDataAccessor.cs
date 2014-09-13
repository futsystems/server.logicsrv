using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant
{
    /// <summary>
    /// 泛型数据访问接口
    /// Bar/Tick的数据访问通过实现该接口 来达到统一访问数据的目的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataAccessor<T> : IDisposable
    {
        // Methods
        //BinaryDatabase<T> _database;
        long Delete(long start, long end);//删除
        long GetCount(long start, long end);//获得总数
        //DateTime GetCurrentDateTime();
        DateTime GetDateTimeAtIndex(long index, out long numSameDatePreceding);//获得某个index所应该对应的时间
        List<T> Load(long start, long end, long maxItems, bool loadFromEnd);//加载对应的时间间隔内的数据
        long Save(IList<T> items);//保存

        long Delete(DateTime start, DateTime end);//删除
        long GetCount(DateTime start, DateTime end);//获得总数
        List<T> Load(DateTime start, DateTime end, long maxItems, bool loadFromEnd);//加载对应的时间间隔内的数据
    }
}
