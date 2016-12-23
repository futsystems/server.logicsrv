using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 委托编号
    /// 从委托进入系统的第一个入口 
    /// 设定委托唯一编号id和日内流水OrderSeq
    /// 
    /// </summary>
    public partial class MsgExchServer
    {

        /// <summary>
        /// 客户端提交委托
        /// </summary>
        /// <param name="o"></param>
        void SrvOnOrderRequest(ISession session,OrderInsertRequest request,IAccount account)
        {
            logger.Info("Got Order:" + request.Order.GetOrderInfo());

            //检查插入委托请求是否有效
            if (!request.IsValid)
            {
                logger.Warn("请求无效");
                return;
            }
            //如果请求没有指定交易帐号 则根据对应的Client信息自动绑定交易帐号
            if (string.IsNullOrEmpty(request.Order.Account))
            {
                request.Order.Account = account.ID;
            }
            //如果指定交易帐号与登入交易帐号不符 则回报异常
            if (account.ID != request.Order.Account)//客户端没有登入或者登入ID与委托ID不符
            {
                logger.Warn("客户端对应的帐户:" + account.ID + " 与委托帐户:" + request.Order.Account + " 不符合");
                return;
            }

            //标注来自客户端的原始委托
            Order order = new OrderImpl(request.Order);//复制委托传入到逻辑层
            order.id = 0;
            order.OrderSource = QSEnumOrderSource.CLIENT;

            //设定委托达到服务器时间
            order.Date = Util.ToTLDate();
            order.Time = Util.ToTLTime();

            //设定TotalSize为 第一次接受到委托时候的Size
            order.TotalSize = order.Size;

            //设定分帐户端信息
            order.FrontIDi = session.FrontIDi;
            order.SessionIDi = session.SessionIDi;
            order.RequestID = request.RequestID;


            order.id = 0;//从外部传入的委托 统一置委托编号为0,由内部统一进行委托编号

            OrderRequestHandler(order, false, true);//外部委托
        }



    }
}
