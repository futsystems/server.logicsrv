using System;


namespace TradingLib.API
{
    /// <summary>
    /// 命令操作接口
    /// 接受系统提供的一个文本字符串作为输入,运行后返回一个文本字符串作为输出
    /// 扩展模块需要暴露消息驱动的操作时,需要将该函数封装成支持ICommandAction的对象放到消息路由表中供内核解析并调用
    /// ISession标识哪个客户端发起了该次调用
    /// </summary>
    public interface ICommandAction
    {
        object Execute(ISession session, string parameters, bool istnetstring = false);
    }
}
