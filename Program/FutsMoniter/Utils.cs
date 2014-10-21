using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public delegate void SymbolImplDel(SymbolImpl sym,bool islast);

    public class Utils
    {

        //public static void RunExportToExcelML(string title, Telerik.WinControls.UI.RadGridView grid)
        //{
        //    System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
        //    saveFileDialog.Filter = "Excel (*.xls)|*.xls";
        //    if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        //    {
        //        return;
        //    }

        //    if (saveFileDialog.FileName.Equals(String.Empty))
        //    {
        //        fmConfirm.Show("请填写输出文件名");
        //        return;
        //    }

        //    ExportToExcelML excelExporter = new ExportToExcelML(grid);

        //    //if (this.radTextBoxSheet.Text != String.Empty)
        //    //{
        //    //    excelExporter.SheetName = this.radTextBoxSheet.Text;

        //    //}
        //    excelExporter.SummariesExportOption = SummariesOption.ExportAll;
        //    //switch (this.radComboBoxSummaries.SelectedIndex)
        //    //{
        //    //    case 0:
        //    //        excelExporter.SummariesExportOption = SummariesOption.ExportAll;
        //    //        break;
        //    //    case 1:
        //    //        excelExporter.SummariesExportOption = SummariesOption.ExportOnlyTop;
        //    //        break;
        //    //    case 2:
        //    //        excelExporter.SummariesExportOption = SummariesOption.ExportOnlyBottom;
        //    //        break;
        //    //    case 3:
        //    //        excelExporter.SummariesExportOption = SummariesOption.DoNotExport;
        //    //        break;
        //    //}

        //    //set max sheet rows
        //    //if (this.radRadioButton1.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
        //    //{
        //    //    excelExporter.SheetMaxRows = ExcelMaxRows._1048576;
        //    //}
        //    //else if (this.radRadioButton2.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On)
        //    //{
        //    //    excelExporter.SheetMaxRows = ExcelMaxRows._65536;
        //    //}

        //    //set exporting visual settings
        //    excelExporter.ExportVisualSettings = false;// this.exportVisualSettings;

        //    try
        //    {
        //        excelExporter.RunExport(saveFileDialog.FileName);

        //        //RadMessageBox.SetThemeName(this.radGridView1.ThemeName);
        //        //DialogResult dr = RadMessageBox.Show("The data in the grid was exported successfully. Do you want to open the file?",
        //        //    "Export to Excel", MessageBoxButtons.YesNo, RadMessageIcon.Question);
        //        //if (dr == DialogResult.Yes)
        //        //{
        //        //    openExportFile = true;
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        fmConfirm.Show("保存文件出错:" + ex.ToString());
        //    }
        //}


        //public static void ExportToPDF(string title,Telerik.WinControls.UI.RadGridView grid)
        //{
        //    System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
        //    saveFileDialog.Filter = "PDF File (*.pdf)|*.pdf";
        //    if (saveFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        //    {
        //        return;
        //    }

        //    if (saveFileDialog.FileName.Equals(String.Empty))
        //    {
        //        fmConfirm.Show("请填写输出文件名");
        //        return;
        //    }


        //    ExportToPDF pdfExporter = new ExportToPDF(grid);
        //    pdfExporter.PdfExportSettings.Title = "My PDF Title";
        //    pdfExporter.PdfExportSettings.PageWidth = 297;
        //    pdfExporter.PdfExportSettings.PageHeight = 210;
        //    pdfExporter.PageTitle = title;
        //    pdfExporter.FitToPageWidth = true;
        //    pdfExporter.SummariesExportOption = SummariesOption.ExportAll;
        //    /*
        //    switch (this.radComboBoxSummaries.SelectedIndex)
        //    {
        //        case 0:
        //            pdfExporter.SummariesExportOption = SummariesOption.ExportAll;
        //            break;
        //        case 1:
        //            pdfExporter.SummariesExportOption = SummariesOption.ExportOnlyTop;
        //            break;
        //        case 2:
        //            pdfExporter.SummariesExportOption = SummariesOption.ExportOnlyBottom;
        //            break;
        //        case 3:
        //            pdfExporter.SummariesExportOption = SummariesOption.DoNotExport;
        //            break;
        //    }
        //     * **/

        //    //set exporting visual settings
        //    pdfExporter.ExportVisualSettings = true;// this.exportVisualSettings;

        //    try
        //    {
        //        pdfExporter.RunExport(saveFileDialog.FileName);

        //        //RadMessageBox.SetThemeName(this.radGridView1.ThemeName);
        //        //DialogResult dr = RadMessageBox.Show("The data in the grid was exported successfully. Do you want to open the file?",
        //        //    "Export to PDF", MessageBoxButtons.YesNo, RadMessageIcon.Question);
        //        //if (dr == DialogResult.Yes)
        //        //{
        //        //    openExportFile = true;
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        fmConfirm.Show("保存文件出错:" + ex.ToString());
        //    }
        //    //catch (IOException ex)
        //    //{
        //    //    RadMessageBox.SetThemeName(this.radGridView1.ThemeName);
        //    //    RadMessageBox.Show(this, ex.Message, "I/O Error", MessageBoxButtons.OK, RadMessageIcon.Error);
        //    //}
        //}

        public static ArrayList GetOffsetCBList()
        {
            ArrayList list = new ArrayList();
            ValueObject<QSEnumOffsetFlag> vo0 = new ValueObject<QSEnumOffsetFlag>();
            vo0.Name = Util.GetEnumDescription(QSEnumOffsetFlag.UNKNOWN);
            vo0.Value = QSEnumOffsetFlag.UNKNOWN;
            list.Add(vo0);

            ValueObject<QSEnumOffsetFlag> vo1 = new ValueObject<QSEnumOffsetFlag>();
            vo1.Name = Util.GetEnumDescription(QSEnumOffsetFlag.OPEN);
            vo1.Value = QSEnumOffsetFlag.OPEN;
            list.Add(vo1);

            ValueObject<QSEnumOffsetFlag> vo2 = new ValueObject<QSEnumOffsetFlag>();
            vo2.Name = Util.GetEnumDescription(QSEnumOffsetFlag.CLOSE);
            vo2.Value = QSEnumOffsetFlag.CLOSE;
            list.Add(vo2);

            ValueObject<QSEnumOffsetFlag> vo3 = new ValueObject<QSEnumOffsetFlag>();
            vo3.Name = Util.GetEnumDescription(QSEnumOffsetFlag.CLOSEYESTERDAY);
            vo3.Value = QSEnumOffsetFlag.CLOSEYESTERDAY;
            list.Add(vo3);
            return list;

        }

        public static ArrayList GetOrderTypeCBList()
        {
            ArrayList list = new ArrayList();
            ValueObject<int> vo1 = new ValueObject<int>();
            vo1.Name = "限价";
            vo1.Value = 0;
            list.Add(vo1);

            ValueObject<int> vo2 = new ValueObject<int>();
            vo2.Name = "市价";
            vo2.Value = 1;
            list.Add(vo2);
            return list;
        }
        public static ArrayList GetTradeableCBList(bool any=true)
        {
            ArrayList list = new ArrayList();
            if (any)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = "<Any>";
                vo.Value = 0;
                list.Add(vo);
            }

            ValueObject<int> vo1 = new ValueObject<int>();
            vo1.Name = "可交易";
            vo1.Value = 1;
            list.Add(vo1);

            ValueObject<int> vo2 = new ValueObject<int>();
            vo2.Name = "不可交易";
            vo2.Value = -1;
            list.Add(vo2);
            return list;
        }

        public static ArrayList GetManagerTypeCBList()
        {
            ArrayList list = new ArrayList();

            ValueObject<QSEnumManagerType> vo1 = new ValueObject<QSEnumManagerType>();
            vo1.Name = Util.GetEnumDescription(QSEnumManagerType.AGENT);
            vo1.Value = QSEnumManagerType.AGENT;
            list.Add(vo1);

            ValueObject<QSEnumManagerType> vo2 = new ValueObject<QSEnumManagerType>();
            vo2.Name = Util.GetEnumDescription(QSEnumManagerType.ACCOUNTENTER);
            vo2.Value = QSEnumManagerType.ACCOUNTENTER;
            list.Add(vo2);

            ValueObject<QSEnumManagerType> vo3 = new ValueObject<QSEnumManagerType>();
            vo3.Name = Util.GetEnumDescription(QSEnumManagerType.RISKER);
            vo3.Value = QSEnumManagerType.RISKER;
            list.Add(vo3);

            ValueObject<QSEnumManagerType> vo4 = new ValueObject<QSEnumManagerType>();
            vo4.Name = Util.GetEnumDescription(QSEnumManagerType.MONITER);
            vo4.Value = QSEnumManagerType.MONITER;
            list.Add(vo4);
            return list;

        }

        public static string GenSymbol(SecurityFamilyImpl sec, int month)
        {
            switch (sec.Type)
            { 
                case SecurityType.FUT:
                    return GenFutSymbol(sec, month);
                default:
                    return sec.Code;
            }
        }



        static string GenFutSymbol(SecurityFamilyImpl sec, int month)
        {
            if (sec.Exchange.EXCode.Equals("CZCE"))
            {
                return sec.Code + month.ToString().Substring(3);
            }
            else
            {
                return sec.Code + month.ToString().Substring(2);
            }
        }
    }
}
