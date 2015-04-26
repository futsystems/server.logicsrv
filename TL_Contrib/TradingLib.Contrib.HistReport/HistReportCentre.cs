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

            //结算前保准分区统计
            TLCtxHelper.EventSystem.BeforeSettleEvent += new EventHandler<SystemEventArgs>(EventSystem_BeforeSettleEvent);

            //结算重置时 情况分区统计Map
            TLCtxHelper.EventSystem.BeforeSettleResetEvent += new EventHandler<SystemEventArgs>(EventSystem_BeforeSettleResetEvent);
        }

        void EventSystem_BeforeSettleResetEvent(object sender, SystemEventArgs e)
        {
            domainStaticMap.Clear();
        }

        void EventSystem_BeforeSettleEvent(object sender, SystemEventArgs e)
        {
            //交易日才堡村数据
            if (TLCtxHelper.CmdSettleCentre.IsTradingday)
            {
                foreach (var st in domainStaticMap.Values)
                {
                    try
                    {
                        ORM.MReport.InsertDomainStatistic(st);
                    }
                    catch (Exception ex)
                    {
                        debug("save domain statistic error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                    }
                }
            }
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
