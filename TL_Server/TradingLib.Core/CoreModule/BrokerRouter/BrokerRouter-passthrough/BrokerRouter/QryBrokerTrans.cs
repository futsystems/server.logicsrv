using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;
using System.Threading;

namespace TradingLib.Core
{


    public class QryBrokerInfoTrans
    {
        ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        const int timeoutMSec = 2000;

        public QryBrokerInfoTrans()
        {
        
        }

        void BindEvent()
        {
            TLCtxHelper.EventSystem.BrokerAccountInfoEvent += new EventHandler<BrokerAccountInfoEventArgs>(BrokerAccountInfoHandler);
        }

        void UnBindEvent()
        {
            TLCtxHelper.EventSystem.BrokerAccountInfoEvent -= new EventHandler<BrokerAccountInfoEventArgs>(BrokerAccountInfoHandler);
        }


        string _brokertoken = string.Empty;
        bool _requested = false;
        /// <summary>
        /// 请求操作
        /// </summary>
        public object[] QryBrokerAccountInfo(object[] args)
        {
            try
            {
                if (!_requested)
                {
                    BindEvent();
                    _requested = true;
                }

                TLXBroker broker = args.GetValue(0) as TLXBroker;

                TimeoutObject.Reset();

                _brokertoken = broker.Token;
                //查询帐户信息
                //broker.QryAccountInfo();

                if (TimeoutObject.WaitOne(timeoutMSec, false))
                {
                    if (_info != null)
                    {
                        return new object[] { _info };
                    }
                    else
                    {
                        throw new Exception("Have no account info");
                    }
                }
                else //超时时间过去后仍然没有事件终止信号
                {
                    throw new Exception("TimeOut");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                _requested = false;
                _brokertoken = string.Empty;
                UnBindEvent();
            }
        }

        BrokerAccountInfo _info = null;
        /// <summary>
        /// 异步响应函数
        /// </summary>
        void BrokerAccountInfoHandler(object sender, BrokerAccountInfoEventArgs arg)
        {
            if (!_requested) return;

            if (_brokertoken.Equals(arg.BrokerToken))
            {
                _info = arg.AccountInfo;
                TimeoutObject.Set();
            }

        }


    }
}
