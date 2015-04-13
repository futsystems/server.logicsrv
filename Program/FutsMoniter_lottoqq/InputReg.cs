using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutsMoniter
{
    public class InputReg
    {
        /// <summary>
        /// 通道token
        /// </summary>
        public static System.Text.RegularExpressions.Regex ConnectorToken = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9-]+$");

        /// <summary>
        /// 路由组标识
        /// </summary>
        public static System.Text.RegularExpressions.Regex RouterGroupName = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9-]+$");
        
        /// <summary>
        /// 登入名
        /// </summary>
        public static System.Text.RegularExpressions.Regex Login = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9-]+$");
        public static System.Text.RegularExpressions.Regex ServerPort = new System.Text.RegularExpressions.Regex(@"^[1-9]\d*$");
    }
}
