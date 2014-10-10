using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
using TradingLib.Quant.Loader;


namespace TradingLib.Quant.GUI
{
    public partial class fmAddNewStrategy : KryptonForm
    {


        public fmAddNewStrategy()
        {
            InitializeComponent();
        }


        StrategyInfo _sinfo=null;
        public StrategyInfo StrategyInfo { get { return _sinfo; } 
            set {
                _sinfo = value;
                if (_sinfo != null)
                {
                    strategyAssemblyName.Text = _sinfo.FileName;
                    strategyName.Text = _sinfo.StrategyClassName;

                }

            } 
        }
        private void ok_Click(object sender, EventArgs e)
        {
            StrategyManager sm = QuantGlobals.Access.GetStrategyManager();
            //if(string.IsNullOrEmpty(this.strategyFriendName.Text))
            //{
            //    MessageBox.Show("请填写策略配置名称");
            //}
            StrategySetup setup = new StrategySetup();
            setup.FriendlyName = this.strategyFriendName.Text;
            setup.StrategyClassName = StrategyInfo.StrategyClassName;
            setup.ConfiFile = QuantGlobals.ProjectConfigDirectory + "/setup.FriendlyName/config.xml";
            setup.StrategyFile = StrategyInfo.FileName;
            
            //调用strategymanager添加该策略配置(策略配置会自动保存)
            if (sm.AddStrategyProject(setup))
            {

                //生成默认的策略配置文件并保存到Project目录
                StrategySetting setting = StrategySetting.GetDefualtSetting(setup);
                //MessageBox.Show("file:" + setting.StrategyFileName + "|"+setup.StrategyFile);
                setting.Save();
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
                
            }
            else
            {
                MessageBox.Show(sm.ErrorText);
            }
        }

    }
}
