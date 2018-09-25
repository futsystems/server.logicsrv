using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

namespace TradingLib.Core
{
    /// <summary>
    /// DataRepository日志记录器
    /// </summary>
    public class DataRepositoryLogger
    {
        StreamWriter _log = null;
        private string fn = string.Empty;
        private string _path = "Dump";

        ILog logger = null;
        public DataRepositoryLogger()
        {
            logger = LogManager.GetLogger("DataRepositoryLog");
            _currentTradingday = TLCtxHelper.ModuleSettleCentre.Tradingday;

            
        }

        /// <summary>
        /// 初始化 此时会以独占的方式访问数据文件 在初始化之前数据库恢复时候会使用该文件
        /// </summary>
        public void Init()
        {
            //setfile();
        }

        int _currentTradingday = 0;

        /// <summary>
        /// 获得日志文件名称
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        static string GetLogFileName(int tradingday)
        {
            return Path.Combine(Util.DataRepositoryDir, string.Format("DATA.{0}.txt", tradingday));
        }

        void setfile00()
        {
            fn = GetLogFileName(_currentTradingday);
            try
            {
                try
                {
                    if (_log != null)
                        _log.Close();
                }
                catch { }
                _log = new StreamWriter(fn,true);
                _log.AutoFlush = true;
            }
            catch (Exception) { _log = null; }
        }

        /// <summary>
        /// 获得DataRepositoryLog 并序列化输出到文件
        /// </summary>
        /// <param name="log"></param>
        public void GotDataRepositoryLog(DataRepositoryLog log)
        {
            try
            {
                //if (_log != null && log!=null)
                //{
                //    if (_currentTradingday != TLCtxHelper.ModuleSettleCentre.Tradingday)
                //    {
                //        _currentTradingday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                //        //setfile();
                //    }
                //    string msg = DataRepositoryLog.Serialize(log);
                //    if (!string.IsNullOrEmpty(msg))
                //    {
                //        _log.WriteLine(msg);
                //    }
                //}
            }
            catch (Exception ex)
            {
                logger.Error("Save Log Error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 加载某个交易日的数据储存记录
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DataRepositoryLog> LoadDataRepositoryLogs(int tradingday)
        {
            string file = GetLogFileName(tradingday);
            List<DataRepositoryLog> list = new List<DataRepositoryLog>();
            if (File.Exists(file))
            {
                //实例化一个文件流--->与写入文件相关联  
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    //实例化一个StreamWriter-->与fs相关联  
                    using (StreamReader sw = new StreamReader(fs))
                    {
                        while (sw.Peek() > 0)
                        {
                            string str = sw.ReadLine();
                            if (string.IsNullOrEmpty(str) || str.StartsWith(";"))
                            {
                                continue;
                            }
                            DataRepositoryLog log = DataRepositoryLog.Deserialize(str);
                            if (log != null && log.RepositoryType!= EnumDataRepositoryType.Unknown)
                            {
                                list.Add(log);
                            }
                        }
                        sw.Close();
                    }
                    fs.Close();
                }
            }
            return list;
            
        }
        

    }
}
