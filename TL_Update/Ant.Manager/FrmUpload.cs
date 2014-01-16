using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ant.Component;
using Beetle;
namespace Ant.Manager
{
    public partial class FrmUpload : Form
    {
        public FrmUpload()
        {
            InitializeComponent();
        }
        public Beetle.TcpSyncChannel<Ant.Component.Protocols.HeadSizePackage> Client = new Beetle.TcpSyncChannel<Component.Protocols.HeadSizePackage>();
        
        private Component.Protocols.SingResponse mSingResponse;
        private int mCount;
        private string mError = "";
        public Queue<FileItem> UpdateItems
        {
            get;
            set;
        }

      
        private void FrmUpload_Load(object sender, EventArgs e)
        {
            mCount = UpdateItems.Count;
            Utils.FileProgress.Draw("", 0, 100);
            Utils.TotalProgress.Draw(string.Format("总进度 {0}/{1}", 0, mCount), 0, mCount);
            imgFile.Image = Utils.FileProgress.Image;
            imtTotal.Image = Utils.TotalProgress.Image ;
            cbAppName.Items.Add("Moniter");
            cbAppName.SelectedIndex = 0;
            if (!string.IsNullOrEmpty(Utils.IPAddress))
            {
                txtIPAddress.Text = Utils.IPAddress;
            }
            else
            {
                txtIPAddress.Text = "127.0.0.1";
            }
            if (!string.IsNullOrEmpty(Utils.Port))
                txtPort.Text = Utils.Port;
           

        }
        protected override void OnClosed(EventArgs e)
        {
            if (Client != null)
                Client.Dispose();
            base.OnClosed(e);
        }
        private void cmdClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void cmdConnection_Click(object sender, EventArgs e)
        {
            try
            {
                if (Client != null)
                    Client.Dispose();
                Client = new TcpSyncChannel<Component.Protocols.HeadSizePackage>();
                Client.Connect(txtIPAddress.Text, int.Parse(txtPort.Text));
               
                Component.Protocols.Sign sing = new Component.Protocols.Sign();
                sing.Name = "AntManager";
                sing.Data = Utils.Ras.Sign(sing.Name);
                Component.Protocols.SingResponse message = (Component.Protocols.SingResponse)Client.Send(sing);

                if (!string.IsNullOrEmpty(message.Status))
                    throw new Exception(message.Status);
                cmdConnection.Enabled = false;
                cmdUpload.Enabled = true;
                Utils.IPAddress = txtIPAddress.Text;
                Utils.Port = txtPort.Text;
                
            }
            catch (Exception e_)
            {
                MessageBox.Show(this, e_.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }   
        private void cmdClose_Click_1(object sender, EventArgs e)
        {
            Close();
        }
        private void cmdUpload_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUnit.Text))
            {
                MessageBox.Show("请输入更新程序域");
                return;
            }
            
            System.Threading.ThreadPool.QueueUserWorkItem(OnPostFile);
            cmdUpload.Enabled = false;
        }
        private void OnPostFile(object state)
        {
            mError = null;
            try
            {
                if (UpdateItems.Count > 0)
                {
                    while (UpdateItems.Count > 0)
                    {
                        FileItem item = UpdateItems.Dequeue();
                        System.IO.FileInfo info = new System.IO.FileInfo(Utils.Path + item.File);
                        item.Size = info.Length;
                        item.PackageSize = Component.FileUtils.PackageSize;
                        item.Packages = Component.FileUtils.GetFilePackages(item.Size);
                        //发送post请求 用于告知上传文件的具体信息 这里需要指定对应的存放目录
                        Component.Protocols.Post post = new Component.Protocols.Post();
                        post.FileName = item.File;
                        post.Packages = item.Packages;
                        post.PackageSize = item.PackageSize;
                        post.Size = item.Size;
                        post.Unit = txtUnit.Text;
                        post.AppName = cbAppName.SelectedItem.ToString();
                        
                        Component.Protocols.PostResponse result = (Component.Protocols.PostResponse)Client.Send(post);
                        Utils.FileProgress.Draw(post.FileName, 0, post.Packages);
                        Utils.TotalProgress.Draw(string.Format("总进度 {0}/{1}", mCount - UpdateItems.Count , mCount), mCount - UpdateItems.Count , mCount);
                        if (string.IsNullOrEmpty(result.Status))
                        {
                            Component.Protocols.PostPackage postpackage = new Component.Protocols.PostPackage();
                            postpackage.Index = 0;
                            postpackage.Data = Component.FileUtils.FileRead(Utils.Path + item.File,
                                postpackage.Index, item.PackageSize);
                            Utils.FileProgress.Draw(item.File, postpackage.Index + 1, item.Packages);
                            Component.Protocols.PostPackageResponse ppr = (Component.Protocols.PostPackageResponse)Client.Send(postpackage);
                            while (ppr.Index + 1 < item.Packages)
                            {
                                postpackage = new Component.Protocols.PostPackage();
                                postpackage.Index = ppr.Index + 1;
                                postpackage.Data = Component.FileUtils.FileRead(Utils.Path + item.File,
                                     postpackage.Index, item.PackageSize);
                                Utils.FileProgress.Draw(item.File, postpackage.Index + 1, item.Packages);
                                ppr = (Component.Protocols.PostPackageResponse)Client.Send(postpackage);
                            }
                        }
                        else
                        {
                            mError = result.Status;
                        }


                    }
                }
               
            }
            catch (Exception e_)
            {
                mError = e_.Message;
                if (Client.IsDisplsed)
                {
                    cmdConnection.Enabled = true;
                    cmdUpload.Enabled = false;
                }
            }
            finally
            {
                
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (Utils.FileProgress)
            {
                imgFile.Refresh();
            }
            lock (Utils.TotalProgress)
            {
                imtTotal.Refresh();
            }
            txtError.Text = mError;
        }
    }
}
