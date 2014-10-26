using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.TNetStrings;

/*
 * 扩展模块Command调用
 * 1.使用ContribCommandAttr对某个方法进行特新标注 包含处理的消息源,命令操作码,帮助和描述等基本信息
 * 2.在插件系统中通过反射获得某个方法支持该特性,然后将特性与方法封装成ContribCommandInfo
 * 3.结合生成的对象实例将ContribCommandInfo封装成支持ICommandAction的操作
 * 4.最终生成ContribCommand 并注册全局函数调用表
 * 
 * ContribCommandAttr 
 *       + MethodInfo   --> ContribCommandInfo
 *                          +object             --> ContribActionWrapper
 *                                                              ->ContribCommand
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * **/
namespace TradingLib.Common
{
    class QSCommandError : QSError
    {
        public string Reason { get { return _reason; } }
        string _reason;
        public QSCommandError(Exception raw,string reason,string label="ContribCommand Execution Error")
            :base(raw,label)
        {
            _reason = reason;
        }
    }

    public class CommandActionWrapper : ICommandAction
    {
        ContribCommandInfo _cmdinfo;
        List<MethodArgument> _argslist;
        object _obj;
        public CommandActionWrapper(object obj, ContribCommandInfo cmdinfo)
        {
            _obj = obj;
            _cmdinfo = cmdinfo;
            //获得该函数的参数列表
            _argslist = PluginHelper.GetArgumentList(cmdinfo.MethodInfo);
        }

        public object Execute(ISession session, string parameters, bool istnetstring = false)
        {
            Util.Debug("Execute got parameters:" + parameters + " argsnum:" + _argslist.Count.ToString());
            string[] p = new string[] { };
            if (istnetstring)//tnetstring 传递参数列表
            {
                byte[] bytemsg = Encoding.UTF8.GetBytes(parameters);
                ArraySegment<byte> codemsg = new ArraySegment<byte>(bytemsg);

                List<string> arglist = new List<string>();
                var res = codemsg.TParse();
                //Utils.Debug("Type:" + res.Data.Type.ToString() + " Value:" + res.Data.ToString());
                arglist.Add(res.Data.ToString());
                while (res.Remain.Count > 0)
                {
                    res = res.Remain.TParse();
                    //Utils.Debug("Type:" + res.Data.Type.ToString() + " Value:" + res.Data.ToString());
                    arglist.Add(res.Data.ToString());
                }
                //传递解析后的参数列表
                p = arglist.ToArray();
            }
            else //,号分割参数列表
            {
                //if (_cmdinfo.Attr.SplitNum > 0)
                {

                //    p = parameters.Split(new char[] { ',' }, _cmdinfo.Attr.SplitNum);//用逗号分隔参数列表
                    //Utils.Debug("限制分割: p3:" + p[2]);
                }
                //else
                {
                    p = parameters.Split(',');//用逗号分隔参数列表
                }
                if (_cmdinfo.Attr.IsJsonArg)
                {
                    p = new string[] { parameters };
                }
                else
                {
                    p = parameters.Split(',');
                }
            }

            //string[] p = parameters.Split(',');//用逗号分隔参数列表
            //如果分隔后只有1个参数,并且该参数为空或者null,则为无附加参数的函数调用,我们将p至空
            if (p.Length == 1 && string.IsNullOrEmpty(p[0]))
            {
                p = new string[] { };
            }
            //如果第一个参数是ISession则我们将session绑定到第一个参数
            if (_argslist.Count>=1 && _argslist[0].Type == QSEnumMethodArgumentType.ISession)
            {
                int numargs = p.Length + 1;
                if (numargs != _argslist.Count)
                {
                    throw new QSCommandError(new Exception(), "Parse arguments error: got " + numargs.ToString() + " but we need:" + _argslist.Count.ToString());
                }

                _argslist[0].Value = session;
                for (int i = 1; i < _argslist.Count; i++)
                {
                    _argslist[i].Value = p[i - 1];
                }
            }
            else
            {
                if (p.Length != _argslist.Count)
                {
                    throw new QSCommandError(new Exception(), "Parse arguments error: got " + p.Length.ToString() + " but we need:" + _argslist.Count.ToString());
                }
                for (int i = 0; i < _argslist.Count; i++)
                {
                    _argslist[i].Value = p[i];
                }
            }
            object[] args = null;
            try
            {
                args = PluginHelper.ParseMethodArgs(_argslist);
            }
            catch (Exception ex)
            {
                throw new QSCommandError(ex,"Parse arguments error: can not convert string parameters to arguments list we needed");
            }

            return  _cmdinfo.MethodInfo.Invoke(_obj,args);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Method:".PadRight(ExComConst.HeaderNum, ' ') +_cmdinfo.MethodInfo.ToString()  + ExComConst.Line);
            sb.Append("Argument List:" + ExComConst.Line);
            foreach (MethodArgument arg in _argslist)
            {
                sb.Append("    "+arg.ToString()+ExComConst.Line);
            }
            return sb.ToString();
        }
    }

    public class ContribCommand:ExCommand
    {
        private ICommandAction m_action;
        private ContribCommandInfo m_info;
        public ContribCommand(object obj, ContribCommandInfo info)
            :base(info.Attr.Source,info.Attr.CmdStr,"Response",info.Attr.Help,info.Attr.Description)
        {
            m_action = new CommandActionWrapper(obj, info);
            m_info = info;
        }
        public ContribCommand(QSEnumCommandSource source, string scmd,ICommandAction action,string helptext,string description)
            :base(source,scmd,"",helptext,description)
        {
            m_action = action;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override object ExecuteCmd(ISession session, string parameters, bool istnetstring = false)
        {
            return m_action.Execute(session, parameters,istnetstring);

            /*
            try
            {
                return m_action.Execute(session, parameters);
            }
            catch (QSCommandError ex)
            {
                TLCtxHelper.Ctx.debug(ex.Label + "\r\n reason@" + ex.Reason + "\r\n RawException:" + ex.RawException.Message.ToString(), QSEnumDebugLevel.ERROR);
                return DEFAULT_REP_WRONG;
            }
            catch (Exception ex)
            {
                TLCtxHelper.Ctx.debug("ExectueCmd Error:\r\n" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return DEFAULT_REP_WRONG;
            }**/

        }
        public string ContribCommandDesp
        {
            get
            {
                return m_info.Attr.Description;
            }
        }
        public string ContribCommandAPI
        { 
            get{

                StringBuilder sb = new StringBuilder();
                sb.Append((ExComConst.SectionPrefix + " CommandAPI:" + m_info.Attr.CmdStr+" ").PadRight(ExComConst.SectionNum, ExComConst.SectionChar) + ExComConst.Line);
                sb.Append("Source:".PadRight(ExComConst.HeaderNum,' ') + m_info.Attr.Source + ExComConst.Line);
                sb.Append("CmdStr:".PadRight(ExComConst.HeaderNum, ' ') + m_info.Attr.CmdStr + ExComConst.Line);
                sb.Append("NeedAuth:".PadRight(ExComConst.HeaderNum, ' ') + m_info.Attr.NeedAuth.ToString() + ExComConst.Line);
                sb.Append("Help:" + ExComConst.Line);
                sb.Append(m_info.Attr.Help + ExComConst.Line);
                sb.Append("Function Name:".PadRight(ExComConst.HeaderNum, ' ') + m_info.MethodInfo.Name + ExComConst.Line);
                sb.Append(m_action.ToString());
                sb.Append("Description:" + ExComConst.Line);
                sb.Append(m_info.Attr.Description + ExComConst.Line);

                return sb.ToString();

            }
        }
    }
}
