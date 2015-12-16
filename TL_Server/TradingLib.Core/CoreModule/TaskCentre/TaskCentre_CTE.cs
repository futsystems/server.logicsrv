using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public partial class TaskCentre
    {
        [ContribCommandAttr(QSEnumCommandSource.CLI, "ptask", "ptask - print task list", "")]
        public string CTE_PrintTask()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CliUtils.SectionHeader(" TaskList "));
            foreach (ITask t in taskUUIDMap.Values.Where(task => task.TaskType == QSEnumTaskType.SPECIALTIME))
            {
                sb.Append(t.GetTaskMemo() + System.Environment.NewLine);
            }
            sb.Append(System.Environment.NewLine);
            foreach (ITask t in taskUUIDMap.Values.Where(task => task.TaskType == QSEnumTaskType.CIRCULATE))
            {
                sb.Append(t.GetTaskMemo() + System.Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
