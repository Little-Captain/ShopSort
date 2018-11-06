using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ZXY_ZXSC
{
    static class Program
    {
        ///// <summary>
        ///// 应用程序的主入口点。
        ///// </summary>
        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new MCS_Index());
        //}
        
            /// <summary>
            /// 应用程序的主入口点。
            /// </summary>
            [STAThread]
            static void Main()
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //Login frmLogin = new Login();
                //if (frmLogin.ShowDialog() == DialogResult.OK)
                //{
                    Application.Run(new MCS_Index());
                //}
            }
    }
}
