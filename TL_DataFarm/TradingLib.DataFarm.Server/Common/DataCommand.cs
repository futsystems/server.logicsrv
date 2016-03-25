using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;

namespace TradingLib.Common.DataFarm
{
    public class DataCommand
    {
        private DataCommandInfo m_info;
        private DataCommandWrapper m_wrapper;

        public DataCommand(object obj, DataCommandInfo info)
        {
            m_wrapper = new DataCommandWrapper(obj, info);
            m_info = info;
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public void ExecuteCmd(IServiceHost host, IConnection conn, string parameters)
        {
            m_wrapper.Execute(host,conn,parameters);
        }


    }

    internal class DataCommandWrapper
    {
        DataCommandInfo _cmdinfo;
        List<MethodArgument> _argslist;
        object _obj;
        public DataCommandWrapper(object obj, DataCommandInfo cmdinfo)
        {
            _obj = obj;
            _cmdinfo = cmdinfo;
            //获得该函数的参数列表
            _argslist = PluginHelper.GetArgumentList(cmdinfo.MethodInfo);
        }

        public object Execute(IServiceHost host, IConnection conn, string parameters)
        {
            string[] p = new string[] { };
            switch (_cmdinfo.Attr.ArgParseType)
            {

                case QSEnumArgParseType.Json:
                    {
                        p = new string[] { parameters };
                        break;
                    }
                case QSEnumArgParseType.CommaSeparated:
                    {
                        p = parameters.Split(',');
                        break;
                    }
                default:
                    throw new Exception("not supported args");
            }

            //如果分隔后只有1个参数,并且该参数为空或者null,则为无附加参数的函数调用,我们将p至空
            if (p.Length == 1 && string.IsNullOrEmpty(p[0]))
            {
                p = new string[] { };
            }

            int numargs = p.Length + 2;
            _argslist[0].Value = host;
            _argslist[1].Value = conn;

            for (int i = 2; i < _argslist.Count; i++)
            {
                _argslist[i].Value = p[i - 2];
            }

            ////如果第一个参数是ISession则我们将session绑定到第一个参数
            //if (_argslist.Count >= 1 && _argslist[0].Type == QSEnumMethodArgumentType.ISession)
            //{
            //    int numargs = p.Length + 1;
            //    //验证参数个数
            //    if (numargs != _argslist.Count)
            //    {
            //        throw new QSCommandError(new Exception(), "Parse arguments error: got " + numargs.ToString() + " but we need:" + _argslist.Count.ToString());
            //    }

            //    _argslist[0].Value = session;
            //    for (int i = 1; i < _argslist.Count; i++)
            //    {
            //        _argslist[i].Value = p[i - 1];
            //    }
            //}
            //else
            //{
            //    if (p.Length != _argslist.Count)
            //    {
            //        throw new QSCommandError(new Exception(), "Parse arguments error: got " + p.Length.ToString() + " but we need:" + _argslist.Count.ToString());
            //    }
            //    for (int i = 0; i < _argslist.Count; i++)
            //    {
            //        _argslist[i].Value = p[i];
            //    }
            //}

            object[] args = null;
            try
            {
                args = PluginHelper.ParseMethodArgs(_argslist);
            }
            catch (Exception ex)
            {
                //throw new QSCommandError(ex, "Parse arguments error: can not convert string parameters to arguments list we needed");
            }

            try
            {
                //调用扩展方法
                return _cmdinfo.MethodInfo.Invoke(_obj, args);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                //扩展命令运行时 抛出原始异常 FutRspError等
                throw ex.InnerException;
            }
        }
    }
}
