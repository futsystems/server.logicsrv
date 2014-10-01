using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Quant.Base;
using System.Threading;

namespace TradingLib.Quant.Plugin
{
    [ExcludedPlugin]
    public class DataStorageWrapper : MarshalByRefObject, IBarDataStorage
    {
        // Fields
        private string lastError = "";
        private IBarDataStorage storage;

        // Methods
        public DataStorageWrapper(IBarDataStorage storage)
        {
            this.storage = storage;
        }

        private void ClearError()
        {
            this.lastError = "";
        }

        public string CompanyName()
        {
            try
            {
                return this.storage.CompanyName();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return null;
            }
        }

        public bool DeleteBars(SecurityFreq symbol)
        {
            this.ClearError();
            try
            {
                return this.storage.DeleteBars(symbol);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return false;
            }
        }

        public bool DeleteTicks(Security symbol)
        {
            this.ClearError();
            try
            {
                return this.storage.DeleteTicks(symbol);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return false;
            }
        }

        public void DoSettings()
        {
            this.ClearError();
            try
            {
                this.storage.DoSettings();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        public void Flush()
        {
            try
            {
                this.storage.Flush();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        public void ForceDefaultSettings()
        {
            this.ClearError();
            try
            {
                this.storage.ForceDefaultSettings();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        public long GetBarCount(SecurityFreq symbol, DateTime startDateTime, DateTime endDateTime)
        {
            this.ClearError();
            try
            {
                return this.storage.GetBarCount(symbol, startDateTime, endDateTime);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return 0;
            }
        }

        public string GetDescription()
        {
            try
            {
                return this.storage.GetDescription();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return null;
            }
        }

        public string GetName()
        {
            try
            {
                return this.storage.GetName();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return null;
            }
        }

        public void HandleException(Exception e)
        {
            if (!(e is ThreadAbortException))
            {
                string message = "The data storage plugin " + this.storage.GetType().FullName + " threw an exception of type " + e.GetType().FullName;
                this.lastError = message;
                PluginException exception = new PluginException(message, e)
                {
                    Source = this.storage.GetType().FullName
                };
                throw exception;
            }
        }

        public string id()
        {
            try
            {
                return this.storage.id();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return null;
            }
        }

        public bool IsProperlyConfigured()
        {
            this.ClearError();
            try
            {
                return this.storage.IsProperlyConfigured();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return false;
            }
        }

        public string LastError()
        {
            if (this.lastError == "")
            {
                try
                {
                    return this.storage.LastError();
                }
                catch (Exception exception)
                {
                    this.HandleException(exception);
                    return this.lastError;
                }
            }
            return this.lastError;
        }

        public List<Bar> LoadBars(SecurityFreq symbol, DateTime startDateTime, DateTime endDateTime, int maxLoadBars, bool loadFromEnd)
        {
            this.ClearError();
            try
            {
                return this.storage.LoadBars(symbol, startDateTime, endDateTime, maxLoadBars, loadFromEnd);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return null;
            }
        }

        public List<Tick> LoadTicks(Security symbol, DateTime startDate)
        {
            this.ClearError();
            try
            {
                return this.storage.LoadTicks(symbol, startDate);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return null;
            }
        }

        public List<Tick> LoadTicks(Security symbol, DateTime startDate, DateTime endDate)
        {
            this.ClearError();
            try
            {
                return this.storage.LoadTicks(symbol, startDate, endDate);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return null;
            }
        }

        public bool RequiresSetup()
        {
            this.ClearError();
            try
            {
                return this.storage.RequiresSetup();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return false;
            }
        }

        public int SaveBars(SecurityFreq symbol, List<Bar> bars)
        {
            this.ClearError();
            try
            {
                return this.storage.SaveBars(symbol, bars);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return 0;
            }
        }

        public void SaveTick(Security symbol, Tick tick)
        {
            this.ClearError();
            try
            {
                this.storage.SaveTick(symbol, tick);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
            }
        }

        public int SaveTicks(Security symbol, List<Tick> ticks)
        {
            this.ClearError();
            try
            {
                return this.storage.SaveTicks(symbol, ticks);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return 0;
            }
        }

        public int UpdateTicks(Security symbol, List<Tick> newTicks)
        {
            this.ClearError();
            try
            {
                return this.storage.UpdateTicks(symbol, newTicks);
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return 0;
            }
        }

        public string Version()
        {
            try
            {
                return this.storage.Version();
            }
            catch (Exception exception)
            {
                this.HandleException(exception);
                return null;
            }
        }
    }

 

 

}
