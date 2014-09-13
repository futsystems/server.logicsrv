using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using TradingLib.Quant.Base;

namespace TradingLib.Quant.GUI
{
    public partial class OptimizationReport : KryptonForm
    {
        public OptimizationReport()
        {
            InitializeComponent();
            Initgrid();
        }

        void Initgrid()
        {
            grid.RowsCount = 1;
            grid.ColumnsCount = 15;
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

            grid[row, 0] = new SourceGrid.Cells.Cell("交易回合列表");
            grid[row, 0].View = categoryView;
            grid[row, 0].ColumnSpan = 2;

            row = grid.RowsCount;
            grid.Rows.Insert(row);
            grid[row, 0] = new SourceGrid.Cells.ColumnHeader("Run#");
            grid[row, 1] = new SourceGrid.Cells.ColumnHeader("初始权益");
            grid[row, 2] = new SourceGrid.Cells.ColumnHeader("期末权益");
            grid[row, 3] = new SourceGrid.Cells.ColumnHeader("交易数目");
            grid[row, 4] = new SourceGrid.Cells.ColumnHeader("胜率");
            grid[row, 5] = new SourceGrid.Cells.ColumnHeader("盈利次数");
            grid[row, 6] = new SourceGrid.Cells.ColumnHeader("累计盈利");
            grid[row, 7] = new SourceGrid.Cells.ColumnHeader("亏损次数");
            grid[row, 8] = new SourceGrid.Cells.ColumnHeader("累计亏损");
            grid[row, 9] = new SourceGrid.Cells.ColumnHeader("净盈利");
            grid[row, 10] = new SourceGrid.Cells.ColumnHeader("最大回测");
            grid[row, 11] = new SourceGrid.Cells.ColumnHeader("最大回测%");
            grid[row, 12] = new SourceGrid.Cells.ColumnHeader("最大连盈");
            grid[row, 13] = new SourceGrid.Cells.ColumnHeader("最大连亏");
           

        }

        void InsertResult(OptimizationResult r)
        {
            int row = grid.RowsCount;
            grid.Rows.Insert(row);
            grid[row, 0] = new SourceGrid.Cells.Cell(r.RunNumber);// ("运行序号");
            grid[row, 1] = new SourceGrid.Cells.Cell(r.FinalStatistic.StartingCapital);// ("合约");
            grid[row, 2] = new SourceGrid.Cells.Cell(r.FinalStatistic.EndingCapital);// ("数量");
            grid[row, 3] = new SourceGrid.Cells.Cell(r.FinalStatistic.TotalFinishedTrades);// ("开仓时间");
            grid[row, 4] = new SourceGrid.Cells.Cell(r.FinalStatistic.WinningTradesPct);// ("价格");
            grid[row, 5] = new SourceGrid.Cells.Cell(r.FinalStatistic.WinningTrades);// ("开仓手续费");
            grid[row, 6] = new SourceGrid.Cells.Cell(r.FinalStatistic.RealizedGrossProfit);// ("平仓时间");
            grid[row, 7] = new SourceGrid.Cells.Cell(r.FinalStatistic.LosingTrades);// ("价格");
            grid[row, 8] = new SourceGrid.Cells.Cell(r.FinalStatistic.RealizedGrossLoss);// ("平仓手续费");
            grid[row, 9] = new SourceGrid.Cells.Cell(r.FinalStatistic.NetProfit);// ("最高价");
            grid[row, 10] = new SourceGrid.Cells.Cell(r.FinalStatistic.MaxDrawDown);// ("最低价");
            grid[row, 11] = new SourceGrid.Cells.Cell(r.FinalStatistic.MaxDrawDownPct);// ("盈利(点)");
            grid[row, 12] = new SourceGrid.Cells.Cell(r.FinalStatistic.MaxConsecutiveWinning);// ("盈利");
            grid[row, 13] = new SourceGrid.Cells.Cell(r.FinalStatistic.MaxConsecutiveLosing);// ("手续费");
            //grid[row, 14] = new SourceGrid.Cells.Cell(r.FinalStatistic.);// ("净盈利");
            
        }
        public void ShowOptimResult(List<OptimizationResult> results)
        {
            foreach (OptimizationResult r in results)
            {
                InsertResult(r);
            }
        }

    }
}
