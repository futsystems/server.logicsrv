using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using System.ComponentModel;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls;

namespace FutSystems.GUI
{
    public class TrackBarEditorElement : RadTrackBarElement
    {
        StopLossEditor editor;

        public TrackBarEditorElement(StopLossEditor editor)
        {
            this.CanFocus = true;
            this.editor = editor;
            this.Maximum = 100;
            this.TickStyle = Telerik.WinControls.Enumerations.TickStyles.Both;
            this.SmallTickFrequency = 5;
            this.LargeTickFrequency = 20;
            this.BodyElement.ScaleContainerElement.TopScaleElement.StretchVertically = true;
            this.BodyElement.ScaleContainerElement.BottomScaleElement.StretchVertically = true;
            this.StretchVertically = false;
        }

        public event EventHandler TrackPositionChanged;

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadTrackBarElement);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            RadGridView grid = this.ElementTree.Control as RadGridView;
            if (grid != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                    case Keys.Enter:
                    case Keys.Up:
                    case Keys.Down:
                        grid.GridBehavior.ProcessKeyDown(e);
                        break;

                    case Keys.Left:
                        if (this.Value > this.Minimum)
                        {
                            this.Value--;
                        }
                        break;

                    case Keys.Right:
                        if (this.Value < this.Maximum)
                        {
                            this.Value++;
                        }
                        break;

                    case Keys.Home:
                        this.Value = this.Minimum;
                        break;

                    case Keys.End:
                        this.Value = this.Maximum;
                        break;
                }
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            int desiredHeight = 40;
            foreach (RadElement element in this.Children)
            {
                element.Measure(new SizeF(availableSize.Width, desiredHeight));
            }
            return new SizeF(1, desiredHeight);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            SizeF size = finalSize;
            size.Width -= 40;
            return base.ArrangeOverride(size);
        }

        public override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            if (this.Parent != null && this.TrackPositionChanged != null)
            {
                this.TrackPositionChanged(this, EventArgs.Empty);
            }
        }
    }

    public class StopLossEditor : BaseGridEditor 
    {
        public override object Value
        {
            get
            {
                return this.TrackBarElement.Value;
            }
            set
            {
                if (value != null && value != DBNull.Value)
                {
                    this.TrackBarElement.Value = Convert.ToInt32(value);
                }
                else
                {
                    this.TrackBarElement.Value = 0;
                }
            }
        }

        public TrackBarEditorElement TrackBarElement
        {
            get
            {
                return this.EditorElement as TrackBarEditorElement;
            }
        }

        public int Minimum
        {
            get
            {
                return (int)this.TrackBarElement.Minimum;
            }
            set
            {
                this.TrackBarElement.Minimum = value;
            }
        }

        public int Maximum
        {
            get
            {
                return (int)this.TrackBarElement.Maximum;
            }
            set
            {
                this.TrackBarElement.Maximum = value;
            }
        }

        public int TickFrequency
        {
            get
            {
                return this.TrackBarElement.TickFrequency;
            }
            set
            {
                this.TrackBarElement.TickFrequency = value;
            }
        }

        public override void Initialize(object owner, object value)
        {
            base.Initialize(owner, value);

            this.EditorElement.Focus();
            this.TrackBarElement.Value = (int)value;
        }

        public override void BeginEdit()
        {
            base.BeginEdit();

            ((GridCellElement)this.EditorElement.Parent).Text = this.Value + " %";
            ((TrackBarEditorElement)this.EditorElement).TrackPositionChanged += new EventHandler(TrackBarEditor_TrackPositionChanged);
        }

        public override bool EndEdit()
        {
            ((TrackBarEditorElement)this.EditorElement).TrackPositionChanged -= new EventHandler(TrackBarEditor_TrackPositionChanged);
            return base.EndEdit();
        }

        void TrackBarEditor_TrackPositionChanged(object sender, EventArgs e)
        {
            ((GridCellElement)this.EditorElement.Parent).Text = this.Value + " %";
            OnValueChanged();
        }

        protected override RadElement CreateEditorElement()
        {
            return new TrackBarEditorElement(this);
        }

        public override Type DataType
        {
            get
            {
                return typeof(Int32);
            }
        }
    }
}
