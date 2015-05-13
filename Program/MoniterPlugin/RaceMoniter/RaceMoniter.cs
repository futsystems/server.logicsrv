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

        void EliminateAccount(string account)
        {
            this.Request("RaceCentre", "EliminateAccount",account);
        }

        void SignRaceAccount(string account)
        {
            this.Request("RaceCentre", "SignRaceAccount", account);
        }

        void PromptAccount(string account)
        {
            this.Request("RaceCentre", "PromptAccount", account);
        }
    }
}
