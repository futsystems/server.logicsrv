using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradingLib.Quant;

using TradingLib.Common;
using System.Threading;

using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;
using TradingLib.Quant.Loader;
using TradingLib.Quant.Common;


namespace TradingLib.Quant.GUI
{
    public delegate void ChartObjectChange(object sender, ChartObjectEventArgs e);
    public delegate void propertyChanged(object sender, PropertyEventArgs e);

    public partial class PropertiesWindow : UserControl
    {
        protected string selectedObjectOwner;
        protected IChartDisplay chart;
        //private xe5110d3d5083f078 x91f347c6e97f1846;

        private propertyChanged propertyChgHandler;
        private ChartObjectChange chartObjChgHandler;
 


        // Events
        public event ChartObjectChange ChartObjectChanged
        {
            add
            {
                ChartObjectChange change2;
                ChartObjectChange change = this.chartObjChgHandler;
                do
                {
                    change2 = change;
                    ChartObjectChange change3 = (ChartObjectChange)Delegate.Combine(change2, value);
                    change = Interlocked.CompareExchange<ChartObjectChange>(ref this.chartObjChgHandler, change3, change2);
                }
                while (change != change2);
            }
            remove
            {
                ChartObjectChange change2;
                ChartObjectChange change = this.chartObjChgHandler;
                do
                {
                    change2 = change;
                    ChartObjectChange change3 = (ChartObjectChange)Delegate.Remove(change2, value);
                    change = Interlocked.CompareExchange<ChartObjectChange>(ref this.chartObjChgHandler, change3, change2);
                }
                while (change != change2);
            }
        }

        public event propertyChanged PropertyChanged
        {
            add
            {
                propertyChanged changed2;
                propertyChanged changed = this.propertyChgHandler;
                do
                {
                    changed2 = changed;
                    propertyChanged changed3 = (propertyChanged)Delegate.Combine(changed2, value);
                    changed = Interlocked.CompareExchange<propertyChanged>(ref this.propertyChgHandler, changed3, changed2);
                }
                while (changed != changed2);
            }
            remove
            {
                propertyChanged changed2;
                propertyChanged changed = this.propertyChgHandler;
                do
                {
                    changed2 = changed;
                    propertyChanged changed3 = (propertyChanged)Delegate.Remove(changed2, value);
                    changed = Interlocked.CompareExchange<propertyChanged>(ref this.propertyChgHandler, changed3, changed2);
                }
                while (changed != changed2);
            }
        }





        public PropertiesWindow()
        {
            InitializeComponent();
        }

        public void ShowIndicatorProperties(IndicatorInfo info)
        {
            this.ShowIndicatorProperties(info, null, null, null);
        }
        public void ShowIndicatorProperties(IndicatorInfo info, IList<string> availableInputs, IndicatorInfo.ChangeDelegate changeHandler, string objectOwner)
        {
            if (info != null)
            {
                //this.xc6fc97f20b550247();设定显示特征
                if (info.Attribute == null)
                {
                    IIndicatorPlugin plugin = PluginHelper.LoadIndicatorPlugin(info.IndicatorId);
                    if (((0 != 0) || ((plugin != null) || (15 == 0))) && (plugin is AttributePlugin))
                    {
                        info.Attribute = ((AttributePlugin)plugin).Attribute;
                    }
                }
            }
            else
            {
                this.ClearIndicatorProperties(objectOwner);
                return;
            }
            this.propertyGrid1.PropertySort = PropertySort.CategorizedAlphabetical;
            IndicatorInfoTypeDescriptor descriptor = new IndicatorInfoTypeDescriptor(info.CloneInfo())
            {
                AvailableInputs = availableInputs
            };
            
            {
                //this.x07dc1d59e2711079.SelectedObject = descriptor;
                this.propertyGrid1.SelectedObject = descriptor;
            }
            if(changeHandler != null)
            {
                descriptor.ValueChanged += changeHandler;
               
            }
            this.selectedObjectOwner = objectOwner;
        }

        public void ClearIndicatorProperties(string objectOwner)
        {
            if ((this.propertyGrid1.SelectedObject is IndicatorInfoTypeDescriptor) && (this.selectedObjectOwner == objectOwner))
            {
                this.propertyGrid1.SelectedObject = null;
            }
        }


        public void ShowChartObjectProperties(IChartObject chartObject, IChartDisplay chart)
        {
            this.propertyGrid1.SelectedObject = chartObject;
            this.chart = chart;
            this.SetVisible();
        }


        private void SetVisible()
        {
            this.propertyGrid1.Visible = true;
            //this._x33b87b423bc4e12e.Visible = false;
        }

        public void ClearChartObjectProperties()
        {
            if (this.propertyGrid1.SelectedObject is IChartObject)
            {
                this.propertyGrid1.SelectedObject = null;
            }
        }
        /// <summary>
        /// 显示策略配置属性
        /// </summary>
        /// <param name="setting"></param>
        public void ShowStrategyProject(StrategySetting setting)
        {
            this.propertyGrid1.PropertySort = PropertySort.CategorizedAlphabetical;
            StrategySettingsPropertiesWrapper wrapper = new StrategySettingsPropertiesWrapper(setting);
            this.propertyGrid1.SelectedObject = new StrategySettingTypeDescriptor(wrapper);////new StrategySettingTypeDescriptor(wrapper, this.propertyGrid1);
            //this.propertyGrid1.PropertyValueChanged +=new PropertyValueChangedEventHandler(propertyGrid1_PropertyValueChanged);
        }

        void descriptor_SetValue(object sender, PropertySpecEventArgs e)
        {
            throw new NotImplementedException();
        }

