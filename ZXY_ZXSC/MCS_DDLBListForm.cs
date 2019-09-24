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

namespace ZXY_ZXSC
{
    public partial class MCS_DDLBListForm : Form
    {
        public MCS_DDLBListForm()
        {
            InitializeComponent();
            this.Text += System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        public string url = "";//接口url
        string jsonstr = "";//json
        string mytj = "";
        DataTable mytableDD = new DataTable();//打印产品
        DataTable tableProduct = new DataTable();//打印表头
        public string pageIndex = "1";//当前页码
        public string pageSize = "20";//每页条数
        public int isuser = 1;
        private void MCS_DDLBListForm_Load(object sender, EventArgs e)
        {
            btback.Enabled = false;
            dataGridView1.RowsDefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.EnableHeadersVisualStyles = false;
            #region  订单表
            mytableDD.Columns.Add("订单号");
            mytableDD.Columns.Add("客户名称");
            mytableDD.Columns.Add("客户电话");
            mytableDD.Columns.Add("地址");
            mytableDD.Columns.Add("订单ID");
            mytableDD.Columns.Add("下单时间");
            mytableDD.Columns.Add("备注");
            #endregion
            requestGetJson(url);

            isuser = 2;
            url = ConfigurationManager.AppSettings["url"] + "orderHistorySearch.html?companyId=" + ConfigurationManager.AppSettings["companyId"] + "&isFrom=4";

            requestGetJson(url + "&startDate="+startDate.Text+"&endDate="+endDate.Text+"&pageIndex=1&pageSize=10");

        }
        //打印
        private void btn_print_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_ddh.Text != "")
                {
                    DataRow[] myrows = null;
                    for (int i = 0; i < mytableDD.Rows.Count; i++)
                    {
                        if (mytableDD.Rows[i]["订单号"].ToString() == txt_ddh.Text && mytableDD.Rows[i]["分拣确认"].ToString() == "已确认")
                        {
                            myrows = mytableDD.Select("订单号='" + txt_ddh.Text + "' and 分拣确认='已确认'");
                        }
                        else
                        {
                            MessageBox.Show("该订单还有未确认的产品，不可打印！");
                            return;
                        }
                    }

                    print(myrows);//打印报表
                }
                else
                {
                    MessageBox.Show("请选择需要打印的订单!");
                }

            }
            catch { }
        }
        //执行打印
        private void print(DataRow[] myrows)//打印报表
        {
            try
            {
                DataRow[] pruduct = mytableDD.Select("订单号='" + txt_ddh.Text + "' and 分拣确认='已确认'");
                for (int i = 0; i < pruduct.Length; i++)
                {
                    DataRow rows = tableProduct.NewRow();
                    rows["产品编号"] = pruduct[i]["产品编号"].ToString();
                    rows["产品名称"] = pruduct[i]["产品名称"].ToString();
                    rows["计量单位"] = pruduct[i]["计量单位"].ToString();
                    rows["实际单位"] = pruduct[i]["实际单位"].ToString();
                    rows["下单数量"] = pruduct[i]["下单数量"].ToString();
                    rows["分拣数量"] = pruduct[i]["分拣数量"].ToString();
                    rows["产品编号"] = pruduct[i]["产品编号"].ToString();
                    tableProduct.Rows.Add(rows);
                }
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
        //执行打印
        private void printForStorage(DataRow[] myrows)//打印报表
        {
            try
            {
                DataRow[] pruduct = mytableDD.Select("订单号='" + txt_ddh.Text + "' and 分拣确认='已确认'");
                for (int i = 0; i < pruduct.Length; i++)
                {
                    DataRow rows = tableProduct.NewRow();
                    rows["产品编号"] = pruduct[i]["产品编号"].ToString();
                    rows["产品名称"] = pruduct[i]["产品名称"].ToString();
                    rows["计量单位"] = pruduct[i]["计量单位"].ToString();
                    rows["实际单位"] = pruduct[i]["实际单位"].ToString();
                    rows["下单数量"] = pruduct[i]["下单数量"].ToString();
                    rows["分拣数量"] = pruduct[i]["分拣数量"].ToString();
                    rows["产品编号"] = pruduct[i]["产品编号"].ToString();
                    tableProduct.Rows.Add(rows);
                }
                DataTable myprinttable = myrows.CopyToDataTable();
                try
                {
                    GridReportForm BForm = new GridReportForm();

                    BForm.myprinttable = tableProduct;
                    BForm.printtable = myprinttable;
                    BForm.reportname = "dd_storage.grf";

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
        //请求接口
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

                if (isuser == 2)
                {
                    OrderData orderData = js.Deserialize<OrderData>(jsonstr);
                    if (orderData.status.ToString().Equals("200"))
                    {
                        Order order = orderData.data;
                        if (order != null)
                        {
                            pageCount.Text = order.totalPages;
                            if (pageCount.Text == "0")
                            {
                                btnext.Enabled = false;
                            }
                            else
                            {
                                btnext.Enabled = true;
                            }
                            pageIndex = order.pageIndex;
                            textBox9.Text = order.pageIndex;
                            pageSize = order.pageSize;
                            txtpagenumber.Text = order.pageSize;
                            List<Product> productList = order.rows;
                            if (productList != null)
                            {
                                foreach (Product prod in productList)
                                {
                                    DataRow dr = mytableDD.NewRow();
                                    mytableDD.Rows.Add(dr);
                                    dr["订单号"] = prod.OrderNo;
                                    dr["订单ID"] = prod.ScOrderID;
                                    dr["客户电话"] = prod.Mobile;
                                    dr["客户名称"] = prod.DepartmentName;
                                    dr["地址"] = prod.Country + " " + prod.Province + " " + prod.City + " " + prod.Address;
                                    dr["备注"] = prod.Remark;
                                    dr["订单号"] = prod.OrderNo;
                                    dr["下单时间"] = prod.OrderTime;
                                }
                            }
                        }
                        else
                        {
                            btnext.Enabled = false;
                            MessageBox.Show("暂无订单！");
                        }

                        yc(dataGridView1, mytableDD);
                    }

                }
                else
                {
                    result results = js.Deserialize<result>(jsonstr);
                    if (results.status.ToString().Equals("200"))
                    {
                        List<user> users = new List<user>();
                        user usess = new user();
                        usess.DepartmentName = "全部";
                        usess.DepartmentID = "0";
                        users.Add(usess);
                        for (int i = 0; i < results.data.Count; i++)
                        {
                            user ne = new user();
                            ne.DepartmentName = results.data[i].DepartmentName;
                            ne.DepartmentID = results.data[i].DepartmentID;
                            users.Add(ne);
                        }
                        com_khmc.DataSource = users;
                        com_khmc.ValueMember = "DepartmentID";
                        com_khmc.DisplayMember = "DepartmentName";
                        com_khmc.SelectedValue = "0";
                    }

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); dataGridView1.DataSource = null; }
        }

        #region 接收参数 实体
        public class Order
        {
            public string totalCount { get; set; }
            public string pageSize { get; set; }
            public string totalPages { get; set; }
            public string pageIndex { get; set; }

            public List<Product> rows { get; set; }
        }
        public class result
        {
            public string status { get; set; }
            public List<user> data { get; set; }
            public string msg { get; set; }
        }
        public class user
        {
            public string DepartmentID { get; set; }
            public string DepartmentName { get; set; }
        }
        public class OrderData
        {
            public string status { get; set; }
            public Order data { get; set; }
            public string msg { get; set; }
        }

        public class Product
        {
            public string Province { get; set; }
            public string OrderTime { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string DepartmentName { get; set; }
            public string OrderNo { get; set; }
            public string OrderCount { get; set; }
            public string Address { get; set; }
            public string ScOrderID { get; set; }
            public string ID { get; set; }
            public string Mobile { get; set; }
            public string Remark { get; set; }
            public bool IsSorted { get; set; }
        }
        #endregion

        //绑值
        private void yc(DataGridView dgv, DataTable mydt)//只允许输入部分列
        {
            try
            {
                dgv.DataSource = mydt.DefaultView;
                string strc1 = "";

                for (int j = 0; j < mydt.Columns.Count; j++) //写列标题 
                {
                    if (strc1.IndexOf("I") >= 0)
                    {
                        dgv.Columns[strc1].Visible = false;
                    }
                    strc1 = mydt.Columns[j].ColumnName.ToUpper();
                  
                    dgv.Columns[strc1].ReadOnly = true;
                    dgv.Columns[strc1].DefaultCellStyle.BackColor = Color.FromArgb(230, 230, 230);
                }
                //自动列宽
                dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch { }
        }
        //查询
        private void btn_search_Click(object sender, EventArgs e)
        {
            url = ConfigurationManager.AppSettings["url"]+"orderHistorySearch.html?companyId=" + ConfigurationManager.AppSettings["companyId"] +"&isFrom=4&scSortingPositionId=" + ConfigurationManager.AppSettings["scSortingPositionId"];
            if (txt_ddh.Text != "")
            {
                if (com_khmc.SelectedValue.ToString() == "0")
                {
                    mytj= "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                }
                else
                {
                    mytj = "&employeeId=" + com_khmc.Text + "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                }
                requestGetJson(url +mytj+ "&pageIndex=" + pageIndex + "&pageSize=" + pageSize);
            }
            else
            {
                if (com_khmc.SelectedValue.ToString() == "0")
                {
                    mytj = "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                }
                else
                {
                    mytj = "&employeeId=" + com_khmc.SelectedValue + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                }
                requestGetJson(url + mytj + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize);

            }
        }
        //输入订单号
        private void txt_ddh_Click(object sender, EventArgs e)
        {
            NumberForm znFrm = new NumberForm();
            znFrm.title = "订单编号";
            znFrm.strReturn = txt_ddh.Text;
            znFrm.ShowDialog();
                try
                {
                    txt_ddh.Text = znFrm.strReturn;
                    if (txt_ddh.Text != "")
                    {
                        if (com_khmc.Text.ToString() == "全部")
                        {
                            mytj = "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                        }
                        else
                        {
                            mytj = "&employeeId=" + com_khmc.Text + "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                        }
                    }
                    else
                    {
                        if (com_khmc.Text.ToString() == "全部")
                        {
                            mytj = "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                        }
                        else
                        {
                            mytj = "&employeeId=" + com_khmc.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                        }

                    }
                }
                catch { }
        }

        private void btback_Click(object sender, EventArgs e)//上一页
        {
            
            try
            {
                if (txt_ddh.Text != "")
                {
                    if (com_khmc.Text.ToString() == "全部")
                    {
                        mytj = "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                    else
                    {
                        mytj = "&employeeId=" + com_khmc.Text + "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                }
                else
                {
                    if (com_khmc.Text.ToString() == "全部")
                    {
                        mytj = "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                    else
                    {
                        mytj = "&employeeId=" + com_khmc.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }

                }
                if (textBox9.Text == "1")
                {
                    btback.Enabled = false;
                }
                else if (pageCount.Text == "0")
                {
                    btback.Enabled = false;
                    btnext.Enabled = false;
                }
                else
                {
                    btback.Enabled = true;
                    textBox9.Text = (int.Parse(textBox9.Text) - 1).ToString();
                    pageIndex = textBox9.Text;
                    pageSize = txtpagenumber.Text;
                    requestGetJson(url +mytj+ "&pageIndex=" + pageIndex + "&pageSize=" + pageSize);
                }

            }
            catch { }
        }
        private void btnext_Click(object sender, EventArgs e)//下一页
        {
            try
            {
                if (txt_ddh.Text != "")
                {
                    if (com_khmc.Text.ToString() == "全部")
                    {
                        mytj = "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                    else
                    {
                        mytj = "&employeeId=" + com_khmc.Text + "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                }
                else
                {
                    if (com_khmc.Text.ToString() == "全部")
                    {
                        mytj = "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                    else
                    {
                        mytj = "&employeeId=" + com_khmc.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }

                }
                if (textBox9.Text == pageCount.Text)
                {
                    btnext.Enabled = false;
                    btback.Enabled = true;
                }
                else
                {
                    btback.Enabled = true;
                    btnext.Enabled = true;
                    textBox9.Text = (int.Parse(textBox9.Text) + 1).ToString();
                    pageIndex = textBox9.Text;
                    pageSize = txtpagenumber.Text;
                    requestGetJson(url +mytj+ "&pageIndex=" + pageIndex + "&pageSize=" + pageSize);
                }
            }
            catch { }

        }

        private void textBox9_Click(object sender, EventArgs e)
        {
            if (pageCount.Text != "0")
            {
                if (txt_ddh.Text != "")
                {
                    if (com_khmc.Text.ToString() == "全部")
                    {
                        mytj = "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                    else
                    {
                        mytj = "&employeeId=" + com_khmc.Text + "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                }
                else
                {
                    if (com_khmc.Text.ToString() == "全部")
                    {
                        mytj = "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                    else
                    {
                        mytj = "&employeeId=" + com_khmc.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }

                }
                NumberForm znFrm = new NumberForm();
                znFrm.title = "选择页码";
                znFrm.pagecount = pageCount.Text;
                znFrm.strReturn = textBox9.Text;
                znFrm.ShowDialog();
                if (znFrm.strReturn.Length > 0)
                {
                    try
                    {
                        textBox9.Text = znFrm.strReturn;
                        pageIndex = textBox9.Text;
                        pageSize = txtpagenumber.Text;
                        requestGetJson(url +mytj+ "&pageIndex=" + pageIndex + "&pageSize=" + pageSize);
                    }
                    catch { }
                }
            }
        }

        private void txtpagenumber_Click(object sender, EventArgs e)
        {
            if (pageCount.Text != "0")
            {
                if (txt_ddh.Text != "")
                {
                    if (com_khmc.Text.ToString() == "全部")
                    {
                        mytj = "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                    else
                    {
                        mytj = "&employeeId=" + com_khmc.Text + "&OrderNo=" + txt_ddh.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                }
                else
                {
                    if (com_khmc.Text.ToString() == "全部")
                    {
                        mytj = "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }
                    else
                    {
                        mytj = "&employeeId=" + com_khmc.Text + "&startDate=" + startDate.Text + "&endDate=" + endDate.Text;
                    }

                }
                NumberForm znFrm = new NumberForm();
                znFrm.title = "每页条数";
                znFrm.pagecount = pageCount.Text;
                znFrm.strReturn = txtpagenumber.Text;
                znFrm.ShowDialog();
                if (znFrm.strReturn.Length > 0)
                {
                    try
                    {
                        txtpagenumber.Text = znFrm.strReturn;
                        pageIndex = textBox9.Text;
                        pageSize = txtpagenumber.Text;
                        requestGetJson(url + mytj + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize);
                    }
                    catch { }
                }
            }
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            string ddh = dataGridView1.Rows[e.RowIndex].Cells["订单ID"].Value.ToString();
            DataRow[] myrows = mytableDD.Select("订单号='" + dataGridView1.Rows[e.RowIndex].Cells["订单号"].Value.ToString() + "'");
                
            MCS_DDDetails nfrm = new MCS_DDDetails();
            nfrm.title = "订单详情";
            nfrm.ddh = ddh;
            nfrm.myrows = myrows;
            nfrm.ShowDialog();
        }
    }
}