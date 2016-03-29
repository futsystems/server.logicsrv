using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;

using STSdb4.General.Persist;
using STSdb4.Data;


namespace TradingLib.Common.DataFarm
{
    public class TLLongPersist : IPersist<IData>
    {
        public void Write(BinaryWriter writer, IData item)
        {
            Data<long> data = (Data<long>)item;
            writer.Write(data.Value);
        }

        public IData Read(BinaryReader reader)
        {
            long value = reader.ReadInt64();

            return new Data<long>(value);
        }
    }

    public class TLLongIndexerPersist : IIndexerPersist<IData>
    {
        public void Store(BinaryWriter writer, Func<int, IData> values, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Data<long> data = (Data<long>)values(i);
                long item = data.Value;

                writer.Write(item);
            }
        }

        public void Load(BinaryReader reader, Action<int, IData> values, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Data<long> data = new Data<long>(reader.ReadInt64());
                values(i, data);
            }
        }
    }



    public class TLBarIndexPersist : IIndexerPersist<IData>
    {
        public void Store(BinaryWriter writer, Func<int, IData> values, int count)
        {
            for (int i = 0; i < count; i++)
            {
                BarImpl bar = ((Data<BarImpl>)values(i)).Value;
                BarImpl.Write(writer, bar);
            }
        }

        public void Load(BinaryReader reader, Action<int, IData> values, int count)
        {
            for (int i = 0; i < count; i++)
            {
                BarImpl bar = BarImpl.Read(reader);
                values(i, new Data<BarImpl>(bar));
            }
        }
    }

    /// <summary>
    /// Bar数据的序列化与反序列化
    /// 用于写入Bar数据和加载Bar数据
    /// </summary>
    public class TLBarPersist : IPersist<IData>
    {
        public void Write(BinaryWriter writer, IData item)
        {
            BarImpl bar = ((Data<BarImpl>)item).Value;
            BarImpl.Write(writer, bar);
        }

        public IData Read(BinaryReader reader)
        {
            BarImpl bar = BarImpl.Read(reader);
            return new Data<BarImpl>(bar);
        }
    }
}