        void descriptor_GetValue(object sender, PropertySpecEventArgs e)
        {
            throw new NotImplementedException();
        }
        /*
        public void ShowProjectProperties(xe5110d3d5083f078 manager, object propertyData)
        {
            this.x91f347c6e97f1846 = manager;
            while (propertyData == null)
            {
                this.xca2d54d71d9f1190();
                return;
            }
            this.xac69a16f21061e4a(propertyData);
        }
        private void xca2d54d71d9f1190()
        {
            this.propertyGrid1.PropertySort = PropertySort.CategorizedAlphabetical;
            this.propertyGrid1.SelectedObject = new xd3f8808f0130c774(new ProjectSettingsPropertiesWrapper(this.x91f347c6e97f1846.ProjectSettings), this.x07dc1d59e2711079);
            this._x33b87b423bc4e12e.xea522ff5ab83cc71(this.x91f347c6e97f1846.ProjectSettings);
            this.propertyGrid1.Visible = false;
            this._x33b87b423bc4e12e.Visible = true;
        }
        **/
 


 







    }

    /**
    internal class xb98eb0573eeadff1 : PropertyGrid
    {
        // Fields
        private int _x20a8f8cdeaa87725 = 1;
        private ProjectSettings _x3cdfbfe1a563314a;
        private Dictionary<int, object> _x3dfff9f3ea97d91f = new Dictionary<int, object>();
        private Dictionary<object, int> _x5678822544f97c68 = new Dictionary<object, int>();
        private NamedFrequency _x799b6691683be19d = new NamedFrequency("Automatic", null);
        private Control _x7d10863c1f6ea214;
        private NamedFrequency _xc7a73360c267815d;
        private VisualTipProvider x9781f864e619b1c7 = new VisualTipProvider();
        private EventHandler<EventArgs> xd07b582cffa58129;

        // Events
        public event EventHandler<EventArgs> xd07b582cffa58129
        {
            add
            {
                EventHandler<EventArgs> handler2;
                EventHandler<EventArgs> handler = this.xd07b582cffa58129;
                do
                {
                    handler2 = handler;
                    EventHandler<EventArgs> handler3 = (EventHandler<EventArgs>)Delegate.Combine(handler2, xbcea506a33cf9111);
                    handler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.xd07b582cffa58129, handler3, handler2);
                }
                while (handler != handler2);
            }
            remove
            {
                EventHandler<EventArgs> handler2;
                EventHandler<EventArgs> handler = this.xd07b582cffa58129;
                do
                {
                    handler2 = handler;
                    EventHandler<EventArgs> handler3 = (EventHandler<EventArgs>)Delegate.Remove(handler2, xbcea506a33cf9111);
                    handler = Interlocked.CompareExchange<EventHandler<EventArgs>>(ref this.xd07b582cffa58129, handler3, handler2);
                }
                while (handler != handler2);
            }
        }

        // Methods
        public xb98eb0573eeadff1()
        {
            VisualTipOfficeRenderer renderer = new VisualTipOfficeRenderer
            {
                Preset = VisualTipOfficePreset.Hazel,
                BackgroundEffect = VisualTipOfficeBackgroundEffect.Gradient
            };
            this.x9781f864e619b1c7.Renderer = renderer;
            this.x9781f864e619b1c7.DisplayMode = VisualTipDisplayMode.Manual;
            this.x9781f864e619b1c7.Opacity = 1.0;
            this._x7d10863c1f6ea214 = new Control();
            this._x7d10863c1f6ea214.Visible = false;
            base.Controls.Add(this._x7d10863c1f6ea214);
        }

        private int _x1e2d627c28735656(object xba08ce632055a1d9)
        {
            int num;
            if (!this._x5678822544f97c68.TryGetValue(xba08ce632055a1d9, out num))
            {
                num = this._x20a8f8cdeaa87725;
                this._x20a8f8cdeaa87725++;
                this._x5678822544f97c68[xba08ce632055a1d9] = num;
                this._x3dfff9f3ea97d91f[num] = xba08ce632055a1d9;
            }
            return num;
        }

        protected internal override void OnDisplayedValuesNeeded(DisplayedValuesNeededEventArgs e)
        {
            if (e.PropertyEnum.Property.Id != this.x46f16e0cbc79348d(xd1b40af56a8385d4.x2e3a096ac4ad25ba))
            {
                goto Label_00C0;
            }
            List<object> list = new List<object> {
            new x1a2dd42a3a5d372d(this._x799b6691683be19d, new Action(this.x58dd117675db9138))
        };
            foreach (NamedFrequency frequency in Settings.Default.SavedFrequencies.SavedFrequencies)
            {
                list.Add(new x1a2dd42a3a5d372d(frequency, new Action(this.x58dd117675db9138)));
            }
            goto Label_00AC;
            if (0 == 0)
            {
                goto Label_00AC;
            }
        Label_008D:
            list.Add(new x1a2dd42a3a5d372d(this._xc7a73360c267815d, new Action(this.x58dd117675db9138)));
            goto Label_00B4;
        Label_00AC:
            if (this._xc7a73360c267815d != null)
            {
                goto Label_008D;
            }
        Label_00B4:
            e.DisplayedValues = list.ToArray();
        Label_00C0:
            base.OnDisplayedValuesNeeded(e);
        }

        protected internal override void OnHyperLinkPropertyClicked(PropertyHyperLinkClickedEventArgs e)
        {
            int num;
            PropertyEnumerator enumerator;
            PropertyEnumerator enumerator2;
            base.OnHyperLinkPropertyClicked(e);
            if (e.PropertyEnum.Property.Id == this.x46f16e0cbc79348d(xd1b40af56a8385d4.x73c1b18bc78aebc5))
            {
                EventHandler<EventArgs> handler = this.xd07b582cffa58129;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
                return;
            }
        Label_00B7:
            if (e.PropertyEnum.Property.Id == this.x46f16e0cbc79348d(xd1b40af56a8385d4.x9af5c27973b52df4))
            {
                string name = null;
                num = 1;
                do
                {
                    name = "NewParameter" + num;
                    if ((((uint)num) & 0) != 0)
                    {
                        return;
                    }
                    num++;
                }
                while (this._x3cdfbfe1a563314a.SystemParameters.Any<SystemParameterInfo>(p => p.Name == name));
                SystemParameterInfo item = new SystemParameterInfo(name, 0.0);
                this._x3cdfbfe1a563314a.SystemParameters.Add(item);
                enumerator = this.x9d988ebc48b1bd77(item, e.PropertyEnum);
                if (((uint)num) >= 0)
                {
                    this.xc7d1c662d0d05fee();
                }
                if ((((uint)num) - ((uint)num)) >= 0)
                {
                    base.ExpandProperty(enumerator, true);
                    goto Label_014A;
                }
            }
            else
            {
                if (e.PropertyEnum.Property.Id != this.x46f16e0cbc79348d(xd1b40af56a8385d4.x4724704de37e5cd4))
                {
                    return;
                }
                SystemParameterInfo info2 = this.x0f42791644d09868(e.PropertyEnum.Parent.Property.Id);
                if (((((uint)num) - ((uint)num)) <= uint.MaxValue) && (e.HyperLinkId != this.x46f16e0cbc79348d(xd1b40af56a8385d4.x8c8ec15461438979)))
                {
                    if (e.HyperLinkId != this.x46f16e0cbc79348d(xd1b40af56a8385d4.xb4491b921d9d7b6f))
                    {
                        if (e.HyperLinkId == this.x46f16e0cbc79348d(xd1b40af56a8385d4.xab9305271002d206))
                        {
                            this.x941577281a424554(info2, 1);
                            return;
                        }
                        return;
                    }
                    this.x941577281a424554(info2, -1);
                    return;
                }
                this.x26c4f0354d9a2c23(info2);
            }
            if (0 == 0)
            {
                if ((((uint)num) - ((uint)num)) >= 0)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        Label_014A:
            enumerator2 = enumerator.Children;
            while ((enumerator2.Property != null) && (enumerator2.Property.Id != this.x46f16e0cbc79348d(xd1b40af56a8385d4.x7f8bc3d5d37a9c51)))
            {
                enumerator2.MoveNext();
                if (2 == 0)
                {
                    goto Label_00B7;
                }
            }
            base.EnsureVisible(enumerator2);
            base.SelectAndFocusProperty(enumerator2, true);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            SystemParameterInfo info;
            PropertyEnumerator children;
            base.OnPropertyChanged(e);
            int id = e.PropertyEnum.Property.Id;
            if (((id != this.x46f16e0cbc79348d(xd1b40af56a8385d4.x8413962f823fe8c2)) && (id != this.x46f16e0cbc79348d(xd1b40af56a8385d4.x1697b4c76b42537f))) && (id != this.x46f16e0cbc79348d(xd1b40af56a8385d4.x50f647e185f9022b)))
            {
                x1a2dd42a3a5d372d xaddaadd;
                if (id == this.x46f16e0cbc79348d(xd1b40af56a8385d4.x2e3a096ac4ad25ba))
                {
                    xaddaadd = (x1a2dd42a3a5d372d)e.PropertyEnum.Property.Value.GetValue();
                    this._x3cdfbfe1a563314a.FrequencySettings = xaddaadd.xfa6d102debc45529;
                }
                else
                {
                    if (((0 == 0) && (id == this.x46f16e0cbc79348d(xd1b40af56a8385d4.x7f8bc3d5d37a9c51))) || (id == this.x46f16e0cbc79348d(xd1b40af56a8385d4.x43a6739cf505a8bb)))
                    {
                        info = this.x0f42791644d09868(e.PropertyEnum.Parent.Property.Id);
                        if (((((uint)id) + ((uint)id)) >= 0) && (info == null))
                        {
                            goto Label_0099;
                        }
                        e.PropertyEnum.Parent.Property.DisplayName = info.Name;
                        goto Label_00AD;
                    }
                    return;
                }
                if (((0 != 0) || (xaddaadd.xfa6d102debc45529 != null)) && !Settings.Default.SavedFrequencies.SavedFrequencies.Contains(xaddaadd.xd0ebfbc150f43d6c))
                {
                    this._xc7a73360c267815d = xaddaadd.xd0ebfbc150f43d6c;
                    if ((((uint)id) + ((uint)id)) > uint.MaxValue)
                    {
                        goto Label_020C;
                    }
                }
                return;
            }
            goto Label_020C;
        Label_000C:
            children.MoveNext();
        Label_0013:
            if (children != children.RightBound)
            {
                if (this.x2f356dc98cc87b99(children.Property.Id) != xd1b40af56a8385d4.x7f8bc3d5d37a9c51)
                {
                    goto Label_000C;
                }
                children.Property.Value.SetValue(info.Name);
                if (8 != 0)
                {
                    goto Label_000C;
                }
            }
            else
            {
                return;
            }
        Label_0099:
            if ((((uint)id) & 0) == 0)
            {
                return;
            }
        Label_00AD:
            e.PropertyEnum.Parent.Property.Comment = info.Description;
            children = e.PropertyEnum.Parent.Children;
            goto Label_0013;
        Label_020C:
            this.x88f6d9ae87243b58(e.PropertyEnum);
        }

        protected override void OnValueValidation(ValueValidationEventArgs e)
        {
            VisualTipDisplayOptions options;
            base.OnValueValidation(e);
            if (((0 != 0) || ((e.ValueValidationResult & PropertyValue.ValueValidationResult.ErrorCode) != 0)) && (base.ValueNotValidBehaviorMode == PropertyGrid.ValueNotValidBehaviorModes.KeepFocus))
            {
                if (this.x9781f864e619b1c7.IsTipDisplayed)
                {
                    this.x9781f864e619b1c7.HideTip();
                    goto Label_003C;
                }
                if (0 == 0)
                {
                    goto Label_003C;
                }
                goto Label_0137;
            }
            this.x9781f864e619b1c7.HideTip();
            return;
        Label_003C:
            if (this.x9781f864e619b1c7.IsTipDisplayed)
            {
                return;
            }
        Label_0137:
            options = VisualTipDisplayOptions.ForwardEscapeKey | VisualTipDisplayOptions.HideOnTextChanged | VisualTipDisplayOptions.HideOnLostFocus | VisualTipDisplayOptions.HideOnMouseDown | VisualTipDisplayOptions.HideOnKeyPress | VisualTipDisplayOptions.HideOnKeyDown;
            Control textBox = (base.InPlaceControl as IInPlaceControl).TextBox;
        Label_014E:
            if (textBox == null)
            {
                textBox = base.InPlaceControl as TextBoxBase;
            }
            while (textBox == null)
            {
                textBox = base.InPlaceControl;
                options &= ~(VisualTipDisplayOptions.HideOnMouseDown | VisualTipDisplayOptions.HideOnKeyPress | VisualTipDisplayOptions.HideOnKeyDown);
                break;
            }
            if (textBox != null)
            {
                VisualTip visualTip = new VisualTip
                {
                    Text = e.Message
                };
                if (0 == 0)
                {
                    visualTip.FooterText = "Enter a valid value or press ESC to cancel.";
                    visualTip.Image = SystemIcons.Error.ToBitmap();
                    do
                    {
                        visualTip.Font = new Font(visualTip.Font, FontStyle.Regular);
                        visualTip.Title = e.PropertyEnum.Property.DisplayName;
                    }
                    while (0 != 0);
                    this.x9781f864e619b1c7.SetVisualTip(e.PropertyEnum, visualTip);
                    Rectangle exclude = textBox.RectangleToScreen(base.InPlaceControl.ClientRectangle);
                    this.x9781f864e619b1c7.ShowTip(visualTip, exclude, textBox, options);
                }
                else
                {
                    goto Label_014E;
                }
            }
        }

        private SystemParameterInfo x0f42791644d09868(int xeaf1b27180c0557b)
        {
            object obj2;
            if (this._x3dfff9f3ea97d91f.TryGetValue(xeaf1b27180c0557b, out obj2))
            {
                return (obj2 as SystemParameterInfo);
            }
            return null;
        }

        private void x26c4f0354d9a2c23(SystemParameterInfo x6fb363657816bda0)
        {
            if (MessageBox.Show(this, "Are you sure you want to delete the \"" + x6fb363657816bda0.Name + "\" system parameter?", "Delete Hyperlink?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (!this._x3cdfbfe1a563314a.SystemParameters.Remove(x6fb363657816bda0))
                {
                    xf266856f631ec016.ShowErrorBox("Unable to remove system parameter from list.");
                }
                else
                {
                    base.DeleteProperty(base.FindProperty(this.x46f16e0cbc79348d(x6fb363657816bda0)));
                    this.xc7d1c662d0d05fee();
                }
            }
        }

        private xd1b40af56a8385d4 x2f356dc98cc87b99(int xeaf1b27180c0557b)
        {
            object obj2;
            if (this._x3dfff9f3ea97d91f.TryGetValue(xeaf1b27180c0557b, out obj2) && (obj2 is xd1b40af56a8385d4))
            {
                return (xd1b40af56a8385d4)obj2;
            }
            return xd1b40af56a8385d4.x4d0b9d4447ba7566;
        }

        private int x46f16e0cbc79348d(SystemParameterInfo x27ecc0340bbfc797)
        {
            return this._x1e2d627c28735656(x27ecc0340bbfc797);
        }

        private int x46f16e0cbc79348d(xd1b40af56a8385d4 xe01ae93d9fe5a880)
        {
            return this._x1e2d627c28735656(xe01ae93d9fe5a880);
        }

        private void x58dd117675db9138()
        {
            base.BeginInvoke(new Action(this.xc68f041957ecb06a));
        }

        private void x88f6d9ae87243b58(PropertyEnumerator x3705ecc024639e8e)
        {
            if ((this._x3cdfbfe1a563314a.LeadBars > 0) && (this._x3cdfbfe1a563314a.DataStartDate != DateTime.MinValue))
            {
                VisualTipDisplayOptions options;
                PropertyEnumerator enumerator;
                Rectangle rectangle;
                VisualTip tip;
                if (this._x3cdfbfe1a563314a.TradeStartDate != DateTime.MinValue)
                {
                    options = VisualTipDisplayOptions.ForwardEscapeKey | VisualTipDisplayOptions.HideOnTextChanged | VisualTipDisplayOptions.HideOnLostFocus | VisualTipDisplayOptions.HideOnMouseDown | VisualTipDisplayOptions.HideOnKeyPress | VisualTipDisplayOptions.HideOnKeyDown;
                    enumerator = x3705ecc024639e8e;
                    rectangle = base.InternalGrid.RectangleToScreen(base.GetItemRect(enumerator));
                    rectangle = base.RectangleToClient(rectangle);
                    do
                    {
                        rectangle.X += base.LeftColumnWidth;
                        rectangle.Width -= base.LeftColumnWidth;
                    }
                    while (0x7fffffff == 0);
                    this._x7d10863c1f6ea214.SetBounds(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
                    tip = new VisualTip
                    {
                        Text = "The data start date, trade start date, and lead bars are all set.  The lead bars value will be ignored."
                    };
                }
                else
                {
                    return;
                }
                tip.FooterText = "Press ESC or click here to dismiss.";
                tip.Image = SystemIcons.Warning.ToBitmap();
                tip.Font = new Font(tip.Font, FontStyle.Regular);
                tip.Title = enumerator.Property.DisplayName;
                this.x9781f864e619b1c7.SetVisualTip(enumerator, tip);
                this.x9781f864e619b1c7.ShowTip(tip, base.RectangleToScreen(rectangle), this._x7d10863c1f6ea214, options);
            }
        }

        private void x941577281a424554(SystemParameterInfo x6fb363657816bda0, int x23e85093ba3a7d1d)
        {
            int num2;
            PropertyEnumerator enumerator;
            int index = this._x3cdfbfe1a563314a.SystemParameters.IndexOf(x6fb363657816bda0);
            if (index < 0)
            {
                xf266856f631ec016.ShowErrorBox("Unable to find system parameter in list.");
            }
            x23e85093ba3a7d1d = Math.Sign(x23e85093ba3a7d1d);
            while (x23e85093ba3a7d1d < 0)
            {
                if (index != 0)
                {
                    break;
                }
                if (((uint)num2) < 0)
                {
                    goto Label_0053;
                }
                return;
            }
            if ((x23e85093ba3a7d1d > 0) && (index == (this._x3cdfbfe1a563314a.SystemParameters.Count - 1)))
            {
                return;
            }
            num2 = index + x23e85093ba3a7d1d;
            this._x3cdfbfe1a563314a.SystemParameters.RemoveAt(index);
            this._x3cdfbfe1a563314a.SystemParameters.Insert(num2, x6fb363657816bda0);
            if ((((uint)num2) >= 0) && (num2 != (this._x3cdfbfe1a563314a.SystemParameters.Count - 1)))
            {
                enumerator = base.FindProperty(this.x46f16e0cbc79348d(this._x3cdfbfe1a563314a.SystemParameters[num2 + 1]));
            }
            else
            {
                enumerator = base.FindProperty(this.x46f16e0cbc79348d(xd1b40af56a8385d4.x9af5c27973b52df4));
            }
            base.DeleteProperty(base.FindProperty(this.x46f16e0cbc79348d(x6fb363657816bda0)));
            PropertyEnumerator propEnum = this.x9d988ebc48b1bd77(x6fb363657816bda0, enumerator);
            this.xc7d1c662d0d05fee();
            base.ExpandProperty(propEnum, true);
            PropertyEnumerator children = propEnum.Children;
        Label_0017:
            if (children.Property != null)
            {
                goto Label_0053;
            }
        Label_0020:
            if (children != base.RightBound)
            {
                base.EnsureVisible(children);
                base.SelectAndFocusProperty(children, false);
            }
            return;
        Label_0053:
            if (children.Property.Id != this.x46f16e0cbc79348d(xd1b40af56a8385d4.x4724704de37e5cd4))
            {
                children.MoveNext();
                goto Label_0017;
            }
            goto Label_0020;
        }

        private PropertyEnumerator x9d988ebc48b1bd77(SystemParameterInfo x6fb363657816bda0, PropertyEnumerator x9ee886d5d4b7e3fe)
        {
            SystemParameterInfo sysParam = x6fb363657816bda0;
            PropertyEnumerator underCategory = base.InsertProperty(x9ee886d5d4b7e3fe, this.x46f16e0cbc79348d(sysParam), sysParam.Name, Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(sysParam), (MethodInfo)methodof(SystemParameterInfo.get_Value)), typeof(object)), new ParameterExpression[0]), sysParam.Description, new Attribute[0]);
            base.AppendProperty(underCategory, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x7f8bc3d5d37a9c51), "Name", Expression.Lambda<Func<object>>(Expression.Property(Expression.Constant(sysParam), (MethodInfo)methodof(SystemParameterInfo.get_Name)), new ParameterExpression[0]), "The name of the system parameter.", new Attribute[0]).Property.Value.Validator = new ParameterNameValidator(this._x3cdfbfe1a563314a);
            PropertyEnumerator enumerator2 = base.AppendProperty(underCategory, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x6d219611702b3552), "Value", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(sysParam), (MethodInfo)methodof(SystemParameterInfo.get_Value)), typeof(object)), new ParameterExpression[0]), "The value of the system parameter, when not running an optimization.", new Attribute[0]);
            enumerator2 = base.AppendProperty(underCategory, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x17b3c7afda00e6c9), "Low Value", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(sysParam), (MethodInfo)methodof(SystemParameterInfo.get_Low)), typeof(object)), new ParameterExpression[0]), "The lowest value for the system parameter when running an optimization.", new Attribute[0]);
            enumerator2 = base.AppendProperty(underCategory, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x0d9438a00b1c6737), "High Value", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(sysParam), (MethodInfo)methodof(SystemParameterInfo.get_High)), typeof(object)), new ParameterExpression[0]), "The highest value for the system parameter when running an optimization.", new Attribute[0]);
            enumerator2 = base.AppendProperty(underCategory, this.x46f16e0cbc79348d(xd1b40af56a8385d4.xa8e64ea3f3ec67cb), "Steps", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(sysParam), (MethodInfo)methodof(SystemParameterInfo.get_NumSteps)), typeof(object)), new ParameterExpression[0]), "The number of different values (ranging from the low to the high value) to use for this parameter when running an optimization.", new Attribute[0]);
            enumerator2 = base.AppendProperty(underCategory, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x43a6739cf505a8bb), "Description", Expression.Lambda<Func<object>>(Expression.Property(Expression.Constant(sysParam), (MethodInfo)methodof(SystemParameterInfo.get_Description)), new ParameterExpression[0]), "A description for the system parameter.", new Attribute[0]);
            return underCategory;
        }

        [CompilerGenerated]
        private void xc68f041957ecb06a()
        {
            x1a2dd42a3a5d372d xaddaadd;
            FrequencySelectionDialog dialog = new FrequencySelectionDialog();
            do
            {
                dialog.AdHocMode = true;
                if (this._xc7a73360c267815d == null)
                {
                    goto Label_0056;
                }
            }
            while (-1 == 0);
            dialog.PluginSettings = this._xc7a73360c267815d.Frequency;
            if (0xff == 0)
            {
                goto Label_0090;
            }
        Label_0056:
            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }
        Label_0090:
            xaddaadd = new x1a2dd42a3a5d372d(this.xfee88a5fa818f19a(dialog.PluginSettings), new Action(this.x58dd117675db9138));
            while (xaddaadd.xd0ebfbc150f43d6c != this._xc7a73360c267815d)
            {
                this._xc7a73360c267815d = null;
                if (0 == 0)
                {
                    break;
                }
            }
            PropertyEnumerator enumerator = base.FindProperty(this.x46f16e0cbc79348d(xd1b40af56a8385d4.x2e3a096ac4ad25ba));
            enumerator.Property.Value.ResetDisplayedValues(PropertyGrid.ResetDisplayedValuesTriggerMode.TriggerOnceNow);
            enumerator.Property.Value.SetValue(xaddaadd);
        }

        private void xc7d1c662d0d05fee()
        {
            PropertyEnumerator enumerator;
            PropertyEnumerator enumerator3;
            PropertyEnumerator children;
            bool flag;
            int num;
            List<HyperlinkInfo> list;
        Label_0000:
            enumerator = base.FindProperty(this.x46f16e0cbc79348d(xd1b40af56a8385d4.x4724704de37e5cd4));
            if ((((uint)flag) | 4) != 0)
            {
                if (((uint)flag) >= 0)
                {
                    if (enumerator == base.RightBound)
                    {
                        PropertyEnumerator enumerator2 = base.FindProperty(this.x46f16e0cbc79348d(xd1b40af56a8385d4.xd375d830e556392b));
                        enumerator3 = base.FindProperty(this.x46f16e0cbc79348d(xd1b40af56a8385d4.x9af5c27973b52df4));
                        children = enumerator2.Children;
                        flag = true;
                        num = 0;
                        if ((((uint)num) & 0) != 0)
                        {
                            return;
                        }
                        goto Label_00D9;
                    }
                    base.DeleteProperty(enumerator);
                    if ((((uint)flag) + ((uint)num)) <= uint.MaxValue)
                    {
                        goto Label_0000;
                    }
                    goto Label_0031;
                }
                goto Label_010D;
            }
            goto Label_00D9;
        Label_0031:
            if ((((uint)num) + ((uint)num)) > uint.MaxValue)
            {
                goto Label_010D;
            }
        Label_0079:
            while (num < (this._x3cdfbfe1a563314a.SystemParameters.Count - 1))
            {
                list.Add(new HyperlinkInfo(this.x46f16e0cbc79348d(xd1b40af56a8385d4.xab9305271002d206), "Move down"));
                if (((uint)flag) >= 0)
                {
                    break;
                }
            }
            list.Add(new HyperlinkInfo(this.x46f16e0cbc79348d(xd1b40af56a8385d4.x8c8ec15461438979), "Delete"));
            base.AppendMultipleHyperLinkProperty(children, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x4724704de37e5cd4), "Reorder/delete", "", list.ToArray());
            flag = false;
            children.MoveNext();
            num++;
        Label_00D9:
            if (children == enumerator3)
            {
                return;
            }
            list = new List<HyperlinkInfo>();
        Label_010D:
            while (!flag)
            {
                list.Add(new HyperlinkInfo(this.x46f16e0cbc79348d(xd1b40af56a8385d4.xb4491b921d9d7b6f), "Move up"));
                goto Label_0031;
            }
            goto Label_0079;
        }

        public void xea522ff5ab83cc71(ProjectSettings x3cdfbfe1a563314a)
        {
            PropertyEnumerator enumerator;
            PropertyEnumerator enumerator3;
            PropertyEnumerator enumerator5;
            ProjectSettings projectSettings;
            if (0x7fffffff != 0)
            {
                goto Label_09BF;
            }
        Label_00A3:
            enumerator = base.AppendProperty(enumerator5, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x04a98c66dbb84add), "Profit Target", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_ProfitTarget)), typeof(object)), new ParameterExpression[0]), "Specifies a profit target at which positions will be closed.", new Attribute[0]);
            enumerator.Property.AddValue("unit", projectSettings, "ProfitTargetType", null);
            enumerator.Property.Look = new PropertyUnitLook();
            enumerator.Property.Feel = base.GetRegisteredFeel("editunit");
            enumerator.Property.GetValue("unit").SetValue(projectSettings.ProfitTargetType);
            enumerator = base.AppendProperty(enumerator5, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x9c76cbae631d17c2), "Stop Loss", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_StopLoss)), typeof(object)), new ParameterExpression[0]), "Specifies a loss at which positions will be closed.", new Attribute[0]);
            enumerator.Property.AddValue("unit", projectSettings, "StopLossType", null);
            enumerator.Property.Look = new PropertyUnitLook();
            enumerator.Property.Feel = base.GetRegisteredFeel("editunit");
            enumerator.Property.GetValue("unit").SetValue(projectSettings.StopLossType);
            enumerator = base.AppendProperty(enumerator5, this.x46f16e0cbc79348d(xd1b40af56a8385d4.xca50321ad81a7af0), "Restrict Orders Sent", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_RestrictOpenOrders)), typeof(object)), new ParameterExpression[0]), "Specifies whether the Max Open Positions settings should limit the number of position open orders submitted.", new Attribute[0]);
            PropertyEnumerator underCategory = base.AppendRootCategory(this.x46f16e0cbc79348d(xd1b40af56a8385d4.xd375d830e556392b), "System Parameters");
            underCategory.Property.Comment = "User-defined parameters which can be used in optimization.";
            enumerator = base.AppendHyperLinkProperty(underCategory, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x9af5c27973b52df4), "Add System Parameter...", "Add a user-defined parameter for the system.");
            foreach (SystemParameterInfo info in projectSettings.SystemParameters)
            {
                this.x9d988ebc48b1bd77(info, enumerator);
            }
            this.xc7d1c662d0d05fee();
            return;
        Label_02B0:
            enumerator = base.AppendProperty(enumerator5, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x756cecbfefdf3bac), "Max Open", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_MaxOpenPositions)), typeof(object)), new ParameterExpression[0]), "The maximum number of positions allowed to be open at any given time.", new Attribute[0]);
            if (0 != 0)
            {
                goto Label_0891;
            }
            enumerator = base.AppendProperty(enumerator5, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x8afd783d221f7f4c), "Max Open/Symbol", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_MaxOpenPositionsPerSymbol)), typeof(object)), new ParameterExpression[0]), "The maximum number of positions allowed to be open at any given time for one symbol.", new Attribute[0]);
            if (-2 != 0)
            {
                goto Label_00A3;
            }
            goto Label_09BF;
        Label_038F:
            enumerator = base.AppendProperty(enumerator3, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x529afb2fcf5075af), "Live Data Start Date", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_LiveDataStartDate)), typeof(object)), new ParameterExpression[0]), "The date for the start of the data loaded to initialize a live system.", new Attribute[0]);
            if (0 == 0)
            {
                PropertyEnumerator enumerator4 = base.AppendRootCategory(0, "General");
                base.AppendManagedProperty(enumerator4, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x2e3a096ac4ad25ba), "Frequency", typeof(x1a2dd42a3a5d372d), new x1a2dd42a3a5d372d(this.xfee88a5fa818f19a(projectSettings.FrequencySettings), new Action(this.x58dd117675db9138)), "The bar frequency to use for the system.  Other frequencies can be created and used in your system code if you need multiple frequencies.  If the frequency is set to \"Automatic\" RightEdge will use the frequency of the bar data as the system frequency.", new Attribute[] { new PropertyDropDownContentAttribute(typeof(DropDownListWithButton)) }).Property.Feel = base.GetRegisteredFeel("list");
                enumerator = base.AppendProperty(enumerator4, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x6867cb623c895854), "Tick-level Simulation", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_UseTicksForSimulation)), typeof(object)), new ParameterExpression[0]), "Specifies that for symbols that have tick data in the data store, the tick data should be used for the simulation.", new Attribute[0]);
                enumerator = base.AppendProperty(enumerator4, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x3bc9273c159b6e7b), "Synchronize Bars", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_SynchronizeBars)), typeof(object)), new ParameterExpression[0]), "If set to true, empty bars will be created when a symbol does not have any data during a time period that another symbol did have data.  This ensures that a given index (look back value) corresponds to the same date/time for all symbols.", new Attribute[0]);
                enumerator5 = base.AppendRootCategory(0, "Position Management");
                enumerator = base.AppendProperty(enumerator5, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x058e2093def132e2), "Allocation", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_Allocation)), typeof(object)), new ParameterExpression[0]), "The amount to allocate for each position when position size is not specified.  How this value is interpreted depends on the Allocation Type property.", new Attribute[0]);
                enumerator = base.AppendProperty(enumerator5, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x375aae79088d33cd), "Allocation Type", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_AllocationType)), typeof(object)), new ParameterExpression[0]), "The allocation method to use when position size is not specified.  Use the Allocation property to set a value for this method.\r\nPercentage: A percentage of the total account value\r\nFixedValue: A fixed position value\r\nFixedSize: A fixed number of shares or contracts", new Attribute[0]);
                enumerator = base.AppendProperty(enumerator5, this.x46f16e0cbc79348d(xd1b40af56a8385d4.xadff8a4aa18c9840), "Bar Count Exit", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_BarTimeout)), typeof(object)), new ParameterExpression[0]), "Specifies the number of bars after which a position should be closed if it hasn't hit the profit target or stop loss.", new Attribute[0]);
                enumerator = base.AppendProperty(enumerator5, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x0fde5defaba73938), "Force Round Lots", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_ForceRoundLots)), typeof(object)), new ParameterExpression[0]), "Specifies if submitted orders will be forced to use round lots.  If true, the number of shares will be rounded to the nearest 100 share increment.", new Attribute[0]);
                if (-1 != 0)
                {
                    if (1 != 0)
                    {
                        goto Label_02B0;
                    }
                    goto Label_00A3;
                }
                goto Label_09BF;
            }
        Label_0884:
            enumerator3 = base.AppendRootCategory(0, "Live");
        Label_0891:
            enumerator = base.AppendProperty(enumerator3, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x49a43637ab2d1097), "Live Lead Bars", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_LiveLeadBars)), typeof(object)), new ParameterExpression[0]), "Number of bars to skip before beginning trades in live trading.", new Attribute[0]);
            if (-2147483648 != 0)
            {
                goto Label_038F;
            }
        Label_08FE:
            this._x5678822544f97c68.Clear();
            this._x3dfff9f3ea97d91f.Clear();
            this._x20a8f8cdeaa87725 = 1;
            PropertyEnumerator enumerator2 = base.AppendRootCategory(0, "Simulation");
            enumerator = base.AppendProperty(enumerator2, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x50f647e185f9022b), "Lead Bars", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_LeadBars)), typeof(object)), new ParameterExpression[0]), "Number of bars to skip before beginning trades in the simulation.", new Attribute[0]);
            if (0 != 0)
            {
                goto Label_02B0;
            }
            enumerator.Property.Value.Validator = new NonNegativeValidator();
            enumerator = base.AppendProperty(enumerator2, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x1697b4c76b42537f), "Data Start Date", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_DataStartDate)), typeof(object)), new ParameterExpression[0]), "The date to start the simulation.", new Attribute[0]);
            enumerator = base.AppendProperty(enumerator2, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x8413962f823fe8c2), "Trade Start Date", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_TradeStartDate)), typeof(object)), new ParameterExpression[0]), "The date to begin trading.", new Attribute[0]);
            enumerator = base.AppendProperty(enumerator2, this.x46f16e0cbc79348d(xd1b40af56a8385d4.xb3f2a0cfe93086dd), "Simulation End Date", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_RunEndDate)), typeof(object)), new ParameterExpression[0]), "The date to end simulation.", new Attribute[0]);
            enumerator = base.AppendProperty(enumerator2, this.x46f16e0cbc79348d(xd1b40af56a8385d4.x983d6e53d614060f), "Starting Capital", Expression.Lambda<Func<object>>(Expression.Convert(Expression.Property(Expression.Constant(projectSettings), (MethodInfo)methodof(ProjectSettings.get_StartingCapital)), typeof(object)), new ParameterExpression[0]), "The amount of capital to begin the simulation with.", new Attribute[0]);
            goto Label_0884;
        Label_09BF:
            projectSettings = x3cdfbfe1a563314a;
            this._x3cdfbfe1a563314a = projectSettings;
            base.Clear();
            if (-2147483648 != 0)
            {
                goto Label_08FE;
            }
            goto Label_038F;
        }

        private NamedFrequency xfee88a5fa818f19a(PluginSettings xb6b3da7953a69f26)
        {
            if (xb6b3da7953a69f26 == null)
            {
                return this._x799b6691683be19d;
            }
            foreach (NamedFrequency frequency in Settings.Default.SavedFrequencies.SavedFrequencies)
            {
                if (-1 == 0)
                {
                    return frequency;
                }
                if (PluginSettings.Equals(xb6b3da7953a69f26, frequency.Frequency))
                {
                    return frequency;
                }
            }
            this._xc7a73360c267815d = new NamedFrequency("Custom Frequency", xb6b3da7953a69f26);
            return this._xc7a73360c267815d;
        }

        // Nested Types
        internal class x1a2dd42a3a5d372d
        {
            // Fields
            [CompilerGenerated]
            private NamedFrequency x4ddeb5bc108533c9;
            [CompilerGenerated]
            private Action x63fc5cc886b4668f;

            // Methods
            public x1a2dd42a3a5d372d(NamedFrequency frequency, Action onButtonClicked)
            {
                this.xd0ebfbc150f43d6c = frequency;
                this.x4332d9d1a3e6fb44 = onButtonClicked;
            }

            public override string ToString()
            {
                return this.x759aa16c2016a289;
            }

            // Properties
            public Action x4332d9d1a3e6fb44
            {
                [CompilerGenerated]
                get
                {
                    return this.x63fc5cc886b4668f;
                }
                [CompilerGenerated]
                private set
                {
                    this.x63fc5cc886b4668f = value;
                }
            }

            public string x759aa16c2016a289
            {
                get
                {
                    return this.xd0ebfbc150f43d6c.Name;
                }
            }

            public NamedFrequency xd0ebfbc150f43d6c
            {
                [CompilerGenerated]
                get
                {
                    return this.x4ddeb5bc108533c9;
                }
                [CompilerGenerated]
                private set
                {
                    this.x4ddeb5bc108533c9 = value;
                }
            }

            public PluginSettings xfa6d102debc45529
            {
                get
                {
                    return this.xd0ebfbc150f43d6c.Frequency;
                }
            }
        }

        private enum xd1b40af56a8385d4
        {
            x4d0b9d4447ba7566,
            x73c1b18bc78aebc5,
            x50f647e185f9022b,
            x1697b4c76b42537f,
            x8413962f823fe8c2,
            xb3f2a0cfe93086dd,
            x983d6e53d614060f,
            x49a43637ab2d1097,
            x529afb2fcf5075af,
            x6867cb623c895854,
            x2e3a096ac4ad25ba,
            x3bc9273c159b6e7b,
            xed8bf8e15976f064,
            x058e2093def132e2,
            x375aae79088d33cd,
            xadff8a4aa18c9840,
            x0fde5defaba73938,
            x756cecbfefdf3bac,
            x8afd783d221f7f4c,
            x04a98c66dbb84add,
            x9c76cbae631d17c2,
            xca50321ad81a7af0,
            xd375d830e556392b,
            x7f8bc3d5d37a9c51,
            x6d219611702b3552,
            x17b3c7afda00e6c9,
            x0d9438a00b1c6737,
            xa8e64ea3f3ec67cb,
            x43a6739cf505a8bb,
            x9af5c27973b52df4,
            x4724704de37e5cd4,
            xb4491b921d9d7b6f,
            xab9305271002d206,
            x8c8ec15461438979
        }
    }**/


}
