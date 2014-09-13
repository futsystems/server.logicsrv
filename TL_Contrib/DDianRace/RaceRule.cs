using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lottoqq.Race
{
    public class RaceRule
    {
        public static decimal StartEquity(QSEnumDDRaceType t)
        {
            switch (t)
            {
                case QSEnumDDRaceType.RaceSim:
                    return 50000;
                default :
                    return 0;
            }
        }
    }
}
