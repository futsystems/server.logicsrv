using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.Base
{
    public class ChartObject
    {
        // Fields
    private Dictionary<string, IIndicatorPlugin> _indicatorlist;//指标列表
    private Dictionary<string, List<ConstructorArgument>> _argument;//构造参数列表
    private ChartPane _chartpanel;
    private ChartOwner _chartowner;
    private string _chartname;
    private object _chartobject;

    // Methods
    public ChartObject(object chart, ChartOwner chartOwner, ChartPane chartPane)
    {
        this._chartobject = chart;
        this._chartpanel = chartPane;
        this._chartowner = chartOwner;
    }

    // Properties
    public object Chart
    {
        get
        {
            return this._chartobject;
        }
        set
        {
            this._chartobject = value;
        }
    }

    public string ChartName
    {
        get
        {
            return this._chartname;
        }
        set
        {
            this._chartname = value;
        }
    }

    public ChartOwner ChartOwner
    {
        get
        {
            return this._chartowner;
        }
        set
        {
            this._chartowner = value;
        }
    }

        
    public Dictionary<string, List<ConstructorArgument>> IndicatorArguments
    {
        get
        {
            if (this._argument == null)
            {
                this._argument = new Dictionary<string, List<ConstructorArgument>>();
            }
            return this._argument;
        }
        set
        {
            this._argument = value;
        }
    }

    public Dictionary<string, IIndicatorPlugin> Indicators
    {
        get
        {
            if (this._indicatorlist == null)
            {
                this._indicatorlist = new Dictionary<string, IIndicatorPlugin>();
            }
            return this._indicatorlist;
        }
        set
        {
            this._indicatorlist = value;
        }
    }

    public ChartPane Pane
    {
        get
        {
            return this._chartpanel;
        }
        set
        {
            this._chartpanel = value;
        }
    }

    }


}
