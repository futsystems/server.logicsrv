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

            setfile();
        }

        int _currentTradingday = 0;
        void setfile()
        {
            fn = Path.Combine(Util.DataRepositoryDir, string.Format("DATA.{0}.txt", _currentTradingday));
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
                if (_log != null && log!=null)
                {
                    if (_currentTradingday != TLCtxHelper.ModuleSettleCentre.Tradingday)
                    {
                        _currentTradingday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                        setfile();
                    }
                    string msg = DataRepositoryLog.Serialize(log);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        _log.WriteLine(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Save Log Error:" + ex.ToString());
            }
        }
        

    }
}
