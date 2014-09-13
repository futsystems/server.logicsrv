using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 定义列panel列用于管理不同的panel,并得到对应的panel
    /// </summary>
    [Serializable]
    public sealed class ChartPaneList : List<ChartPane>
    {
        // Methods
        public int GetPaneIndex(string name, bool create)
        {
            if (string.IsNullOrEmpty(name))
            {
                return 0;
            }
            for (int i = 0; i < base.Count; i++)
            {
                if (base[i].Name == name)
                {
                    return i;
                }
            }
            if (create)
            {
                ChartPane item = new ChartPane
                {
                    Name = name
                };
                base.Add(item);
                return (base.Count - 1);
            }
            return -1;
        }

        // Properties
        public ChartPane this[string Name]
        {
            get
            {
                foreach (ChartPane pane in this)
                {
                    if (pane.Name == Name)
                    {
                        return pane;
                    }
                }
                return null;
            }
        }
    }

}
