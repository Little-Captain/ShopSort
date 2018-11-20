using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using gregn6Lib;
using System.IO.Ports;

namespace ZXY_ZXSC
{
    public partial class GridReportForm : Form
    {
        public DataTable printtable = new DataTable();
        public DataTable myprinttable = new DataTable();
        public string reportname = "";
        private GridppReport Report = new GridppReport();
        public bool ptintview = true;
        public string fclist = "";


        public GridReportForm()
        {
            InitializeComponent();
        }

        public void GridReportForm_Load(object sender, EventArgs e)
        {
            startPrint();
        }

        public void startPrint()
        {
            string path = Application.StartupPath.ToLower();//获取启动了应用程序的可执行文件的路径，不包括可执行文件的名称
            string FileName = path + "\\" + reportname;
            try { Report.LoadFromFile(FileName); }
            catch { }
            try { Report.DetailGrid.Recordset.ConnectionString = ""; }
            catch { }

            //连接报表事件
            Report.ProcessBegin += new _IGridppReportEvents_ProcessBeginEventHandler(FillRecordToReport);

            if (ptintview)
            {
                try { axGRPrintViewer1.Report = Report; }
                catch { }
                //启动报表运行
                try { axGRPrintViewer1.Start(); }
                catch { }
            }
            else { Report.Print(false); }//无预览，直接打印
        }


        private struct MatchFieldPairType
        {
            public IGRField grField;
            public int MatchColumnIndex;
        }
        // 将 DataTable 的数据转储到 Grid++Report 的数据集中
        public void FillRecordToReport()
        {
            try
            {
                #region 表头尾数据获取

                for (int i = 0; i < myprinttable.Columns.Count; i++)
                {
                    try
                    {
                        Report.ParameterByName(myprinttable.Columns[i].ColumnName).AsString = myprinttable.Rows[0][i].ToString();
                    }
                    catch { }
                }
                #region 有副窗进行转换
                DataTable dt = new DataTable();
                for (int i = 0; i < printtable.Columns.Count; i++)
                {
                    try
                    {
                        #region 将属于窗项赋值到新的datatable中


                        string[] array = fclist.TrimEnd(',').Split(',');
                        if (fclist.Length > 0)
                        {
                            DataRow dss = dt.NewRow();
                            for (int j = 0; j < array.Length; j++)
                            {
                                dt.Columns.Add(array[j].ToString());
                                dss[j] = array[j].ToString();//列名填充

                            }
                            dt.Rows.Add(dss);
                            DataRow[] drA = printtable.Select("1=1");
                            if (drA.Length > 0)
                            {
                                foreach (DataRow drVal in drA)
                                {

                                    dt.ImportRow(drVal);
                                }
                            }

                            #endregion
                            //DataRow da = dt.NewRow();
                            #region 属于副窗的记录填充到下一行
                            DataRow[] dd = dt.Select("1=1");
                            if (drA.Length > 0)
                            {
                                foreach (DataRow dc in dd)
                                {

                                    printtable.Rows.Add(dc.ItemArray);
                                }
                            }
                        }

                        #endregion
                        #region 移除原本属于副窗的记录


                        for (int j = 0; j < array.Length; j++)
                        {

                            //da[j] = printtable.Rows[0][array[j].Trim()].ToString();
                            printtable.Columns.Remove(array[j].Trim());//移除第一行属于副窗的记录

                        }
                        #endregion


                    }
                    catch { }
                }




                #endregion


                #region 打印表中数据获取

                for (int i = 0; i < printtable.Columns.Count; i++)
                {
                    try
                    {
                        Report.ParameterByName("C" + (i + 1).ToString()).AsString = printtable.Columns[i].ColumnName;
                    }
                    catch { }
                }
                #endregion
                MatchFieldPairType[] MatchFieldPairs = new MatchFieldPairType[Math.Min(Report.DetailGrid.Recordset.Fields.Count, printtable.Columns.Count)];

                #region //根据字段名称与列名称进行匹配，建立DataReader字段与Grid++Report记录集的字段之间的对应关系
                int MatchFieldCount = 0;
                for (int i = 0; i < printtable.Columns.Count; ++i)
                {

                    foreach (IGRField fld in Report.DetailGrid.Recordset.Fields)
                    {

                        if (String.Compare(fld.Name, "C" + (i + 1).ToString(), true) == 0)//与定义的打印模板进行匹配（C+序号）
                        {
                            MatchFieldPairs[MatchFieldCount].grField = fld;
                            MatchFieldPairs[MatchFieldCount].MatchColumnIndex = i;
                            ++MatchFieldCount;
                            break;
                        }
                    }
                }
                #endregion

                #region // 将 DataTable 中的每一条记录转储到 Grid++Report 的数据集中去
                IGRRecordset Recordset = Report.DetailGrid.Recordset;//数据集
                foreach (DataRow dr in printtable.Rows)
                {
                    try { Recordset.Append(); }
                    catch { }

                    for (int i = 0; i < MatchFieldCount; ++i)
                    {
                        if (!dr.IsNull(MatchFieldPairs[i].MatchColumnIndex))
                            MatchFieldPairs[i].grField.Value = dr[MatchFieldPairs[i].MatchColumnIndex];
                    }

                    try { Recordset.Post(); }
                    catch { }
                }
                #endregion

                #endregion
            }
            catch { }
        }
    }
}
