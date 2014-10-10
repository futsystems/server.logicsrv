using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using TradingLib.Quant;
//using System.Windows.Forms;
using System.IO;
using TradingLib.API;
using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;

namespace TradingLib.Quant.Plugin
{
    /// <summary>
    /// 从文件加载相关配置
    /// </summary>
    [Serializable]
    public class SharedSystemRunData : ISharedSystemRunData
    {
        // Fields
        private PluginSettings _selectedSystemFrequency;

        // Methods
        public SharedSystemRunData()
        {
        }

        protected SharedSystemRunData(SerializationInfo info, StreamingContext context)
        {
            SerializationReader reader = new SerializationReader((byte[])info.GetValue("data", typeof(byte[])));
            this.DeserializeOwnedData(reader, context);
        }

        public void DeserializeOwnedData(SerializationReader reader, object context)
        {
            
            this.RunSettings = (StrategyRunSettings)reader.ReadObject();
            this.InternalSettings = (InternalSystemRunSettings)reader.ReadObject();
            this.SelectedSystemFrequency = (PluginSettings)reader.ReadObject();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter writer = new SerializationWriter();
            this.SerializeOwnedData(writer, context);
            info.AddValue("data", writer.ToArray());
        }

        /// <summary>
        /// 获取symbol对应的Frequency插件
        /// </summary>
        /// <returns></returns>
        public FrequencyPlugin GetOrCreateSystemFrequencyPlugin()
        {
            if (this.RunSettings.BarFrequency == null)
            {
                BarFrequency[] numArray = (from s in this.RunSettings.Symbols select s.Frequency).ToArray();//this.RunSettings.Symbols.ToAr//(from s in this.RunSettings.Symbols select s.Frequency).Distinct<int>().ToArray<int>();
                if (numArray.Length != 1)
                {
                    if (numArray.Length == 0)
                    {
                        throw new InvalidOperationException("No symbols selected to run with system");
                    }
                    throw new InvalidOperationException("System frequency was not specified, and could not be inferred from system symbol list because they were not all the same frequency.");
                }
                //this.RunSettings.BarFrequency = new TimeFrequency(numArray[0]);
            }
            return this.RunSettings.BarFrequency;
        }

        public void SerializeOwnedData(SerializationWriter writer, object context)
        {
            this.UpdateSelectedFrequency();
            writer.WriteObject(this.RunSettings);
            writer.WriteObject(this.InternalSettings);
            writer.WriteObject(this.SelectedSystemFrequency);
        }

        public SharedSystemRunData ShallowClone()
        {
            return (SharedSystemRunData)base.MemberwiseClone();
        }

        public void UpdateSelectedFrequency()
        {
            if (this.RunSettings.BarFrequency != null)
            {
                this._selectedSystemFrequency = PluginSettings.Create(this.RunSettings.BarFrequency, typeof(FrequencyPlugin));
            }
        }

        // Properties
        public InternalSystemRunSettings InternalSettings { get; set; }

        public StrategyRunSettings RunSettings { get; set; }

        public PluginSettings SelectedSystemFrequency
        {
            get
            {
                return this._selectedSystemFrequency;
            }
            set
            {
                this._selectedSystemFrequency = value;
                if (this._selectedSystemFrequency == null)
                {
                    this.RunSettings.BarFrequency = null;
                }
                else
                {
                    //MessageBox.Show(this._selectedSystemFrequency.ToString());
                    //MessageBox.Show(QuantGlobals.PluginManager.ToString());
                    this.RunSettings.BarFrequency = (FrequencyPlugin)PluginGlobals.PluginManager.CreatePlugin(this._selectedSystemFrequency);
                }
            }
        }

        public static SharedSystemRunData ReadRunDataFile(string filename)
        {
            try
            {
                SharedSystemRunData data = new SharedSystemRunData();
                if (File.Exists(filename))
                {
                    using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                    {
                        using (SerializationReader reader = new SerializationReader(stream))
                        {
                            data.DeserializeOwnedData(reader, new StreamingContext());
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("error reading..");
                return null;
            }

        }

        public static bool WriteDefaultRunDataFile(string filename)
        {
            return true;

        }


    }

 

}
