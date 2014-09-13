using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Reflection;
using System.Configuration;
using System.IO;
using TradingLib;
using TradingLib.Common;

namespace Broker.CTP
{
	public partial class FormLoginTrade : Form
	{

        ConfigHelper cfg;// = new ConfigHelper(CfgConstBrokerCTP.XMLFN);
		public FormLoginTrade(ConfigHelper _cfg)
		{
            cfg = _cfg;
			InitializeComponent();
            int savepwd = Convert.ToInt16(cfg.GetConfig(CfgConstBrokerCTP.SavePwd));
            savePWDCheckBox.Checked = savepwd == 1 ? true : false;
		}

		private void UserLogin_Load(object sender, EventArgs e)
		{
            
			//从设置里读取服务器列表
            string[] servers = cfg.GetConfig(CfgConstBrokerCTP.Servers).Split(',');
            int idx = Convert.ToInt16(cfg.GetConfig(CfgConstBrokerCTP.ServerIndex));

			for (int i = 0; i < servers.Length; i++)//Properties.Settings.Default.Servers.Count; i++)
			{
				string server = servers[i];
				this.cbServer.Items.Add(server.Split('|')[0]);
				//for (int j = server.Split('|').Length; j < 8; j++)
				//	Properties.Settings.Default.Servers[i] = Properties.Settings.Default.Servers[i] + "|";
			}
			if (this.cbServer.Items.Count > 0)
				this.cbServer.SelectedIndex = idx;	//默认设置
           
		}

		private void FormLoginTrade_FormClosed(object sender, FormClosedEventArgs e)
		{
			//Properties.Settings.Default.Save();
            //if(savePWDCheckBox.va)
		}

		private void buttonSetServer_Click(object sender, EventArgs e)
		{
           
			using (FormServers fs = new FormServers())
			{
				fs.dataGridViewServers.Columns.Add("Broker", "经纪公司");
				fs.dataGridViewServers.Columns.Add("BrokerID", "经纪代码");
				//fs.dataGridViewServers.Columns.Add("MDAddr", "行情服务器地址");
				//fs.dataGridViewServers.Columns.Add("MDPort", "端口");
				fs.dataGridViewServers.Columns.Add("TradeAddr", "交易服务器地址");
				fs.dataGridViewServers.Columns.Add("TradePort", "端口");
				fs.dataGridViewServers.Columns.Add("Invorter", "用户");
				fs.dataGridViewServers.Columns["Invorter"].Visible = false;
				fs.dataGridViewServers.Columns.Add("PWD", "密码");
				fs.dataGridViewServers.Columns["PWD"].Visible = false;
				fs.dataGridViewServers.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				fs.dataGridViewServers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
				fs.dataGridViewServers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
				fs.dataGridViewServers.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;	//不换行
				//fs.dataGridViewServers.Columns["Broker"].Width = 60;
				//fs.dataGridViewServers.Columns["BrokerID"].Width = 60;
				//fs.dataGridViewServers.Columns["MDAddr"].Width = 60;
				//fs.dataGridViewServers.Columns["MDPort"].Width = 60;
				//fs.dataGridViewServers.Columns["TradeAddr"].Width = 60;
				//fs.dataGridViewServers.Columns["TradePort"].Width = 60;
				//从设置里读取服务器列表
                string[] servers = cfg.GetConfig(CfgConstBrokerCTP.Servers).Split(',');
				for (int i = 0; i < servers.Length; i++)
				{
					try
					{
						fs.dataGridViewServers.Rows.Add(servers[i].Split('|'));//
					}
					catch { }
				}

                List<string> srvlist = new List<string>();
				if (fs.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					//添加到设置里
					//Properties.Settings.Default.Servers.Clear();
					this.cbServer.Items.Clear();
					for (int i = 0; i < fs.dataGridViewServers.Rows.Count; i++)
					{
						string server = string.Empty;	//保存服务器信息
						for(int j = 0 ;j<fs.dataGridViewServers.Columns.Count;j++)
						{
							if (fs.dataGridViewServers.Rows[i].Cells[j].Value != null)
							{
								server += fs.dataGridViewServers.Rows[i].Cells[j].Value + "|";
							}
						}
						if (!string.IsNullOrWhiteSpace(server))
						{
							server += "|";
							//server.Remove(server.Length - 1);	//去掉尾"|"
							this.cbServer.Items.Add((string)fs.dataGridViewServers.Rows[i].Cells[0].Value);	//加到列表中
							//Properties.Settings.Default.Servers.Add(server);
                            srvlist.Add(server);
						}
					}
					if (this.cbServer.Items.Count > 0)
						this.cbServer.SelectedIndex = 0;
                    cfg.SetConfig(CfgConstBrokerCTP.Servers, string.Join(",", srvlist.ToArray()));
				}
			}
            
		}

		private void cbServer_SelectedIndexChanged(object sender, EventArgs e)
		{
            cfg.SetConfig(CfgConstBrokerCTP.ServerIndex, this.cbServer.SelectedIndex.ToString());
            string[] servers = cfg.GetConfig(CfgConstBrokerCTP.Servers).Split(',');
            int savepwd = Convert.ToInt16(cfg.GetConfig(CfgConstBrokerCTP.SavePwd));

			//Properties.Settings.Default.ServerIndex = this.cbServer.SelectedIndex;	//设置
            this.tbUserID.Text = servers[this.cbServer.SelectedIndex].Split('|')[4];//Properties.Settings.Default.Servers[this.cbServer.SelectedIndex].Split('|')[6];

            if (savepwd == 1)
                this.tbPassword.Text = servers[this.cbServer.SelectedIndex].Split('|')[5];
            
		}

		private void button1_Click(object sender, EventArgs e)
		{
            /*
			Properties.Settings.Default.Servers.Clear();
			Properties.Settings.Default.ServerIndex = 0;
             * */
		}

        

      
	}
}
