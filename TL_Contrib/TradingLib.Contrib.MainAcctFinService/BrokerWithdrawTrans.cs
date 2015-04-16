using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;

namespace TradingLib.Contrib.MainAcctFinService
{

    public class BrokerTransTracker
    {
        ILog logger = LogManager.GetLogger(typeof(BrokerTransTracker));


        public BrokerTransTracker()
        {
            TLCtxHelper.EventSystem.BrokerTransferEvent += new EventHandler<BrokerTransferEventArgs>(EventSystem_BrokerTransferEvent);
        }

        void EventSystem_BrokerTransferEvent(object sender, BrokerTransferEventArgs e)
        {
            this.GotBrokerTransfer(e);
        }


        ConcurrentDictionary<int, BrokerTransField> transmap = new ConcurrentDictionary<int, BrokerTransField>();



        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            transmap.Clear();
        }


        /// <summary>
        /// 接口出入金操作回报超时时间
        /// </summary>
        const int TimeOutSec = 30;

        public void CheckBrokerTransTimeOut()
        {
            foreach (var f in transmap.Values)
            {
                if (DateTime.Now.Subtract(Util.ToDateTime(f.DateTime)).TotalSeconds > TimeOutSec)
                {
                    FinGlobal.FinServiceTracker.UpdateFeeStatus(f.Fee, QSEnumFeeStatus.Fail,"回报超时");
                }
            }
        }

        /// <summary>
        /// 向某个接口提交一个Fee操作
        /// </summary>
        /// <param name="token"></param>
        /// <param name="f"></param>
        public void GotFeePlaced(TLBroker broker, Fee f)
        {
            logger.Debug(string.Format("向Broker:{0}提交出金 {1}",broker.Token,f.Amount));

            if (transmap.Keys.Contains(f.ID))
            {
                logger.Warn(string.Format("Fee:{0} is alread registed", f.ID));
                return;
            }

            transmap.TryAdd(f.ID, new BrokerTransField(broker.Token, f));

            //调用接口执行出金操作
            broker.Withdraw((double)f.Amount, "");
        }

        /// <summary>
        /// 获得接口出入金回报
        /// </summary>
        /// <param name="arg"></param>
        public void GotBrokerTransfer(BrokerTransferEventArgs arg)
        {
            BrokerTransField  f= transmap.Values.Where(bs => { return bs.BrokerToken.Equals(arg.BrokerToken) && bs.Fee.Amount == arg.Amount; }).FirstOrDefault();

            if (f == null) return;//没有记录该出入金操作请求

            //出入金操作成功
            if (arg.ErrorID == 0)
            {
                FinGlobal.FinServiceTracker.FeeCollected(f.Fee);
                FinGlobal.FinServiceTracker.UpdateFeeStatus(f.Fee, QSEnumFeeStatus.Success);
            }
            else
            {
                FinGlobal.FinServiceTracker.UpdateFeeStatus(f.Fee, QSEnumFeeStatus.Fail,arg.ErrorMsg);
            }

            //处理完毕后从列表中删除
            transmap.TryRemove(f.Fee.ID, out f);
        }
    }
    internal class BrokerTransField
    {
        public BrokerTransField(string token, Fee f)
        {
            this.BrokerToken = token;
            this.Fee = f;
            this.DateTime = Util.ToTLDateTime();
        }

        /// <summary>
        /// 主帐户通道编号
        /// </summary>
        public string BrokerToken { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public long DateTime { get; set; }

        /// <summary>
        /// 对应的收费对象
        /// </summary>
        public Fee Fee { get; set; }

    }
}
