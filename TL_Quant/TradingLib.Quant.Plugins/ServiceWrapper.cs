using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Threading;

using TradingLib.Quant.Base;

namespace TradingLib.Quant.Plugin
{
    [Serializable]
    public class ServiceWrapper : MarshalByRefObject, IQService, IDisposable
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }
        // Fields
        private BarDataWrapper _barDataWrapper;
        private BrokerWrapper _brokerWrapper;
        private bool _nullBarData;
        private bool _nullBroker;
        private bool _nullTick;
        internal SynchronizationContext _synchContext;
        private TickWrapper _tickWrapper;
        private string errorText = "";
        private IQService service;
        private EventHandler<ServiceEventArgs> ServiceEventh;
        private string serviceName = "";

        // Events
        public event EventHandler<ServiceEventArgs> ServiceEvent
        {
            add
            {
                EventHandler<ServiceEventArgs> handler2;
                EventHandler<ServiceEventArgs> serviceEvent = this.ServiceEventh;
                do
                {
                    handler2 = serviceEvent;
                    EventHandler<ServiceEventArgs> handler3 = (EventHandler<ServiceEventArgs>)Delegate.Combine(handler2, value);
                    serviceEvent = Interlocked.CompareExchange<EventHandler<ServiceEventArgs>>(ref this.ServiceEventh, handler3, handler2);
                }
                while (serviceEvent != handler2);
            }
            remove
            {
                EventHandler<ServiceEventArgs> handler2;
                EventHandler<ServiceEventArgs> serviceEvent = this.ServiceEventh;
                do
                {
                    handler2 = serviceEvent;
                    EventHandler<ServiceEventArgs> handler3 = (EventHandler<ServiceEventArgs>)Delegate.Remove(handler2, value);
                    serviceEvent = Interlocked.CompareExchange<EventHandler<ServiceEventArgs>>(ref this.ServiceEventh, handler3, handler2);
                }
                while (serviceEvent != handler2);
            }
        }

        // Methods
        public ServiceWrapper(IQService service, SynchronizationContext synchContext)
        {
            this.service = service;
            this.serviceName = service.GetType().FullName;
            this._synchContext = synchContext;
            SynchronizationContext context1 = this._synchContext;
            service.ServiceEvent += new EventHandler<ServiceEventArgs>(this.service_ServiceEvent);
            service.SendDebugEvent +=new DebugDelegate(debug);
        }

        public string Author()
        {
            this.ClearError();
            try
            {
                return this.service.Author();
            }
            catch (Exception exception)
            {
                return this.DoError(exception);
            }
        }

        public void ClearError()
        {
            this.errorText = "";
        }

        public string CompanyName()
        {
            this.ClearError();
            try
            {
                return this.service.CompanyName();
            }
            catch (Exception exception)
            {
                return this.DoError(exception);
            }
        }

        public bool Connect(ServiceConnectOptions connectOptions)
        {
            this.ClearError();
            try
            {
                return this.service.Connect(connectOptions);
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return false;
            }
        }

        public string Description()
        {
            this.ClearError();
            try
            {
                return this.service.Description();
            }
            catch (Exception exception)
            {
                return this.DoError(exception);
            }
        }

        public bool Disconnect()
        {
            this.ClearError();
            try
            {
                return this.service.Disconnect();
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return false;
            }
        }

        public void Dispose()
        {
            this.ClearError();
            try
            {
                this.service.Dispose();
            }
            catch (Exception exception)
            {
                this.DoError(exception);
            }
        }

        public string DoError(Exception e)
        {
            Exception exception;
            if (e is ThreadAbortException)
            {
                return null;
            }
            string message = "The service plugin " + this.serviceName + " threw an exception of type " + e.GetType().FullName;
            this.errorText = message;
            if (e.GetType().IsSerializable)
            {
                exception = e;
            }
            else
            {
                message = ((message + " (which was not serializeable)\r\n") + e.Message + "\r\n") + e.StackTrace;
                //Trace.WriteLine(message);
                exception = null;
            }
            PluginException exception2 = new PluginException(message, exception)
            {
                Source = this.serviceName
            };
            throw exception2;
        }

        public IBarDataRetrieval GetBarDataInterface()
        {
            if (this._nullBarData)
            {
                return null;
            }
            if (this._barDataWrapper != null)
            {
                return this._barDataWrapper;
            }
            this.ClearError();
            try
            {
                IBarDataRetrieval barDataInterface = this.service.GetBarDataInterface();
                if (barDataInterface == null)
                {
                    this._nullBarData = true;
                    return null;
                }
                this._barDataWrapper = new BarDataWrapper(this, barDataInterface);
                return this._barDataWrapper;
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return null;
            }
        }

        public IExBroker GetBrokerInterface()
        {
            if (this._nullBroker)
            {
                return null;
            }
            if (this._brokerWrapper != null)
            {
                return  this._brokerWrapper;
            }
            this.ClearError();
            try
            {
                IExBroker brokerInterface = this.service.GetBrokerInterface();
                if (brokerInterface == null)
                {
                    this._nullBroker = true;
                    return null;
                }
                this._brokerWrapper = new BrokerWrapper(this, brokerInterface);
                return this._brokerWrapper;
                //return null;// this._brokerWrapper;
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return null;
            }
        }

        public string GetError()
        {
            if (this.errorText != "")
            {
                return this.errorText;
            }
            this.ClearError();
            try
            {
                return this.service.GetError();
            }
            catch (Exception exception)
            {
                return this.DoError(exception);
            }
        }

        public ITickRetrieval GetTickDataInterface()
        {
            if (this._nullTick)
            {
                return null;
            }
            if (this._tickWrapper != null)
            {
                return this._tickWrapper;
            }
            this.ClearError();
            try
            {
                ITickRetrieval tickDataInterface = this.service.GetTickDataInterface();
                if (tickDataInterface == null)
                {
                    this._nullTick = true;
                    return null;
                }
                this._tickWrapper = new TickWrapper(this, tickDataInterface);
                return this._tickWrapper;
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return null;
            }
        }

        public bool HasCustomSettings()
        {
            this.ClearError();
            try
            {
                return false;//this.service.HasCustomSettings();
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return false;
            }
        }

        public string id()
        {
            this.ClearError();
            try
            {
                return this.service.id();
            }
            catch (Exception exception)
            {
                return this.DoError(exception);
            }
        }

        public bool Initialize(SerializableDictionary<string, string> settings)
        {
            this.ClearError();
            try
            {
                return this.service.Initialize(settings);
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return false;
            }
        }

        public bool NeedsAuthentication()
        {
            this.ClearError();
            try
            {
                return this.service.NeedsAuthentication();
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return false;
            }
        }

        public bool NeedsPort()
        {
            this.ClearError();
            try
            {
                return this.service.NeedsPort();
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return false;
            }
        }

        public bool NeedsServerAddress()
        {
            this.ClearError();
            try
            {
                return this.service.NeedsServerAddress();
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return false;
            }
        }

        private void service_ServiceEvent(object sender, ServiceEventArgs e)
        {
            try
            {
                EventHandler<ServiceEventArgs> eventHandler = this.ServiceEventh;
                if (eventHandler != null)
                {
                    SendOrPostCallback d = delegate
                    {
                        eventHandler(this, e);
                    };
                    if (this._synchContext == null)
                    {
                        d(null);
                    }
                    else
                    {
                        this._synchContext.Post(d, null);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ServiceName()
        {
            this.ClearError();
            try
            {
                return this.service.ServiceName();
            }
            catch (Exception exception)
            {
                return this.DoError(exception);
            }
        }

        public bool ShowCustomSettingsForm(ref SerializableDictionary<string, string> settings)
        {
            this.ClearError();
            try
            {
                return this.service.ShowCustomSettingsForm(ref settings);
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return false;
            }
        }

        public bool SupportsMultipleInstances()
        {
            this.ClearError();
            try
            {
                return false;
                //return this.service.su
            }
            catch (Exception exception)
            {
                this.DoError(exception);
                return false;
            }
        }

        public string Version()
        {
            this.ClearError();
            try
            {
                return this.service.Version();
            }
            catch (Exception exception)
            {
                return this.DoError(exception);
            }
        }

        // Properties
        public bool HisDataAvailable
        {
            get
            {
                this.ClearError();
                try
                {
                    return this.service.HisDataAvailable;
                }
                catch (Exception exception)
                {
                    this.DoError(exception);
                    return false;
                }
            }
        }

        public bool BrokerExecutionAvailable
        {
            get
            {
                this.ClearError();
                try
                {
                    return this.service.BrokerExecutionAvailable;
                }
                catch (Exception exception)
                {
                    this.DoError(exception);
                    return false;
                }
            }
        }

        public string Password
        {
            get
            {
                this.ClearError();
                try
                {
                    return this.service.Password;
                }
                catch (Exception exception)
                {
                    return this.DoError(exception);
                }
            }
            set
            {
                this.ClearError();
                try
                {
                    this.service.Password = value;
                }
                catch (Exception exception)
                {
                    this.DoError(exception);
                }
            }
        }

        public int Port
        {
            get
            {
                this.ClearError();
                try
                {
                    return this.service.Port;
                }
                catch (Exception exception)
                {
                    this.DoError(exception);
                    return 0;
                }
            }
            set
            {
                this.ClearError();
                try
                {
                    this.service.Port = value;
                }
                catch (Exception exception)
                {
                    this.DoError(exception);
                }
            }
        }

        public string ServerAddress
        {
            get
            {
                this.ClearError();
                try
                {
                    return this.service.ServerAddress;
                }
                catch (Exception exception)
                {
                    return this.DoError(exception);
                }
            }
            set
            {
                this.ClearError();
                try
                {
                    this.service.ServerAddress = value;
                }
                catch (Exception exception)
                {
                    this.DoError(exception);
                }
            }
        }

        public bool TickDataAvailable
        {
            get
            {
                this.ClearError();
                try
                {
                    return this.service.TickDataAvailable;
                }
                catch (Exception exception)
                {
                    this.DoError(exception);
                    return false;
                }
            }
        }

        public string UserName
        {
            get
            {
                this.ClearError();
                try
                {
                    return this.service.UserName;
                }
                catch (Exception exception)
                {
                    return this.DoError(exception);
                }
            }
            set
            {
                this.ClearError();
                try
                {
                    this.service.UserName = value;
                }
                catch (Exception exception)
                {
                    this.DoError(exception);
                }
            }
        }
    }


}
