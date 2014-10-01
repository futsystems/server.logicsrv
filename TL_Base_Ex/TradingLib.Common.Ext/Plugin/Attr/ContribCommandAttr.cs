using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 用于标注扩展模块的命令,指明该函数响应什么消息形成Message->Command调用的模式
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true)]
    public class ContribCommandAttr:TLAttribute
    {
        /// <summary>
        /// 与该命令绑定的消息源
        /// </summary>
        public QSEnumCommandSource Source { get { return _source; } }

        /// <summary>
        /// 该命令操作标识
        /// </summary>
        public string CmdStr { get { return _cmdstr; }}

        /// <summary>
        /// 命令帮助
        /// </summary>
        public string Help { get { return _help; } }

        /// <summary>
        /// 命令说明
        /// </summary>
        public string Description { get { return _description; } }


        QSEnumCommandSource _source;
        string _cmdstr;
        string _help;
        string _description;

        /// <summary>
        /// 模块源
        /// </summary>
        public string SourceContrib { get { return _sourceContrib; } }
        string _sourceContrib="";

        /// <summary>
        /// 事件字符串
        /// </summary>
        public string EventStr { get { return _eevent; } }
        string _eevent="";

        public QSContribCommandHandleType HandlerType {get{return _htype;}}
        QSContribCommandHandleType _htype = QSContribCommandHandleType.MessageHandler;

        bool _needauth = true;
        /// <summary>
        /// 调用该函数是否需要授权
        /// </summary>
        public bool NeedAuth { get { return _needauth; } set { _needauth = true; } }

        /// <summary>
        /// 消息处理命令
        /// 用于绑定到对应消息处理路由表相应对应消息源的消息
        /// </summary>
        /// <param name="source">处理消息来源</param>
        /// <param name="cmd">命令操作码 标识了该命令</param>
        /// <param name="help">帮助</param>
        /// <param name="description">描述</param>
        public ContribCommandAttr(QSEnumCommandSource source,string cmd,string help,string description,bool needauth=true)
        {
            _htype = QSContribCommandHandleType.MessageHandler;
            _source = source;
            _cmdstr = cmd;
            _help = help;
            _description = description;
            _needauth = needauth;
        }

        /// <summary>
        /// 事件处理命令
        /// 用于绑定到某个扩展模块的某个事件
        /// </summary>
        /// <param name="contrib"></param>
        /// <param name="eventstr"></param>
        /// <param name="cmd"></param>
        /// <param name="help"></param>
        /// <param name="desctiption"></param>
        public ContribCommandAttr(string contrib, string eventstr, string cmd, string help, string desctiption, bool needauth = true)
        {
            _htype = QSContribCommandHandleType.EventHandler;
            _sourceContrib = contrib;
            _eevent = eventstr;

            _cmdstr = cmd;
            _help = help;
            _description = desctiption;
            _needauth = needauth;
        }
    }
    /// <summary>
    /// 扩展命令处理类别
    /// 1.响应消息源消息
    /// 2.绑定到其他扩展模块事件
    /// </summary>
    public enum QSContribCommandHandleType
    { 
        MessageHandler,
        EventHandler,
    }
}
