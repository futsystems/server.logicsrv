using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Reflection;
using TradingLib.API;
using TradingLib.Quant.Base;


namespace TradingLib.Quant.Plugin
{
    public class PluginReference : MarshalByRefObject
    {
        // Fields
        private readonly object _plugin;
        private object _wrapper;

        // Methods
        public PluginReference(object plugin)
        {
            this._plugin = plugin;
            this._wrapper = null;
        }

        public object GetPlugin(AppDomain targetDomain)
        {
            object obj2;
            if (targetDomain == AppDomain.CurrentDomain)
            {
                return this.Plugin;
            }
            if (this._wrapper != null)
            {
                return this._wrapper;
            }
            if (this.Plugin is IIndicator)
            {
                obj2 = new IndicatorWrapper((IIndicator)this.Plugin);
            }
                /*
            else if (this.Plugin is ISeriesCalculator)
            {
                obj2 = new SeriesCalculatorWrapper((ISeriesCalculator)this.Plugin);
            }
            else if (this.Plugin is ITrigger)
            {
                obj2 = new TriggerWrapper((ITrigger)this.Plugin);
            }
            else if (this.Plugin is IAction)
            {
                obj2 = new ActionWrapper((IAction)this.Plugin);
            }
            else if (this.Plugin is IRiskAssessment)
            {
                obj2 = new RiskAssessmentWrapper((IRiskAssessment)this.Plugin);
            }**/
            else if (this.Plugin is IDataStore)
            {
                //obj2 = //new DataStorageWrapper((IDataStore)this.Plugin);
                obj2 = (IDataStore)this.Plugin;
            }
            else
            {
                obj2 = null;
                /*
                if (!(this.Plugin is ISystemResultPlugin))
                {
                    throw new RightEdgeError("The plugin did not implement an interface for which there is a wrapper class: " + this.Plugin.GetType().FullName);
                }
                obj2 = new SystemResultWrapper((ISystemResultPlugin)this.Plugin);**/
            }
            this._wrapper = obj2;
            return obj2;
        }

        public string GetPluginXml()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter output = new StringWriter(sb);
            XmlWriterSettings settings = new XmlWriterSettings
            {
                CloseOutput = true,
                Indent = false,
                NewLineChars = string.Empty
            };
            using (XmlWriter writer2 = XmlWriter.Create(output, settings))
            {
                new XmlSerializer(this._plugin.GetType()).Serialize(writer2, this._plugin);
            }
            return sb.ToString();
        }

        public void SetPluginProperty(string name, object value)
        {
            this._plugin.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance).SetValue(this._plugin, value, null);
        }

        // Properties
        public object Plugin
        {
            get
            {
                return this._plugin;
            }
        }

        public AppDomain PluginDomain
        {
            get
            {
                return AppDomain.CurrentDomain;
            }
        }
    }

 

}
