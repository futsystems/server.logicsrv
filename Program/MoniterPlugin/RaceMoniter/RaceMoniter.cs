using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MoniterControl;
using TradingLib.Protocol;



namespace TradingLib.RaceMoniter
{
    [MoniterControlAttr("RaceMoniter", "比赛模块", QSEnumControlPlace.WorkSpace)]
    public partial class RaceMoniter : MonitorControl
    {
        public RaceMoniter()
        {
            InitializeComponent();
            this.Load += new EventHandler(RaceMoniter_Load);
        }

        void RaceMoniter_Load(object sender, EventArgs e)
        {
            ctRaceList1.QryRaceListEvent += new VoidDelegate(QryRaceList);
            ctRaceList1.OpenNewRaceEvent += new VoidDelegate(OpenNewRace);
            ctRaceList1.ExamineRaceEvent += new VoidDelegate(ExamineRace);

            ctRaceService1.QryRaceEvent += new Action<string, string>(QryRaceService);
            ctRaceService1.PrompotAccountEvent += new Action<string>(PromptAccount);
            ctRaceService1.EliminateAccountEvent += new Action<string>(EliminateAccount);
            ctRaceService1.SignRaceAccountEvent += new Action<string>(SignRaceAccount);
        }



        public override string Title
        {
            get
            {
                return "比赛模块";
            }
        }


        [CallbackAttr("RaceCentre", "QryRaceList")]
        public void OnQryRaceList(string result)
        {
            ctRaceList1.OnRaceList(result,true);
        }

        [CallbackAttr("RaceCentre", "QryRaceService")]
        public void OnQryRaceService(string result)
        {

            ctRaceService1.OnRaceService(result,true);
        }

        /// <summary>
        /// 查询比赛列表
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="mgrid"></param>
        void QryRaceList()
        {
            this.Request("RaceCentre", "QryRaceList", "");
        }

        void OpenNewRace()
        {
            this.Request("RaceCentre", "OpenNewRace", "");
        }

        void QryRaceService(string account,string racetype)
        {

            this.JsonRequest("RaceCentre", "QryRaceService", new  { account = account, race_status = racetype });
        }

        /// <summary>
        /// 淘汰交易帐户
        /// </summary>
        /// <param name="account"></param>
        void EliminateAccount(string account)
        {
            this.Request("RaceCentre", "EliminateAccount",account);
        }

        /// <summary>
        /// 报名参加比赛
        /// </summary>
        /// <param name="account"></param>
        void SignRaceAccount(string account)
        {
            this.Request("RaceCentre", "SignRaceAccount", account);
        }

        /// <summary>
        /// 晋级交易帐户
        /// </summary>
        /// <param name="account"></param>
        void PromptAccount(string account)
        {
            this.Request("RaceCentre", "PromptAccount", account);
        }

        /// <summary>
        /// 手工考核比赛
        /// </summary>
        void ExamineRace()
        {
            this.Request("RaceCentre", "ExamineRace","");
        }

    }
}
