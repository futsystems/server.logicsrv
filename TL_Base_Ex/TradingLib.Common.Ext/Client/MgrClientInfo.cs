using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public class MgrClientInfo:ClientInfoBase
    {

        public MgrClientInfo()
            :base()
        {
            //this.MGRLoginName = string.Empty;
            //this.MGRID = 0;
            //this.MGRFK = 0;
        }

        public void BindManger(Manager manager)
        {
            //this.MGRLoginName = manager.Login;
            //this.MGRID = manager.ID;
            //this.MGRFK = manager.mgr_fk;
            this.Manager = manager;
        }

        public Manager Manager { get; private set; }


        //public string MGRLoginName { get; private set; }


        ///// <summary>
        ///// 如果管理端登入成功 则会将对应的Manager绑定到该管理端对象上
        ///// 对应的Manager对象ID
        ///// </summary>
        //public int MGRID { get; private set; }

        ///// <summary>
        ///// 管理主域ID
        ///// </summary>
        //public int MGRFK { get; private set; }
    }
}
