using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;
using TradingLib.Mixins.JsonObject;

namespace FutsMoniter
{
    public partial class fmPaySlipAgent : ComponentFactory.Krypton.Toolkit.KryptonForm,IEventBinder
    {
        public fmPaySlipAgent()
        {
            InitializeComponent();

            lbdatetime.Text = DateTime.Now.ToString("yy-MM-dd HH:mm:ss");
            if (Globals.EnvReady)
            {
                lbmanager.Text = Globals.Manager.Login;
            }
            this.Load += new EventHandler(fmPaySlipAgent_Load);
        }

        void fmPaySlipAgent_Load(object sender, EventArgs e)
        {
            Globals.RegIEventHandler(this);
        }

        public void OnInit()
        {

            Globals.LogicEvent.RegisterCallback("MgrExchServer", "QryAgentPaymentInfo", this.OnQryAgentPaymentInfo);
        }

        public void OnDisposed()
        {
            Globals.LogicEvent.UnRegisterCallback("MgrExchServer", "QryAgentPaymentInfo", this.OnQryAgentPaymentInfo);
        }

        string GetFileName()
        {
            return "付-" + DateTime.Now.ToString("yyMMdd") + "-" + lbagentname.Text + "-" + lbref.Text + "-" + lbamount.Text;
        }


        JsonWrapperCashOperation op = null;

        public void SetCashOperation(JsonWrapperCashOperation cashoperation)
        {
            op = cashoperation;
            lbamount.Text = Util.FormatDecimal(cashoperation.Amount);
            lbref.Text = cashoperation.Ref;
            if (Globals.EnvReady)
            {
                Globals.TLClient.ReqQryAgentPaymentInfo(op.mgr_fk);
            }
        }

        void OnQryAgentPaymentInfo(string jsonstr, bool islast)
        {
            //JsonData jd = TradingLib.Mixins.LitJson.JsonMapper.ToObject(jsonstr);
            //int code = int.Parse(jd["Code"].ToString());
            JsonWrapperAgentPaymentInfo info = MoniterUtils.ParseJsonResponse<JsonWrapperAgentPaymentInfo>(jsonstr);
            if (info != null)
            {
                //JsonWrapperAgentPaymentInfo info = TradingLib.Mixins.LitJson.JsonMapper.ToObject<JsonWrapperAgentPaymentInfo>(jd["Playload"].ToJson());
                //if (info != null)
                {
                    GotJsonWrapperAgentPaymentInfo(info);
                }
            }
            else//如果没有配资服
            {

            }
        }

        delegate void del1(JsonWrapperAgentPaymentInfo info);
        void GotJsonWrapperAgentPaymentInfo(JsonWrapperAgentPaymentInfo info)
        {
            if (InvokeRequired)
            {
                Invoke(new del1(GotJsonWrapperAgentPaymentInfo), new object[] { info });
            }
            else
            {
                lbmgrid.Text = info.BaseMGRFK.ToString();
                lbagentname.Text = info.Name;
                lbmobile.Text = info.Mobile;
                lbqq.Text = info.QQ;

                if (info.BankAccount != null)
                {
                    lbname.Text = info.BankAccount.Name;
                    lbbankac.Text = info.BankAccount.Bank_AC;
                    lbbankname.Text = info.BankAccount.Bank.Name;
                    lbbankbranch.Text = info.BankAccount.Branch;
                }


            }
        }
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!SaveSlip())
            {
                return;
            }
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        /// <summary>
        /// 保存支付申请单
        /// </summary>
        /// <returns></returns>
        bool SaveSlip()
        {
            Bitmap bit = new Bitmap(slip.Width, slip.Height);//实例化一个和窗体一样大的bitmap
            Graphics g = Graphics.FromImage(bit);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;//质量设为最高
            //g.CopyFromScreen(this.Left, this.Top, 0, 0, new Size(this.Width, this.Height));//保存整个窗体为图片
            g.CopyFromScreen(slip.PointToScreen(Point.Empty), Point.Empty, slip.Size);//只保存某个控件（这里是panel游戏区）

            string filename = GetFileName() + ".png";
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Filter = "png (*.png)|*.png";
            saveFileDialog.FileName = filename;
            if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return false;
            }

            if (saveFileDialog.FileName.Equals(String.Empty))
            {
                fmConfirm.Show("请填写输出文件名");
                return false;
            }
            bit.Save(saveFileDialog.FileName);//默认保存格式为PNG，保存成jpg格式质量不是很好
            return true;
        }
    }
}
