using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    public interface IBarDataStorage
    {
        // Methods
        string CompanyName();
        bool DeleteBars(SecurityFreq symbol);
        bool DeleteTicks(Security symbol);
        void DoSettings();
        void Flush();
        void ForceDefaultSettings();
        long GetBarCount(SecurityFreq symbol, DateTime startDateTime, DateTime endDateTime);
        string GetDescription();
        string GetName();
        string id();
        bool IsProperlyConfigured();
        string LastError();
        List<Bar> LoadBars(SecurityFreq symbol, DateTime startDateTime, DateTime endDateTime, int maxLoadBars, bool loadFromEnd);
        List<Tick> LoadTicks(Security symbol, DateTime startDate);
        List<Tick> LoadTicks(Security symbol, DateTime startDate, DateTime endDate);
        bool RequiresSetup();
        int SaveBars(SecurityFreq symbol, List<Bar> bars);
        void SaveTick(Security symbol, Tick tick);
        int SaveTicks(Security symbol, List<Tick> ticks);
        int UpdateTicks(Security symbol, List<Tick> newTicks);
        string Version();
    }


}
