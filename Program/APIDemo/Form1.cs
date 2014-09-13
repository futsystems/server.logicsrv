using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using ZeroMQ;
using System.Threading;
using System.Diagnostics;


namespace APIDemo
{
    public partial class Form1 : Form
    {
        void debug(string msg)
        {
            ctDebug1.GotDebug(msg);
        }
        public Form1()
        {
            InitializeComponent();
            Program.SendDebugEvent +=new DebugDel(debug);
        }

        string ReqAddress
        {
            get
            {
                int baseport = int.Parse(Port.Text);
                return string.Format("tcp://{0}:{1}", IPAddress.Text, (baseport + 1).ToString());
            }
        }
        void reqbrokername()
        {
            string rep = string.Empty;
            ZmqContext context = ZmqContext.Create();
            ZmqSocket requester = context.CreateSocket(SocketType.REQ);
            string cstr = ReqAddress;
            debug("Req Socket Connect to:" + cstr);
            requester.Connect(cstr);

            IPacket package = new BrokerNameRequest();
            debug(package.ToString());
            ZMessage zmsg = new ZMessage(package.Data);
            //发送消息并得到服务端回报
            zmsg.Send(requester);
            byte[] response =new byte[0];
            int size=0;
            response = requester.Receive(response,new TimeSpan(0, 0, IPUtil.SOCKETREPLAYTIMEOUT),out size);
            TradingLib.Common.Message message = TradingLib.Common.Message.gotmessage(response);
            debug("got raw size:" + size +" type:"+message.Type +" content:"+message.Content);

            BrokerNameResponse br = new BrokerNameResponse();
            br.Deserialize(message.Content);
            debug("provider type:" + br.Provider.ToString() + " name:" + br.BrokerName);
        }
        private void btnBrokerNameRequest_Click(object sender, EventArgs e)
        {
            new Thread(reqbrokername).Start();
        }

        int requestid=0;
        private void btnBrokerName_Click(object sender, EventArgs e)
        {
            BrokerNameRequest br = new BrokerNameRequest();
            br.SetRequestID(requestid++);
            debug("BrokerNameRequest:" + br.ToString());

            debug("BrokerName Content:" + br.Content);
            BrokerNameRequest request = RequestTemplate<BrokerNameRequest>.SrvRecvRequest("fid", "cid", br.Content);
            debug("BrokerNameRequest:" + request.ToString());

            BrokerNameResponse response = ResponseTemplate<BrokerNameResponse>.SrvSendRspResponse(br);

            debug("BrokerNameResponse:" + response.ToString());
        }

        private void btnVersion_Click(object sender, EventArgs e)
        {
            VersionRequest vq = new VersionRequest();
            vq.SetRequestID(requestid++);
            vq.DeviceType = "device";
            vq.ClientVersion = "version";

            debug("version request:" + vq.ToString());
            debug("---");
            VersionRequest request = RequestTemplate<VersionRequest>.SrvRecvRequest("fid", "cid", vq.Content);
            debug("versoin request:" + request.ToString());


        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            BrokerNameRequest br = new BrokerNameRequest();
            br.SetRequestID(requestid++);
            debug("BrokerNameRequest:" + br.ToString());
            string content = br.Content;

            int num = int.Parse(convertnum.Text);

            Stopwatch packetgen = new Stopwatch();
            Stopwatch packetconvert = new Stopwatch();
            Stopwatch packetconvert2 = new Stopwatch();
            packetgen.Start();
            for(int i=0;i<num;i++)
            {
                BrokerNameRequest request = RequestTemplate<BrokerNameRequest>.SrvRecvRequest("fid", "cid", br.Content);
            }
            packetgen.Stop();
            debug("packet生成消耗:"+packetgen.Elapsed.ToString());


            packetconvert.Start();
            for (int i = 0; i < num; i++)
            {
                IPacket request = RequestTemplate<BrokerNameRequest>.SrvRecvRequest("fid", "cid", br.Content);
            }
            packetconvert.Stop();
            debug("packetconvert生成消耗:" + packetconvert.Elapsed.ToString());

            IPacket req2 = RequestTemplate<BrokerNameRequest>.SrvRecvRequest("fid", "cid", br.Content);

            packetconvert2.Start();
            for (int i = 0; i < num; i++)
            {
                BrokerNameRequest req = req2 as BrokerNameRequest;
            }
            packetconvert2.Stop();
            debug("packetconvert2生成消耗:" + packetconvert2.Elapsed.ToString());


        }

