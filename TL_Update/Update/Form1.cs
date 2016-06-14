using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ant.Component.Protocols;


namespace Update
{
    public partial class Form1 : Form
    {
        private Beetle.TcpSyncChannel<Ant.Component.Protocols.HeadSizePackage> mChannel;


        private Queue<Ant.Component.FileItem> mDownloads = null;
        private string tmpFolder = "";
        private int mUpateCount = 0;

        public string[] Arges
        {
            get;
            set;
        }
        private Ant.Component.UpdateInfo mLocalInfo = new Ant.Component.UpdateInfo();
        private Ant.Component.UpdateInfo mUpdateInfo = new Ant.Component.UpdateInfo();
        private IList<Ant.Component.FileItem> mCopyFiles = new List<Ant.Component.FileItem>(100);

        public Form1()
        {
            try
            {
                InitializeComponent();
                //667411488000000000
                //MessageBox.Show(new DateTime(667411488000000000).ToString());
                Beetle.TcpUtils.Setup("beetle");

                tmpFolder = "tmp" + DateTime.Now.ToString("yyyyMMdd"); ;
                Utils.FileProgress.Draw("", 0, 100);
                Utils.TotalProgress.Draw("更新进度 ", 0, 10);
                imgFile.Image = Utils.FileProgress.Image;
                imtTotal.Image = Utils.TotalProgress.Image;
                Utils.LoadINI();
                mLocalInfo.Load(Utils.GetFileFullName(Utils.UPDATE_FILE));
                //利用线程池进行工作项目的安排
                System.Threading.ThreadPool.QueueUserWorkItem(Connect);

                this.Load += new EventHandler(Form1_Load);
            }
            catch (Exception e_)
            {
                MessageBox.Show(this, e_.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Form1_Load(object sender, EventArgs e)
        {
            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);
        }

        void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Utils.AppName))
                System.Diagnostics.Process.Start(Utils.AppName);
        }



