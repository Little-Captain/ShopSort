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

namespace ZXY_ZXSC
{
    public partial class updateVersion : Form
    {
        public updateVersion()
        {
            InitializeComponent();

            this.button1.Enabled = false;
            this.button1.Click += button1_Click;
            this.Text = "更新...";
            UpdateDownLoad();
            // Update();
        }

        void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public delegate void ChangeBarDel(System.Net.DownloadProgressChangedEventArgs e);

        private void UpdateDownLoad()
        {
            WebClient wc = new WebClient();
            wc.DownloadProgressChanged += wc_DownloadProgressChanged;
            wc.DownloadFileAsync(new Uri("http://www.shitong666.cn/BlogWriter.zip"), "Update.zip");//要下载文件的路径,下载之后的命名
        }
        //  int index = 0;
        void wc_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Action act = () =>
            {
                this.progressBar1.Value = e.ProgressPercentage;
                this.lblMessage.Text = e.ProgressPercentage + "%";

            };
            this.Invoke(act);

            if (e.ProgressPercentage == 100)
            {
                //下载完成之后开始覆盖

                ZipHelper.Unzip();//调用解压的类
                this.button1.Enabled = true;

            }
        }
    }
}