        TLClientNet tlclient = null;
        private void btnStatClient_Click(object sender, EventArgs e)
        {
            new Thread(initclient).Start();
            //tlclient.

        }

        void initclient()
        {
            tlclient = new TLClientNet(new string[] { IPAddress.Text }, int.Parse(Port.Text),true);
            tlclient.OnDebugEvent +=new DebugDelegate(debug);
            tlclient.DebugLevel = QSEnumDebugLevel.DEBUG;
            tlclient.OnLoginEvent += new LoginResponseDel(tlclient_OnLoginEvent);
            
            tlclient.Start();
            
        }

        void tlclient_OnLoginEvent(LoginResponse response)
        {
            currentaccount.Text = response.Account;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string userid = loginid.Text;
            string pass = passwd.Text;

            tlclient.ReqLogin(userid, pass);
        }

        private void btnQryOrder_Click(object sender, EventArgs e)
        {
            tlclient.ReqQryOrder(qryorder_symbol.Text, long.Parse(qryorder_orderid.Text));
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            Order o = new OrderImpl();
            o.symbol = sendorder_symbol.Text;
            o.size = int.Parse(sendorder_size.Text);
            o.side = true;
            o.price = decimal.Parse(sendorder_price.Text);

            tlclient.ReqOrderInsert(o);

        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            Order o = new OrderImpl();
            o.symbol = sendorder_symbol.Text;
            o.size = int.Parse(sendorder_size.Text);
            o.side = false;
            o.price = decimal.Parse(sendorder_price.Text);

            tlclient.ReqOrderInsert(o);
        }

        private void btnQryOrder_Click_1(object sender, EventArgs e)
        {
            tlclient.ReqQryOrder(qryorder_symbol.Text, long.Parse(qryorder_orderid.Text));
        }

        private void btnQryTrade_Click(object sender, EventArgs e)
        {
            tlclient.ReqQryTrade(qrytrade_symbol.Text);
        }

        private void btnQryPosition_Click(object sender, EventArgs e)
        {
            tlclient.ReqQryPosition(qrypos_symbol.Text);
        }

        private void btnQryMaxVol_Click(object sender, EventArgs e)
        {
            tlclient.ReqQryMaxOrderVol(qrymaxvol_symbol.Text);
        }

        private void btnQryAcc_Click(object sender, EventArgs e)
        {
            tlclient.ReqQryAccountInfo();
        }

        private void btnQryInvestor_Click(object sender, EventArgs e)
        {
            tlclient.ReqQryInvestor();
        }

        private void btnDemo_Click(object sender, EventArgs e)
        {
            using (demoklass v = new demoklass())
            {
                Program.debug("useing demoklass instance");
                v.message();
                return;
            }
        }

        private void btnSub_Click(object sender, EventArgs e)
        {
            string[] syms = symlist.Text.Split(',');
            tlclient.ReqRegisterSymbols(syms);
        }

        private void btnCancelorder_Click(object sender, EventArgs e)
        {
            try
            {
                OrderAction action = new OrderActionImpl();

                action.ActionFlag = QSEnumOrderActionFlag.Delete;
                action.OrderID = long.Parse(orderaction_orderid.Text);
                action.OrderRef = orderaction_orderref.Text;

                action.Exchagne = orderaction_exchid.Text;
                action.OrderExchID = orderaction_sysid.Text;


                tlclient.ReqOrderAction(action);
            }
            catch (Exception ex)
            {
                debug("orderaction error:" + ex.ToString());
            }
        }


    }

    public class demoklass:IDisposable
    {
        public void message()
        {
            Program.debug("klass function here");
        }
        public demoklass()
        { 
            Program.debug("demo klass construce");
        }

        ~demoklass()
        {
            Program.debug("demo klass desstruce");
        }

        public void Dispose()
        { 
            Program.debug("dispose here");
        }
    }

}




