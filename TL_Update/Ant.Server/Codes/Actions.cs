using System;
using System.Collections.Generic;
using System.Text;

namespace Ant.Server.Codes
{
    class Actions:Beetle.IMessageHandler
    {
        void debug(string msg)
        {
            Utils.Debug("Actions:" + msg);
        }
        public Smark.Core.AsyncDelegate<Smark.Core.Log.LogType, string> Output
        {
            get;
            set;
        }
        public void Sing(Beetle.TcpChannel channel, Component.Protocols.Sign e)
        {
            Component.Protocols.SingResponse response = new Component.Protocols.SingResponse();
            if (Utils.Ras.Verify(e.Name, e.Data))
            {
                channel.Status = Beetle.ChannelStatus.Security;
                Output(Smark.Core.Log.LogType.None, string.Format("{0} Sing ok!", channel.EndPoint));
            }
            else
            {

                response.Status = "Sing error!";
                Output(Smark.Core.Log.LogType.Warning, string.Format("{0} Sing error!", channel.EndPoint));
            }
            channel.Send(response);
            if (!string.IsNullOrEmpty(response.Status))
            {
                System.Threading.Thread.Sleep(2000);
                channel.Dispose();
            }
        }
        public const string CURRENT_POSTFILE="_CURRENTPOSTFILE";
        public const string CURRENT_GETFILE = "_CURRENTGETFILE";
        public const string CURRENT_GETFILENAME = "_CURRENTGETFILENAME";
        public const string CURRENT_UNIT = "_UNIT";
        public const string CURRENT_APPNAME = "_APPNAME";
        public void Post(Beetle.TcpChannel channel, Component.Protocols.Post e)
        {
            debug("Post: filename:" + e.FileName + " packages:" + e.Packages + " packagesize:" + e.PackageSize +" unit:"+e.Unit +" appname:"+e.AppName);
            
            Component.Protocols.PostResponse response = new Component.Protocols.PostResponse();
            try
            {
                if (channel.Status == Beetle.ChannelStatus.Security)
                {
                    channel[CURRENT_POSTFILE] = e;
                    string dir = System.IO.Path.GetDirectoryName(e.FileName);
                    //生成当前操作目录
                    dir = System.IO.Path.Combine(new string[] { "data", e.Unit, e.AppName, dir });
                    if (!string.IsNullOrEmpty(dir))
                    {
                        if (!System.IO.Directory.Exists(Utils.GetFileFullName(dir)))
                        {
                            System.IO.Directory.CreateDirectory(Utils.GetFileFullName(dir));
                        }
                    }
                    e.FileName = Utils.GetFileFullName(System.IO.Path.Combine(new string[] {"data", e.Unit, e.AppName,e.FileName}));
                    debug("Post file name:"+e.FileName);
                    Component.FileUtils.CreateFile(e.FileName+".up", e.Size);
                    Output(Smark.Core.Log.LogType.None, string.Format("{0} upload {1}", channel.EndPoint, System.IO.Path.GetFileName(e.FileName)));


                }
                else
                {
                    response.Status = "无效签名!";
                    Output(Smark.Core.Log.LogType.Warning, string.Format("{0} upload error {1}", channel.EndPoint, response.Status));
                }
            }
            catch (Exception e_)
            {
                response.Status = e_.Message;
            }
            channel.Send(response);
        }
        public void PostPackage(Beetle.TcpChannel channel, Component.Protocols.PostPackage e)
        {
            debug("PostPackage");
            Component.Protocols.PostPackageResponse response = new Component.Protocols.PostPackageResponse();
            response.Index = e.Index;
            Component.Protocols.Post info = (Component.Protocols.Post)channel[CURRENT_POSTFILE];
            try
            {
                if (info != null)
                {
                    if (e.Check())
                    {
                        Component.FileUtils.FileWrite(info.FileName+".up", e.Index, info.PackageSize, e.Data);
                        if (e.Index + 1 == info.Packages)
                        {
                            if (System.IO.File.Exists(info.FileName))
                                System.IO.File.Delete(info.FileName);
                            System.IO.File.Move(info.FileName + ".up", info.FileName);
                            Output(Smark.Core.Log.LogType.None, string.Format("{0} upload {1} Completed", channel.EndPoint, System.IO.Path.GetFileName(info.FileName)));
                        }
                    }
                    else
                    {
                        response.Status = "文件校验错误!";
                    }
                }
                else
                {
                    response.Status = "不存在文件信息!";
                }
            }
            catch (Exception e_)
            {
                response.Status = e_.Message;
            }
            if(!string.IsNullOrEmpty(response.Status))
                Output(Smark.Core.Log.LogType.Warning, string.Format("{0} upload error {1}", channel.EndPoint, response.Status));
            channel.Send(response);
        }

