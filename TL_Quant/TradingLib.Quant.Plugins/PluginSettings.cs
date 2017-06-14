using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

using TradingLib.Quant.Base;

namespace TradingLib.Quant.Plugin
{
    [Serializable]
    public sealed class PluginSettings
    {
        // Methods
        public PluginSettings Clone()
        {
            PluginSettings settings = (PluginSettings)base.MemberwiseClone();
            if (this.PluginToken != null)
            {
                settings.PluginToken = this.PluginToken.Clone();
            }
            return settings;
        }

        public static PluginSettings Create(object obj, Type interfaceType)
        {
            PluginSettings settings = new PluginSettings
            {
                PluginToken = PluginToken.Create(obj.GetType(), interfaceType)
            };
            settings.PluginXml = new PluginReference(obj).GetPluginXml();
            return settings;
        }
        /*
        public static PluginSettings CreateTimeFrequencySettings(BarFrequency f)
        {

            TimeFrequency frequency = new TimeFrequency(f);
            
            return Create(frequency, typeof(FrequencyPlugin));
        }**/

        public override bool Equals(object obj)
        {
            PluginSettings settings = obj as PluginSettings;
            return Equals(this, settings);
        }

        public static bool Equals(PluginSettings p1, PluginSettings p2)
        {
            if ((p1 == null) || (p2 == null))
            {
                return ((p1 == null) && (p2 == null));
            }
            return (p1.PluginToken.IsSamePlugin(p2.PluginToken) && (p1.PluginXml == p2.PluginXml));
        }

        public static PluginSettings FromXml(string xml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(PluginSettings));
                return (PluginSettings)serializer.Deserialize(reader);
            }
        }

        public override int GetHashCode()
        {
            return (((this.PluginToken == null) ? 0 : this.PluginToken.GetHashCode()) ^ ((this.PluginXml == null) ? 0 : this.PluginXml.GetHashCode()));
        }

        public string ToXml()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter output = new StringWriter(sb);
            XmlWriterSettings settings = new XmlWriterSettings
            {
                CloseOutput = true,
                Indent = true,
                NewLineChars = Environment.NewLine
            };
            using (XmlWriter writer2 = XmlWriter.Create(output, settings))
            {
                new XmlSerializer(typeof(PluginSettings)).Serialize(writer2, this);
            }
            return sb.ToString();
        }

        // Properties
        public PluginToken PluginToken { get; set; }

        public string PluginXml { get; set; }
    }


}
