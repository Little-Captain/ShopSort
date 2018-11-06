using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZXY_ZXSC
{
    public partial class Login : Form
    {
        public Login()
        {
            Welcome fw = new Welcome();
            fw.Show();//show出欢迎窗口
            System.Threading.Thread.Sleep(2000);//欢迎窗口停留时间2s
            fw.Close();
            InitializeComponent();
        }
        
        private void btnExit_Click(object sender, EventArgs e)    //单击关闭按钮事件
        {
            Application.Exit();
        }
        

        private void btnOk_Click(object sender, EventArgs e)
        {
            // MessageBox.Show("登录成功！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //以下开始显示主窗体 并关闭登录窗体
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
