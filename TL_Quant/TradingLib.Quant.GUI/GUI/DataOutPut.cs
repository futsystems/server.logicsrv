using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;

namespace TradingLib.Quant.GUI
{
    public partial class DataOutPut : KryptonForm
    {
        BackgroundWorker bw = new BackgroundWorker();
        public DataOutPut()
        {
            InitializeComponent();
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }


       
        public SecurityFreq SecurityFreq { get; set; }

        QSENumOutPutType _type = QSENumOutPutType.Tick;
        public QSENumOutPutType Type { get { return _type; } set { _type = value; } }

        //private delegate BarStreamData GetStreamData();

        //GetStreamData GetSteamDataFunc;

        OutPut fileoutput;
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            status.Text = "开始";
            //获得保存文件名
            string filename = e.Argument.ToString();
            fileoutput = new OutPut(filename);
            //获得时间起止
            DateTime Start = start.Value;
            DateTime End = end.Value;

            /*
            PluginReference reference = QuantGlobals.PluginManager.CreatePlugin(QuantGlobals.DataStoreSetting, AppDomain.CurrentDomain);

            IDataStore datastore = (IDataStore)reference.Plugin;

            BarDataStreamer _barDataStreamer = new BarDataStreamer(datastore, new SecurityFreq[] { SecurityFreq }, Start, Start, 0, End, false, Type == QSENumOutPutType.Tick?true:false);

            QuantGlobals.GDebug("准备保存");
            //通过载入数据的不同获得相应最合适的获得数据的函数调用
            int totalItems= (int)(_barDataStreamer.TotalBars + _barDataStreamer.TotalTicks);
            //回放bar数据
            if (_barDataStreamer.TotalBars > 0 && _barDataStreamer.TotalTicks == 0)
            {
                GetSteamDataFunc = _barDataStreamer.GetNextBarItem;
            }
            //只回放Tick数据
            else if (_barDataStreamer.TotalTicks > 0 && _barDataStreamer.TotalBars == 0)
            {
                GetSteamDataFunc = _barDataStreamer.GetNextTickItem;
            }

            QuantGlobals.GDebug("共加载:"+totalItems.ToString());
            int num = 0;
            DateTime minValue = DateTime.Now;
            while (true)
            {
                BarStreamData nextItem = GetSteamDataFunc();//_barDataStreamer.GetQuickNextTickItem();//单合约80tick/secend;//.GetNextTickItem();// 通过快速Tick数据回放 加速到74万/secned.GetNextItem();//获得一个bar或者一个tick事件

                if (nextItem == null)
                {
                    //结束 跳出循环
                    break;
                }
                //保存Tick数据
                if (Type == QSENumOutPutType.Tick)
                    fileoutput.SaveTick(nextItem.Tick.Tick);
                else if (Type == QSENumOutPutType.Bar)
                    fileoutput.SaveBar(nextItem.NewBar.BarDictionary.Values.ToArray()[0]);

                num++;
                if (DateTime.Now.Subtract(minValue).TotalSeconds > 1)//降低更新频率
                {
                    progress((double)num/totalItems);
                    minValue = Util.ToDateTime(nextItem.Tick.Tick.date, nextItem.Tick.Tick.time);

                }
            }**/
        }
        delegate void pdouble(double p);
        void progress(double pect)
        {
            int p = (int)(pect * 100);
            if (p > 100) p = 100;
            if (p < 0) p = 0;
            // if being called from a background thread, 
            // invoke UI thread to update screen
            if (statusStrip1.InvokeRequired)
                statusStrip1.Invoke(new pdouble(progress), new object[] { pect });
            else
            {
                _progress.Value = p;
                _progress.Invalidate();
            }
            
        }
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (fileoutput != null)
                fileoutput.Finished();
            progress(0);
            status.Text = "完成";
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy)
            {
                fmConfirm.Show("有输出任务在运行");
            
            }
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "CSV文件|*.CSV";
            saveFileDialog1.InitialDirectory = "D:\\";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            else
            {
                string fileName = saveFileDialog1.FileName;
                bw.RunWorkerAsync(fileName);
            }
        }


    }

    public enum QSENumOutPutType
    { 
        Tick,
        Bar,
    }
}
