using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MDClient;
namespace DataClient
{

    public partial class Form1 : Form
    {
        ILog logger = LogManager.GetLogger("main");
        MDHandler handler = null;

        public Form1()
        {
            logger.Info("DataClient form init");
            InitializeComponent();
            handler = new MDHandler();
            handler.TickEvent += new Action<Tick>(handler_TickEvent);
            handler.BarRspEvent += new Action<Bar, RspInfo, int, bool>(handler_BarRspEvent);
            handler.BarsRspEvent += new Action<List<BarImpl>, RspInfo, int, bool>(handler_BarsRspEvent);
        }

        void handler_BarsRspEvent(List<BarImpl> arg1, RspInfo arg2, int arg3, bool arg4)
        {
            debug("bar cnt:" + arg1.Count() + " islast:" + arg4.ToString() + " requestid:" + arg3.ToString());
            foreach (var bar in arg1)
            {
                debug("bar:" + bar.ToString());
            }
        }

        void handler_BarRspEvent(Bar arg1, RspInfo arg2, int arg3, bool arg4)
        {
            debug("requestid:" + arg3.ToString());
        }

        void handler_TickEvent(Tick obj)
        {
            debug("tick:" + obj.ToString());
        }

        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            InitClient();
        }

        //TLSocket_TCP client = null;
        void InitClient()
        {
            
        }

        TLClient<TLSocket_TCP> cli;
        TradingLib.MDClient.MDClient mdclient = null;
        private void btnMQClient_Click(object sender, EventArgs e)
        {
            //mdclient = new TradingLib.MDClient.MDClient("121.40.171.230", 5060, 5060);
            //mdclient = new TradingLib.MDClient.MDClient("114.55.72.206", 5060, 5060);
            mdclient = new TradingLib.MDClient.MDClient("127.0.0.1", 5060, 5060);
            mdclient.RegisterHandler(handler);

            mdclient.Start();
            
        }

        int bcnt = 0;
        //void cli_OnPacketEvent(IPacket obj)
        //{
        //    switch (obj.Type)
        //    {
        //        case MessageTypes.BARRESPONSE:
        //            {
        //                CliOnBarResponse(obj as RspQryBarResponse);
        //                break;
        //            }
        //        default:
        //            break;
        //    }
        //}

        //void CliOnBarResponse(RspQryBarResponse response)
        //{ 
        //     bcnt++;
        //     if (response.IsLast)
        //     {
        //         logger.Info("Total got bar:" + bcnt.ToString());
        //         bcnt = 0;
        //     }
        //}

        private void btnQryService_Click(object sender, EventArgs e)
        {
            
        }

        private void btnStopMQClient_Click(object sender, EventArgs e)
        {
            if (cli != null)
            {
                cli.Stop();
            }
        }

        private void btnQryBar_Click(object sender, EventArgs e)
        {
            mdclient.QryBar(Symbol.Text,(int)interval.Value,DateTime.MinValue, end.Value,(int)maxcount.Value);
        }

        private void btnRegisterSymbol_Click(object sender, EventArgs e)
        {

            mdclient.RegisterSymbol(reg_symbol.Text);
        }

        private void btnUnRegSymbol_Click(object sender, EventArgs e)
        {
            mdclient.UnRegisterSymbol(reg_symbol.Text);
        }

        private void btnQryMT_Click(object sender, EventArgs e)
        {
            XQryMarketTimeRequest request = RequestTemplate<XQryMarketTimeRequest>.CliSendRequest(0);
            cli.TLSend(request);
        }

        private void btnQryExchange_Click(object sender, EventArgs e)
        {
            XQryExchangeRequuest request = RequestTemplate<XQryExchangeRequuest>.CliSendRequest(0);
            cli.TLSend(request);
        }

        private void btnQrySec_Click(object sender, EventArgs e)
        {
            XQrySecurityRequest request = RequestTemplate<XQrySecurityRequest>.CliSendRequest(0);
            cli.TLSend(request);
        }

        private void btnQrySymbol_Click(object sender, EventArgs e)
        {
            XQrySymbolRequest request = RequestTemplate<XQrySymbolRequest>.CliSendRequest(0);
            cli.TLSend(request);
        }

        string GetPriceFormat(decimal pricetick)
        {
            string[] p = pricetick.ToString().Split('.');
            if (p.Length <= 1)
                return "{0:F0}";
            else
                return "{0:F" + p[1].ToCharArray().Length.ToString() + "}";
        }


        private void btnPriceFormat_Click(object sender, EventArgs e)
        {
            decimal x = 123.3434343M;

            debug("x:" + string.Format(GetPriceFormat(decimal.Parse(pricetick.Text)), x));
        }

        private void btnTick_Click(object sender, EventArgs e)
        {
            mdclient.DemoTick(int.Parse(md_time.Text), md_price.Value);
        }

        private void btnReqMDHistTables_Click(object sender, EventArgs e)
        {
            mdclient.ReqMDHistTables();
        }








    }
}
