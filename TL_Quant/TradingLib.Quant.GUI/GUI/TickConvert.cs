using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;

namespace TradingLib.Quant.GUI
{
    public partial class TickConvert : KryptonForm
    {
        public const string PROGRAM = "TikConverter";
        BackgroundWorker bw = new BackgroundWorker();
        const double GOODRATIO = .95;//数据优良率
        Converter _conval = Converter.None;//当前选中的数据格式
        int _ticksprocessed = 0;//处理的tick数
        int _approxtotal = 0;//大约含有多少tick数据
        public TickConvert()
        {
            InitializeComponent();
            _con.Items.AddRange(Enum.GetNames(typeof(Converter)));
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }
        #region 功能函数
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
            debugControl1.GotDebug(msg);
        }
        //格式选择 对应不同的输入参数 日后需要完善多种Tick格式
        private void _con_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get converter
            _conval = (Converter)Enum.Parse(typeof(Converter), _con.Text, true);

        }

        #endregion

        List<Tick> _TotalTick = new List<Tick>();
        #region 后台任务线程

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            convargs ca = (convargs)e.Argument;
            string[] filenames = ca.files;
            bool g = e.Result != null ? (bool)e.Result : true;

            int ds = (int)1;// _defaultsize.Value;
            for (int i = 0; i < filenames.Length; i++)
            {
                string file = filenames[i];
                debug(PROGRAM+":导入Tick文件: " + Path.GetFileNameWithoutExtension(file));
                _TotalTick.Clear();
                //检查文件是否存在
                if (!File.Exists(file))
                {
                    debug(PROGRAM+":源tick数据文件不存在: " + file);
                    continue;
                }
                bool status = true;
                //尝试转换Tick数据 如果给了symbol,则每个文件对应一个sym
                try
                {
                    // convert file
                    convert(_conval, file, ds,SecurityFreq.Security.Symbol);
                }
                catch (Exception ex)
                {
                    debug(PROGRAM+":Tick数据文件格式是否正确?  转换Tick数据: " + file + " 的时候发生错误: " + ex.Message + ex.StackTrace);
                    status = false;
                }

                // report progress
                if (!status)
                    debug(PROGRAM+":转换Tick数据文件: " + file + " 失败");
                g &= status;
            }
            e.Result = g;

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // status back to user
            bool g = (bool)e.Result;
            debug(PROGRAM+":导入" + _ticksprocessed.ToString("N0")+" Ticks");
            if (g) debug(PROGRAM+":导入文件成功.");
            else debug(PROGRAM+":导入文件失败");
            //重置进度条与处理tick总数
            // reset progress bar
            progress(0);
            // reset ticks processed
            _ticksprocessed = 0;
        }


        #endregion


        #region Tick数据转换

        public SecurityFreq SecurityFreq { get; set; }
        IDataStore bstore = null;//new BinaryDataStore();
        
        bool convert(Converter con, string filename, int tradesize, string sym)
        {
            int bads = 0;
            int thistotal = _ticksprocessed;
            bool g = true;
            // setup input file
            StreamReader infile = null;
            //打开文件处理对应的头部信息
            try
            {
                // open input file
                switch (con)
                {
                    case Converter.MultiCharts:
                        // The symbol for data being imported is obtained from the filename
                        // Selected files for import must be named SYMBOL.ext - eg AAPL.txt, GOOG.txt
                        infile = new StreamReader(filename);
                        string header = infile.ReadLine(); // ignore first line header of input file
                        debug(PROGRAM+":MC文件头" + header);
                        break;
                }

            }
            catch (Exception ex) { debug(PROGRAM+":读取头文件失败:" + ex.Message); g = false; }
            
            // setup previous tick and current tick
            Tick pk = new TickImpl();
            Tick k = null;
            do
            {
                try
                {
                    // get next tick from the file
                    switch (con)
                    {
                        case Converter.MultiCharts:
                            k = MultiCharts.parseline(infile.ReadLine(),sym);
                            break;
                    }
                }
                catch (Exception ex) { bads++; continue; }
                //检验处理信息
                if (k == null)
                {
                    debug(PROGRAM+":尝试转换失败: " + con.ToString());
                    return false;
                }
                // bad tick
                if (k.date == 0) { bads++; continue; }
                // if dates don't match, we need to write new output file RawTick模式是将日期分开处理
                //检查日期是否有跨越,日期跨越后我们将Tick数据分开进行保存
                if (k.date != pk.date)
                {
                    try
                    {
                        // report progress
                        progress((double)_ticksprocessed / _approxtotal);
                        //将tick数据按日期分批存入
                        bstore.GetTickStorage(SecurityFreq.Security).Save(_TotalTick);
                        _TotalTick.Clear();
                    }
                    catch (Exception ex) { debug(ex.Message); g = false; }
                }
                try
                {
                    // write the tick
                    _TotalTick.Add(k);
                    // save this tick as previous tick
                    pk = k;
                    // count the tick as processed
                    _ticksprocessed++;
                }
                catch (Exception ex) { debug(PROGRAM+":缓存Tick数据List失败: " + ex.Message); g = false; }

            }
            // keep going until input file is exhausted
            while (!infile.EndOfStream);
            
            // close input file
            infile.Close();
            // get percentage of good ticks
            double goodratio = (_ticksprocessed - thistotal - bads) / (_ticksprocessed - (double)thistotal);
            if (goodratio < GOODRATIO)
            {
                debug(PROGRAM+":Tick转换成功率小于 " + GOODRATIO.ToString("P0") + " ;转换失败");
                g = false;
            }
            // return status
            return g;
        }

        #endregion

        Dictionary<string, string> _filesyms = new Dictionary<string, string>();
        string _path = string.Empty;

        private void convert_Click(object sender, EventArgs e)
        {
            // make sure we only convert one group at a time
            if (bw.IsBusy) { debug(PROGRAM+":后台有转换任务在运行,请等待前次任务结束..."); return; }
            //获得数据读写插件
            if (bstore == null)
            { 
               PluginReference reference = QuantGlobals.PluginManager.CreatePlugin(QuantGlobals.DataStoreSetting, AppDomain.CurrentDomain);

               bstore = (IDataStore)reference.Plugin;
            }
          
            OpenFileDialog of = new OpenFileDialog();
            // allow selection of multiple inputs
            of.Multiselect = true;
            // keep track of bytes so we can approximate progress
            long bytes = 0;
            if (of.ShowDialog() == DialogResult.OK)
            {
                List<string> symbols = new List<string>();
                //遍历所有的文件,并记录对应的Symbol
                foreach (string file in of.FileNames)
                {
                    _path = Path.GetDirectoryName(file);
                    string sn = Path.GetFileName(file);
                    // get size of current file and append to total size
                    FileInfo fi = new FileInfo(file);
                    bytes += fi.Length;
                }
                // estimate total ticks
                _approxtotal = (int)((double)bytes / 40);
                // reset progress bar
                progress(0);
                // start background thread to convert
                bw.RunWorkerAsync(new convargs(of.FileNames));
                debug(PROGRAM+":开始导入Tick数据到本地数据库");
            }
        }

    }
    //转换参数包含需要转换的文件 以及symbol
    internal class convargs
    {

        internal string[] files;
        
        internal convargs(string[] filenames)
        {
            files = filenames;
        }
      
    }

    public class OutPut
    {

        public long ProcessedNum { get { return _processed; } }
        long _processed = 0;


        StreamWriter sw;
        FileStream fs;
        public OutPut(string fileName)
        {

            fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            sw = new StreamWriter(fs, System.Text.Encoding.Default);
        }
        string d = ",";
        public void SaveTick(Tick k)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(k.date);
            sb.Append(d);
            sb.Append(k.time);
            sb.Append(d);
            sb.Append(k.trade);
            sb.Append(d);
            sb.Append(k.size);
            sb.Append(d);
            sb.Append(k.ask);
            sb.Append(d);
            sb.Append(k.os);
            sb.Append(d);
            sb.Append(k.bid);
            sb.Append(d);
            sb.Append(k.bs);
            sw.WriteLine(sb.ToString());
        }

        public void SaveBar(Bar b)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(b.Bardate);
            sb.Append(d);
            sb.Append(b.Bartime);
            sb.Append(d);
            sb.Append(b.Open);
            sb.Append(d);
            sb.Append(b.High);
            sb.Append(d);
            sb.Append(b.Low);
            sb.Append(d);
            sb.Append(b.Close);
            sb.Append(d);
            sb.Append(b.Volume);
            sb.Append(d);
            sb.Append(b.OpenInterest);
            sw.WriteLine(sb.ToString());
        }

        public void Finished()
        {
            sw.Close();
            fs.Close();

        }



    }
}
