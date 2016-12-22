using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 用于标注扩展模块的命令,指明该函数响应什么消息形成Message->Command调用的模式
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DataCommandAttr : TLAttribute
    {
        /// <summary>
        /// 该命令操作标识
        /// </summary>
        public string CmdStr { get { return _cmdstr; } }

        /// <summary>
        /// 命令帮助
        /// </summary>
        public string Help { get { return _help; } }

        /// <summary>
        /// 命令说明
        /// </summary>
        public string Description { get { return _description; } }

        string _cmdstr;
        string _help;
        string _description;


        QSEnumArgParseType _parsetype = QSEnumArgParseType.CommaSeparated;
        public QSEnumArgParseType ArgParseType { get { return _parsetype; } }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="help"></param>
        /// <param name="description"></param>
        /// <param name="parsetype"></param>
        public DataCommandAttr(string cmd, string help, string description, QSEnumArgParseType parsetype = QSEnumArgParseType.CommaSeparated)
        {
            _cmdstr = cmd;
            _help = help;
            _description = description;
            _parsetype = parsetype;
        }
    }
}
