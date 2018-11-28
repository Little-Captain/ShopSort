using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Updating
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string downloadURL = "";
            string exe = "";
            if (args.Length == 2)
            {
                downloadURL = args[0];
                exe = args[1];
            }
            Application.Run(new Update(downloadURL, exe));
        }
    }
}
