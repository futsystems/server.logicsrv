﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace LicManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            btnGenSrv.Click += new EventHandler(btnGenerate_Click);
            btnGenTerminal.Click += new EventHandler(btnGenTerminal_Click);
        }

        void btnGenTerminal_Click(object sender, EventArgs e)
        {
            LicenseGenerator licensegen = new LicenseGenerator();
            licensegen.LoadMasterKeyFromFile("masterkey.key");
            licensegen.AddAdditonalLicenseInformation("deploy", deploy2.Text);
            licensegen.AddAdditonalLicenseInformation("broker_srv",brokerServer.Text);
            licensegen.AddAdditonalLicenseInformation("update_srv",updateServer.Text);
           
            licensegen.Hardware_Enabled = false;
            licensegen.Expiration_Date_Enabled = false;

            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = string.Format("{0}_terminal.license", deploy2.Text);
            // set filters - this can be done in properties as well
            savefile.Filter = "License files (*.license)|*.license";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                licensegen.CreateLicenseFile(savefile.FileName);
            }
        }

        void btnGenerate_Click(object sender, EventArgs e)
        {
            LicenseGenerator licensegen = new LicenseGenerator();
            licensegen.LoadMasterKeyFromFile("masterkey.key");
            licensegen.AddAdditonalLicenseInformation("deploy", deploy.Text);
            licensegen.AddAdditonalLicenseInformation("cnt_counter", ((int)counter_cnt.Value).ToString());
            licensegen.AddAdditonalLicenseInformation("cnt_account", ((int)account_cnt.Value).ToString());
            licensegen.AddAdditonalLicenseInformation("cnt_agent", ((int)agent_cnt.Value).ToString());
            licensegen.AddAdditonalLicenseInformation("enable_api", enableAPI.Checked ? "1" : "0");
            licensegen.AddAdditonalLicenseInformation("enable_app", enableAPP.Checked ? "1" : "0");
            licensegen.AddAdditonalLicenseInformation("expire", expreDate.Value.ToString("yyyyMMdd"));
            
            
            licensegen.Hardware_Enabled = true;
            licensegen.HardwareID = hardwareId.Text;
            licensegen.Expiration_Date_Enabled = true;
            licensegen.ExpirationDate = expreDate.Value;
            licensegen.HardwareID_Board = true;
            licensegen.HardwareID_CPU = true;
            licensegen.HardwareID_MAC = true;

            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = string.Format("{0}_server.license", deploy.Text);
            // set filters - this can be done in properties as well
            savefile.Filter = "License files (*.license)|*.license";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                licensegen.CreateLicenseFile(savefile.FileName);
            }

            
        }
    }
}