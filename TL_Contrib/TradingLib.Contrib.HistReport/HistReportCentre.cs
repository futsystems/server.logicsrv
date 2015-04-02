using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib
{
    [ContribAttr(HistReportCentre.ContribName, "HistReportCentre扩展", "用于执行数据库数据查询并将统计结果返回到管理端")]
    public partial class HistReportCentre:BaseSrvObject, IContrib
    {
        const string ContribName = "HistReportCentre";

        public HistReportCentre()
            : base(HistReportCentre.ContribName)
        { 
            
        }


        /// <summary>
        /// 加载
        /// </summary>
        public void OnLoad() 
        {
        
        
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void OnDestory() 
        {
        
        }

        /// <summary>
        /// 启动
        /// </summary>
        public void Start() 
        {
        
        
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop() 
        {
        
        
        }



    }
}
