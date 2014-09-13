using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 优化器父类,所有的优化器均从该类继承
    /// </summary>
    public abstract class OptimizationPlugin
    {
        // Fields
        private IOptimizationServices _optservice;

        // Methods
        protected OptimizationPlugin()
        {
        }
        /// <summary>
        /// 取消优化过程
        /// </summary>
        public virtual void CancelOptimization()
        {
            this._optservice.AbortOptimization();
        }
        /// <summary>
        /// 新建优化过程窗口
        /// </summary>
        /// <param name="cancelCallback"></param>
        /// <returns></returns>
        public virtual Form CreateProgressWindow(Action cancelCallback)
        {
            return this._optservice.CreateDefaultProgressWindow(cancelCallback);
        }

        /// <summary>
        /// 设定总的运行次数,用于更新总的优化进度
        /// </summary>
        /// <param name="totalrun"></param>
        public virtual void SetTotalRunNumber(int totalrun)
        {
            this._optservice.SetTotalRunNumber(totalrun);
        }
        /// <summary>
        /// 绑定optimizationservices
        /// </summary>
        /// <param name="services"></param>
        public void Initialize(IOptimizationServices services)
        {
            this._optservice = services;
        }
        /// <summary>
        /// 从配置文件加载优化参数集合
        /// </summary>
        /// <param name="runSettings"></param>
        /// <param name="filename"></param>
        public virtual void LoadOptimizationSettingsFromFile(StrategyRunSettings runSettings, string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                string str2;
                string listSeparator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                while ((str2 = reader.ReadLine()) != null)
                {
                    string[] fields = str2.Split(new string[] { listSeparator, "\t" }, StringSplitOptions.None);
                    if (fields.Length >= 4)
                    {
                        double num;
                        double num2;
                        int num3;
                        StrategyParameterInfo info = this.OptimizationParameters.FirstOrDefault<StrategyParameterInfo>(p => p.Name == fields[0]);
                        if (((info != null) && double.TryParse(fields[1], out num)) && (double.TryParse(fields[2], out num2) && int.TryParse(fields[3], out num3)))
                        {
                            info.Low = num;
                            info.High = num2;
                            info.NumSteps = num3;
                        }
                    }
                }
            }
        }

        public abstract List<OptimizationResult> RunOptimization(StrategyRunSettings runSettings);
        public OptimizationResult RunSystem(StrategyRunSettings runSettings, StrategyProgressUpdate progressCallback)
        {
            return this._optservice.RunSystem(runSettings, progressCallback);
        }

        //显示优化设定窗口
        public virtual bool ShowOptimizationSettings(StrategyRunSettings runSettings, IWin32Window owner)
        {
            return this._optservice.ShowDefaultOptimizationSettingsDialog(owner);
        }
        /// <summary>
        /// 更新进度信息
        /// </summary>
        /// <param name="progressItems"></param>
        protected void UpdateProgress(List<ProgressItem> progressItems, int currentRun)
        {
            this._optservice.UpdateProgress(progressItems, currentRun);
        }

        //更新进度(包含总进度信息)
        protected void UpdateProgress(string overallText, double overallProgress, string currentRunText, double currentRunProgress)
        {
            List<ProgressItem> progressItems = new List<ProgressItem> {
            new ProgressItem(overallText, overallProgress),
            new ProgressItem(currentRunText, currentRunProgress)
        };
            this.UpdateProgress(progressItems, -1);
        }

        // Properties
        //策略优化参数
        protected List<StrategyParameterInfo> OptimizationParameters
        {
            get
            {
                return this._optservice.OptimizationParameters;
            }
        }

        public int ThreadsToUse { get; set; }

        // Nested Types
        public class ProgressItem
        {
            private double progress;
            private string text;

            // Methods
            public ProgressItem(string text, double progress)
            {
                if ((progress < 0.0) || (progress > 1.0))
                {
                    throw new ArgumentOutOfRangeException("Progress parameter must be between 0 and 1.  Value: " + progress);
                }
                this.Text = text;
                this.Progress = progress;
            }

            // Properties
            public double Progress
            {
                get
                {
                    return this.progress;
                }
                private set
                {
                    this.progress = value;
                }
            }

            public string Text
            {
                get
                {
                    return this.text;
                }
                private set
                {
                    this.text = value;
                }
            }
        }
    }
}
