using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutsMoniter
{
    public class InputReg
    {
        public static System.Text.RegularExpressions.Regex ConnectorToken = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9-]+$");
        public static System.Text.RegularExpressions.Regex ServerPort = new System.Text.RegularExpressions.Regex(@"^[1-9]\d*$");
    }
}
