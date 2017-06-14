using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Loader
{
    [Serializable]
    public class ServiceSetup
    {
        // Fields
        private SerializableDictionary<string, string> _customSettings;
        public string AssemblyName;
        public string FriendlyName;
        public string id;
        public string Password;
        public int Port;
        public string ServerAddress;
        public string ServicePluginName;
        public string Username;

        // Methods
        public ServiceSetup()
        {
            this.FriendlyName = "";
            this.id = "";
            this.ServicePluginName = "";
            this.ServerAddress = "";
            this.Username = "";
            this.Password = "";
            this.AssemblyName = "";
        }

        public ServiceSetup(ServiceSetup other)
        {
            this.FriendlyName = "";
            this.id = "";
            this.ServicePluginName = "";
            this.ServerAddress = "";
            this.Username = "";
            this.Password = "";
            this.AssemblyName = "";
            this.FriendlyName = other.FriendlyName;
            this.id = other.id;
            this.ServicePluginName = other.ServicePluginName;
            this.ServerAddress = other.ServerAddress;
            this.Port = other.Port;
            this.Username = other.Username;
            this.Password = other.Password;
            this.AssemblyName = other.AssemblyName;
            foreach (KeyValuePair<string, string> pair in other.CustomSettings)
            {
                this.CustomSettings.Add(pair.Key, pair.Value);
            }
        }

        // Properties
        public SerializableDictionary<string, string> CustomSettings
        {
            get
            {
                if (this._customSettings == null)
                {
                    this._customSettings = new SerializableDictionary<string, string>();
                }
                return this._customSettings;
            }
            set
            {
                this._customSettings = value;
            }
        }
    }

 

}
