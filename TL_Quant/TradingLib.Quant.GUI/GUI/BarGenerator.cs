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
using System.IO;


namespace TradingLib.Quant.GUI
{
    public partial class BarGenerator : KryptonForm
    {

        public const string PROGRAM = "BarGenerator";
        BackgroundWorker bw = new BackgroundWorker();

        IDataStore bdatasotre = null;//new BinaryDataStore();
        public BarGenerator()
        {

            InitializeComponent();
            bartintervaltype.Items.AddRange(Enum.GetNames(typeof(BarInterval)));
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bartintervaltype.SelectedIndex = 0;


        }



        private void generator_Click(object sender, EventArgs e)
        {
            // make sure we only convert one group at a time
            if (bw.IsBusy) { debug("后代有转换任务在运行,请等待前次任务结束..."); return; }

            Security  sec= new SecurityImpl(symbol.Text);
            DateTime start = new DateTime(2011,4,16);
            DateTime end = new DateTime(2013,2,22);

            BarInterval type = (BarInterval)Enum.Parse(typeof(BarInterval), bartintervaltype.Text, true);
            int repeat = (int)interval.Value;

            bw.RunWorkerAsync(new genbarargs(sec,type,repeat,start,end));
            debug("生成Bar数据....");

        }

        #region 后台任务线程
        long _totalticknum = 0;
        long _ticksprocessed = 0;
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            bool g = true;


            genbarargs ca = (genbarargs)e.Argument;
            DateTime start = ca.Start;
            DateTime end = ca.End;
            BarFrequency freq = ca.Freq;
            Security sec = ca.Security;


            _totalticknum = bdatasotre.GetTickStorage(sec).GetCount(start, end);
            long t=0;
            DateTime ts = bdatasotre.GetTickStorage(sec).GetDateTimeAtIndex(0, out t);
            DateTime te = bdatasotre.GetTickStorage(sec).GetDateTimeAtIndex(20, out t);
            //DateTime te = bdatasotre.GetTickStorage(sec).GetCurrentDateTime();
            debug("Tick数据开时间:" + ts.ToString());
            debug("Tick数据结束时间:" + te.ToString());

            debug("将要处理的Tick数目:" + _totalticknum.ToString());
            if (_totalticknum == 0)
            {
                debug("该合约没有tick数据");
                g = false;
            }
            else
            {
                Tick pk = new TickImpl();
                BarListImpl barlist = new BarListImpl(sec.Symbol,freq.Interval,freq.Type);

                //如果转换的Tick数据超过100天,则我们需要分批进行转换
                if (end.Subtract(start).TotalDays>100)
                {
                    DateTime nstart = start;
                    DateTime nend = start;
                    while (nend < end)
                    {
                        nstart = nend;
                        nend = nstart.AddMonths(2);
                        if (nend >= end) nend = end;

                        debug("加载Tick数据 请稍后.....("+nstart.ToString() +" - "+nend.ToString() +"0");
                        IList<Tick> klist = bdatasotre.GetTickStorage(sec).Load(nstart, nend, -1, true);
                        debug("Tick数据加载完毕");
                        foreach (Tick k in klist)
                        {
                            k.symbol = sec.Symbol;
                            if (k.date != pk.date)
                            {
                                try
                                {
                                    // report progress
                                    progress((double)_ticksprocessed / _totalticknum);
                                }
                                catch (Exception ex) { debug(ex.Message); g = false; }
                            }
                            try
                            {
                                _ticksprocessed++;
                                //Tick数据的微调处理
                                if (k.time == 113000 || k.time == 151500)
                                {

                                    DateTime dt = Util.ToDateTime(k.date, k.time);
                                    dt = dt.Subtract(new TimeSpan(0, 0, 1));
                                    k.date = Util.ToTLDate(dt);
                                    k.time = Util.ToTLTime(dt);
                                }
                                if(k.time == 9)
                                barlist.newTick(k);
                                // save this tick as previous tick
                                pk = k;
                            }
                            catch (Exception ex) { debug("生成Bar数据出错: " + ex.Message); g = false; }
                        }
                        
                        //if (nend > end) nend = end;
                    }
                }
                if (g)
                    debug("共生成Bar数据:" + barlist.Count.ToString());
                if (isSaveBar.Checked)
                {
                    debug("保存Bar数据到本地文件");
                    List<Bar> bl = new List<API.Bar>();
                    foreach (Bar b in barlist)
                    {
                        bl.Add(b);
                    }
                    bdatasotre.GetBarStorage(new SecurityFreq(sec, freq)).Save(bl);
                    debug("保存Bar数据完毕");
                }

                
                
            }
            e.Result = g;
            //return g;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // status back to user
            bool g = (bool)e.Result;
            debug("处理 ticks: " + _ticksprocessed.ToString("N0"));
            if (g) debug("converted files successfully.");
            else debug("errors converting files.");
            //重置进度条与处理tick总数
            // reset progress bar
            progress(0);
            // reset ticks processed
            _ticksprocessed = 0;
        }


        #endregion


        #region 功能函数
        private void bartintervaltype_SelectedIndexChanged(object sender, EventArgs e)
        {

            BarInterval val = (BarInterval)Enum.Parse(typeof(BarInterval), bartintervaltype.Text, true);
            if (val == API.BarInterval.CustomTicks || val == API.BarInterval.CustomTime || val == API.BarInterval.CustomVol)
            {
                interval.Enabled = true;
            }
            else
            {
                interval.Enabled = false;
            }


        }
        delegate void pdouble(double p);
        //显示进度
        void progress(double percent)
        {
            int p = (int)(percent * 100);
            if (p > 100) p = 100;
            if (p < 0) p = 0;
            // if being called from a background thread, 
            // invoke UI thread to update screen
            if (statusStrip1.InvokeRequired)
                statusStrip1.Invoke(new pdouble(progress), new object[] { percent });
            else
            {
                _progress.Value = p;
                _progress.Invalidate();
            }
        }
        //日志输出
        void debug(string msg)
        {
            
        }

        #endregion

    }

    internal class genbarargs
    {

        internal Security Security;
        internal BarFrequency Freq = BarFrequency.OneMin;
        internal DateTime Start= DateTime.MinValue;
        internal DateTime End=DateTime.MaxValue;
        
        internal genbarargs(Security sec,BarInterval inttype, int interval)
        {
            Security = sec;
            switch(inttype)
            {
                case  API.BarInterval.CustomTicks:
                case API.BarInterval.CustomTime:
                case API.BarInterval.CustomVol:
                    Freq = new BarFrequency(inttype,interval);
                    break;
                default:
                    Freq = new BarFrequency(inttype);
                    break;
            }
           

        }
        internal genbarargs(Security sec,BarInterval inttype, int interval, DateTime start, DateTime end)
            :this(sec,inttype,interval)
        {
            Start = start;
            End = end;
        }
    }
}
