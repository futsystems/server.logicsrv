using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class OptimizationResult : IOwnedDataSerializableAndRecreatable, IOwnedDataSerializable, ISerializable
    {
        // Methods
        public OptimizationResult()
        {
            this.ParameterValues = new List<KeyValuePair<string, double>>();
        }

        protected OptimizationResult(SerializationInfo info, StreamingContext context)
        {
            SerializationReader reader = new SerializationReader((byte[])info.GetValue("data", typeof(byte[])));
            this.DeserializeOwnedData(reader, context);
        }

        public void DeserializeOwnedData(SerializationReader reader, object context)
        {
            this.RunNumber = reader.ReadInt32();
            int capacity = reader.ReadInt32();
            this.ParameterValues = new List<KeyValuePair<string, double>>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                string key = reader.ReadString();
                double num3 = reader.ReadDouble();
                this.ParameterValues.Add(new KeyValuePair<string, double>(key, num3));
            }
            this.ResultsFile = reader.ReadString();
            this.FinalStatistic = (BarStatistic)reader.ReadObject();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            SerializationWriter writer = new SerializationWriter();
            this.SerializeOwnedData(writer, context);
            info.AddValue("data", writer.ToArray());
        }

        public void SerializeOwnedData(SerializationWriter writer, object context)
        {
            writer.Write(this.RunNumber);
            writer.Write(this.ParameterValues.Count);
            foreach (KeyValuePair<string, double> pair in this.ParameterValues)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }
            writer.Write(this.ResultsFile);
            writer.WriteObject(this.FinalStatistic);
        }

        // Properties
        /// <summary>
        /// 最终的统计状态
        /// </summary>
        public BarStatistic FinalStatistic { get; set; }
        /// <summary>
        /// 所采用的策略参数组合
        /// </summary>
        public List<KeyValuePair<string, double>> ParameterValues { get; set; }
        /// <summary>
        /// 运行策略结果文件
        /// </summary>
        public string ResultsFile { get; set; }

        //public List<RiskAssessmentResults> RiskResults { get; set; }
        /// <summary>
        /// 运行次序
        /// </summary>
        public int RunNumber { get; set; }
    }


}
