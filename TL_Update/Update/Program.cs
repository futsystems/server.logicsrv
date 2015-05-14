using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Update
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(params string[] arges)
        {

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Form1 main = new Form1();
                if (arges != null)
                    main.Arges = arges;
                Application.Run(main);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
