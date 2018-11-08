using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.IO.Ports;
using System.Configuration;
using System.Diagnostics;

namespace ZXY_ZXSC
{
    public partial class MCS_DDLBForm : Form
    {
        public MCS_DDLBForm()
        {
            InitializeComponent();
            this.Text += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        public string ddurl = "";//接口url
        string scOrderId = "";//订单id
        string jsonstr = "";
        int islist = 0;//判断是确认按钮还是下一条
        string count = "";
        public string orderID = "";
        SerialPort sp1 = new SerialPort();//称的服务
        public int type = 1;
        public DataTable mytableCP = new DataTable();//打印表的产品

        public DataTable mytableDD = new DataTable();//打印表的产品
        DataTable tableProduct = new DataTable();//打印表的表头
        public string NeedWeighted = "";
        public int ck_Open= 0;
        public string lx = "";
        public string khmc="";
        public string ddbh = "";
        public string xdsj = "";
        public string remark = "";
        public string url="";
        private void MCS_DDLBForm_Load(object sender, EventArgs e)
        {
            try
            {
                ck_open.Checked = true;
                if (ck_open.Checked == true) {
                    ck_Open = 0;
                }
                int types = type; lbl_lx.Text = lx;
                if (types == 2)
                {
                    mytableDD.Columns.Add("序号");
                    mytableDD.Columns.Add("产品名称");
                    mytableDD.Columns.Add("生产批号");
                    mytableDD.Columns.Add("生产日期");
                    mytableDD.Columns.Add("保质期");
                    mytableDD.Columns.Add("下单数量");

                    mytableDD.Columns.Add("分拣单位");
                    mytableDD.Columns.Add("分拣数量");
                    mytableDD.Columns.Add("单价");
                    mytableDD.Columns.Add("总价");
                    mytableDD.Columns.Add("是否过秤");
                    mytableDD.Columns.Add("分拣确认");
                    mytableDD.Columns.Add("产品编号");
                    mytableDD.Columns.Add("实际单位");

                    tableProduct.Columns.Add("客户名称");
                    tableProduct.Columns.Add("客户电话");
                    tableProduct.Columns.Add("地址");
                    tableProduct.Columns.Add("下单时间");
                    tableProduct.Columns.Add("备注");
                    tableProduct.Columns.Add("订单号");

                    islist = 1;
                    button1.Visible = true;
                    requestGetJson(url);
                    yc(dataGridView1, mytableDD);
                    btn_print.Visible = true;
                    label1.Text = "客户名称：";
                    label3.Text = "订单编号：";
                    lbl_name.Text = khmc;
                    lbl_time.Text = xdsj;
                    lbl_ddh.Text = ddbh;
                    lbl_remark.Text = remark;
                    label5.Text = "下单时间：";
                }
                else
                {
                    button1.Visible = false;
                    btn_print.Visible = false;
                    label3.Text = "产品编号：";
                    lbl_name.Text = khmc;
                    lbl_time.Text = xdsj;
                    label1.Text = "产品名称：";
                    lbl_ddh.Text = ddbh;
                    remarkLabel.Visible = false;
                    label5.Text = "下单合计：";

                    yc(dataGridView1, mytableCP);
                }
                dataGridView1.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView1.EnableHeadersVisualStyles = false;

              

                //requestGetJson(ddurl);
                sp1.DataReceived += Sp1_DataRevice;
                sp1.PortName = ConfigurationManager.AppSettings["PortName"].ToString();
                sp1.BaudRate = 9600;
                sp1.DataBits = 8;
                sp1.StopBits = StopBits.One;
                sp1.DtrEnable = true;
                sp1.RtsEnable = true;
                sp1.Open();
            }
            catch { }
        }

        private bool completed()
        {
            //dataGridView1.Rows[e.RowIndex].Cells["分拣确认"].Value
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells["分拣确认"].Value.ToString() != "已确认")
                {
                    return false;
                }
            }
            return true;
        }

