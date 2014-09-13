using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    [Serializable]
    public class StrategyParameters
    {
        // Fields
        private Dictionary<string, double> dict;

        // Methods
        public StrategyParameters()
        {
            this.dict = new Dictionary<string, double>();
        }

        public StrategyParameters(Dictionary<string, double> dict)
        {
            this.dict = new Dictionary<string, double>(dict);
        }

        public bool Contains(string ParameterName)
        {
            return this.dict.ContainsKey(ParameterName);
        }

        // Properties
        public double this[string ParameterName]
        {
            get
            {
                double num=1;

                if (!this.dict.TryGetValue(ParameterName, out num))
                {
                    /*
                    MessageBox.Show("含有参数:" + dict.Keys.Count.ToString() + string.Join(",", dict.Keys.ToArray()));
                    //throw new TradingLib.Common.QSQuantError("No system parameter was set up with the name \"" + ParameterName + "\".");
                    fmParameterInput fm = new fmParameterInput(ParameterName);
                    if (fm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        num = fm.Value;
                    }**/
                    //将策略参数添加到我们的Parameter列表
                    dict.Add(ParameterName, num);

                }
                return num;
            }
        }

        public List<string> Keys
        {
            get
            {
                return new List<string>(this.dict.Keys);
            }
        }
    }

}
