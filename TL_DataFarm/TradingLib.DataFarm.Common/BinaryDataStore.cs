using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;

namespace TradingLib.DataFarm.Common
{
    public sealed class BinaryDataStore : IDataStore, IDisposable
    {
        private class DataAccessor<T> : IDataAccessor<T>, IDisposable
        {
            private BinaryDatabase<T> _database;

            private string _filename;

            public DataAccessor(BinaryDatabase<T> database, string filename)
            {
                this._database = database;
                this._filename = filename;
            }

            /// <summary>
            /// 加载数据
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <param name="maxItems"></param>
            /// <param name="loadFromEnd"></param>
            /// <returns></returns>
            public List<T> Load(DateTime start, DateTime end, long maxItems, bool loadFromEnd)
            {
                return this._database.LoadItems(this._filename, start, end, maxItems, loadFromEnd);
            }

            /// <summary>
            /// 获得数据项目数
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <returns></returns>
            public long GetCount(DateTime start, DateTime end)
            {
                return this._database.GetItemCount(this._filename, start, end);
            }

            /// <summary>
            /// 保存数据项
            /// </summary>
            /// <param name="items"></param>
            /// <returns></returns>
            public long Save(List<T> items)
            {
                return this._database.SaveItems(this._filename, items);
            }


            public void Append(T item)
            {
                this._database.Append(this._filename, item);
            }
            /// <summary>
            /// 删除数据项
            /// </summary>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <returns></returns>
            public long Delete(DateTime start, DateTime end)
            {
                return this._database.DeleteItems(this._filename, start, end);
            }

            public DateTime GetDateTimeAtIndex(long index, out long numSameDatePreceding)
            {
                return this._database.GetDateTimeAtIndex(this._filename, index, out numSameDatePreceding);
            }

            public void Dispose()
            {
                this._database.CloseReaders(this._filename);
                this._database.CloseWriters(this._filename);
            }
        }

        private BarDatabase _barDatabase;

        private TickDatabase _tickDatabase;

        
        string DataPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"database");
            }
        }

        string BarDataPath
        {
            get { return Path.Combine(DataPath, "bar"); }
        }
        string TickDataPath
        {
            get { return Path.Combine(DataPath, "tick"); }
        }

        public BinaryDataStore()
        {
            this._barDatabase = new BarDatabase();
            this._tickDatabase = new TickDatabase();
        }

        public void EnsureDataDir()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.DataPath) && !Directory.Exists(this.DataPath))
                {
                    Directory.CreateDirectory(this.DataPath);
                }
                if (!string.IsNullOrEmpty(this.BarDataPath) && !Directory.Exists(this.BarDataPath))
                {
                    Directory.CreateDirectory(this.BarDataPath);
                }
                if (!string.IsNullOrEmpty(this.TickDataPath) && !Directory.Exists(this.TickDataPath))
                {
                    Directory.CreateDirectory(this.TickDataPath);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private string GetFileName(SymbolFreq symbol)
        {
            this.EnsureDataDir();
            string text = symbol.ToUniqueId();
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            char[] array = invalidFileNameChars;
            for (int i = 0; i < array.Length; i++)
            {
                char oldChar = array[i];
                text = text.Replace(oldChar, '_');
            }
            return Path.Combine(this.BarDataPath, text + ".dat");
        }

        private string GetTickFileName(string symbol)
        {
            this.EnsureDataDir();
            string text = symbol;
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            char[] array = invalidFileNameChars;
            for (int i = 0; i < array.Length; i++)
            {
                char oldChar = array[i];
                text = text.Replace(oldChar, '_');
            }
            return Path.Combine(this.TickDataPath, text + "_tick.dat");
        }

        public void Flush()
        {
            this._barDatabase.Flush();
            this._tickDatabase.Flush();
        }

        public IDataAccessor<Bar> GetBarStorage(SymbolFreq symbol)
        {
            return new BinaryDataStore.DataAccessor<Bar>(this._barDatabase, this.GetFileName(symbol));
        }

        public IDataAccessor<Tick> GetTickStorage(string symbol)
        {
            return new BinaryDataStore.DataAccessor<Tick>(this._tickDatabase, this.GetTickFileName(symbol));
        }

        public void FlushAll()
        {
            this.Flush();
        }

        public void Dispose()
        {
            this.Flush();
        }
    }
}
