using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace FutSystems.GUI
{
    public interface IAccountView
    {
        event VoidDelegate QueryAccountInfo;
        event VoidDelegate QueryRaceInfo;


        void GotAccountInfo(AccountInfo info);//获得帐户信息
        //void GotRaceInfo(IRaceInfo info);//获得比赛信息
        //void GotFinServiceInfo(IFinServiceInfo info);//获得配资信息

    }
}
