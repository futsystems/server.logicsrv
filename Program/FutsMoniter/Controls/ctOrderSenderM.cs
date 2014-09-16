using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;


namespace FutsMoniter
{
    public partial class ctOrderSenderM : UserControl
    {

        IAccountLite _account = null;
        Symbol _symbol = null;
        public event OrderDelegate SendOrderEvent;
        public ctOrderSenderM()
        {
            InitializeComponent();
            Factory.IDataSourceFactory(cboffsetflag).BindDataSource(Utils.GetOffsetCBList());
            Factory.IDataSourceFactory(cbordertype).BindDataSource(Utils.GetOrderTypeCBList());
        }

        /// <summary>
        /// 绑定帐户
        /// </summary>
        /// <param name="acc"></param>
        public void SetAccount(IAccountLite acc)
        {
            _account = acc;
            account.Text = _account.Account;
        }

        public void SetSymbol(Symbol sym)
        {
            _symbol = sym;
            symbol.Text = _symbol.Symbol; 
        }
        private void btnBuy_Click(object sender, EventArgs e)
        {
            try
            {
                genOrder(true);
            }
            catch (Exception ex)
            {
                Globals.Debug("error:" + ex.ToString());
            }
        }

        private void btnSell_Click(object sender, EventArgs e)
        {
            try
            {
                genOrder(false);
            }
            catch (Exception ex)
            {
                Globals.Debug("error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 生成对应的买 卖委托并发送出去
        /// </summary>
        /// <param name="f"></param>
        private void genOrder(bool f)
        {
            if (!ValidAccount()) return;
            if (!validSecurity()) return;
            if (!validSize()) return;
            if (!validPrice()) return;

            //生成对应的委托
            Order work = new OrderImpl(_symbol.Symbol, 0);
            work.Account = _account.Account;
            work.LocalSymbol = _symbol.Symbol;
            work.side = f;
            work.size = Math.Abs((int)size.Value);
            work.OffsetFlag = (QSEnumOffsetFlag)cboffsetflag.SelectedValue;
            if (ismarket)
            {
                work.price = 0;
                work.stopp = 0;
            }
            else
            {
                bool islimit = this.islimit;
                decimal limit = islimit ? (decimal)(price.Value) : 0;
                decimal stop = !islimit ? (decimal)(price.Value) : 0;
                work.price = limit;
                work.stopp = stop;
            }
            work.id = 0;
            SendOrder(work);
        }


        bool ismarket { get { return int.Parse(cbordertype.SelectedValue.ToString())==1; } }
        bool islimit { get { return int.Parse(cbordertype.SelectedValue.ToString()) == 0; } }

        bool ValidAccount()
        {
            if (_account == null)
            {
                fmConfirm.Show("请选择帐户!");
                return false;
            }
            else
                return true;
        }
        /// <summary>
        /// 检查当前是否选中合约
        /// </summary>
        /// <returns></returns>
        bool validSecurity()
        {
            if (_symbol == null)
            {
                fmConfirm.Show("请选择合约！");
                return false;
            }
            else
                return true;
        }

        /// <summary>
        /// 检查当前设置手数
        /// </summary>
        /// <returns></returns>
        bool validSize()
        {
            if ((int)(size.Value) == 0)
            {
                fmConfirm.Show("请设置手数");
                return false;
            }
            else
            {
                return true;
            }
        }

        bool validPrice()
        {
            if (ismarket || (decimal)(price.Value) > 0)
            {
                return true;
            }
            else
            {
                fmConfirm.Show("请设定价格");
                return false;
            }
        }
        


        void SendOrder(Order order)
        {

            if (SendOrderEvent != null)
                SendOrderEvent(order);
        }
    }
}
