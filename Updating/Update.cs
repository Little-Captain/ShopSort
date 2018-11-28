using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Ionic.Zip;

namespace Updating
{
    public partial class Update : Form
    {
        public Update(string downloadURL, string exe)
        {
            InitializeComponent();

            this.Text = "更新中...";
            originExeFileName = exe;
            UpdateDownLoad(downloadURL);
        }

        public delegate void ChangeBarDelegate(System.Net.DownloadProgressChangedEventArgs e);

        private void UpdateDownLoad(string downloadURL)
        {
            try
            {
                Uri uri = new Uri(downloadURL);
                WebClient download = new WebClient();
                download.DownloadProgressChanged += downloadProgressChanged;
                download.DownloadFileAsync(uri, filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误码：001\n" + ex.ToString(), "更新出现故障");
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        void downloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Action act = () =>
            {
                this.progressBar1.Value = e.ProgressPercentage;
                this.lblMessage.Text = e.ProgressPercentage + "%";

            };
            this.Invoke(act);

            if (e.ProgressPercentage == 100 && !isUnzipping)
            {
                isUnzipping = true;
                ZipTool.unzip(System.Environment.CurrentDirectory, filename, unzipHandle);
            }
        }

        void unzipHandle(object sender, ExtractProgressEventArgs e)
        {
            try
            {
                double extracted = e.EntriesExtracted;
                double total = e.EntriesTotal;
                int percentage = (int)(extracted / total * 100);
                if (percentage < 0 || percentage > 100)
                {
                    return;
                }
                this.progressBar2.Value = percentage;
                this.installLabel.Text = percentage + "%";
                if (percentage == 100)
                {
                    // 更新完成，启动最新程序
                    ProcessStartInfo startInfo = new ProcessStartInfo(System.Environment.CurrentDirectory + @"\" + originExeFileName);
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                    startInfo.WorkingDirectory = System.Environment.CurrentDirectory;
                    startInfo.UseShellExecute = false;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.Arguments = "";
                    Process p = new Process();
                    p.StartInfo = startInfo;
                    p.Start();
                    System.Environment.Exit(System.Environment.ExitCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("错误码：002\n" + ex.ToString(), "更新出现故障");
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        bool isUnzipping = false;
        string originExeFileName;

        string filename
        {
            get
            {
                return "Update.zip";
            }
        }

        private void completedBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否要取消更新？", "取消更新", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.Environment.Exit(System.Environment.ExitCode);
            }
        }

        private const int NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ClassStyle |= NOCLOSE_BUTTON;
                return createParams;
            }
        }
    }
}
