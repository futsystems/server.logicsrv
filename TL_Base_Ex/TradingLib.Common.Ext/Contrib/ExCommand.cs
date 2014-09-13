using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    ///
    /// 当客户端提交或者管理端提交消息,如果要增加哦消息处理类别
    /// 原来的框架需要改写核心路由,并且核心组件需要知道扩展组件的存在才可以调用
    /// 或者核心框架添加对应事件,然后扩展模块通过对事件响应的模式来增加该消息处理项目
    /// 
    /// 为了增加框架灵活性,我们尝试设计一种通过反射动态增加消息的结构
    /// 
    /// 扩展模块设计响应函数,然后在响应函数头部添加特性标注 命令，参数，等一些列信息
    /// 系统框架在针对模块进行扫描,将对应的函数封装成回调后注入命令列表
    /// 系统框架路由部分通过查找对应扩展模块消息响应路由列表来实现调用
    /// 
    /// 在消息类别中增加扩展ContribReq/ContribRep
    /// 消息内容为ContribName,Command,Arguments
    /// 
    /// MessageCommandAttr(响应对象[客户端,管理端,web管理端],命令名,)
    /// 参数说明
    /// public void FunctionXXXx(xxx,xxx,xxx,xxx)
    /// {
    ///     do somthing ...
    ///     kernel.sendout();返回发送
    /// }
    ///




    public class ExCommand
    {
        public static  string DEFAULT_REP_OK = "REP_OK";
        public static  string DEFAULT_REP_WRONG = "REP_WRONG";
        public ExCommand(QSEnumCommandSource source,string cmd,string response,string help,string descrip)
        {
            m_source = source;
            m_command = cmd;
            m_response = response;
            m_helptext = help;
            m_description = descrip;
        }


        protected QSEnumCommandSource m_source;//命令来源
        protected string m_command;//命令标识
        protected string m_response;//响应
        protected string m_helptext;//帮助
        protected string m_description;//描述

        public QSEnumCommandSource Source
        {
            get { return m_source; }
        }
        public string Command
        {
            get { return m_command; }
        }

        public string CommandHelp
        {
            get
            {
                char c = ' ';
                return m_command.PadRight(20, c) + m_helptext + ExComConst.Line;
            }
        }

        public string CommandDescription
        {
            get
            {
                return m_description;
            }
        }



        public virtual object ExecuteCmd(ISession session, string parameters,bool istnetstring = false)
        {
            return m_response + "\r\n";
        }

    }



}
