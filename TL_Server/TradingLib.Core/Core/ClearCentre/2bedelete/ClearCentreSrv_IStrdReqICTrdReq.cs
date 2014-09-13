//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using System.Threading;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.MySql;

//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 对外发送委托与取消委托操作
//    /// </summary>
//    public partial class ClearCentre
//    {
//        #region *【ISTrdReq, ICTrdReq】清算中心对外交易操作
//        //IdTracker idt = new IdTracker();//用于管理order等项目的ID
//        /// <summary>
//        /// 接收到客户端的委托请求
//        /// </summary>
//        public event OrderDelegate newSendOrderRequest;//service接收到委托请求
//        /// <summary>
//        /// 接收到客户端的取消委托请求
//        /// </summary>
//        public event LongDelegate newOrderCancelRequest;//service接收到取消委托请求

//        public void SendOrder0(Order o)
//        {
//            try
//            {
//                //if (!HaveAccount(o.Account)) return;
//                o.date = Util.ToTLDate(DateTime.Now);
//                o.time = Util.ToTLTime(DateTime.Now);
//                /*
//                Security _sec = getSecurity(o);
//                if (_sec == null)
//                {
//                    debug("无有效合约", QSEnumDebugLevel.ERROR);
//                    return;
//                }
                
//                o.Exchange = _sec.DestEx;
//                o.LocalSymbol = o.symbol;
//                 * */

//                if (newSendOrderRequest != null)
//                    newSendOrderRequest(o);
//            }
//            catch (Exception ex)
//            {
//                debug("发送委托异常:" + o.ToString() + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }


//        public void CancelOrder(long number)
//        {
//            try
//            {
//                if (newOrderCancelRequest != null)
//                    newOrderCancelRequest(number);
//            }
//            catch (Exception ex)
//            {
//                debug("取消委托异常:" + number.ToString() + ex.ToString(), QSEnumDebugLevel.ERROR);
//            }
//        }



        

//        #endregion 
      
//    }
//}
