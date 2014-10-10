namespace TradingLib.Quant.GUI
{
    partial class ChartForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Easychart.Finance.ExchangeIntraday exchangeIntraday1 = new Easychart.Finance.ExchangeIntraday();
            this.sizeToolControl = new Easychart.Finance.Win.SizeToolControl();
            this.WinChartControl = new Easychart.Finance.Win.ChartWinControl();
            this.SuspendLayout();
            // 
            // sizeToolControl
            // 
            this.sizeToolControl.ChartControl = this.WinChartControl;
            this.sizeToolControl.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.sizeToolControl.Location = new System.Drawing.Point(0, 371);
            this.sizeToolControl.Name = "sizeToolControl";
            this.sizeToolControl.Size = new System.Drawing.Size(770, 20);
            this.sizeToolControl.TabIndex = 1;
            // 
            // WinChartControl
            // 
            this.WinChartControl.CausesValidation = false;
            this.WinChartControl.DefaultFormulas = null;
            this.WinChartControl.Designing = false;
            this.WinChartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WinChartControl.EndTime = new System.DateTime(((long)(0)));
            this.WinChartControl.FavoriteFormulas = "VOLMA;RSI;CCI;OBV;ATR;FastSTO;SlowSTO;ROC;TRIX;WR;AD;CMF;PPO;StochRSI;ULT;BBWidth" +
                ";PVO";
            exchangeIntraday1.TimePeriods = new Easychart.Finance.TimePeriod[0];
            exchangeIntraday1.TimeZone = -4D;
            this.WinChartControl.IntradayInfo = exchangeIntraday1;
            this.WinChartControl.Location = new System.Drawing.Point(0, 0);
            this.WinChartControl.MaxPrice = 0D;
            this.WinChartControl.MinPrice = 0D;
            this.WinChartControl.Name = "WinChartControl";
            this.WinChartControl.PriceLabelFormat = null;
            this.WinChartControl.Size = new System.Drawing.Size(770, 371);
            this.WinChartControl.StartTime = new System.DateTime(((long)(0)));
            this.WinChartControl.TabIndex = 2;
            // 
            // ChartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(770, 391);
            this.Controls.Add(this.WinChartControl);
            this.Controls.Add(this.sizeToolControl);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "ChartForm";
            this.Text = "ChartForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Easychart.Finance.Win.SizeToolControl sizeToolControl;
        private Easychart.Finance.Win.ChartWinControl WinChartControl;

    }
}