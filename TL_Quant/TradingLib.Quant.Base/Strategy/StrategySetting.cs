using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Windows.Forms;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 策略设置,用于保存成XML格式的配置文件
    /// sharedSystemRunData是通过StrategySetting与界面的配置数据组合成的策略运行数据
    /// </summary>
    [Serializable]
    public class StrategySetting//:IXmlSerializable
    {
        public StrategySetting()
        { 
            
        }

        public static StrategySetting Load(StrategySetup setup)
        {
            try
            {
                string file = Path.Combine(new string[] { BaseGlobals.ProjectConfigDirectory, setup.FriendlyName, "config.xml" });
                string path = Path.Combine(new string[] { BaseGlobals.ProjectConfigDirectory, setup.FriendlyName });

                //如果文件夹不存在则创建文件夹
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                //如果不存在则生成默认strategysetting
                if (!File.Exists(file))
                {
                    TextWriter textWriter = new StreamWriter(File.Create(file));
                    new XmlSerializer(typeof(StrategySetting)).Serialize(textWriter, StrategySetting.GetDefualtSetting(setup));
                    textWriter.Close();
                }

                TextReader textReader = new StreamReader(file);
                XmlSerializer serializer = new XmlSerializer(typeof(StrategySetting));
                StrategySetting setting = (StrategySetting)serializer.Deserialize(textReader);
                textReader.Close();
                //QuantGlobals.GDebug(setting.AssemblyName);
                return setting;
            }
            catch (Exception ex)
            {
                return null;
            }

            

        
        }
        public void Save()
        {
            TextWriter textWriter = null;

            try
            {
                //xml配置文件默认生成方式 project/friendlyname/config.xml
                string file = Path.Combine(new string[] { BaseGlobals.ProjectConfigDirectory, FriendlyName, "config.xml" });
                string path = Path.Combine(new string[] { BaseGlobals.ProjectConfigDirectory, FriendlyName });
                //如果文件夹不存在则创建文件夹
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!File.Exists(file))
                {
                    textWriter = new StreamWriter(File.Create(file));
                   
                }
                else
                {
                    textWriter = new StreamWriter(file);
                }
                    
                //QuantGlobals.GDebug("保存策略配置到文件:" + file);
                
                
                new XmlSerializer(typeof(StrategySetting)).Serialize(textWriter,this);
                textWriter.Close();
            }
            catch (Exception exception)
            {
                //QuantGlobals.GDebug("error:" + exception.ToString());
            }
            finally
            {
                if (textWriter != null)
                {
                    textWriter.Close();
                }
            }
        }
        public static StrategySetting GetDefualtSetting(StrategySetup setup)
        {
            StrategySetting setting = new StrategySetting();
            setting.FriendlyName = setup.FriendlyName;//策略配置友名
            setting.AssemblyName = setup.StrategyClassName;//策略类名
            setting.StrategyRunDataFile = "";//QuantGlobals.ProjectConfigDirectory + "//" + setup.FriendlyName +"//rundata";//运行配置文件
            setting.StrategyFileName = setup.StrategyFile;
            setting.DataStartTime = new DateTime(2011, 1, 1);//数据开始时间
            setting.EndTime = DateTime.MinValue;//数据结束时间 到最新数据为止
            setting.TradeStartTime = new DateTime(2011, 1, 1);//交易开始时间
            setting.StartCapital = 1000000;//初始资金100万
            setting.TickSimulation = true;//tick回测
            //setting.Frequency = BarFrequency.OneMin;//默认1分钟
            setting.CreateTicksFromBars = false;//是否开启Bar生成Tick模拟
            setting.RestrictOpenOrders = false;
            setting.IgnoreSystemWarnings = false;
            setting.HighBeforeLowDuringSimulation = true;
            return setting;
        }
        /// <summary>
        /// 策略程序集文件名
        /// </summary>
        public string StrategyFileName { get; set; }
        /// <summary>
        /// 是否开启PendingOrders检查
        /// </summary>
        public bool RestrictOpenOrders { get; set; }
        /// <summary>
        /// 忽略系统警告信息
        /// </summary>
        public bool IgnoreSystemWarnings { get; set; }
        /// <summary>
        /// Bar数据生成规则
        /// </summary>
        public bool HighBeforeLowDuringSimulation { get; set; }
        /// <summary>
        /// 是否利用Bar生成模拟tick 数据
        /// </summary>
        public bool CreateTicksFromBars { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string StrategyRunDataFile { get; set; }
        /// <summary>
        /// 策略配置友名
        /// </summary>
        public string FriendlyName { get; set; }
        /// <summary>
        /// 类名(FullName)
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// 数据开始日期
        /// </summary>
        public DateTime DataStartTime { get; set; }
        /// <summary>
        /// 交易开始日期
        /// </summary>
        public DateTime TradeStartTime { get; set; }
        /// <summary>
        /// 数据结束日期
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 初始资金
        /// </summary>
        public double StartCapital { get; set; }
        /// <summary>
        /// 逐tick模拟
        /// </summary>
        public bool TickSimulation { get; set; }
        

        /// <summary>
        /// 策略对应的参数列表
        /// </summary>
        public List<StrategyParameterInfo> SystemParameters { get; set; }

        /// <summary>
        /// 主频率
        /// </summary>
        public BarFrequency Frequency { get; set; }

        public delegate void ChangeDelegate(StrategySetting setting);

        /*
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {

            reader.Read();
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == GetType().ToString())
            {
                this.Type = (BarInterval)Enum.Parse(typeof(BarInterval), reader["Type"]);
                this.Interval = int.Parse(reader["Interveal"]);
                if (this.Type == BarInterval.CustomTime)
                    this.timespan = new TimeSpan(this._interval);
                //reader.Read();
            }
            else if (reader.NodeType == XmlNodeType.EndElement)
            {
                reader.Read();
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string str2;
                    if (((str2 = reader.LocalName) != null) && (str2 == "Type"))
                    {
                        BarInterval type = (BarInterval)Enum.Parse(typeof(BarInterval),reader.ReadElementContentAsString());
                        
                        this._type = type;
                    }
                    if (((str2 = reader.LocalName) != null) && (str2 == "Interveal"))
                    {
                        this._interval = int.Parse(reader.ReadElementContentAsString());
                        if(this.Type == BarInterval.CustomTime)
                            this.timespan = new TimeSpan(this._interval);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    //reader.Read();
                    return;
                }
            }
           // reader.Read(); // Skip <Pound>
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Common")
            {
                this.StrategyFileName = reader.ReadElementContentAsString();
                MessageBox.Show(this.StrategyFileName);
                this.RestrictOpenOrders = reader.ReadElementContentAsBoolean();
                MessageBox.Show(this.RestrictOpenOrders.ToString());
                this.IgnoreSystemWarnings = reader.ReadElementContentAsBoolean();
                this.HighBeforeLowDuringSimulation = reader.ReadElementContentAsBoolean();
                this.CreateTicksFromBars = reader.ReadElementContentAsBoolean();
                this.StrategyRunDataFile = reader.ReadElementContentAsString();
                this.FriendlyName = reader.ReadElementContentAsString();
                this.AssemblyName = reader.ReadElementContentAsString();
                this.DataStartTime = reader.ReadElementContentAsDateTime();
                this.DataStartTime = reader.ReadElementContentAsDateTime();
                this.EndTime = reader.ReadElementContentAsDateTime();
                this.StartCapital = reader.ReadElementContentAsDouble();
                this.TickSimulation = reader.ReadElementContentAsBoolean();
                reader.Read();
            }
            
            while(reader.Read())
            {
                MessageBox.Show("type:" + reader.NodeType.ToString() + "|" + reader.LocalName);

                if (reader.NodeType == XmlNodeType.Element)
                {
                    string name = reader.LocalName;
                    //reader.MoveToContent();
                    MessageBox.Show("got node name:"+name);
                    this.StrategyFileName = reader.ReadElementContentAsString();
                    this.RestrictOpenOrders = reader.ReadElementContentAsBoolean();
                    this.IgnoreSystemWarnings = reader.ReadElementContentAsBoolean();
                    this.HighBeforeLowDuringSimulation = reader.ReadElementContentAsBoolean();
                    this.CreateTicksFromBars = reader.ReadElementContentAsBoolean();
                    this.StrategyRunDataFile = reader.ReadElementContentAsString();
                    this.FriendlyName = reader.ReadElementContentAsString();
                    this.AssemblyName = reader.ReadElementContentAsString();
                    this.DataStartTime = reader.ReadElementContentAsDateTime();
                    this.DataStartTime = reader.ReadElementContentAsDateTime();
                    if (name == "EndTime") this.EndTime = reader.ReadElementContentAsDateTime();
                    if (name == "StartCapital") this.StartCapital = reader.ReadElementContentAsDouble();
                    if (name == "TickSimulation") this.TickSimulation = reader.ReadElementContentAsBoolean();

                    
                }
                //else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    //reader.Read();
                    //return;
                }
                
            //}
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Basic");
            writer.WriteElementString("StrategyFileName", this.StrategyFileName.ToString());
            writer.WriteElementString("RestrictOpenOrders", this.RestrictOpenOrders.ToString());
            writer.WriteElementString("IgnoreSystemWarnings", this.IgnoreSystemWarnings.ToString());
            writer.WriteElementString("HighBeforeLowDuringSimulation", this.HighBeforeLowDuringSimulation.ToString());
            writer.WriteElementString("CreateTicksFromBars", this.CreateTicksFromBars.ToString());
            writer.WriteElementString("StrategyRunDataFile", this.StrategyRunDataFile.ToString());
            writer.WriteElementString("FriendlyName", this.FriendlyName.ToString());
            writer.WriteElementString("AssemblyName", this.AssemblyName.ToString());
            writer.WriteElementString("DataStartTime", this.DataStartTime.ToString());
            writer.WriteElementString("TradeStartTime", this.TradeStartTime.ToString());
            writer.WriteElementString("EndTime", this.EndTime.ToShortDateString());
            writer.WriteElementString("StartCapital ", this.StartCapital.ToString());
            writer.WriteElementString("TickSimulation", this.TickSimulation.ToString());
            writer.WriteEndElement();

            //writer.WriteStartElement(GetType().ToString());
            //writer.WriteAttributeString("Type", this.Type.ToString());
            //writer.WriteAttributeString("Interveal", this.Interval.ToString());
            //writer.WriteEndElement();

            //writer.WriteElementString("Type", this.Type.ToString());
            //writer.WriteElementString("Interveal", this.Interval.ToString());
        }

        **/
    }
}