        private void ChangeStatus(Color color, string text)
        {
            Invoke(new Smark.Core.AsyncDelegate<Color, string>((c, s) =>
            {
                txtStatus.ForeColor = c;
                txtStatus.Text = s;
            }), color, text);
        }
        protected void Connect(object state)
        {
            try
            {
                if (Arges.Length > 0)
                    Utils.IPAddress = Arges[0];
                if (Arges.Length > 1)
                    Utils.Port = Arges[1];
                if (Arges.Length > 2)
                    Utils.AppName = Arges[2];
                if (Arges.Length > 3)
                    Utils.AutoClose = Arges[3];
                if (string.IsNullOrEmpty(Utils.IPAddress))
                {
                    throw new Exception("不存在服务器地址信息!");
                }
                if (string.IsNullOrEmpty(Utils.Port))
                {
                    throw new Exception("不存在服务器地址信息!");
                }

                mChannel = new Beetle.TcpSyncChannel<HeadSizePackage>();
                mChannel.Connect(Utils.IPAddress, int.Parse(Utils.Port));

                //1.通过GetUpdateInfo获得对应的更新回报 更新回报就是对应的update_info.xml
                Ant.Component.Protocols.GetUpdateInfo update = new Ant.Component.Protocols.GetUpdateInfo();
                update.Unit = Utils.Unit;
                update.AppName = Utils.AppNameUpdate;

                GetUpdateInfoResponse getinfo = (GetUpdateInfoResponse)mChannel.Send(update);
                ChangeStatus(Color.Black, "获取文件更新信息...");
                if (string.IsNullOrEmpty(getinfo.Status))//如果有状态信息 就显示状态
                {
                    mUpdateInfo.LoadXml(getinfo.Info);//加载更新xml
                    mDownloads = mLocalInfo.Comparable(mUpdateInfo);//比较更新 xml 用于获得最新的下载列表
                    if (mDownloads.Count == 0)
                    {
                        ChangeStatus(Color.Black, "当前是最新版本!"); //没有下载内容则为最新班额不能
                    }
                    else
                    {
                        ChangeStatus(Color.Black, "更新中...");
                        mUpateCount = mDownloads.Count;

                        Utils.TotalProgress.Draw(string.Format("更新进度 {0}/{1}", 0, mUpateCount), 0, mUpateCount);

                        string tmpf = Utils.GetFileFullName(tmpFolder);
                        if (!System.IO.Directory.Exists(tmpf))
                            System.IO.Directory.CreateDirectory(tmpf);

                        while (mDownloads.Count > 0)
                        {
                            Ant.Component.FileItem item = mDownloads.Dequeue();//出队一个下载内容

                            Ant.Component.Protocols.Get get = new Ant.Component.Protocols.Get();
                            get.FileName = item.File;//System.IO.Path.Combine(new string[] { Utils.Unit, Utils.AppNameUpdate, item.File });
                            get.Unit = Utils.Unit;
                            get.AppName = Utils.AppNameUpdate;

                            GetResponse getresponse = (GetResponse)mChannel.Send(get);//通过Get获得该文件具体信息

                            if (string.IsNullOrEmpty(getresponse.Status))
                            {
                                //获得文件信息正常
                                string path = System.IO.Path.GetDirectoryName(item.File);
                                if (!string.IsNullOrEmpty(path))
                                {
                                    string createFolder = Utils.GetFileFullName(tmpFolder + "\\" + path);
                                    if (!System.IO.Directory.Exists(createFolder))
                                        System.IO.Directory.CreateDirectory(createFolder);

                                }
                                Ant.Component.FileUtils.CreateFile(Utils.GetFileFullName(tmpFolder + "\\" + item.File + ".update"), getresponse.Size);

                                //获得具体的文件
                                Ant.Component.Protocols.GetPackage getpackage = new Ant.Component.Protocols.GetPackage();
                                int page = 0;
                                getpackage.Index = 0;
                                GetPackageResponse gpr = (GetPackageResponse)mChannel.Send(getpackage);
                                page = gpr.Index + 1;

                                Utils.FileProgress.Draw(item.File, page, getresponse.Packages);
                                Ant.Component.FileUtils.FileWrite(Utils.GetFileFullName(tmpFolder + "\\" + item.File + ".update"), gpr.Index, getresponse.PackageSize, gpr.Data);
                                //实现分页下载
                                while (page < getresponse.Packages)
                                {
                                    if (string.IsNullOrEmpty(gpr.Status))
                                    {

                                        getpackage = new Ant.Component.Protocols.GetPackage();
                                        getpackage.Index = page;
                                        gpr = (GetPackageResponse)mChannel.Send(getpackage);
                                        page = gpr.Index + 1;
                                        Utils.FileProgress.Draw(item.File, page, getresponse.Packages);
                                        Ant.Component.FileUtils.FileWrite(Utils.GetFileFullName(tmpFolder + "\\" + item.File + ".update"), gpr.Index, getresponse.PackageSize, gpr.Data);
                                    }
                                    else
                                    {
                                        ChangeStatus(Color.Red, getresponse.Status);
                                        return;
                                    }
                                }
                                mCopyFiles.Add(item);
                            }
                            else
                            {
                                ChangeStatus(Color.Red, getresponse.Status);
                                return; ;
                            }
                            Utils.TotalProgress.Draw(string.Format("更新进度 {0}/{1}", mUpateCount - mDownloads.Count, mUpateCount), mUpateCount - mDownloads.Count, mUpateCount);
                        }
                        CopyTempFiles();
                    }
                }
                else
                {
                    ChangeStatus(Color.Red, getinfo.Status);
                    return;
                }

            }
            catch (Exception e_)
            {
                ChangeStatus(Color.Red, e_.Message);
            }
        }
        private void OnError(object sender, Beetle.ChannelErrorEventArgs e_)
        {
            ChangeStatus(Color.Red, "程序处理错误:" + e_.Exception.Message);
        }



        private void CopyTempFiles()
        {
            ChangeStatus(Color.Black, "下载完成，更新本地文件...");
            foreach (Ant.Component.FileItem item in mCopyFiles)
            {
                string filename = Utils.GetFileFullName(item.File);
                if (System.IO.File.Exists(filename))
                    System.IO.File.Delete(filename);
                string path = System.IO.Path.GetDirectoryName(item.File);
                if (!string.IsNullOrEmpty(path))
                {
                    string createFolder = Utils.GetFileFullName(path);
                    if (!System.IO.Directory.Exists(createFolder))
                        System.IO.Directory.CreateDirectory(createFolder);

                }
                System.IO.File.Move(Utils.GetFileFullName(tmpFolder + "\\" + item.File + ".update"),
                    filename);
            }
            mLocalInfo.Save(Utils.GetFileFullName(Utils.UPDATE_FILE));
            System.IO.Directory.Delete(Utils.GetFileFullName(tmpFolder), true);
            ChangeStatus(Color.Black, "更新完成!");
            bool autoclose;
            System.Threading.Thread.Sleep(1000);
            if (bool.TryParse(Utils.AutoClose, out autoclose))
            {
                if (autoclose)
                {
                    Invoke(new Action<Form1>(f =>
                    {
                        f.Close();
                    }), this);
                }
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
        }



        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("explorer", "http://www.ikende.com/");
        }
    }
}
