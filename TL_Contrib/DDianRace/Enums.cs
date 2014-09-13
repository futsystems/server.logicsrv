using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Lottoqq.Race
{
    public enum QSEnumDDRaceType
    {
        [Description("模拟比赛")]
        RaceSim,
        [Description("实盘稳定组")]
        RealStable,
        [Description("实盘波动组")]
        RealUndulate,
    }

    /*
    public enum QSEnumDDRaceStatus
    {
        [Description("未报名")]
        NORACE=0,//未加入比赛(选手未申请加入比赛)
        [Description("正在模拟比赛")]
        INPRERACE=1,//模拟比赛,模拟比赛不进行淘汰,如果达到要求 选手可以选择重置帐户
        [Description("淘汰")]
        DDELIMINATE=2,//如果实盘选手未达到比赛条件，则系统会淘汰该实盘选手
        [Description("实盘稳定组审核")]
        INREALSTABLE_Check = 3,//晋级实盘25万
        [Description("实盘稳定组")]
        INREALSTABLE=3,//晋级实盘25万
        [Description("实盘波动组审核")]
        INREALUndulate_Check = 4,//晋级实盘50万
        [Description("实盘波动组")]
        INREALUndulate=4,//晋级实盘50万
    }
     * **/
}
