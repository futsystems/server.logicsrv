using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.MiniService
{

    /// <summary>
    /// 迷你服务数据库设置对象
    /// </summary>
    public class MiniServiceSetting
    {

        public int ID { get; set; }


        public string Account {get;set;}

        public bool Active {get;set;}
    }
}