        //打印
        private void btn_print_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbl_ddh.Text != "")
                {
                    DataTable cpPrint = mytableDD.Copy();
                    DataRow[] myrows = null;
                    if (completed())
                    {
                        for (int i = 0; i < cpPrint.Rows.Count; i++)
                        {
                            myrows = cpPrint.Select("分拣确认='已确认'");
                        }
                    }
                    else
                    {
                        MessageBox.Show("该订单还有未确认的产品，不可打印！");
                        return;
                    }
                    //for (int i = 0; i < cpPrint.Rows.Count; i++)
                    //{
                    //    if (cpPrint.Rows[i]["分拣确认"].ToString() == "已确认")
                    //    {
                    //        myrows = cpPrint.Select("分拣确认='已确认'");
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("该订单还有未确认的产品，不可打印！");
                    //        return;
                    //    }
                    //}
                    int num = 0;
                    foreach(DataRow item in cpPrint.Rows)
                    {
                        num++;

                        item["序号"] = num;
                        item["单价"] = "";
                        if (item["生产日期"].ToString().Trim() == "")
                        {
                            if (int.Parse(DateTime.Now.Hour.ToString()) > 12)
                            {
                                item["生产批号"] = item["产品编号"].ToString() + DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                            }
                            else
                            {
                                item["生产批号"] = item["产品编号"].ToString() + DateTime.Now.ToString("yyyyMMdd");
                            }
                        }
                        else
                        {
                            if (int.Parse(DateTime.Parse(item["生产日期"].ToString()).Hour.ToString()) > 12)
                            {
                                item["生产批号"] = item["产品编号"].ToString() + DateTime.Parse(item["生产日期"].ToString()).AddDays(1).ToString("yyyyMMdd");
                                item["生产日期"] = DateTime.Parse(item["生产日期"].ToString()).AddDays(1).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                item["生产批号"] = item["产品编号"].ToString() + DateTime.Parse(item["生产日期"].ToString()).ToString("yyyyMMdd");
                                item["生产日期"] = DateTime.Parse(item["生产日期"].ToString()).ToString("yyyy-MM-dd");
                            }
                        }
                        string dw = item["分拣单位"].ToString();
                        string fjsl = item["分拣数量"].ToString();
                        item["分拣数量"] = fjsl + dw;
                        //item["总价"] = decimal.Parse(item["单价"].ToString()) * decimal.Parse(item["分拣数量"].ToString());
                    }
                    print(myrows);//打印报表
                }
                else
                {
                    MessageBox.Show("暂无订单！");
                }

            }
            catch { }
        }
        //打印方法
        private void print(DataRow[] myrows)//打印报表
        {
            try
            {
                DataTable myprinttable = myrows.CopyToDataTable();
                try
                {
                    GridReportForm BForm = new GridReportForm();

                    BForm.myprinttable = tableProduct;
                    BForm.printtable = myprinttable;
                    BForm.reportname = "dd.grf";
                    BForm.ptintview = true;//false无预览，直接打印
                    BForm.ShowDialog();

                }
                catch { }
            }
            catch { }
        }
        //启动称
        private void Sp1_DataRevice(object sender, SerialDataReceivedEventArgs e)
        {
            if (sp1.IsOpen==true)
            {
                Byte[] receivedData = new Byte[sp1.BytesToRead];
                sp1.Read(receivedData, 0, receivedData.Length);
                AddContent(new UTF8Encoding().GetString(receivedData));
            }
        }
        //接收称的数据
        private void AddContent(string content)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                if (content.Equals("="))
                {
                    //赋值给Cell
                    //1、转换数据顺序
                    if (count.Length == 7)
                    {
                        char[] cArray = count.ToCharArray();
                        string reverse = String.Empty;
                        for (int i = cArray.Length - 1; i > -1; i--)
                        {
                            reverse += cArray[i];
                        }

                        try
                        {
                            double actualWeight = double.Parse(reverse);
                            actualWeight *= 2;
                            //2、将实际重量赋给Cell
                            if (dataGridView1.Rows[row].Cells["是否过秤"].Value.ToString().Equals("是")&&dataGridView1.Rows[row].Cells["分拣确认"].Value.ToString().Equals("确认")&&ck_Open==0)
                            {
                                    dataGridView1.Rows[row].Cells["分拣数量"].Value = Math.Round(actualWeight, 2);
                            }
                          
                        }
                        catch { }
                    }


                    count = "";
                }
                else
                {
                    count = count + content;
                }

            }));
        }

        //历史订单
        //private void btn_history_Click(object sender, EventArgs e)
        //{
        //    MCS_DDLBListForm mlForm = new MCS_DDLBListForm();

        //    mlForm.ShowDialog();
        //}
        private void requestGetJson(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                jsonstr = stream.ReadLine();
                JavaScriptSerializer js = new JavaScriptSerializer();
                if (islist == 1)
                {
                    tableProduct.Clear();
                    dataGridView1.DataSource = null;
                    dataGridView1.Columns.Clear();
                    dataGridView1.DataSource = null;
                    mytableDD.Clear();

                    OrderData orderData = js.Deserialize<OrderData>(jsonstr);

                    if (orderData.status.ToString().Equals("200"))
                    {
                        Order order = orderData.data;
                        lbl_name.Text = order.DepartmentName;
                        lbl_ddh.Text = order.OrderNo;
                        lbl_time.Text = order.OrderTime;
                        scOrderId = orderID;
                        DataRow tableProductRow = tableProduct.NewRow();
                        tableProductRow["客户名称"] = order.DepartmentName;
                        tableProductRow["客户电话"] = order.Mobile;
                        tableProductRow["地址"] = order.Country + order.Province + order.City + order.Address;
                        tableProductRow["下单时间"] = order.OrderTime;
                        tableProductRow["备注"] = order.Remark;
                        tableProductRow["订单号"] = ddbh;
                        tableProduct.Rows.Add(tableProductRow);
                        List<Product> productList = order.listDetail;
                        if (productList != null)
                        {
                            foreach (Product prod in productList)
                            {
                                DataRow dr = mytableDD.NewRow();
                                dr["产品编号"] = prod.ScProductID;
                                dr["产品名称"] = prod.ProductName;
                                dr["下单数量"] = prod.OrderCount+prod.Unit;
                                dr["单价"] = prod.SellingPrice;
                                dr["分拣单位"] = prod.ActualUnit;
                                dr["保质期"] = prod.QualityTime + "天";
                                dr["生产日期"] = prod.DispatchingDate;
                                try
                                {
                                    if (prod.ActualCount.ToString().Trim() == "" || prod.ActualCount == null)
                                    {
                                        dr["分拣数量"] = "";
                                    }
                                    else
                                    {
                                        dr["分拣数量"] = decimal.Round(decimal.Parse(prod.ActualCount), 2).ToString();
                                    }
                                }
                                catch { }
                                dr["是否过秤"] = prod.NeedWeighted == "True" ? "是" : "否";
                                if (prod.IsSorted == false)
                                {
                                    dr["分拣确认"] = "确认";
                                }
                                else
                                {
                                    dr["分拣确认"] = "已确认";
                                }
                                mytableDD.Rows.Add(dr);
                            }
                            yc(dataGridView1, mytableDD);

                            dataGridView1.ClearSelection();

                        }
                    }
                }
                else
                {
                    result orderData = js.Deserialize<result>(jsonstr);

                }
            }
            catch (Exception ex) {  }
        }
        #region 接收参数 实体
        public class Order
        {
            public string DepartmentName { set; get; }
            public string OrderTime { set; get; }
            public string OrderNo { set; get; }
            public int ID { set; get; }
            public string ScOrderID { set; get; }
            //public int UnsortedNumber { set; get; }
            public string Remark { get; set; }
            public string Mobile { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
            public string Province { get; set; }
            public string City { get; set; }
            public List<Product> listDetail { set; get; }
        }
        public class result
        {
            public string status { set; get; }
            public string data { set; get; }
            public string msg { set; get; }
        }
        public class OrderData
        {
            public string status { set; get; }
            public Order data { set; get; }
            public string msg { set; get; }
        }

        public class Product
        {
            public string  ActualCount { get; set; }
            public string ProductCode { get; set; }
            public decimal OrderCount { set; get; }
            public string QualityTime { get; set; }
            public string OrderNo { set; get; }
            public float SellingPrice { set; get; }
            public string DispatchingDate { set; get; }
            public int ScProductID { set; get; }
            public string ProductName { set; get; }
            public string ActualUnit { set; get; }
            public string NeedWeighted { get; set; }
            public string Unit { set; get; }
            public bool IsSorted { set; get; }
        }
        #endregion

        private void yc(DataGridView dgv, DataTable mydt)//只允许输入部分列
        {
            try
            {
                dgv.DataSource = mydt.DefaultView;
                string strc1 = "";
               
                for (int j = 0; j < mydt.Columns.Count; j++) //写列标题 
                {
                    strc1 = mydt.Columns[j].ColumnName.ToUpper();
                    dgv.Columns[strc1].SortMode = DataGridViewColumnSortMode.NotSortable;
                    if (strc1.IndexOf("I") >= 0)
                    {
                        dgv.Columns[strc1].Visible = false;
                    }
                    dgv.Columns[strc1].DefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                    if (type ==1)
                    {
                        if(strc1.Trim() == "产品编号" || strc1.Trim() == "产品名称" ||strc1.Trim()=="单位")
                        {
                            dgv.Columns[strc1].Visible = false;
                        }
                    }
                    if (strc1.Trim() == "订单号"||strc1.Trim()=="生产日期" || strc1.Trim() == "产品编号" || strc1.Trim() == "序号" || strc1.Trim() == "生产批号" || strc1.Trim()=="保质期"||strc1.Trim() == "订单编号"|| strc1.Trim() == "实际单位"||strc1.Trim()=="单价"||strc1.Trim()=="总价")
                    {
                        dgv.Columns[strc1].Visible = false;
                    }
                    if (strc1.Trim() != "分拣数量" || strc1.Trim() != "分拣确认")
                    {
                        dgv.Columns[strc1].ReadOnly = true;
                    }
                }
                
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    if (dgv.Rows[i].Cells["分拣确认"].Value.ToString() == "确认")
                    {
                        dgv.Rows[i].Cells["分拣确认"].Style.BackColor = Color.FromArgb(135, 206, 250);
                    }
                    else
                    {
                        dgv.Rows[i].Cells["分拣确认"].ReadOnly = true;
                        dgv.Rows[i].Cells["分拣确认"].Style.BackColor = Color.FromArgb(230, 230, 230);
                    }
                }
                //自动列宽
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch { }
        }

        int row = -1;

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.RowIndex != -1)
                {
                    if (row != -1)
                    {
                        if (dataGridView1.Columns[e.ColumnIndex].HeaderText != "分拣确认")
                        {
                            if (dataGridView1.Rows[row].Cells["分拣确认"].Value.ToString().Equals("确认"))
                            {
                                dataGridView1.Rows[row].Cells["分拣数量"].Value = "";
                                row = -1;
                            }
                        }
                    }
                    if (dataGridView1.Columns[e.ColumnIndex].HeaderText.Equals("分拣数量") &&
                        dataGridView1.Rows[e.RowIndex].Cells["分拣确认"].Value.ToString().Equals("确认") &&
                        dataGridView1.Rows[e.RowIndex].Cells["是否过秤"].Value.ToString().Equals("是"))
                    {
                        row = e.RowIndex;
                        if (ck_Open == 1)
                        {
                            string sx = dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].Value.ToString();
                            NumberForm znFrm = new NumberForm();
                            znFrm.Text = "分拣数量";
                            znFrm.strReturn = sx;
                            znFrm.ShowDialog();
                            if (znFrm.strReturn.Length > 0)
                            {
                                try
                                {
                                    double sl = double.Parse(znFrm.strReturn.ToString());
                                    dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].Value = sl.ToString();
                                }
                                catch { }
                            }
                        }
                    }
                    else
                    {
                        row = -1;
                        if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "分拣确认")
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells["分拣确认"].Value.ToString() == "确认")
                            {

                                string orderCount = dataGridView1.Rows[e.RowIndex].Cells["下单数量"].Value.ToString();
                                string actualUnit = dataGridView1.Rows[e.RowIndex].Cells["分拣单位"].Value.ToString();
                                //string isNeedGC = dataGridView1.Rows[e.RowIndex].Cells["是否过秤"].Value.ToString();
                                if (dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].Value.ToString()=="")
                                {
                                    if (orderCount.Contains(actualUnit))
                                    {
                                        try
                                        {
                                            int orderCharCount = orderCount.Length;
                                            int unitCharCount = actualUnit.Length;
                                            string countStr = orderCount.Remove(orderCharCount - unitCharCount, unitCharCount);
                                            double count = double.Parse(countStr);
                                            dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].Value = count.ToString();
                                        }
                                        catch { }
                                    }
                                }
                                string actualCount = dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].Value.ToString();
                                string scProductId = dataGridView1.Rows[e.RowIndex].Cells["产品编号"].Value.ToString();
                                if (actualCount == "" || actualCount == "0")
                                {
                                    actualCount = "0";
                                    
                                    dataGridView1.Rows[e.RowIndex].Cells["分拣确认"].Style.BackColor = Color.FromArgb(135, 206, 250);
                                    return;
                                }
                                string isFrom = "4";
                                string dw = "";
                                islist = 0; string url = "";
                                if (type == 2)
                                {
                                    url = ConfigurationManager.AppSettings["url"] + "orderSorting.html?actualCount=" + actualCount + "&scProductId=" + scProductId + "&scOrderId=" + scOrderId + "&isFrom=" + isFrom + "";
                                }
                                else
                                {
                                    url = ConfigurationManager.AppSettings["url"] + "orderSorting.html?actualCount=" + actualCount + "&scProductId=" + dataGridView1.Rows[e.RowIndex].Cells["产品id"].Value.ToString() + "&scOrderId=" + dataGridView1.Rows[e.RowIndex].Cells["订单id"].Value.ToString() + "&isFrom=" + isFrom + "";
                                }
                                foreach (DataRow item in mytableDD.Rows)
                                {
                                    if (int.Parse(DateTime.Now.Hour.ToString()) > 12)
                                    {
                                        item["生产日期"] = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                                    }
                                    else
                                    {
                                        item["生产日期"] = DateTime.Now.ToString("yyyy-MM-dd");
                                    }
                                }
                                if (MessageBox.Show("是否确认分拣数量为："+ dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].Value.ToString()+"?", "分拣确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    requestGetJson(url);
                                    dataGridView1.Rows[e.RowIndex].Cells["分拣确认"].Value = "已确认";
                                    dataGridView1.Rows[e.RowIndex].Cells["分拣确认"].Style.BackColor = Color.FromArgb(230, 230, 230);
                                    dataGridView1.Rows[e.RowIndex].Cells["分拣确认"].ReadOnly = true;
                                    dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].ReadOnly = true;
                                }
                                else
                                {
                                    dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].Value = "";
                                }
                            }
                        }
                        else if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "分拣数量" && dataGridView1.Rows[e.RowIndex].Cells["是否过秤"].Value.ToString().Equals("否"))
                        {
                            if (dataGridView1.Rows[e.RowIndex].Cells["分拣确认"].Value.ToString() == "确认")
                            {
                                string sx = dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].Value.ToString();
                                NumberForm znFrm = new NumberForm();
                                znFrm.Text = "分拣数量";
                                znFrm.strReturn = sx;
                                znFrm.ShowDialog();
                                if (znFrm.strReturn.Length > 0)
                                {
                                    try
                                    {
                                        double sl = double.Parse(znFrm.strReturn.ToString());
                                        sl = Math.Round(sl, 2);
                                        dataGridView1.Rows[e.RowIndex].Cells["分拣数量"].Value = sl.ToString();
                                    }
                                    catch { }
                                }
                            }
                            row = e.RowIndex;
                        }

                    }
                }
            }
            catch { }    }

        private void ck_open_CheckedChanged(object sender, EventArgs e)
        {
            if (ck_open.Checked == true)
            {
                ck_Open =0;
            }
            else
            {
                ck_Open = 1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbl_ddh.Text != "")
                {
                    DataTable cpPrint = mytableDD.Copy();
                    DataRow[] myrows = null;
                    if (completed())
                    {
                        for (int i = 0; i < cpPrint.Rows.Count; i++)
                        {
                            myrows = cpPrint.Select("分拣确认='已确认'");
                        }
                    }
                    else
                    {
                        MessageBox.Show("该订单还有未确认的产品，不可打印！");
                        return;
                    }
                    //for (int i = 0; i < cpPrint.Rows.Count; i++)
                    //{
                    //    if (cpPrint.Rows[i]["分拣确认"].ToString() == "已确认")
                    //    {
                    //        myrows = cpPrint.Select("分拣确认='已确认'");
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("该订单还有未确认的产品，不可打印！");
                    //        return;
                    //    }
                    //}
                    int num = 0;
                    foreach (DataRow item in cpPrint.Rows)
                    {
                        num++;
                        item["序号"] = num;

                        //item["单价"] = "";
                        if (item["生产日期"].ToString().Trim() == "")
                        {
                            if (int.Parse(DateTime.Now.Hour.ToString()) > 12)
                            {
                                item["生产批号"] = item["产品编号"].ToString() + DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                                
                            }
                            else
                            {
                                item["生产批号"] = item["产品编号"].ToString() + DateTime.Now.ToString("yyyyMMdd");
                            }
                        }
                        else
                        {
                            if (int.Parse(DateTime.Parse(item["生产日期"].ToString()).Hour.ToString()) > 12)
                            {
                                item["生产批号"] = item["产品编号"].ToString() + DateTime.Parse(item["生产日期"].ToString()).AddDays(1).ToString("yyyyMMdd");
                                item["生产日期"]= DateTime.Parse(item["生产日期"].ToString()).AddDays(1).ToString("yyyy-MM-dd");
                            }
                            else
                            {
                                item["生产批号"] = item["产品编号"].ToString() + DateTime.Parse(item["生产日期"].ToString()).ToString("yyyyMMdd");
                                item["生产日期"] = DateTime.Parse(item["生产日期"].ToString()).ToString("yyyy-MM-dd");
                            }
                        }
                        item["总价"] = decimal.Parse(item["单价"].ToString()) * decimal.Parse(item["分拣数量"].ToString());

                        string dw = item["分拣单位"].ToString();
                        string fjsl = item["分拣数量"].ToString();
                        item["分拣数量"] = fjsl + dw;
                    }
                    print(myrows);//打印报表
                }
                else
                {
                    MessageBox.Show("暂无订单！");
                }

            }
            catch (Exception ex){ }
        }

        private void MCS_DDLBForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {

                sp1.DtrEnable = false;
                sp1.RtsEnable = false;
                sp1.Close();
            }
            catch { }
        }
    }
}