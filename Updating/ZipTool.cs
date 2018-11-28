using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Updating
{
    class ZipTool
    {
        public static void unzip(string dir, string filename, EventHandler<ExtractProgressEventArgs> progressHandle)
        {
            try
            {
                ZipFile zip = new ZipFile(dir + @"\" + filename);
                zip.ExtractProgress += progressHandle;
                zip.ExtractAll(dir, ExtractExistingFileAction.OverwriteSilently);
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误码：003\n" + ex.ToString(), "更新出现故障");
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }
    }
}
