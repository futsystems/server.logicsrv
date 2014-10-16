using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FutsMoniter
{
    public partial class ctGridExport : UserControl
    {
        public ctGridExport()
        {
            InitializeComponent();
        }

        public Telerik.WinControls.UI.RadGridView Grid { get; set; }
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (this.Grid == null)
            {
                return;
            }
            Utils.RunExportToExcelML("", Grid);

        }

        private void btnExportPDF_Click(object sender, EventArgs e)
        {
            if (this.Grid == null)
            {
                return;
            }
            Utils.ExportToPDF("", Grid);
        }
    }
}
