using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TradingLib.DataFarm.Common
{
    internal abstract class BinaryDatabase<T>
    {
        private const bool KeepOpen = false;

        private readonly int _itemSize;

        private Dictionary<string, BinaryReader> readFileHandles = new Dictionary<string, BinaryReader>();

        private Dictionary<string, BinaryWriter> writeFileHandles = new Dictionary<string, BinaryWriter>();

        public BinaryDatabase(int itemSize)
        {
            this._itemSize = itemSize;
        }

        /// <summary>
        /// 获得当前文件位置对应对象的日期
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        protected abstract DateTime ReadCurrentDateTime(BinaryReader br);

        /// <summary>
        /// 读取一个数据项目
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        protected abstract T ReadItem(byte[] buffer, long startPos);

        /// <summary>
        /// 写入一个数据项目
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="item"></param>
        protected abstract void WriteItem(BinaryWriter writer, T item);

        /// <summary>
        /// 获得某个对象的日期
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected abstract DateTime GetTime(T item);

        /// <summary>
        /// 返回某个index处的时间
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="index"></param>
        /// <param name="numToSkip"></param>
        /// <returns></returns>
        public DateTime GetDateTimeAtIndex(string filename, long index, out long numToSkip)
        {
            DateTime result;
            using (CloseWrapper<BinaryReader> reader = this.GetReader(filename))
            {
                BinaryReader obj = reader.Obj;
                obj.BaseStream.Seek(index * (long)this._itemSize, SeekOrigin.Begin);
                DateTime dateTime = this.ReadCurrentDateTime(obj);//获得指定数据个数对应的时间
                long num = this.SeekDate(obj, dateTime) / (long)this._itemSize;
                numToSkip = index - num;
                result = dateTime;
            }
            return result;
        }

        /// <summary>
        /// 加载某个时间段的数据集
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxItems"></param>
        /// <param name="loadFromEnd"></param>
        /// <returns></returns>
        public List<T> LoadItems(string filename, DateTime start, DateTime end, long maxItems, bool loadFromEnd)
        {
            List<T> list = new List<T>();
            using (CloseWrapper<BinaryReader> reader = this.GetReader(filename))
            {
                BinaryReader obj = reader.Obj;
                if (obj != null)
                {
                    long num;
                    long num2;
                    this.SeekDates(obj, start, end, out num, out num2);
                    List<T> result;
                    if (num2 <= 0L)//时间段内没有任何数据 则返回空数据
                    {
                        result = list;
                        return result;
                    }
                    if (maxItems > 0L)//如果设定了最大返回数
                    {
                        long num3 = num2 / (long)this._itemSize;
                        if (num3 > maxItems)//如果包含数据大于设定的最大数据
                        {
                            if (loadFromEnd)//从结束时间加载数据
                            {
                                long num4 = num3 - maxItems;
                                num += num4 * (long)this._itemSize;
                                num2 = maxItems * (long)this._itemSize;
                            }
                            else//开始时间加载数据
                            {
                                num2 = maxItems * (long)this._itemSize;
                            }
                        }
                    }
                    result = this.ReadItems(obj, num, num2);
                    return result;
                }
            }
            return list;
        }

        /// <summary>
        /// 返回某个时间段的数据个数
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public long GetItemCount(string filename, DateTime start, DateTime end)
        {
            if (File.Exists(filename))
            {
                using (CloseWrapper<BinaryReader> reader = this.GetReader(filename))
                {
                    BinaryReader obj = reader.Obj;
                    long num;
                    long num2;
                    this.SeekDates(obj, start, end, out num, out num2);
                    if (num2 > 0L)
                    {
                        return num2 / (long)this._itemSize;
                    }
                }
            }
            return 0L;
        }

        /// <summary>
        /// 在文件末尾追加一条记录
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="item"></param>
        public void Append(string filename, T item)
        {
            using (CloseWrapper<BinaryWriter> writer = this.GetWriter(filename, false))
            {
                BinaryWriter obj = writer.Obj;
                if (obj != null)
                {
                    this.WriteItem(obj, item);
                }
            }
        }
        /// <summary>
        /// 保存数据项
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        public long SaveItems(string filename, IList<T> items)
        {
            if (items.Count == 0)
            {
                return 0L;
            }
            DateTime t = DateTime.MinValue;
            foreach (T current in items)
            {
                DateTime time = this.GetTime(current);
                if (time < t)
                {
                    throw new ArgumentException("Items must be sorted by date,time:" + time.ToString() + " current Tick:" + current.ToString());
                }
                t = time;
            }
            DateTime time2 = this.GetTime(items[0]);//开始时间
            DateTime time3 = this.GetTime(items[items.Count - 1]);//结束时间
            List<T> list = this.LoadItems(filename, time3, DateTime.MaxValue, -1L, true);//加载结束时间之后的所有数据
            while (list.Count > 0 && this.GetTime(list[0]) <= time3)//
            {
                list.RemoveAt(0);
            }
            long length;
            using (CloseWrapper<BinaryReader> reader = this.GetReader(filename))
            {
                length = this.SeekDate(reader.Obj, time2);
            }
            using (CloseWrapper<BinaryWriter> writer = this.GetWriter(filename, false))
            {
                writer.Obj.BaseStream.SetLength(length);
            }
            int num = this.AppendItemsToFile(filename, items);
            if (list.Count > 0)
            {
                this.AppendItemsToFile(filename, list);
            }
            return (long)num;
        }

        /// <summary>
        /// 删除某个时间段的数据
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public long DeleteItems(string filename, DateTime start, DateTime end)
        {
            this.CloseReaders(filename);
            this.CloseWriters(filename);
            //任意一个时间不为极值
            if (!(start == DateTime.MinValue) || !(end == DateTime.MaxValue))
            {
                long itemCount = this.GetItemCount(filename, DateTime.MinValue, DateTime.MaxValue);
                List<T> list = this.LoadItems(filename, end, DateTime.MaxValue, -1L, true);
                while (list.Count > 0 && this.GetTime(list[0]) <= end)
                {
                    list.RemoveAt(0);
                }
                long length;
                using (CloseWrapper<BinaryReader> reader = this.GetReader(filename))
                {
                    length = this.SeekDate(reader.Obj, start);//查找开始时间位置
                }
                using (CloseWrapper<BinaryWriter> writer = this.GetWriter(filename, false))
                {
                    writer.Obj.BaseStream.SetLength(length);//设定当前写入位置为开始时间
                }
                this.AppendItemsToFile(filename, list);//插入剩余数据
                long itemCount2 = this.GetItemCount(filename, DateTime.MinValue, DateTime.MaxValue);
                return itemCount - itemCount2;//
            }
            //开始于结束时间均为极值 删除所有数据
            if (File.Exists(filename))
            {
                long itemCount3 = this.GetItemCount(filename, DateTime.MinValue, DateTime.MaxValue);
                File.Delete(filename);
                return itemCount3;
            }
            return 0L;
        }

        public void Flush()
        {
            foreach (BinaryReader current in this.readFileHandles.Values)
            {
                current.Close();
            }
            this.readFileHandles.Clear();
            foreach (BinaryWriter current2 in this.writeFileHandles.Values)
            {
                current2.Flush();
            }
        }

        /// <summary>
        /// 从某个位置开始读取多少数据返回list
        /// </summary>
        /// <param name="br"></param>
        /// <param name="startPos"></param>
        /// <param name="readLength"></param>
        /// <returns></returns>
        private List<T> ReadItems(BinaryReader br, long startPos, long readLength)
        {
            long itemCount = readLength / (long)this._itemSize;
            br.BaseStream.Position = startPos;
            byte[] buffer = br.ReadBytes((int)readLength);
            int offset = 0;
            List<T> list = new List<T>((int)itemCount);
            int i = 0;
            while ((long)i < itemCount)
            {
                list.Add(this.ReadItem(buffer, (long)offset));
                offset += this._itemSize;//读取位置便宜
                i++;
            }
            return list;
        }
        /// <summary>
        /// 定位一个时间区间
        /// </summary>
        /// <param name="br"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="seekStart">开始位置</param>
        /// <param name="seekLength">数据区间长度</param>
        private void SeekDates(BinaryReader br, DateTime start, DateTime end, out long seekStart, out long seekLength)
        {
            seekStart = this.SeekDate(br, start);//返回开始时间的自己位置
            long num = this.SeekDate(br, end);//返回结束位置
            br.BaseStream.Position = num;
            DateTime dateTime = this.ReadCurrentDateTime(br);
            //如果当前时间不是最小值并且时间小于等查询结束时间 
            if (dateTime != DateTime.MinValue && dateTime <= end)
            {
                seekLength = num - seekStart + (long)this._itemSize;
                return;
            }
            
            seekLength = num - seekStart;//
        }

        /// <summary>
        /// 在文件最末尾插入数据对象
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        private int AppendItemsToFile(string filename, IEnumerable<T> items)
        {
            int num = 0;
            using (CloseWrapper<BinaryWriter> writer = this.GetWriter(filename, false))
            {
                BinaryWriter obj = writer.Obj;
                if (obj != null)
                {
                    obj.BaseStream.Seek(0L, SeekOrigin.End);
                    foreach (T current in items)
                    {
                        this.WriteItem(obj, current);
                        num++;
                    }
                }
            }
            return num;
        }

        /// <summary>
        /// 查找某个日期对应的字节位置数据项末尾
        /// 最小时间位置为0,最大时间位置为文件末尾
        /// </summary>
        /// <param name="br"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private long SeekDate(BinaryReader br, DateTime date)
        {
            if (date == DateTime.MinValue)
            {
                return 0L;
            }
            if (date == DateTime.MaxValue)
            {
                return br.BaseStream.Length;
            }
            long start = 0L;
            long end = br.BaseStream.Length / (long)this._itemSize - 1L;//假定目标位置为最文件最末尾
            while (end - start >= 10L)
            {
                long current = (start + end) / 2L;//二分法取当中位置
                br.BaseStream.Seek(current * (long)this._itemSize, SeekOrigin.Begin);
                DateTime dateTime = this.ReadCurrentDateTime(br);
                if (dateTime == DateTime.MinValue)
                {
                    throw new Exception("Error seeking for date: " + date.ToString());
                }
                if (dateTime == date)//如果日期为查询日期则目标位置为当前位置
                {
                    end = current;
                }
                else if (dateTime < date)//如果查询日期大于当前日期 则当前位置设定为起始位置，否则结束位置设定为当前位置
                {
                    start = current;
                }
                else
                {
                    end = current;
                }
            }//二分法查找 知道star end间隔小于10

            for (long i = start; i <= end; i += 1L)
            {
                br.BaseStream.Seek(i * (long)this._itemSize, SeekOrigin.Begin);
                DateTime dateTime = this.ReadCurrentDateTime(br);
                if (dateTime >= date)//如果当前数据时间大于等查询事件 则返回该事件(数据是按时间从小到大排列)
                {
                    return i * (long)this._itemSize;
                }
            }
            //如果查询的时间不存在则返回最后一个位置的新位置
            return (end + 1L) * (long)this._itemSize;
        }


        public void CloseReaders(string filename)
        {
            if (this.readFileHandles.ContainsKey(filename))
            {
                this.readFileHandles[filename].Close();
                this.readFileHandles.Remove(filename);
            }
        }

        public void CloseWriters(string filename)
        {
            if (this.writeFileHandles.ContainsKey(filename))
            {
                this.writeFileHandles[filename].Close();
                this.writeFileHandles.Remove(filename);
            }
        }

        private CloseWrapper<BinaryReader> GetReader(string filename)
        {
            this.CloseWriters(filename);
            if (this.readFileHandles.ContainsKey(filename))
            {
                return new CloseWrapper<BinaryReader>(this.readFileHandles[filename], true);
            }
            FileStream input = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            BinaryReader writer = new BinaryReader(input);
            return new CloseWrapper<BinaryReader>(writer, true);
        }

        private CloseWrapper<BinaryWriter> GetWriter(string filename, bool truncate)
        {
            this.CloseReaders(filename);
            if (this.writeFileHandles.ContainsKey(filename))
            {
                return new CloseWrapper<BinaryWriter>(this.writeFileHandles[filename], true);
            }
            FileStream output;
            if (truncate)
            {
                output = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            }
            else
            {
                output = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            }
            BinaryWriter writer = new BinaryWriter(output);
            return new CloseWrapper<BinaryWriter>(writer, true);
        }
    }
}
