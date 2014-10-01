using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.Quant.Base;


namespace TradingLib.Quant.Engine
{
    [Serializable]
    public class StrategyResults : IStrategyResults
    {
        // Fields
        //public List<RiskAssessmentResults> RiskResults;
  
        private StrategyRunSettings x165931ad22499e3c;

        private TimeSpan x41a028ab5303ce50;

        private StrategyData _strategydata;

        // Methods
        private StrategyResults()
        {
            //this.RiskResults = new List<RiskAssessmentResults>();
        }

        public StrategyResults(StrategyData systemData, StrategyRunSettings runSettings)
        {
            //this.RiskResults = new List<RiskAssessmentResults>();
            this.Data = systemData;
            this.RunSettings = runSettings;
        }

        public void DeserializeOwnedData(SerializationReader reader, object context)
        {
            this.Data = new StrategyData();
            this.Data.DeserializeOwnedData(reader,context);
            //this.Data = (StrategyData)reader.ReadObject();//整个对象处理
            //StrategyData data = 
            //this.RiskResults = (List<RiskAssessmentResults>)reader.ReadObject();
            this.RunSettings = (StrategyRunSettings)reader.ReadObject();
            this.RunLength = reader.ReadTimeSpan();
        }

        public static StrategyResults Load(string filename)
        {
            StrategyResults results;
            using (FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                using (SerializationReader reader = new SerializationReader(stream))
                {
                    string str;
                    try
                    {
                        str = reader.ReadString();
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw new QSQuantError(string.Format("This system results file appears to have been created with a different build of RightEdge, and cannot be used  with this build of RightEdge ({0}).", EngineGlobals.BuildNumber), exception);
                    }

                    if (str != EngineGlobals.BuildNumber)
                    {
                        throw new QSQuantError(string.Format("This system results file was created with a different build of RightEdge ({0}), and cannot be used  with this build of RightEdge ({1}).", str, EngineGlobals.BuildNumber));
                    }
                    results = (StrategyResults)reader.ReadObject();
                }
            }
            return results;
        }

        public static void Save(StrategyResults results, string filename)
        {
            using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                using (SerializationWriter writer = new SerializationWriter(stream))
                {
                    writer.Write(EngineGlobals.BuildNumber);
                    writer.WriteObject(results);
                    writer.AppendTokenTables();
                }
            }
        }

        public void SerializeOwnedData(SerializationWriter writer, object context)
        {
            //writer.WriteObject(this.Data);//整个对象处理
            this.Data.SerializeOwnedData(writer, context);//按需写入需要的数据 这个比较合理 也容易查询错误来源
            //writer.WriteObject(this.RiskResults);
            writer.WriteObject(this.RunSettings);//保存策略运行配置文件
            writer.Write(this.RunLength);
        }

        // Properties
        public IStrategyData Data
        {
            
            get
            {
                return this._strategydata;
            }
            private set
            {
                this._strategydata = value as StrategyData;
            }
        }

        public TimeSpan RunLength
        {
            get
            {
                return this.x41a028ab5303ce50;
            }
            set
            {
                this.x41a028ab5303ce50 = value;
            }
        }

        public StrategyRunSettings RunSettings
        {

            get
            {
                return this.x165931ad22499e3c;
            }
            private set
            {
                this.x165931ad22499e3c = value;
            }
        }
    }


}
