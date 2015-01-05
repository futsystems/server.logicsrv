using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.ServiceManager
{
    public partial class CoreManager
    {
        //[ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qrycore", "qrycore - 查询核心模块信息", "用于Web端查询核心模块信息")]
        //public string QryCore()
        //{
        //    List<JsonWrapperCore> list = TLCtxHelper.Ctx.QryCore();
        //    JsonWriter w = ReplyHelper.NewJWriterSuccess();
        //    ReplyHelper.FillJWriter(list.ToArray(), w);
        //    ReplyHelper.EndWriter(w);

        //    return w.ToString();

        //}

        //[ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qrycontrib", "qrycontrib - 查询扩展模块信息", "用于Web端查询扩展模块信息")]
        //public string QryContrib()
        //{
        //    List<JsonWrapperContrib> list = TLCtxHelper.Ctx.QryContrib();
        //    JsonWriter w = ReplyHelper.NewJWriterSuccess();
        //    ReplyHelper.FillJWriter(list.ToArray(), w);
        //    ReplyHelper.EndWriter(w);

        //    return w.ToString();
        //}

        //[ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qrycli", "qrycli - 查询服务端支持的命令行命令", "用于Web端查询服务端支持的命令行命令")]
        //public string QryConnector()
        //{
        //    List<JsonWrapperCLICmd> list = TLCtxHelper.Ctx.QryCLICmd();

        //    JsonWriter w = ReplyHelper.NewJWriterSuccess();
        //    ReplyHelper.FillJWriter(list.ToArray(), w);
        //    ReplyHelper.EndWriter(w);

        //    return w.ToString();

        //}

        //[ContribCommandAttr(QSEnumCommandSource.MessageWeb, "stopcore", "stopcore - 停止核心服务", "停止核心服务")]
        //public string CTE_StopCore()
        //{
        //    debug("stop at here",QSEnumDebugLevel.INFO);
        //    _messageExchagne.Stop();
        //    return "goto";

        //}

        //[ContribCommandAttr(QSEnumCommandSource.MessageWeb, "startcore", "startcore - 停止核心服务", "停止核心服务")]
        //public string CTE_StartCore()
        //{
        //    debug("start at here", QSEnumDebugLevel.INFO);
        //    _messageExchagne.Start();
        //    return "goto";

        //}
    }
}
