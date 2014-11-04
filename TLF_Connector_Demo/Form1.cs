using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using QuantBox.XAPI.Callback;
using QuantBox;
using QuantBox.XAPI;



namespace QuantBox.Demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
        }
        static void OnConnectionStatus(object sender, ConnectionStatus status, ref RspUserLoginField userLogin, int size1)
        {
            Console.WriteLine("11111" + status);
        }
        static void OnRtnDepthMarketData(object sender, ref DepthMarketDataField marketData)
        {
            Console.WriteLine(marketData.InstrumentID);
            Console.WriteLine(marketData.ExchangeID);
            Console.WriteLine(marketData.LastPrice);
        }

        static MarketDataApi api;
        private void btnInit_Click(object sender, EventArgs e)
        {
            Queue queue = new Queue(@"lib\TLQueue.dll");
        //{
        //    ApiManager.QueuePath = @"lib\TLQueue.dll";

        //    api = ApiManager.CreateMarketDataApi(@"lib\QuantBox_CTP_Quote.dll");

        //    api.Server.BrokerID = "1017";
        //    api.Server.Address = "tcp://ctpmn1-front1.citicsf.com:51213";

        //    api.User.UserID = "00000015";
        //    api.User.Password = "123456";

        //    api.OnConnectionStatus = OnConnectionStatus;
        //    api.OnRtnDepthMarketData = OnRtnDepthMarketData;

        //    api.Connect();

        //    api.Subscribe("IF1411", "");
        }

      
        
    }
}
