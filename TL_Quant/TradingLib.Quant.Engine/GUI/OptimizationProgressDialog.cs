using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Quant.Base;


namespace TradingLib.Quant.Engine
{
    public partial class OptimizationProgressDialog : Form,IOptimizationProgressUpdate
    {
        public event VoidDelegate CancelOptimEvent;
        public OptimizationProgressDialog()
        {
            InitializeComponent();
            //this.CreateProgressBar(3);
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            if (CancelOptimEvent != null)
                CancelOptimEvent();

        }


        List<ctOptimizationProgress> progresslist = new List<ctOptimizationProgress>();

        void CreateProgressBar(int count)
        {
            if (count == progresslist.Count) return;
            for (int i = 0; i < count; i++)
            {
                ctOptimizationProgress control = new ctOptimizationProgress(i);
                this.panel.Controls.Add(control);
                control.Location = new Point(3, i * 40);
                progresslist.Add(control);
            }
            this.Height = count * 40+55;
            this.panel.ResumeLayout();

            
        
        }

        public void SetTotalRunNumber(int totalrun)
        {
            _totalitems = totalrun;
            totalnumlabel.Text = totalrun.ToString();
        }
        public void UpdateProgress(List<OptimizationPlugin.ProgressItem> progressItems,int currentitem)
        {
            this.CreateProgressBar(progressItems.Count);
            for (int i = 0; i < progressItems.Count; i++)
            {
                progresslist[i].GotProgressItem(progressItems[i]);
            }
            if (currentitem >= 0)
                TotalProgress(currentitem);


            
        }
        int _totalitems = 1;
        void TotalProgress(int currentitem)
        {
            int p = (int)100 * currentitem / _totalitems;
            if (p > 100) p = 100;
            if (p < 0) p = 0;
            totalprogress.Value = p;
            totalprogress.Invalidate();
            currentrunnumlabel.Text = currentitem.ToString();
                
            
        }
    }
}
