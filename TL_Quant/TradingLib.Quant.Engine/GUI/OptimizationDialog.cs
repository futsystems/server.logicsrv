using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Windows.Forms;

using TradingLib.Quant.Base;
using TradingLib.Quant.Plugin;


namespace TradingLib.Quant.Engine
{
    public partial class OptimizationDialog :Form
    {
        private List<OptimizationParameter> _parameters;
        public OptimizationDialog()
        {
            InitializeComponent();
        }

        private void ok_Click(object sender, EventArgs e)
        {
            
            base.DialogResult = DialogResult.OK;
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            base.DialogResult = DialogResult.Cancel;

        }

        public int ThreadToOpen { get { return (int)threadnum.SelectedItem; } }
        //初始化优化参数表格
        public OptimizationDialog(IList<StrategyParameterInfo> infoList,string friendlyname)
        {
            

            this._parameters = new List<OptimizationParameter>(infoList.Count);
            foreach (StrategyParameterInfo info in infoList)
            {
                this.Parameters.Add(new OptimizationParameter(info));
            }
            this.InitializeComponent();
            InitGrid();
            foreach (OptimizationParameter parameter in this.Parameters)
            {
                //ParameterRow row = new ParameterRow(parameter, this);
                //this.sandGrid.Rows.Add(row);
                //row.Init();
                InsertRow(parameter);
            }
            this.UpdateTotalRuns();

            this.Text = friendlyname + " 优化参数设置";
            int k = Environment.ProcessorCount;
            for (int i = 1; i <= k * 2; i++)
            {
                threadnum.Items.Add(i);


            }
            threadnum.SelectedItem = k;
        }

        void InitGrid()
        {
            grid.RowsCount = 1;
            grid.ColumnsCount = 5;
            grid.FixedRows = 1;
            grid.FixedColumns = 1;
            grid.SelectionMode = SourceGrid.GridSelectionMode.Row;
            //grid.AutoStretchColumnsToFitWidth = true;
            //grid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            //grid.Columns[0].Width = 150;
            //grid.Columns[1].Width = 120;
            //grid.Columns[2].Width = 120;
            //grid.Columns[3].Width = 120;
            //grid.Columns[4].Width = 120;

            //grid.Columns[0].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            //grid.Columns[1].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            //grid.Columns[2].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            //grid.Columns[3].AutoSizeMode = SourceGrid.AutoSizeMode.None;
            //grid.Columns[4].AutoSizeMode = SourceGrid.AutoSizeMode.EnableAutoSize | SourceGrid.AutoSizeMode.EnableStretch;
            grid.AutoStretchColumnsToFitWidth = true;
            //grid.AutoSizeCells();
            grid.Columns.StretchToFit();


            SourceGrid.Cells.Views.Cell categoryView = new SourceGrid.Cells.Views.Cell();
            categoryView.Background = new DevAge.Drawing.VisualElements.BackgroundLinearGradient(Color.RoyalBlue, Color.LightBlue, 0);
            categoryView.ForeColor = Color.FromKnownColor(KnownColor.ActiveCaptionText);
            categoryView.TextAlignment = DevAge.Drawing.ContentAlignment.MiddleCenter;
            categoryView.Border = DevAge.Drawing.RectangleBorder.NoBorder;
            Font font = new Font(Font, FontStyle.Bold);

            categoryView.Font = font;


            int row = 0;
            grid[row, 0] = new SourceGrid.Cells.ColumnHeader("参数名称");
            grid[row, 1] = new SourceGrid.Cells.ColumnHeader("最小值");
            grid[row, 2] = new SourceGrid.Cells.ColumnHeader("最大值");
            grid[row, 3] = new SourceGrid.Cells.ColumnHeader("步长");
            grid[row, 4] = new SourceGrid.Cells.ColumnHeader("步数");
            
            
           
        }

        Dictionary<int, OptimizationParameter> optimparameterMap = new Dictionary<int, OptimizationParameter>();
        
        void InsertRow(OptimizationParameter p)
        {
            int row = grid.RowsCount;
            optimparameterMap.Add(row, p);//记录行号与对应的参数

            grid.Rows.Insert(row);
            SourceGrid.Cells.Views.Cell captionModel = new SourceGrid.Cells.Views.Cell();
            captionModel.BackColor = Color.LightGray;

            UpdateColumns update = new UpdateColumns(grid,p);
            update.ValueChanged += new VoidDelegate(UpdateTotalRuns);
            grid[row, 0] = new SourceGrid.Cells.Cell(p.Name);// ("序号");
            grid[row, 0].View = captionModel;
            grid[row, 1] = new SourceGrid.Cells.Cell(p.Low, typeof(double));// ("时间");
            grid[row, 1].AddController(update);
            grid[row, 2] = new SourceGrid.Cells.Cell(p.High, typeof(double));// ("合约");
            grid[row, 2].AddController(update);
            grid[row, 3] = new SourceGrid.Cells.Cell(p.StepSize.ToString("F2"), typeof(double));// ("方向");
            grid[row, 3].AddController(update);
            grid[row, 4] = new SourceGrid.Cells.Cell(p.NumSteps, typeof(int));// ("数量");
            grid[row, 4].AddController(update);
            
            
        }



        //更新运算次数
        public void UpdateTotalRuns()
        {
            long num = 1;
            foreach (OptimizationParameter parameter in this.Parameters)
            {
                if (parameter.NumSteps > 0)
                {
                    num *= parameter.NumSteps;
                }
            }
            this.runnum.Text = num.ToString();
        }

 

 



        public List<OptimizationParameter> Parameters
        {
            get
            {
                return this._parameters;
            }
        }

        class UpdateColumns : SourceGrid.Cells.Controllers.ControllerBase
        {
            //private int mDependencyColumn;
            SourceGrid.Grid grid;
            OptimizationParameter parameter;
            public UpdateColumns(SourceGrid.Grid g,OptimizationParameter p)
            {
                //mDependencyColumn = dependencyColumn;
                grid = g;
                parameter = p;
            }

            public event VoidDelegate ValueChanged;


            void SetParameterValue(int row, int col)
            {
                OptimizationParameter p = parameter;
                if (col == 1)
                    p.Low = (double)grid[row, col].Value;
                else if (col == 2)
                    p.High = (double)grid[row, col].Value;
                else if (col == 3)
                    p.StepSize = (double)grid[row, col].Value;
                else if (col == 4)
                    p.NumSteps = (int)grid[row, col].Value;

            }
            void UpdateRowValue(int index)
            {
                OptimizationParameter p = parameter;

                grid[index, 1].Value = p.Low;
                grid[index, 2].Value = p.High;
                grid[index, 3].Value = p.StepSize;//.ToString("F2");
                grid[index, 4].Value = p.NumSteps;
            }

            public override void OnValueChanged(SourceGrid.CellContext sender, EventArgs e)
            {
                base.OnValueChanged(sender, e);

                //I use the OnValueChanged to link the value of 2 cells
                // changing the value of the other cell
                int row = sender.Position.Row;
                int col = sender.Position.Column;
                SetParameterValue(row, col);
                UpdateRowValue(row);
                if (ValueChanged != null)
                    ValueChanged();
            }

           
        }


    }

}
