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
    public class DataRepDump
    {
        StreamWriter _log = null;
        private string fn = string.Empty;
        private string _path = "Dump";

        ILog logger = null;
        public DataRepDump(string path)
        {
            logger = LogManager.GetLogger("DataRepositoryLog");
            _path = path;
            setfile();
        }

        void setfile()
        {
            fn = Path.Combine(_path, "DATA");
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

        public void GotDataRepositoryLog(DataRepositoryLog log)
        {
            try
            {
                if (_log != null)
                {
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
