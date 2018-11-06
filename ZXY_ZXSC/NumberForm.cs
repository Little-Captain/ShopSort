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
    public partial class NumberForm : Form
    {
        public NumberForm()
        {
            InitializeComponent();
        }
        public string title = "";
        public string strReturn = "";
        public string pagecount = "";
        private void NumberForm_Load(object sender, EventArgs e)
        {
            this.AcceptButton = btn_ok;
            this.Text = title;
            txt_result.Text = strReturn;
            if (this.Text == "订单编号" || this.Text == "选择页码" || this.Text == "每页条数")
            {
                btn_point.Enabled = false;
                if (txt_result.Text.Length < 1)
                {
                    btn_quit.Enabled = false;
                }
                else
                {
                    btn_quit.Enabled = true;
                }
            }
        }

        private void NumberForm_Activated(object sender, EventArgs e)
        {
           txt_result.Focus();
        }
        #region 数字键盘
        private void btn_one_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "1";
            }
            else
            {
                txt_result.Text += "1";
            }
        }

        private void btn_two_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "2";
            }
            else
            {
                txt_result.Text += "2";
            }
        }

        private void btn_three_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "3";
            }
            else
            {
                txt_result.Text += "3";
            }
        }
        private void btn_four_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "4";
            }
            else
            {
                txt_result.Text += "4";
            }
        }

        private void btn_five_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "5";
            }
            else
            {
                txt_result.Text += "5";
            }
        }

        private void btn_six_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "6";
            }
            else
            {
                txt_result.Text += "6";
            }
        }

        private void btn_seven_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "7";
            }
            else
            {
                txt_result.Text += "7";
            }
        }

        private void btn_eight_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "8";
            }
            else
            {
                txt_result.Text += "8";
            }
        }

        private void btn_nine_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "9";
            }
            else
            {
                txt_result.Text += "9";
            }
        }

        private void btn_zero_Click(object sender, EventArgs e)
        {
            if (this.Text == "订单编号")
            {
                txt_result.Text += "0";
            }
            else if (txt_result.Text != "0")
            {
                txt_result.Text += "0";
            }
        }
        //00
        private void btn_twozero_Click(object sender, EventArgs e)
        {
            if (this.Text == "订单编号")
            {
                txt_result.Text += "00";
            }
            else
            {
                if (txt_result.Text != "0")
                {
                    txt_result.Text += "00";
                }
            }
        }
        #endregion

        //退格
        private void btn_quit_Click(object sender, EventArgs e)
        {
            if (this.Text == "订单编号")
            {
                if (txt_result.Text.ToString().Trim().Length >= 1)
                {
                    txt_result.Text = txt_result.Text.Substring(0, txt_result.Text.Length - 1);
                }
                else
                {
                    txt_result.Text = "";
                    btn_quit.Enabled = false;
                }
            }
            else{
                if (txt_result.Text.ToString().Trim().Length == 1)
                {
                    txt_result.Text = "0";
                }
                else
                {
                    txt_result.Text = txt_result.Text.Substring(0, txt_result.Text.Length - 1);
                }
            }
        }

        //清除
        private void btn_clear_Click(object sender, EventArgs e)
        {
            if (this.Text == "订单编号")
            {
                txt_result.Text = "";
            }
            else { txt_result.Text = "0"; }
        }
      
        //小数点
        private void btn_point_Click(object sender, EventArgs e)
        {
            if (txt_result.Text == "0")
            {
                txt_result.Text = "0.";
            }
            else if(txt_result.Text.IndexOf(".")<1)
            {
                txt_result.Text += ".";
            }
        }
        //ok
        private void btn_ok_Click(object sender, EventArgs e)
        {
            strReturn = txt_result.Text;
            this.Close();
        }
        //显示框数据改变事件
        private void txt_result_TextChanged(object sender, EventArgs e)
        {
            if (txt_result.Text.Length < 1)
            {
                btn_quit.Enabled = false;
            }
            else
            {
                btn_quit.Enabled = true;
            }
            if (this.Text == "选择页码") {
                if (int.Parse(txt_result.Text.ToString()) == int.Parse(pagecount))
                {
                    btn_one.Enabled = false;
                    btn_two.Enabled = false;
                    btn_three.Enabled = false;
                    btn_four.Enabled = false;
                    btn_five.Enabled = false;
                    btn_six.Enabled = false;
                    btn_seven.Enabled = false;
                    btn_eight.Enabled = false;
                    btn_nine.Enabled = false;
                    btn_zero.Enabled = false;
                    btn_twozero.Enabled = false;
                }
                else
                {

                    btn_one.Enabled = true;
                    btn_two.Enabled = true;
                    btn_three.Enabled = true;
                    btn_four.Enabled = true;
                    btn_five.Enabled = true;
                    btn_six.Enabled = true;
                    btn_seven.Enabled = true;
                    btn_eight.Enabled = true;
                    btn_nine.Enabled = true;
                    btn_zero.Enabled = true;
                    btn_twozero.Enabled = true;
                }
            }
        }

    }
}
