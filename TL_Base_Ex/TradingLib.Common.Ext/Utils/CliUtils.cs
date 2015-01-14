using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class CliUtils
    {
        public const char SECCHAR = '-';
        public const string SECPRIFX = "------";
        public const int SECNUM = 60;
        public static string SectionHeader(string title)
        {
            return (SECPRIFX + " " + title + " ").PadRight(SECNUM, SECCHAR) + System.Environment.NewLine;
        }
    }
}