        /// <summary>
        /// 获得更新信息
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="e"></param>
        public void GetUpdateInfo(Beetle.TcpChannel channel, Component.Protocols.GetUpdateInfo e)
        {
            debug("GetUpdateInfo:" + e.ID.ToString() +" unit:"+e.Unit +" app:"+e.AppName);
            Component.Protocols.GetUpdateInfoResponse response = new Component.Protocols.GetUpdateInfoResponse();
            string fp = System.IO.Path.Combine(new string[] {"data", e.Unit, e.AppName, Utils.UPDATE_FILE });
            string file = Utils.GetFileFullName(fp);
            debug("update file fullname:" + file);
            if(System.IO.File.Exists(file))
            {
                using (System.IO.StreamReader reader = new System.IO.StreamReader(file, Encoding.UTF8))
                {
                    response.Info = reader.ReadToEnd();
                }
                Output(Smark.Core.Log.LogType.None, string.Format("{0} get update info!", channel.EndPoint));
            }
            else{
               response.Status="不存在更新信息!";
               Output(Smark.Core.Log.LogType.Warning, string.Format("{0} get update info error {1}!", channel.EndPoint, response.Status));
            }
            channel.Send(response);
        }

        /// <summary>
        /// 在update_info.xml中获得对应的文件名称，然后通过该函数查询文件信息
        /// 步骤是查询一个文件，获得一个文件
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="e"></param>
        public void Get(Beetle.TcpChannel channel, Component.Protocols.Get e)
        {
            debug("Get:" + e.ID.ToString() +" filename:"+e.FileName +" unit:"+e.Unit +" appname:"+e.AppName);

            Component.Protocols.GetResponse response = new Component.Protocols.GetResponse();
            string[] plist = new string[] { "data", e.Unit, e.AppName, e.FileName };
            string fp = System.IO.Path.Combine(plist);
            debug("********:"+fp);
            string filename = Utils.GetFileFullName(fp);
            debug("file fullname:" + filename);
            if (System.IO.File.Exists(filename))
            {
                System.IO.FileInfo info = new System.IO.FileInfo(filename);
                response.Size = info.Length;
                response.Packages = Component.FileUtils.GetFilePackages(response.Size);
                response.PackageSize = Component.FileUtils.PackageSize;
                channel[CURRENT_GETFILE] = response;
                channel[CURRENT_GETFILENAME] = e.FileName;
                channel[CURRENT_UNIT] = e.Unit;
                channel[CURRENT_APPNAME] = e.AppName;
                Output(Smark.Core.Log.LogType.None, string.Format("{0} get file {1}!", channel.EndPoint, e.FileName));
            }
            else
            {
                response.Status = "文件不存在!";
                Output(Smark.Core.Log.LogType.Warning, string.Format("{0} get file {1} not found!", channel.EndPoint, e.FileName));
            }
            channel.Send(response);

        }
        public void GetPackage(Beetle.TcpChannel channel, Component.Protocols.GetPackage e)
        {
            debug("GetPackage: idx:" + e.Index.ToString());

            Component.Protocols.GetPackageResponse response = new Component.Protocols.GetPackageResponse();
            response.Index = e.Index;
            Component.Protocols.GetResponse getfile = (Component.Protocols.GetResponse)channel[CURRENT_GETFILE];
            string name = (string)channel[CURRENT_GETFILENAME];
            string unit = (string)channel[CURRENT_UNIT];
            string appname = (string)channel[CURRENT_APPNAME];
            string fn = System.IO.Path.Combine(new string[] { "data",unit,appname,name });
            //读取文件,并将文件内容放入response.Data
            if (getfile != null)
            {
                try
                {
                    string filename = Utils.GetFileFullName(fn);
                    response.Data = Component.FileUtils.FileRead(filename, e.Index, getfile.PackageSize);
                    
                }
                catch (Exception e_)
                {
                    response.Status = e_.Message;

                }
            }
            else
            {
                response.Status = "文件不存在!";
            }
            if(!string.IsNullOrEmpty(response.Status))
                Output(Smark.Core.Log.LogType.Warning, string.Format("{0} get file package error {1}!", channel.EndPoint, response.Status));
            channel.Send(response);
        }
        public const string LASTTIME_TAG = "_LASTTIME";
        public void ProcessMessage(Beetle.TcpChannel channel, Beetle.MessageHandlerArgs message)
        {
            //debug("ProcessMessage" + message.Message);
            channel[LASTTIME_TAG] = DateTime.Now;
        }
        public DateTime LastTime(Beetle.TcpChannel channel)
        {
            return (DateTime)channel[LASTTIME_TAG];
        }
    }
}
