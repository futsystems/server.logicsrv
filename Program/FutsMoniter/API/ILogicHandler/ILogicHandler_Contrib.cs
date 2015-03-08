using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    /// <summary>
    /// 扩展命令处理接口
    /// 当客户端向服务端提交一个扩展请求时,服务端处理完毕后会有如下几种回报
    /// 查询(有返回应答)，执行命令(无返回应答,改变了相关对象状态 需要通知其他管理端)
    /// 1.RspMGROperationResponse 操作回报 操作成功 操作失败等
    /// 2.通过扩展命令接口进行查询，则对应的数据以相同module cmd 字段返回，result中包含了jsonreply序列化内容
    /// 3.通过扩展命令接口执行操作，则对应的对象以设定的cmd字段返回，result中包含了通知内容，有可能会向多个管理端发送通知
    /// </summary>
    public partial interface ILogicHandler
    {

        void OnMGRContribResponse(string module, string cmd, string result,bool islast);


        void OnMGRContribNotify(string module, string cmd, string result);
    }
}
