using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ZXY_ZXSC
{
    public partial class MCS_Index : Form
    {
        public MCS_Index()
        {
            InitializeComponent();

            //this.Text += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //checkUpdate();//检查更新
        }

        // 0：路线  1：产品  2：订单
        public int type = 0;

        // 网络基本网络地址
        public string baseURL = ConfigurationManager.AppSettings["url"];
        // 路线列表
        private string routeURL = "";
        // 按产品请求数据的
        private string sortByProductURL = "";
        // 订单列表
        private string sortByOrderURL = "";
        // 按产品分拣, 单个产品分拣确认
        public string sortByProductCheckURL = "";
        // 预打印拣货单(按产品)
        private string prePrintProductURL = "";
        // 订单详情
        private string orderDetailURL = "";

        // 打印表的产品
        DataTable mytableDD = new DataTable();

        // 首页-产品表
        private DataTable productTable = new DataTable();

        // 首页-订单表
        private DataTable orderTable = new DataTable();

        // 预打印拣货单表(按产品)
        private DataTable prePrintProductTable = new DataTable();

        private DataTable tableDD = new DataTable();

        private DataTable tableCP = new DataTable();

        private void MCS_DDLBForm_Load(object sender, EventArgs e)
        {
            #region 按产品数据表

            productTable.Columns.Add("产品编号");
            productTable.Columns.Add("产品名称");
            productTable.Columns.Add("单位");
            productTable.Columns.Add("保质期");
            productTable.Columns.Add("下单数量");
            productTable.Columns.Add("产品id");
            #endregion

            #region 按订单数据表
            orderTable.Columns.Add("订单编号");

            orderTable.Columns.Add("客户名称");
            orderTable.Columns.Add("订单ID");
            orderTable.Columns.Add("客户电话");
            orderTable.Columns.Add("地址");
            orderTable.Columns.Add("下单时间");
            orderTable.Columns.Add("状态");
            orderTable.Columns.Add("备注");
            orderTable.Columns.Add("操作");

            #endregion

            tableDD.Columns.Add("订单编号");
            tableDD.Columns.Add("客户名称");

            #region 订单详情
            mytableDD.Columns.Add("产品名称");
            mytableDD.Columns.Add("下单数量");
            mytableDD.Columns.Add("单位");
            mytableDD.Columns.Add("是否过秤");
            mytableDD.Columns.Add("订单编号");
            mytableDD.Columns.Add("客户名称");
            mytableDD.Columns.Add("产品编号");
            mytableDD.Columns.Add("实际单位");
            mytableDD.Columns.Add("保质期");
            mytableDD.Columns.Add("分拣数量");
            mytableDD.Columns.Add("分拣确认");
            #endregion

            dataGridView1.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            routeURL = baseURL + "listRouteDesktop.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4";
            requestGetJson(routeURL);

            btn_print.Visible = false;
            type = 2;
            sortByOrderURL = baseURL + "sorteByOrder.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4&scRouteId=" + com_lx.SelectedValue + "";
            requestGetJson(sortByOrderURL);

            #region 按产品分拣单打印
            prePrintProductTable.Columns.Add("客户名称");
            prePrintProductTable.Columns.Add("产品名称");
            prePrintProductTable.Columns.Add("下单数量");
            prePrintProductTable.Columns.Add("单位");
            prePrintProductTable.Columns.Add("备注");
            #endregion
        }


        //检查更新
        public void checkUpdate()
        {
            SoftUpdate app = new SoftUpdate(Application.ExecutablePath, "BlogWriter");
            try
            {
                if (app.IsUpdate && MessageBox.Show("检查到新版本，是否更新？", "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    //更新（调用更新的exe，这个是单独的一个程序，下面再说怎么写）
                    string fileName = Application.StartupPath + @"\Updata.exe";
                    Process p = new Process();
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.FileName = fileName;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.Arguments = "";//参数以空格分隔，如果某个参数为空，可以传入””
                    p.Start();
                    System.Environment.Exit(System.Environment.ExitCode);   //结束主线程
                                                                            // p.WaitForExit(); //这里就不能等他结束了
                                                                            // string output = p.StandardOutput.ReadToEnd(); //this.Dispose();//关闭主程序


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //打印
        private void btn_print_Click(object sender, EventArgs e)
        {
            try
            {
                if (type == 1)
                {
                    //MessageBox.Show("推荐最多选择5个产品！");
                    try { tableCP.Clear(); tableCP.Columns.Clear(); } catch { }

                    type = 3;
                    prePrintProductTable.Clear();
                    prePrintProductURL = baseURL + "sorteOrder.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4&scRouteId=" + com_lx.SelectedValue + "";
                    requestGetJson(prePrintProductURL);
                    type = 1;
                    if (prePrintProductTable.Rows.Count > 0)
                    {

                        tableCP.Columns.Add("门店");
                        //使用方法
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            try
                            {
                                if (dataGridView1.Rows[i].Cells["选择"].Value.ToString() == "True")
                                {
                                    if (tableCP.Columns.Count < 6)
                                    {
                                        tableCP.Columns.Add(dataGridView1.Rows[i].Cells["产品名称"].Value.ToString() + "(" + dataGridView1.Rows[i].Cells["单位"].Value.ToString() + ")");
                                    }
                                    else
                                    {
                                        MessageBox.Show("推荐最多选择5个产品！");
                                        break;
                                    }
                                }
                            }
                            catch { continue; }
                        }
                        if (tableCP.Columns.Count == 1)
                        {
                            MessageBox.Show("暂未选择产品");
                        }
                        tableCP.Columns.Add("备注");

                        //添加行
                        for (int i = 0; i < prePrintProductTable.Rows.Count; i++)
                        {
                            string customerName = prePrintProductTable.Rows[i]["客户名称"].ToString();
                            string prodName = prePrintProductTable.Rows[i]["产品名称"].ToString() + "(" + prePrintProductTable.Rows[i]["单位"].ToString() + ")";
                            double sl = double.Parse(prePrintProductTable.Rows[i]["下单数量"].ToString());
                            string remark = prePrintProductTable.Rows[i]["备注"].ToString();
                            DataRow[] arrChkExist = tableCP.Select("门店='" + customerName + "'");

                            if (tableCP.Columns.Contains(prodName))
                            {
                                if (arrChkExist.Length > 0)
                                {
                                    double currentSL = 0;
                                    //string currentRemark = "";

                                    try { currentSL = double.Parse(arrChkExist[0][prodName].ToString()); } catch { }
                                    //try { currentRemark = arrChkExist[0]["备注"].ToString(); } catch { }

                                    //if (remark.Trim().Length > 0)
                                    //{
                                    //    if (currentRemark.Trim().Length > 0)
                                    //        remark = remark + "," + currentRemark;
                                    //}

                                    arrChkExist[0][prodName] = currentSL + sl;
                                    //if (remark.Trim().Length > 0)
                                        arrChkExist[0]["备注"] = remark;
                                }
                                else
                                {
                                    DataRow dr = tableCP.NewRow();
                                    dr["门店"] = customerName;


                                    dr[prodName] = sl;
                                    dr["备注"] = remark;

                                    tableCP.Rows.Add(dr);
                                }
                            }
                        }

                        DataRow sumRow = tableCP.NewRow();
                        sumRow["门店"] = "合计";
                        for (int i = 1; i < tableCP.Columns.Count - 1; i++)
                        {
                            string ColumnName = tableCP.Columns[i].ToString();
                            double d = 0;
                            foreach (DataRow row in tableCP.Rows)
                            {
                                if (row[ColumnName].ToString().Trim() != "")
                                {
                                    d += double.Parse(row[ColumnName].ToString());
                                }
                            }
                            sumRow[ColumnName] = d;
                        }
                        tableCP.Rows.Add(sumRow);
                        DataRow[] tableCPRow = tableCP.Select("1=1");
                        print(tableCPRow);
                    }
                    else
                    {
                        MessageBox.Show("暂无数据！");
                        type = 1;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); dataGridView1.DataSource = null; }
        }
        private double ColumnSum(DataTable dt, string ColumnName)
        {
            double d = 0;
            foreach (DataRow row in dt.Rows)
            {
                d += double.Parse(row[ColumnName].ToString());
            }
            return d;
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
                    BForm.printtable = myprinttable;
                    if (type == 1)
                    {
                        BForm.reportname = "cp_jhd.grf";
                        type = 1;
                    }
                    else
                    {
                        BForm.myprinttable = tableDD;
                        BForm.reportname = "dd_jhd.grf";
                        type = 2;
                    }
                    BForm.ptintview = true;//false无预览，直接打印
                    BForm.ShowDialog();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); dataGridView1.DataSource = null; }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); dataGridView1.DataSource = null; }
        }
        #region 接收参数 实体
        #region 按产品接口
        public class Products
        {
            public string ProductCode { get; set; }//产品编号
            public string OrderCount { set; get; }//下单数量
            public string Unit { set; get; }//单位
            public string ProductName { set; get; }//产品名称
            public string QualityTime { set; get; }//保质期
            public int ScProductID { set; get; }//产品ID

            public static string CodeKey = "ProductCode";
            public static string CountKey = "OrderCount";
            public static string UnitKey = "Unit";
            public static string NameKey = "ProductName";
            public static string QualityTimeKey = "QualityTime";
            public static string IDKey = "ScProductID";
        }
        public class ProductData
        {
            public string status { set; get; }
            public List<Products> data { set; get; }
            public string msg { set; get; }
        }
        #endregion
        #region 按产品打印接口
        public class ProductPrint
        {
            public string DepartmentName { set; get; }//客户名称
            public string OrderCount { set; get; }//下单数量
            public string Unit { set; get; }//单位
            public string ProductName { set; get; }//产品名称
            public string Remark { set; get; }//备注
            public int OrderNum { set; get; } // 顺序
        }
        public class ProductPrintData
        {
            public string status { set; get; }
            public List<ProductPrint> data { set; get; }
            public string msg { set; get; }
        }
        #endregion
        #region 路线接口参数
        public class DesciptionData
        {
            public string status { get; set; }
            public List<Desciptions> data { get; set; }
            public string msg { get; set; }
        }
        public class Desciptions
        {
            public int ScRouteID { get; set; }
            public string Route { get; set; }
        }
        #endregion
        #region 按订单接口
        public class Orders
        {
            public string StatusName { get; set; }
            public string DepartmentName { set; get; }
            public string OrderTime { set; get; }
            public string OrderNo { set; get; }
            public int ID { set; get; }
            public string ScOrderID { set; get; }
            public int UnsortedNumber { set; get; }
            public string Remark { get; set; }
            public string Mobile { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
            public string Province { get; set; }
            public string City { get; set; }
        }
        public class OrdersData
        {
            public string status { set; get; }
            public List<Orders> data { set; get; }
            public string msg { set; get; }
        }
        #endregion
        #region 按订单打印
        public class OrderProduct
        {
            public string QualityTime { get; set; }
            public string ActualCount { get; set; }
            public decimal OrderCount { set; get; }
            //public int ScOrderID { set; get; }
            //public float SellingPrice { set; get; }
            public string OrderNo { set; get; }
            public int ScProductID { set; get; }
            public string ProductName { set; get; }
            public string ActualUnit { set; get; } // 分拣单位
            public string NeedWeighted { get; set; }
            public string Unit { set; get; } // 下单单位
            public bool IsSorted { set; get; }
        }
        public class Order
        {
            //public string ActualCount { get; set; }
            //public decimal OrderCount { set; get; }
            //public string QualityTime { get; set; }
            ////public int ScOrderID { set; get; }
            ////public float SellingPrice { set; get; }
            //public string OrderNo { set; get; }
            //public int ScProductID { set; get; }
            //public string ProductName { set; get; }
            //public string ActualUnit { set; get; } // 分拣单位
            //public string NeedWeighted { get; set; }
            //public string Unit { set; get; } // 下单单位
            //public bool IsSorted { set; get; }
            public List<OrderProduct> listDetail { set; get; }
            public string DepartmentName { set; get; }
        }
        public class OrderData
        {
            public string status { set; get; }
            public Order data { set; get; }
            public string msg { set; get; }
        }
        #endregion
        #region 产品详情接口
        public class Product
        {
            public string Unit { get; set; }
            public bool IsSorted { get; set; }
            public string ActualCount { get; set; }
            public string DepartmentName { get; set; }
            public string NeedWeighted { get; set; }
            public string ScOrderID { get; set; }//订单id
            public string ProductCode { get; set; }//产品编号
            public string OrderCount { set; get; }//下单数量
            public string ActualUnit { set; get; }//单位
            public string ProductName { set; get; }//产品名称
            public string QualityTime { set; get; }//保质期
            public int ScProductID { set; get; }//产品ID
        }
        public class ProductsData
        {
            public string status { set; get; }
            public List<Product> data { set; get; }
            public string msg { set; get; }
        }
        #endregion
        #endregion

        string jsonstr = "";
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
                if (jsonstr != null)
                {
                    switch (type)
                    {
                        case 0:
                            #region 路线接口
                            DesciptionData desciptionData = js.Deserialize<DesciptionData>(jsonstr);
                            if (desciptionData.status.ToString().Equals("200"))
                            {
                                List<Desciptions> desciption = new List<Desciptions>();
                                for (int i = 0; i < desciptionData.data.Count; i++)
                                {
                                    Desciptions ne = new Desciptions();
                                    ne.Route = desciptionData.data[i].Route;
                                    ne.ScRouteID = desciptionData.data[i].ScRouteID;
                                    desciption.Add(ne);
                                }
                                com_lx.DataSource = desciption;
                                com_lx.ValueMember = "ScRouteID";
                                com_lx.DisplayMember = "Route";
                                com_lx.SelectedIndex = 0;
                                type = 2;
                            }
                            break;
                        #endregion
                        case 1:
                            #region 按产品接口
                            productTable.Rows.Clear();
                            ProductData cpData = js.Deserialize<ProductData>(jsonstr);
                            if (cpData.status.ToString().Equals("200"))
                            {
                                List<Products> ProductList = cpData.data;
                                if (ProductList.Count > 0)
                                {
                                    for (int i = 0; i < ProductList.Count; i++)
                                    {
                                        DataRow cprow = productTable.NewRow();
                                        cprow["产品编号"] = ProductList[i].ProductCode;
                                        cprow["产品名称"] = ProductList[i].ProductName;
                                        cprow["保质期"] = ProductList[i].QualityTime;
                                        cprow["产品id"] = ProductList[i].ScProductID;
                                        cprow["下单数量"] = ProductList[i].OrderCount + ProductList[i].Unit;
                                        cprow["单位"] = ProductList[i].Unit;
                                        productTable.Rows.Add(cprow);
                                    }
                                }

                                yc(dataGridView1, productTable);
                                dataGridView1.ClearSelection();
                            }
                            #endregion
                            break;
                        case 2:
                            #region 按订单接口
                            orderTable.Rows.Clear();
                            OrdersData ordersData = js.Deserialize<OrdersData>(jsonstr);

                            if (ordersData.status.ToString().Equals("200"))
                            {
                                List<Orders> OrderList = ordersData.data;
                                if (OrderList.Count > 0)
                                {
                                    for (int i = 0; i < OrderList.Count; i++)
                                    {
                                        DataRow tableProductRow = orderTable.NewRow();
                                        tableProductRow["客户名称"] = OrderList[i].DepartmentName;
                                        tableProductRow["订单ID"] = OrderList[i].ScOrderID;
                                        tableProductRow["客户电话"] = OrderList[i].Mobile;
                                        tableProductRow["地址"] = OrderList[i].Country + OrderList[i].Province + OrderList[i].City + OrderList[i].Address;
                                        tableProductRow["下单时间"] = OrderList[i].OrderTime;
                                        tableProductRow["备注"] = OrderList[i].Remark;
                                        tableProductRow["订单编号"] = OrderList[i].OrderNo;
                                        if (OrderList[i].StatusName != "待分拣")
                                        {
                                            tableProductRow["状态"] = "已完成分拣";
                                            tableProductRow["操作"] = "打印";
                                        }
                                        else
                                        {
                                            tableProductRow["状态"] = "未完成分拣";
                                            tableProductRow["操作"] = "分拣";
                                        }
                                        orderTable.Rows.Add(tableProductRow);
                                    }
                                }

                                yc(dataGridView1, orderTable);
                                dataGridView1.ClearSelection();
                            }
                            #endregion
                            break;
                        case 3:
                            #region 按产品打印接口
                            ProductPrintData pruductPrintData = js.Deserialize<ProductPrintData>(jsonstr);
                            if (pruductPrintData.status.ToString().Equals("200"))
                            {
                                List<ProductPrint> ProductList = pruductPrintData.data;
                                if (ProductList.Count > 0)
                                {
                                    DataTable tmpTable = new DataTable();
                                    tmpTable.Columns.Add("客户名称");
                                    tmpTable.Columns.Add("产品名称");
                                    tmpTable.Columns.Add("下单数量");
                                    tmpTable.Columns.Add("单位");
                                    tmpTable.Columns.Add("备注");
                                    tmpTable.Columns.Add("Sort", System.Type.GetType("System.Int32"));
                                    for (int i = 0; i < ProductList.Count; i++)
                                    {
                                        DataRow cpPrintrow = tmpTable.NewRow();
                                        cpPrintrow["客户名称"] = ProductList[i].DepartmentName;
                                        cpPrintrow["产品名称"] = ProductList[i].ProductName;
                                        cpPrintrow["下单数量"] = ProductList[i].OrderCount;
                                        cpPrintrow["单位"] = ProductList[i].Unit;
                                        cpPrintrow["备注"] = ProductList[i].Remark;
                                        cpPrintrow["Sort"] = ProductList[i].OrderNum;
                                        tmpTable.Rows.Add(cpPrintrow);
                                    }
                                    tmpTable.DefaultView.Sort = "Sort ASC";
                                    tmpTable = tmpTable.DefaultView.ToTable();
                                    tmpTable.Columns.Remove("Sort");
                                    prePrintProductTable = tmpTable.Copy();
                                }
                            }
                            #endregion
                            break;
                        case 4:
                            #region 按订单打印接口
                            OrderData orderData = js.Deserialize<OrderData>(jsonstr);

                            if (orderData.status.ToString().Equals("200"))
                            {
                                List<OrderProduct> productList = orderData.data.listDetail;
                                if (productList != null)
                                {

                                    for (int i = 0; i < productList.Count; i++)
                                    {
                                        DataRow dr = mytableDD.NewRow();
                                        dr["客户名称"] = orderData.data.DepartmentName;
                                        dr["订单编号"] = productList[i].OrderNo;
                                        dr["产品编号"] = productList[i].ScProductID;
                                        dr["产品名称"] = productList[i].ProductName;
                                        dr["下单数量"] = productList[i].OrderCount;
                                        dr["单位"] = productList[i].Unit;
                                        dr["实际单位"] = productList[i].ActualUnit;
                                        dr["保质期"] = productList[i].QualityTime + "天";
                                        dr["分拣数量"] = productList[i].ActualCount;
                                        dr["是否过秤"] = productList[i].NeedWeighted == "True" ? "是" : "否";
                                        if (productList[i].IsSorted == false)
                                        {
                                            dr["分拣确认"] = "确认";
                                        }
                                        else
                                        {
                                            dr["分拣确认"] = "已确认";
                                        }
                                        mytableDD.Rows.Add(dr);
                                    }
                                }
                            }
                            #endregion
                            break;
                        case 5:
                            #region 按产品分拣确认接口
                            ProductsData cpdetailData = js.Deserialize<ProductsData>(jsonstr);
                            if (cpdetailData.status.ToString().Equals("200"))
                            {
                                List<Product> ProductList = cpdetailData.data;
                                if (ProductList.Count > 0)
                                {
                                    for (int i = 0; i < ProductList.Count; i++)
                                    {
                                        DataRow cprow = tableCP.NewRow();
                                        cprow["订单id"] = ProductList[i].ScOrderID;
                                        cprow["产品编号"] = ProductList[i].ProductCode;
                                        cprow["产品名称"] = ProductList[i].ProductName;
                                        cprow["保质期"] = ProductList[i].QualityTime;
                                        cprow["产品id"] = ProductList[i].ScProductID;
                                        cprow["分拣数量"] = ProductList[i].ActualCount;
                                        cprow["下单数量"] = ProductList[i].OrderCount + ProductList[i].Unit;
                                        cprow["客户名称"] = ProductList[i].DepartmentName;
                                        cprow["分拣单位"] = ProductList[i].ActualUnit;
                                        cprow["是否过秤"] = ProductList[i].NeedWeighted == "True" ? "是" : "否";
                                        if (ProductList[i].IsSorted == false)
                                        {
                                            cprow["分拣确认"] = "确认";
                                        }
                                        else
                                        {
                                            cprow["分拣确认"] = "已确认";
                                        }
                                        tableCP.Rows.Add(cprow);
                                    }
                                }

                            }
                            #endregion
                            break;
                    }
                }
            }
            catch (Exception ex) { }

        }
        private void yc(DataGridView dgv, DataTable mydt)//只允许输入部分列
        {
            try
            {
                dgv.DataSource = null;
                dgv.Columns.Clear();
                dgv.DataSource = mydt.DefaultView;
                string strc1 = "";
                DataGridViewCheckBoxColumn dtCheck = new DataGridViewCheckBoxColumn();
                dtCheck.HeaderText = "选择";
                dtCheck.Name = "选择";
                if (type == 1)
                {
                    dgv.Columns.Add(dtCheck);
                }
                for (int j = 0; j < dgv.Columns.Count; j++) //写列标题 
                {
                    strc1 = dgv.Columns[j].Name.ToUpper();
                    if (strc1 == "选择")
                    {
                        dgv.Columns[strc1].ReadOnly = false;
                    }
                    else
                    {
                        dgv.Columns[strc1].ReadOnly = true;
                    }
                    dgv.Columns[strc1].SortMode = DataGridViewColumnSortMode.NotSortable;

                    dgv.Columns[strc1].DefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                    if (strc1.IndexOf("I") >= 0 || strc1.Trim() == "客户电话" || strc1.Trim() == "单位" || strc1.Trim() == "保质期" || strc1.Trim() == "地址")
                    {
                        dgv.Columns[strc1].Visible = false;
                    }
                }
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                //自动列宽
            }
            catch (Exception ex) { }
        }
        //rediobutton事件
        private void rioCP_CheckedChanged(object sender, EventArgs e)
        {
            if (rioCP.Checked)
            {
                btn_print.Visible = false;
                type = 2;
                sortByOrderURL = baseURL + "sorteByOrder.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4&scRouteId=" + com_lx.SelectedValue + "";
                requestGetJson(sortByOrderURL);
            }
            else if (rioDD.Checked)
            {
                btn_print.Visible = true;
                type = 1;
                sortByProductURL = baseURL + "sorteByProduct.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4&scRouteId=" + com_lx.SelectedValue + "";
                requestGetJson(sortByProductURL);
            }
        }
        //选择路线
        private void com_lx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (rioCP.Checked)
            {
                type = 2;
                sortByOrderURL = baseURL + "sorteByOrder.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4&scRouteId=" + com_lx.SelectedValue + "";
                requestGetJson(sortByOrderURL);
            }
            else if (rioDD.Checked)
            {
                type = 1;
                sortByProductURL = baseURL + "sorteByProduct.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4&scRouteId=" + com_lx.SelectedValue + "";
                requestGetJson(sortByProductURL);
            }

        }

        private void btn_history_Click(object sender, EventArgs e)
        {
            MCS_DDLBListForm mlForm = new MCS_DDLBListForm();
            mlForm.url = baseURL + "departmentList.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4&scRouteId=" + com_lx.SelectedValue + "";
            mlForm.ShowDialog();
        }


        //private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    try
        //    {
        //        if (type == 2)
        //        {
        //            if (dataGridView1.Columns[e.ColumnIndex].HeaderText.ToString() == "订单编号")
        //            {
        //                type = 4;
        //                tableDD.Clear();
        //                mytableDD.Clear();
        //                prePrintProductTable.Clear();
        //                orderDetailURL = baseURL + "orderDetail.html?isFrom=4&scOrderId=" + dataGridView1.Rows[e.RowIndex].Cells["订单ID"].Value.ToString() + "";
        //                requestGetJson(orderDetailURL);
        //                type = 2;
        //                DataRow rows = tableDD.NewRow();
        //                rows["订单编号"] = dataGridView1.Rows[e.RowIndex].Cells["订单编号"].Value.ToString();
        //                rows["客户名称"] = dataGridView1.Rows[e.RowIndex].Cells["客户名称"].Value.ToString();
        //                tableDD.Rows.Add(rows);
        //                if (mytableDD.Rows.Count > 0)
        //                {

        //                    DataRow[] cpPrintRow = mytableDD.Select();

        //                    print(cpPrintRow);
        //                }
        //                else
        //                {
        //                    MessageBox.Show("暂无数据！");
        //                }
        //            }
        //            else
        //            {
        //                type = 4;
        //                mytableDD.Clear();
        //                orderDetailURL = baseURL + "orderDetail.html?isFrom=4&scOrderId=" + dataGridView1.Rows[e.RowIndex].Cells["订单ID"].Value.ToString() + "";
        //                requestGetJson(orderDetailURL);
        //                MCS_DDLBForm mform = new MCS_DDLBForm();
        //                mform.lx = com_lx.Text.ToString();
        //                mform.type = 2;
        //                mform.khmc = dataGridView1.Rows[e.RowIndex].Cells["客户名称"].Value.ToString();
        //                mform.xdsj = dataGridView1.Rows[e.RowIndex].Cells["下单时间"].Value.ToString();
        //                mform.ddbh = dataGridView1.Rows[e.RowIndex].Cells["订单编号"].Value.ToString();
        //                mform.mytableDD = mytableDD;
        //                mform.ShowDialog();
        //                type = 2;
        //            }
        //        }
        //        else if(type==1)
        //        {
        //            type = 5;
        //            tableCP = productTable.Copy();
        //            if (tableCP.Columns.Count ==6)
        //            {
        //                tableCP.Columns.Add("订单id");
        //                tableCP.Columns.Add("是否过秤");
        //                tableCP.Columns.Add("分拣数量");
        //                tableCP.Columns.Add("分拣确认");
        //            }
        //            tableCP.Clear();
        //            sortByProductCheckURL = baseURL + "sorteByProductCheck.html?isFrom=4&scRouteId=" + com_lx.SelectedValue + "&companyId=" + ConfigurationManager.AppSettings["companyId"] + "&scProductId=" + int.Parse(dataGridView1.Rows[e.RowIndex].Cells["产品id"].Value.ToString());
        //            requestGetJson(sortByProductCheckURL);
        //            type = 1;
        //            MCS_DDLBForm mform = new MCS_DDLBForm();
        //            mform.lx = com_lx.Text.ToString();
        //            mform.type = 1;

        //            mform.khmc = dataGridView1.Rows[e.RowIndex].Cells["产品名称"].Value.ToString();
        //            mform.xdsj = dataGridView1.Rows[e.RowIndex].Cells["下单数量"].Value.ToString();
        //            mform.ddbh = dataGridView1.Rows[e.RowIndex].Cells["产品编号"].Value.ToString();
        //            mform.mytableDD = tableCP;
        //            mform.ShowDialog();
        //        }
        //    }
        //    catch(Exception ex) { }
        //}

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (type == 2)
                {
                    if (dataGridView1.Columns[e.ColumnIndex].HeaderText.ToString() == "客户名称")
                    {
                        // 打印拣货单
                        type = 4;
                        tableDD.Clear();
                        mytableDD.Clear();
                        prePrintProductTable.Clear();
                        orderDetailURL = baseURL + "orderDetail.html?isFrom=4&scOrderId=" + dataGridView1.Rows[e.RowIndex].Cells["订单ID"].Value.ToString() + "";
                        requestGetJson(orderDetailURL);
                        type = 2;
                        DataRow rows = tableDD.NewRow();
                        rows["订单编号"] = dataGridView1.Rows[e.RowIndex].Cells["订单编号"].Value.ToString();
                        rows["客户名称"] = dataGridView1.Rows[e.RowIndex].Cells["客户名称"].Value.ToString();
                        tableDD.Rows.Add(rows);
                        if (mytableDD.Rows.Count > 0)
                        {
                            DataRow[] cpPrintRow = mytableDD.Select();
                            
                            print(cpPrintRow);
                        }
                        else
                        {
                            MessageBox.Show("暂无数据！");
                        }
                    }
                    else if (dataGridView1.Columns[e.ColumnIndex].HeaderText.ToString() == "操作")
                    {
                        // 进入分拣, 或者打印
                        type = 4;
                        mytableDD.Clear();
                        orderDetailURL = baseURL + "orderDetail.html?isFrom=4&scOrderId=" + dataGridView1.Rows[e.RowIndex].Cells["订单ID"].Value.ToString() + "";
                        requestGetJson(orderDetailURL);
                        type = 2;
                        MCS_DDLBForm mform = new MCS_DDLBForm();
                        mform.lx = com_lx.Text.ToString();
                        mform.type = 2;
                        mform.orderID = dataGridView1.Rows[e.RowIndex].Cells["订单ID"].Value.ToString();
                        mform.khmc = dataGridView1.Rows[e.RowIndex].Cells["客户名称"].Value.ToString();
                        mform.xdsj = dataGridView1.Rows[e.RowIndex].Cells["下单时间"].Value.ToString();
                        mform.ddbh = dataGridView1.Rows[e.RowIndex].Cells["订单编号"].Value.ToString();
                        mform.remark = dataGridView1.Rows[e.RowIndex].Cells["备注"].Value.ToString();
                        mform.url = orderDetailURL;
                        //mform.mytableDD = mytableDD;
                        mform.ShowDialog();
                        type = 2;
                    }
                }
                else if (type == 1)
                {
                    type = 5;
                    tableCP = productTable.Copy();
                    if (tableCP.Columns.Count == 6)
                    {
                        tableCP.Columns.Add("订单id");
                        tableCP.Columns.Add("客户名称");
                        tableCP.Columns.Add("分拣单位");
                        tableCP.Columns.Add("分拣数量");
                        tableCP.Columns.Add("是否过秤");
                        tableCP.Columns.Add("分拣确认");
                    }
                    tableCP.Clear();
                    sortByProductCheckURL = baseURL + "sorteByProductCheck.html?isFrom=4&scRouteId=" + com_lx.SelectedValue + "&companyId=" + ConfigurationManager.AppSettings["companyId"] + "&scProductId=" + int.Parse(dataGridView1.Rows[e.RowIndex].Cells["产品id"].Value.ToString());
                    requestGetJson(sortByProductCheckURL);
                    type = 1;
                    MCS_DDLBForm mform = new MCS_DDLBForm();
                    mform.lx = com_lx.Text.ToString();
                    mform.type = 1;
                    mform.khmc = dataGridView1.Rows[e.RowIndex].Cells["产品名称"].Value.ToString();
                    mform.xdsj = dataGridView1.Rows[e.RowIndex].Cells["下单数量"].Value.ToString();
                    mform.ddbh = dataGridView1.Rows[e.RowIndex].Cells["产品编号"].Value.ToString();
                    mform.mytableCP = tableCP;
                    mform.ShowDialog();
                }
            }
            catch (Exception ex) { }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].HeaderText == "选择")
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = true;
            }
        }

        private void MCS_Index_Activated(object sender, EventArgs e)
        {
            if (type == 1)
            {
                btn_print.Visible = true;
                type = 1;
                sortByProductURL = baseURL + "sorteByProduct.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4&scRouteId=" + com_lx.SelectedValue + "";
                requestGetJson(sortByProductURL);
            }
            else if (type == 2)
            {
                btn_print.Visible = false;
                type = 2;
                sortByOrderURL = baseURL + "sorteByOrder.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4&scRouteId=" + com_lx.SelectedValue + "";
                requestGetJson(sortByOrderURL);
            }
        }
    }
}
