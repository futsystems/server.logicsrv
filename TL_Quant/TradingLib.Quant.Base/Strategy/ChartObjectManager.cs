using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class ChartObjectManager : IOwnedDataSerializableAndRecreatable, IOwnedDataSerializable, ISerializable
    {
        // Fields
        private IStrategyData baseSystem;
        private Dictionary<Security, List<IChartObject>> changedChartObjects;
        private Dictionary<Security, List<IChartObject>> chartObjects;
        private Dictionary<string, IChartObject> namedChartObjects;

        // Methods
        public ChartObjectManager()
        {
            this.chartObjects = new Dictionary<Security, List<IChartObject>>();
            this.changedChartObjects = new Dictionary<Security, List<IChartObject>>();
            this.namedChartObjects = new Dictionary<string, IChartObject>();
        }

        public ChartObjectManager(IStrategyData baseSystem)
        {
            this.chartObjects = new Dictionary<Security, List<IChartObject>>();
            this.changedChartObjects = new Dictionary<Security, List<IChartObject>>();
            this.namedChartObjects = new Dictionary<string, IChartObject>();
            this.baseSystem = baseSystem;
        }

        protected ChartObjectManager(SerializationInfo info, StreamingContext context)
        {
            this.chartObjects = new Dictionary<Security, List<IChartObject>>();
            this.changedChartObjects = new Dictionary<Security, List<IChartObject>>();
            this.namedChartObjects = new Dictionary<string, IChartObject>();
            SerializationReader reader = new SerializationReader((byte[])info.GetValue("data", typeof(byte[])));
            this.DeserializeOwnedData(reader, context);
        }

        public void Add(Security symbol, IChartObject chartObject)
        {
            List<IChartObject> list = null;
            if (this.chartObjects.ContainsKey(symbol))
            {
                list = this.chartObjects[symbol];
            }
            else
            {
                list = new List<IChartObject>();
                this.chartObjects[symbol] = list;
            }
            chartObject.ChartObjectChanged += new ChartObjectChangedDelegate(this.chartObject_ChartObjectChanged);
            list.Add(chartObject);
            if (this.changedChartObjects.ContainsKey(symbol))
            {
                this.changedChartObjects[symbol].Add(chartObject);
            }
            else
            {
                List<IChartObject> list2 = new List<IChartObject> {
                chartObject
            };
                this.changedChartObjects.Add(symbol, list2);
            }
        }

        public void Add(Security symbol, string name, IChartObject chartObject)
        {
            this.Add(symbol, chartObject);
            string key = symbol.ToString() + "|" + name;
            if (this.namedChartObjects.ContainsKey(key))
            {
                this.namedChartObjects[key] = chartObject;
            }
            else
            {
                this.namedChartObjects.Add(key, chartObject);
            }
        }

        private void chartObject_ChartObjectChanged(IChartObject chartObject)
        {
            foreach (Security symbol in this.chartObjects.Keys)
            {
                foreach (IChartObject obj2 in this.chartObjects[symbol])
                {
                    if (obj2.GetObjectId() == chartObject.GetObjectId())
                    {
                        if (this.changedChartObjects.ContainsKey(symbol))
                        {
                            if (!this.changedChartObjects[symbol].Contains(chartObject))
                            {
                                this.changedChartObjects[symbol].Add(chartObject);
                            }
                        }
                        else
                        {
                            List<IChartObject> list = new List<IChartObject> {
                            chartObject
                        };
                            this.changedChartObjects.Add(symbol, list);
                        }
                    }
                }
            }
        }

        public void ClearChanges()
        {
            this.changedChartObjects.Clear();
        }

        public void DeserializeOwnedData(SerializationReader reader, object context)
        {
            this.chartObjects = (Dictionary<Security, List<IChartObject>>)reader.ReadObject();
        }

        public Dictionary<Security, List<IChartObject>> GetChanges()
        {
            return this.changedChartObjects;
        }

        public IChartObject GetNamedChartObject(Security symbol, string name)
        {
            string key = symbol.ToString() + "|" + name;
            IChartObject obj2 = null;
            if (this.namedChartObjects.ContainsKey(key))
            {
                obj2 = this.namedChartObjects[key];
            }
            return obj2;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter writer = new SerializationWriter();
            this.SerializeOwnedData(writer, context);
            info.AddValue("data", writer.ToArray());
        }

        public void SerializeOwnedData(SerializationWriter writer, object context)
        {
            writer.WriteObject(this.chartObjects);
        }

        public void SetSystemData(IStrategyData systemData)
        {
            this.baseSystem = systemData;
        }

        // Properties
        public List<IChartObject> this[Security symbol]
        {
            get
            {
                if (this.chartObjects.ContainsKey(symbol))
                {
                    return this.chartObjects[symbol];
                }
                return null;
            }
        }
    }


}
