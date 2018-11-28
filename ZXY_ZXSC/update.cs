using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;

namespace ZXY_ZXSC
{
    class UpdateTool
    {
        #region 暴露给外部使用的字段和方法
        // 下载URL
        public static string downloadURL = "";
        // 检查是否需要更新
        public static bool needUpdate()
        {
            try
            {
                string versionStr;
                string downloadURL;

                getServerVersion(baseURL, out versionStr, out downloadURL);

                Version serverVersion = new Version(versionStr);
                if (localVersion.CompareTo(serverVersion) >= 0)
                {
                    return false;
                }
                else
                {
                    UpdateTool.downloadURL = downloadURL;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 模型
        class UpdateModel
        {
            public string Version { set; get; }
            public string Url { set; get; }
        }
        class UpdateBoxingModel
        {
            public string status { set; get; }
            public UpdateModel data { set; get; }
            public string msg { set; get; }
        }
        #endregion

        #region 私有属性和方法
        static Version localVersion
        {
            get
            {
                return Assembly.LoadFile(Application.ExecutablePath).GetName().Version;
            }
        }

        static string baseURL
        {
            get
            {
                return ConfigApp.valueItem("url") + "sysSortingVersion.html?isFrom=4";
            }
        }

        static void getServerVersion(string url, out string version, out string downloadURL)
        {
            version = "";
            downloadURL = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                StreamReader stream = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string jsonStr = stream.ReadLine();
                if (jsonStr != null)
                {
                    JavaScriptSerializer json = new JavaScriptSerializer();
                    UpdateBoxingModel boxingModel = json.Deserialize<UpdateBoxingModel>(jsonStr);
                    if (boxingModel.status.Equals("200"))
                    {
                        version = boxingModel.data.Version;
                        downloadURL = boxingModel.data.Url;
                    }
                }
            }
            catch { }
        }
        #endregion
    }
}
