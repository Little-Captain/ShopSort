using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ZXY_ZXSC
{
    public partial class MCS_DDDetails : Form
    {
        public MCS_DDDetails()
        {
            InitializeComponent();
            this.Text += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public string ddh = "";
        public string title = "";
        public string url = "";
        public string jsonstr = "";
        public DataRow[] myrows;
        DataTable mytableDD = new DataTable();//打印表的产品
        DataTable tableProduct = new DataTable();//打印表的表头
        private void MCS_DDDetails_Load(object sender, EventArgs e)
        {
            dataGridView1.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.Text = title;
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

            lbl_address.Text = myrows[0]["地址"].ToString();
            lbl_ddh.Text = myrows[0]["订单号"].ToString();
            label2.Text = myrows[0]["客户名称"].ToString();
            lbl_phone.Text = myrows[0]["客户电话"].ToString();
            lbl_time.Text = myrows[0]["下单时间"].ToString();

            url = ConfigurationManager.AppSettings["url"] + "orderDetail.html?isFrom=4&scOrderId=" + ddh + "&scSortingPositionId=" + ConfigurationManager.AppSettings["scSortingPositionId"]; ;
            requestGetJson(url);
        }

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

                tableProduct.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                mytableDD.Clear();

                OrderData orderData = js.Deserialize<OrderData>(jsonstr);

                if (orderData.status.ToString().Equals("200"))
                {
                    Order order = orderData.data;
                    DataRow tableProductRow = tableProduct.NewRow();
                    tableProductRow["客户名称"] = order.DepartmentName;
                    tableProductRow["客户电话"] = order.Mobile;
                    tableProductRow["地址"] = order.Country + order.Province + order.City + order.Address;
                    tableProductRow["下单时间"] = order.OrderTime;
                    tableProductRow["备注"] = order.Remark;
                    tableProductRow["订单号"] = myrows[0]["订单号"].ToString();
                    tableProduct.Rows.Add(tableProductRow);
                    List<Product> productList = order.listDetail;
                    if (productList != null)
                    {
                        foreach (Product prod in productList)
                        {
                            DataRow dr = mytableDD.NewRow();
                            mytableDD.Rows.Add(dr);
                            dr["产品编号"] = prod.ScProductID;
                            dr["产品名称"] = prod.ProductName;
                            dr["下单数量"] = prod.OrderCount + prod.Unit;
                            dr["单价"] = prod.SellingPrice;
                            //dr["单位"] = prod.Unit;
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
                        }
                        yc(dataGridView1, mytableDD);
                    }
                }
            }
            catch { }
        }
        #region 接收参数 实体
        public class OrderData
        {
            public string status { set; get; }
            public Order data { set; get; }
            public string msg { set; get; }
        }
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
            public List<Product> listDetail { get; set; }
        }
        public class Product
        {
            public string OrderNo { get; set; }
            public string NeedWeighted { get; set; }
            public string ActualCount { get; set; }
            public string QualityTime { get; set; }
            public string OrderCount { set; get; }
            public int ScOrderID { set; get; }
            public float SellingPrice { set; get; }
            public int ScProductID { set; get; }
            public string ProductCode { get; set; }
            public string ProductName { set; get; }
            public string ActualUnit { set; get; }
            public string DispatchingDate { set; get; }
            public string Unit { set; get; }
            public bool IsSorted { set; get; }
        }
        #endregion
        //打印
        private void btn_print_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbl_ddh.Text != "")
                {
                    DataTable cpPrint = mytableDD.Copy();
                    DataRow[] myrows = null;

                    myrows = cpPrint.Select();
                    int num = 0;
                    foreach (DataRow item in cpPrint.Rows)
                    {
                        num++;

                        item["序号"] = num;
                        item["单价"] = "";

                        if (int.Parse(DateTime.Parse(item["生产日期"].ToString()).Hour.ToString()) >= 12)
                        {
                            item["生产批号"] = item["产品编号"].ToString() + DateTime.Parse(item["生产日期"].ToString()).AddDays(1).ToString("yyyyMMdd");
                            item["生产日期"] = DateTime.Parse(item["生产日期"].ToString()).AddDays(1).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            item["生产批号"] = item["产品编号"].ToString() + DateTime.Parse(item["生产日期"].ToString()).ToString("yyyyMMdd");
                            item["生产日期"] = DateTime.Parse(item["生产日期"].ToString()).ToString("yyyy-MM-dd");
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

                    bool isPreview = true;
                    try
                    {
                        isPreview = Convert.ToBoolean(ConfigApp.valueItem("PrintPreview"));
                    }
                    catch { }
                    BForm.ptintview = isPreview;
                    if (isPreview)
                    {
                        BForm.ShowDialog();
                    }
                    else
                    {
                        BForm.startPrint();
                    }
                }
                catch { }
            }
            catch { }
        }
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

                    if (strc1.Trim() == "订单号" || strc1.Trim() == "生产日期" || strc1.Trim() == "是否过秤" || strc1.Trim() == "产品编号" || strc1.Trim() == "序号" || strc1.Trim() == "生产批号" || strc1.Trim() == "实际单位" || strc1.Trim() == "订单编号" || strc1.Trim() == "分拣确认" || strc1.Trim() == "单价" || strc1.Trim() == "总价")
                    {
                        dgv.Columns[strc1].Visible = false;
                    }
                }

                //自动列宽
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (lbl_ddh.Text != "")
                {
                    DataTable cpPrint = mytableDD.Copy();
                    DataRow[] myrows = null;
                    myrows = cpPrint.Select();
                    int num = 0;
                    foreach (DataRow item in cpPrint.Rows)
                    {
                        num++;
                        item["序号"] = num;


                        if (int.Parse(DateTime.Parse(item["生产日期"].ToString()).Hour.ToString()) >= 12)
                        {
                            item["生产批号"] = item["产品编号"].ToString() + DateTime.Parse(item["生产日期"].ToString()).AddDays(1).ToString("yyyyMMdd");
                            item["生产日期"] = DateTime.Parse(item["生产日期"].ToString()).AddDays(1).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            item["生产批号"] = item["产品编号"].ToString() + DateTime.Parse(item["生产日期"].ToString()).ToString("yyyyMMdd");
                            item["生产日期"] = DateTime.Parse(item["生产日期"].ToString()).ToString("yyyy-MM-dd");
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
            catch (Exception ex) { }
        }


    }
}
