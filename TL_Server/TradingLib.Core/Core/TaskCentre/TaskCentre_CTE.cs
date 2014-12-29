using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;


namespace TradingLib.Core
{
    public partial class TaskCentre
    {
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "qrytask", "qrytask - 查询服务端的任务列表", "用于Web端查询服务端的任务列表")]
        public string QryConnector()
        {
            //List<JsonWrapperTask> list = TLCtxHelper.Ctx.QryTask();

            //JsonWriter w = ReplyHelper.NewJWriterSuccess();
            //ReplyHelper.FillJWriter(list.ToArray(), w);
            //ReplyHelper.EndWriter(w);

            //return w.ToString();
            return "";
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "ptask", "ptask - print task list", "")]
        public string CTE_PrintTask()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ExUtil.SectionHeader(" TaskList "));
            foreach (ITask t in TLCtxHelper.Ctx.TaskList.Where(task=>task.TaskType == QSEnumTaskType.SPECIALTIME))
            {
                sb.Append(t.GetTaskMemo() + ExComConst.Line);
            }
            sb.Append(ExComConst.Line);
            foreach (ITask t in TLCtxHelper.Ctx.TaskList.Where(task => task.TaskType == QSEnumTaskType.CIRCULATE))
            {
                sb.Append(t.GetTaskMemo() + ExComConst.Line);
            }
            return sb.ToString();
        }
    }
}
