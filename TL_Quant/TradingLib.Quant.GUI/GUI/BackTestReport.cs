using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using ComponentFactory.Krypton.Navigator;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;


namespace TradingLib.Quant.GUI
{
    public partial class BackTestReport : KryptonForm
    {
        BackTestData _result;

        public BackTestReport()
        {
            InitializeComponent();
        }

        public void ShowBackTestReport(BackTestData result)
        {
            //QuantGlobals.GDebug("生成回测报告");
            _result = result;
            double startcapital = result.StrategyData.StartingCapital;

            this.tabholder.Pages.Clear();

            PluginFinder finder = new PluginFinder(false);
            finder.SetSearchPath(PluginGlobals.PluginDirectory);

            //根据全局ID获得plugin
            string id = "{2A331CB4-C621-2848-60AE-8B79DF20EA08}";
            GenBackTestPage(id, finder);
            id = "{2A331CB4-C441-2848-70AE-8B79DF20UY08}";
            GenBackTestPage(id, finder);

            id = "{E9717E89-AA9E-094F-10C1-77A88FA41F71}";
            GenBackTestPage(id, finder);

            id = "{1565CB41-3EC0-E634-8C22-DC008D8B4B6B}";
            GenBackTestPage(id, finder);

            id = "{07C7A930-B7D1-5E38-467F-4C7C8034AD0B}";
            GenBackTestPage(id, finder);

            
        }

        void GenBackTestPage(string id,PluginFinder finder)
        {
            IBackTestReportPlugin plugin;
            SerializableDictionary<string, string> settingsDictionary=null;
            Control control;
            BackTestReportAttribute attribute = PluginHelper.LoadBackTestReport(id);
            //SystemResultsSetupManager manager = new SystemResultsSetupManager();

            //注意插件所使用的而外dll需要复制到程序目录 否则会造成插件无法加载
            List<BackTestReportAttribute> backtestpluginlist = finder.LoadBackTestReportList();
            //QuantGlobals.GDebug("Find BackTestPlugin num:" + backtestpluginlist.Count.ToString());
            //QuantGlobals.GDebug();
            //MessageBox.Show(_result.StrategyResults.Data.StratgyHistory.StrategyStatistics.ToString());
            foreach (BackTestReportAttribute attr in backtestpluginlist)
            {
                QuantGlobals.GDebug(attr.Name + " " + attr.Author + " " + attr.CompanyName + " " + attr.Id);
            }

            if (attribute != null)
            {
                QuantGlobals.GDebug("找到了对应的attr");
                if (finder == null)
                {
                    finder = new PluginFinder(false);
                    finder.SetSearchPath(QuantGlobals.PluginDirectory);

                }
                plugin = finder.ConstructBackTestRepoert(id, false);
                if (plugin != null)
                {
                    //SystemResultsSetup settings;
                    //settings = manager.GetSettings(attribute.Id);
                    //settingsDictionary = null;
                    //if (settings == null)
                    //{
                    //    settingsDictionary = new SerializableDictionary<string, string>();
                    //    goto Label_0093;
                    //}


                }
                else
                {
                    return;

                }

                QuantGlobals.GDebug("创建报告插件...");
                control = plugin.CreateReport(this._result,settingsDictionary);//

                string name = attribute.Name;
                KryptonPage page = new KryptonPage();
                page.TextTitle = name;
                page.Text = name;
                //page.ImageSmall = image;
                KryptonPanel p = new KryptonPanel();

                page.Controls.Add(p);

                //page.ToolTipImage = image;
                //page.ToolTipTitle = title;
                p.Dock = DockStyle.Fill;
                p.Controls.Add(control);
                control.Dock = DockStyle.Fill;
                tabholder.Pages.Add(page);
                //kryptonNavigator1.Pages.Add(page);
                //displayform.Show();
                //return page;


            }

            //return null;

        }
    }
}
