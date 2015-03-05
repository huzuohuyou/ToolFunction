using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Configuration;
using System.Threading;
using System.Data.OleDb;
using System.IO;
using System.Xml;
using System.Data.OracleClient;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace ToolFunction
{
    public class CommonFunction
    {
        #region 属性
        private DataSet myDs = new DataSet();
        private static string excelstring = "Provider=Microsoft.Ace.OleDb.12.0;data source='{0}';Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";
        private Thread t = null;
        public static string ErrorLogPath = null;
        public static Dictionary<string, TabPage> dicpage = new Dictionary<string, TabPage>();
        public static string m_strConnectionString = "";
        public static OleDbConnection m_oleConn = null;
        public static OleDbCommand m_oleCmd = null;
        public static OleDbTransaction m_oleTrans = null;
        public static OracleConnection m_oraConn = null;
        public static OracleCommand m_oraCmd = null;
        public static OracleTransaction m_oraTrans = null;
        public static OdbcConnection m_odbcConn = null;
        public static OdbcCommand m_odbcCmd = null;
        public static OdbcTransaction m_odbcTrans = null;
        public static SqlConnection m_sqlConn = null;
        public static SqlCommand m_sqlCmd = null;
        public static SqlTransaction m_sqlTrans = null;
        #endregion

        #region 构造函数
        public CommonFunction()
        {

        }
        #endregion

        #region 程序设置主键，string类型
        //        public static strng SetKey()
        //        {
        //            DateTime currentTime = DateTime.Now;
        //            string strtime = currentTime.Minute.ToString() + ":"
        //                              + currentTime.Second.ToString() + ":"
        //                              + currentTime.Millisecond.ToString();
        //            return int.p
        //strtime;
        //        }
        #endregion

        #region 错误日志
        /// <summary>
        /// 统一化错误日志输出，以后就不用每个方法都写try...catch了，目前对需要获取返回值的方法支持还不够。
        /// </summary>
        /// <param name="obj">对象类</param>
        /// <param name="mymethod">方法名</param>
        /// <param name="param">方法参数列表</param>
        public static void WriteErrorLog(Object obj, string mymethod, object[] param)
        {
            try
            {
                Type t = obj.GetType();
                MethodInfo mi = t.GetMethod(mymethod);
                mi.Invoke(obj, param);
            }
            catch (Exception ex)
            {
                StreamWriter sw = new StreamWriter(@"D:\ErrorLog.txt", true);
                sw.WriteLine(DateTime.Now.ToString() + "-----------------------------\n");
                sw.WriteLine(ex.ToString());
                sw.Close();
                MessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// 反射调取属性构造器
        /// </summary>
        /// <param name="obj">类</param>
        /// <param name="mymethod">方法名</param>
        /// <param name="param">方法参数</param>
        public static void SetProperitys(Object obj, string mymethod, object[] param)
        {
            try
            {
                Type t = obj.GetType();

                MethodInfo mi = t.GetMethod(mymethod);
                mi.Invoke(obj, param);
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }
        }

        /// <summary>
        /// 反射调取属性封装字段赋值
        /// </summary>
        /// <param name="obj">类</param>
        /// <param name="p_strProperityName">属性名</param>
        /// <param name="p_strValue">属性值</param>
        public static void SetProperitys(Object obj, string p_strProperityName,  string p_strValue)
        {
            try
            {
                Type t = obj.GetType();
                t.GetProperty(p_strProperityName).SetValue(obj, p_strValue, null);
            }
            catch (Exception ex)
            {
                WriteLog(ex.ToString());
            }
        }

        /// <summary>
        /// 用FileStream写文件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static void FileStreamWriteFile(string str)
        {
            byte[] byData;
            //char[] charData;
            try
            {
                FileStream nFile = new FileStream(Application.StartupPath + @"\ErrorLog.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
                ////获得字符数组
                //charData = str.ToCharArray();
                ////初始化字节数组
                //byData = new byte[charData.Length];
                //将字符数组转换为正确的字节格式
                //Encoder enc = Encoding.UTF8.GetEncoder();
                //enc.GetBytes(charData, 0, charData.Length, byData, 0, true);
                //nFile.Seek(0, SeekOrigin.Begin);
                byData = Encoding.UTF8.GetBytes(str);
                nFile.Write(byData, 0, 1000);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 记录错误信息
        /// </summary>
        /// <param name="mess">错误信息</param>
        public static void WriteErrorLog(string mess)
        {
            StreamWriter sw = null;
            FileStream fs = null;
            try
            {
                string filepath = Application.StartupPath + @"\ErrorLog.txt";
                if (!File.Exists(filepath))
                {
                    File.Create(filepath);
                }
                fs = new FileStream(filepath, FileMode.Append);
                using (sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now.ToString() + "-----------------------------\n");
                    sw.WriteLine(mess);
                    sw.Flush();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }

        }

        /// <summary>
        /// 写日志信息
        /// </summary>
        /// <param name="mess">个性化信息</param>
        public static void WriteLog(string mess)
        {
            WriteLog(new Exception(), mess);
        }
        /// <summary>
        /// 写日志信息
        /// </summary>
        /// <param name="p_expEx">异常信息</param>
        /// <param name="p_strMess">个性化信息</param>
        public static void WriteLog(Exception p_expEx, string p_strMess)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(Application.StartupPath + @"\" + Application.ProductName + "Log.xml");
            }
            catch (FileNotFoundException)
            {
                CreateLogFile(doc);
            }
            AppendLogMessage(doc, p_expEx, p_strMess);
        }
        /// <summary>
        /// 没有日志文件创建日志文件
        /// </summary>
        /// <param name="doc">xml文件</param>
        public static void CreateLogFile(XmlDocument doc)
        {
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "GB2312", null);
            doc.AppendChild(dec);
            XmlNode root = doc.CreateElement("系统错误日志");
            doc.AppendChild(root);
            doc.Save(Application.StartupPath + @"\" + Application.ProductName + "Log.xml");
        }
        /// <summary>
        /// 成功载入Log文件并添加节点日志信息
        /// </summary>
        /// <param name="doc">载入的xml文件</param>
        /// <param name="ex">异常信息</param>
        /// <param name="mess">传入个性化信息</param>
        private static void AppendLogMessage(XmlDocument doc, Exception ex, string mess)
        {
            try
            {
                //载入日志文件
                doc.Load(Application.StartupPath + @"\" + Application.ProductName + "Log.xml");
                //创建节点(一级)
                XmlNode root = doc.SelectSingleNode("系统错误日志");
                //创建节点（二级）
                XmlNode node = doc.CreateElement("Log");
                //创建节点（三级）
                XmlElement element1 = doc.CreateElement("Time");
                element1.InnerText = DateTime.Now.ToString("yyyy-MM-dd");
                node.AppendChild(element1);
                XmlElement element2 = doc.CreateElement("User");
                element2.InnerText = "User";
                node.AppendChild(element2);
                XmlElement element3 = doc.CreateElement("StackTrace");
                element3.InnerText = ex.ToString();
                node.AppendChild(element3);
                XmlElement element4 = doc.CreateElement("Message");
                element4.InnerText = mess;
                node.AppendChild(element4);
                root.AppendChild(node);
                doc.Save(Application.StartupPath + @"\" + Application.ProductName + "Log.xml");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 窗体域usercontrol的绑定
        /// <summary>
        /// 简单的代码重用。因为每次显示窗体的代码都是一样的。
        /// </summary>
        /// <param name="f">容器窗体</param>
        /// <param name="uc">内容容器</param>
        public static DialogResult ShowForm( UserControl uc)
        {
            Form f = new Form();
            uc.Dock = DockStyle.None;
            f.Size = new Size(uc.Width + 2, uc.Height + 2);
            f.BackColor = Color.Blue;
            f.FormBorderStyle = FormBorderStyle.None;
            f.StartPosition = FormStartPosition.CenterParent;
            uc.Padding = new Padding(2,2,0,0);
            uc.BackColor = Color.White;
            uc.Left = 1;
            uc.Top = 1;
            f.Controls.Add(uc);
            return f.ShowDialog();
        }
        /// <summary>
        /// 添加一个用户控件到一个1024*768的窗体中
        /// </summary>
        /// <param name="uc"></param>
        public static void AddForm2(UserControl uc)
        {
            Form f = new Form();
            //f.TopMost = true;
            f.Size = new Size(1024, 730);
            f.StartPosition = FormStartPosition.Manual;
            f.Controls.Add(uc);
            uc.Dock = DockStyle.Fill;
            f.ShowDialog();
        }
        /// <summary>
        /// 项panel容器中添加控件
        /// </summary>
        /// <param name="p">容器panel</param>
        /// <param name="uc">显示的usercontrol</param>
        public static void AddForm3(Panel p, UserControl uc)
        {
            p.Controls.Clear();
            p.Controls.Add(uc);
            uc.Dock = DockStyle.Fill;

        }
        #endregion

        #region 等待窗体
        /// <summary>
        /// 向tabcontrol假如卡片。
        /// </summary>
        /// <param name="tc">指定的tabcontrol</param>
        public static void AddTabControl(TabControl tc, string title, Control c)
        {
            if (dicpage.ContainsKey(title))
            {
                tc.SelectTab(title);
                return;
            }
            TabPage tp = new TabPage(title);
            tp.Name = title;
            dicpage.Add(title, tp);
            tp.Controls.Add(c);
            c.Dock = DockStyle.Fill;
            tc.TabPages.Add(dicpage[title]);
            tc.SelectTab(title);
        }

        /// <summary>
        /// 设置窗体样式，并将控件加入到窗体中
        /// </summary>
        private static void ShowWaiting()
        {
            //Form f = new Form();
            //f.TopMost = true;
            //f.MaximizeBox = false;
            //f.MinimizeBox = false;
            //f.Size = new Size(339, 88);
            //f.FormBorderStyle = FormBorderStyle.None;
            uctlPleaseWaiting pw = new uctlPleaseWaiting();
            ShowForm( pw);
        }
        /// <summary>
        /// 创建一个新的线程，并开启。
        /// </summary>
        public void WaitingThreadStart()
        {
            t = new Thread(new ThreadStart(ShowWaiting));
            t.Start();
        }
        /// <summary>
        /// 终止进程
        /// </summary>
        public void WaitingThreadStop()
        {
            t.Abort();
        }
        #endregion

        #region EXCEL与datatable相互转换
        /// <summary>
        /// 将数据集导出成为excel
        /// </summary>
        /// <param name="name">导出excel的名称</param>
        /// <param name="ds">所导出的数据集</param>
        public static void AddExcel(string name, DataTable dt)
        {
            string fileName = name + ".xls";
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            int rowIndex = 1;
            int colIndex = 0;
            excel.Application.Workbooks.Add(true);
            foreach (DataColumn col in dt.Columns)
            {
                colIndex++;
                excel.Cells[1, colIndex] = col.ColumnName;
            }

            foreach (DataRow row in dt.Rows)
            {
                rowIndex++;
                colIndex = 0;
                for (colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                {
                    excel.Cells[rowIndex, colIndex + 1] = row[colIndex].ToString();
                }
            }

            excel.Visible = false;
            excel.ActiveWorkbook.SaveAs(fileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7, null, null, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);
            excel.Quit();
            excel = null;
            GC.Collect();//垃圾回收 
        }

        public static void SaveAsExcel(string name, DataTable dt)
        {
            OleDbConnection cnnxls = new OleDbConnection(string.Format(excelstring, name));
            string insertsql = "";
            string insertcolumnstring = "";
            string insertvaluestring = "";
            string fileName = name + ".xls";
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            excel.Application.Workbooks.Add(true);
            int colIndex = 0;
            foreach (DataColumn col in dt.Columns)
            {
                colIndex++;
                excel.Cells[1, colIndex] = col.ColumnName;
                insertcolumnstring += string.Format("{0},", col.ColumnName);
            }
            excel.ActiveWorkbook.SaveAs(fileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7, null, null, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);

            //新增记录  
            //conn.execute(sql);

            insertcolumnstring = insertcolumnstring.Trim(',');
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    row[dc].ToString();
                    insertvaluestring += string.Format("'{0}',", row[dc].ToString().Replace("'", "''"));
                }
                string sql = string.Format("insert into [Sheet1$]({0}) values({1})", insertcolumnstring, insertvaluestring);
                OleDbDataAdapter myDa = new OleDbDataAdapter(sql, cnnxls);
                myDa.InsertCommand.ExecuteNonQuery();
            }
            excel.Visible = false;
            //excel.ActiveWorkbook.SaveAs(fileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel7, null, null, false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);
            excel.Quit();
            excel = null;
            GC.Collect();//垃圾回收 
        }

        #endregion

        #region XML与dataset相互转换
        /// <summary>
        /// 将xml对象内容字符串转换为DataSet
        /// </summary>
        /// <param name="xmlData">字符串</param>
        /// <returns>返回dataset对象</returns>
        public static DataSet ConvertXMLToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                //从stream装载到XmlTextReader
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }


        /// <summary>
        ///  将xml文件转换为DataSet
        /// </summary>
        /// <param name="xmlFile">xml文件地址</param>
        /// <returns>dataset对象</returns>
        public static DataSet ConvertXMLFileToDataSet(string filepath)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                XmlDocument xmld = new XmlDocument();
                xmld.Load(filepath);

                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmld.InnerXml);
                //从stream装载到XmlTextReader
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                //xmlDS.ReadXml(xmlFile);
                return xmlDS;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }


        /// <summary>
        ///  将DataSet转换为xml对象字符串
        /// </summary>
        /// <param name="xmlDS">dataset对象</param>
        /// <returns>xml字符串</returns>
        public static string ConvertDataSetToXML(DataSet xmlDS)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;

            try
            {
                stream = new MemoryStream();
                //从stream装载到XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);

                //用WriteXml方法写入文件.
                xmlDS.WriteXml(writer);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);

                UnicodeEncoding utf = new UnicodeEncoding();
                return utf.GetString(arr).Trim();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        public static void SetXMLValue(string xmlFile)
        {
            //XmlDocument xmld = new XmlDocument();
            //xmld.Load(xmlFile);
            //XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmld.NameTable);
            //XmlNodeList xmln = xmld.SelectNodes(xmlFile, nsmgr);
            //XmlNode newnode = new XmlNode("www");
            //xmln.AppendChild(newnode);
        }

        /// <summary>
        /// 将数据集转换为ptName指定的格式存储于xmlFile路径中
        /// </summary>
        /// <param name="xmlDS">需要转换的数据</param>
        /// <param name="xmlFile">输出路径</param>
        /// <param name="sqlgetlayout">获取各式</param>
        /// <param name="doc">xml文件</param>
        public static System.Collections.Generic.Dictionary<string, XmlElement> ConverDataToXMLFile(XmlDocument doc, DataSet xmlDS, DataTable dtgetlayout)
        {
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "GB2312", null);
            doc.AppendChild(dec);
            Dictionary<string, XmlElement> xmldic = new Dictionary<string, XmlElement>();
            try
            {
                foreach (DataRow drlayout in dtgetlayout.Rows)
                {
                    foreach (DataRow drxml in xmlDS.Tables[0].Rows)
                    {
                        foreach (DataColumn dcxml in xmlDS.Tables[0].Columns)
                        {
                            if (drlayout["field_name"].ToString().Equals(dcxml.ToString()))
                            {
                                XmlElement xe1 = doc.CreateElement((drlayout["field_name"].ToString()));
                                xe1.InnerText = drxml[dcxml].ToString();
                                xmldic.Add(dcxml.ToString(), xe1);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return xmldic;
        }

        public static void ConverDataSetToXMLFile(string strxml, string xmlFile)
        {
            try
            {
                StreamWriter sw = new StreamWriter(xmlFile);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sw.WriteLine(strxml.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 将xml数据集转转化为xml文件
        /// </summary>
        /// <param name="dsxml">数据集</param>
        /// <param name="xmlFile">路径</param>
        /// <returns>返回字符串</returns>
        public static string ConverDataSetToXMLFile(DataSet dsxml, string xmlFile)
        {
            try
            {
                StringBuilder strxml = new StringBuilder(ConvertDataSetToXML(dsxml));
                StreamWriter sw = new StreamWriter(xmlFile);
                strxml.Replace("NewDataSet", "第一层");
                strxml.Replace("ds", "第二层");
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sw.WriteLine(strxml.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return "";
        }

        /// <summary>
        /// 将DataSet转换为xml文件
        /// </summary>
        /// <param name="xmlDS">dtaset对象</param>
        /// <param name="xmlFile">文件输出路径</param>
        public static void ConvertDataSetToXMLFile(DataSet xmlDS, string xmlFile)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;

            try
            {
                stream = new MemoryStream();
                //从stream装载到XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);

                //用WriteXml方法写入文件.
                xmlDS.WriteXml(writer);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);

                //返回Unicode编码的文本
                UnicodeEncoding utf = new UnicodeEncoding();
                StreamWriter sw = new StreamWriter(xmlFile);
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sw.WriteLine(utf.GetString(arr).Trim());
                sw.Close();
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }
        #endregion

        #region 操作数据库

        /// <summary>
        /// 将MongoCursor转化为DataTable《未实现》
        /// </summary>
        /// <param name="mc">输入MongoCursor</param>
        /// <returns>生成的DataTable</returns>
        public static DataTable ConverMongoCursorToDataTable(MongoCursor mc)
        {
            DataTable dt = new DataTable();
            //列的初始化
            foreach (string column in mc)
            {
                
            }
            //生成行
            foreach (BsonDocument item in mc)
            {
                
            }
            return dt;
        
        }

        /// <summary>
        /// 查询MongoDB
        /// </summary>
        /// <param name="collenction">集合名称</param>
        /// <param name="qd">QueryCocument条件</param>
        /// <returns>返回结果</returns>
        public static MongoCursor QueryMongoCollection(string collenction,QueryDocument  qd)
        {
            MongoCursor<BsonDocument> result = null;
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test"); // WriteConcern defaulted to Acknowledged
            MongoCollection col = database.GetCollection(collenction);
            try
            {
                //查找全部
                //result = col.FindAllAs<BsonDocument>();
                //条件查找
                result = col.FindOneAs<MongoCursor<BsonDocument>>(qd);
            }
            catch (Exception ex)
            {
                CommonFunction.WriteErrorLog(ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collenction"></param>
        /// <param name="bd"></param>
        public static void UpdateMongoCollection(string collenction,BsonDocument bd)
        {
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test"); // WriteConcern defaulted to Acknowledged
            MongoCollection col = database.GetCollection(collenction);
            //update
            //col.Update(collenction, (x => x.ID == collenction.ID));
           
        }

        /// <summary>
        /// MongoDb插入数据
        /// </summary>
        /// <param name="collenction">集合名称</param>
        /// <param name="bd">所插入的数据</param>
        /// <returns>插入数据返回的结果</returns>
        public static WriteConcernResult InsertMongoCollection(string collenction, BsonDocument bd)
        {
            var connectionString = "mongodb://localhost";
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase("test"); // WriteConcern defaulted to Acknowledged
            MongoCollection col = database.GetCollection(collenction);
            //insert
            WriteConcernResult wcr = null;
            try
            {
                wcr = col.Insert(bd);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return wcr;
        }



        /// <summary>
        /// OleDb驱动 执行查询操作 x86平台
        /// </summary>
        /// <param name="p_strSql">查询sql语句</param>
        /// <param name="p_dicDictionary">字典参数</param>
        /// <param name="p_strTablename">产生的datatable名称</param>
        /// <param name="cmd">cmd</param>
        /// <returns>返回表</returns>
        static public DataTable OleExecuteBySQL(string p_strSql, SortedDictionary<string, string> p_dicDictionary, string p_strTablename)
        {
            if ("" == m_strConnectionString)
            {
                MessageBox.Show("未设置数据库连接字符串！");
            }
            DataTable _dtTable = new DataTable(p_strTablename);
            m_oleConn = new OleDbConnection(m_strConnectionString);
            m_oleCmd = m_oleConn.CreateCommand();
            m_oleConn.Open();
            OleChangeSelectCommand(p_strSql, p_dicDictionary, ref m_oleCmd);
            try
            {
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(m_oleCmd))
                {
                    adapter.Fill(_dtTable);
                }
            }
            catch (Exception exp)
            {
                WriteLog(exp, p_strSql);
            }
            finally
            {
                m_oleConn.Dispose();
                m_oleCmd.Dispose();
            }
            return _dtTable;
        }


        /// <summary>
        /// OdbcDb驱动 执行查询操作 x86平台
        /// </summary>
        /// <param name="p_strSql">查询sql语句</param>
        /// <param name="p_dicDictionary">字典参数</param>
        /// <param name="p_strTablename">产生的datatable名称</param>
        /// <param name="cmd">cmd</param>
        /// <returns>返回表</returns>
        static public DataTable OdbcExecuteBySQL(string p_strSql, Dictionary<string, string> p_dicDictionary, string p_strTablename)
        {
            if ("" == m_strConnectionString)
            {
                MessageBox.Show("未设置数据库连接字符串！");
            }
            DataTable _dtTable = new DataTable(p_strTablename);
            m_odbcConn = new OdbcConnection(m_strConnectionString);
            m_odbcCmd = m_odbcConn.CreateCommand();
            m_odbcConn.Open();
            OdbcChangeSelectCommand(p_strSql, p_dicDictionary, ref m_odbcCmd);
            try
            {
                using (OdbcDataAdapter adapter = new OdbcDataAdapter(m_odbcCmd))
                {
                    adapter.Fill(_dtTable);
                }
            }
            catch (Exception exp)
            {
                WriteLog(exp, p_strSql);
            }
            finally
            {
                m_odbcConn.Dispose();
                m_odbcCmd.Dispose();
            }
            return _dtTable;
        }

        /// <summary>
        /// odbc 参数化sql
        /// </summary>
        /// <param name="p_strSql">sql语句</param>
        /// <param name="p_dicDictionary">参数字典</param>
        /// <param name="m_odbcCmd">cmd</param>
        private static void OdbcChangeSelectCommand(string p_strSql, Dictionary<string, string> p_dicDictionary, ref OdbcCommand m_odbcCmd)
        {
            m_odbcCmd.Parameters.Clear();
            string sqltxt = p_strSql;
            int nIndex = sqltxt.IndexOf('@');
            while (-1 != nIndex)
            {
                if (nIndex > -1)
                {
                    foreach (object obj in p_dicDictionary.Keys)
                    {
                        string strParm = "@" + obj.ToString();
                        int n = sqltxt.IndexOf(strParm);
                        if (nIndex == sqltxt.IndexOf(strParm, nIndex))
                        {
                            string values;
                            p_dicDictionary.TryGetValue(obj.ToString(), out values);
                            m_odbcCmd.Parameters.Add(new OleDbParameter(strParm, values));
                        }
                    }
                }
                if (sqltxt.Length > nIndex)
                {
                    nIndex = sqltxt.IndexOf('@', nIndex + 1);
                }
                else
                    nIndex = -1;
            }
            m_odbcCmd.CommandText = sqltxt;
        }

        /// <summary>
        /// sqlclient驱动 执行查询操作
        /// </summary>
        /// <param name="p_strSql">查询sql语句</param>
        /// <param name="p_dicDictionary">字典参数</param>
        /// <param name="p_strTablename">产生的datatable名称</param>
        /// <param name="cmd">cmd</param>
        /// <returns>返回表</returns>
        static public DataTable SqlExecuteBySQL(string p_strSql, Dictionary<string, string> p_dicDictionary, string p_strTablename)
        {
            if ("" == m_strConnectionString)
            {
                MessageBox.Show("未设置数据库连接字符串！");
            }
            DataTable _dtTable = new DataTable(p_strTablename);
            m_sqlConn = new SqlConnection(m_strConnectionString);
            m_sqlCmd = m_sqlConn.CreateCommand();
            m_sqlConn.Open();
            ChangeSelectCommand(p_strSql, p_dicDictionary, ref m_sqlCmd);
            try
            {
                using (DbDataAdapter adapter = new SqlDataAdapter(m_sqlCmd))
                {
                    adapter.Fill(_dtTable);
                }
            }
            catch (Exception exp)
            {
                WriteLog(exp, p_strSql);
            }
            finally
            {
                m_sqlConn.Dispose();
                m_sqlCmd.Dispose();
            }
            return _dtTable;
        }

        /// <summary>
        /// 执行查询操作
        /// </summary>
        /// <param name="p_strSql">查询sql语句</param>
        /// <param name="p_dicDictionary">字典参数</param>
        /// <param name="p_strTablename">产生的datatable名称</param>
        /// <param name="cmd">cmd</param>
        /// <returns>返回表</returns>
        static public DataTable OraExecuteBySQL(string p_strSql, Dictionary<string, string> p_dicDictionary, string p_strTablename)
        {
            if ("" == m_strConnectionString)
            {
                MessageBox.Show("未设置数据库连接字符串！");
            }
            DataTable _dtTable = new DataTable(p_strTablename);
            m_oraConn = new OracleConnection(m_strConnectionString);
            m_oraCmd = m_oraConn.CreateCommand();
            m_oraConn.Open();
            ChangeSelectCommand(p_strSql, p_dicDictionary, ref m_oraCmd);
            try
            {
                using (OracleDataAdapter adapter = new OracleDataAdapter(m_oraCmd))
                {
                    adapter.Fill(_dtTable);
                }
            }
            catch (Exception exp)
            {
                WriteLog(exp, p_strSql);
            }
            finally
            {
                m_oraConn.Dispose();
                m_oraCmd.Dispose();
            }
            return _dtTable;
        }

        /// <summary>
        /// 开启事务
        /// </summary>
        public static void BeginTransaction()
        {
            try
            {
                m_oleConn = new OleDbConnection(m_strConnectionString);
                m_oleConn.Open();
                m_oleCmd = m_oleConn.CreateCommand();
                m_oleTrans = m_oleConn.BeginTransaction();
                m_oleCmd.Transaction = m_oleTrans;
            }
            catch (Exception ex)
            {
                CommonFunction.WriteErrorLog(ex.ToString());
            }
           
        }

        /// <summary>
        /// 停止事务
        /// </summary>
        public static void EndTransaction()
        {
            try
            {
                if (m_oleConn!=null)
                {
                    m_oleTrans.Commit();
                }
            }
            catch (Exception ex)
            {
                CommonFunction.WriteErrorLog(ex.ToString());
            }
            
        }

        /// <summary>
        /// 在方法外部声明事务，作为参数传入。当任务完成手动提交事务(测试中)
        /// </summary>
        /// <param name="p_strSql">执行的sql</param>
        /// <param name="dictionary">参数字典</param>
        /// <returns>影响的结果条数</returns>
        static public int OleExecuteTrans(string p_strSql, SortedDictionary<string, string> p_dictParam)
        {
            if (m_oleConn == null)
            {
                return 0;
            }
            if (m_oleCmd == null)
            {
                return 0;
            }
            int n = 0;
            OleChangeSelectCommand(p_strSql, p_dictParam, ref m_oleCmd);
            try
            {
                n = m_oleCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                if (m_oleConn != null && m_oleTrans != null)
                {
                    m_oleTrans.Rollback();
                }
                WriteLog(exp, p_strSql);
                n = 0;
            }
            finally
            {
                if (m_oleConn != null)
                {
                    m_oleConn.Dispose();
                    m_oleConn = null;
                }
                if (m_oleCmd != null)
                {
                    m_oleCmd.Dispose();
                    m_oleCmd = null;
                }
                if (m_oleTrans != null)
                {
                    m_oleTrans.Dispose();
                }
            }
            return n;
        }

        /// <summary>
        /// 在方法外部声明事务，作为参数传入。当任务完成手动提交事务(测试中)
        /// </summary>
        /// <param name="p_strSql">执行的sql</param>
        /// <param name="dictionary">参数字典</param>
        /// <returns>影响的结果条数</returns>
        static public int OraExecuteTrans(string p_strSql, Dictionary<string, string> p_dictParam)
        {
            if (m_oraConn == null)
            {
                return 0;
            }
            if (m_oraCmd == null)
            {
                return 0;
            }
            int n = 0;
            OraChangeSelectCommand(p_strSql, p_dictParam, ref m_oraCmd);
            try
            {
                n = m_oleCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                if (m_oleConn != null && m_oleTrans != null)
                {
                    m_oleTrans.Rollback();
                }
                WriteLog(exp, p_strSql);
                n = 0;
            }
            finally
            {
                if (m_oraConn != null)
                {
                    m_oraConn.Dispose();
                }
                if (m_oraCmd != null)
                {
                    m_oraCmd.Dispose();
                }
                if (m_oraTrans != null)
                {
                    m_oraTrans.Dispose();
                }
            }
            return n;
        }

        /// <summary>
        /// 采用OleDb方式驱动 执行增，删，改操作 x86平台
        /// </summary>
        /// <param name="p_strSql">操作的sql</param>
        /// <param name="p_dictParam">字典参数</param>
        /// <param name="cmd">cmd</param>
        /// <returns>返回结果</returns>
        static public int OleExecuteNonQuery(string p_strSql, SortedDictionary<string, string> p_dictParam)
        {
            int _iExeCount = 0;
            m_oleConn = new OleDbConnection(m_strConnectionString);
            m_oleCmd = m_oleConn.CreateCommand();
            m_oleConn.Open();
            OleChangeSelectCommand(p_strSql, p_dictParam, ref m_oleCmd);
            try
            {
                _iExeCount = m_oleCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                WriteLog(exp, p_strSql);
                _iExeCount = -1;
            }
            finally
            {
                m_oleConn.Dispose();
                m_oleCmd.Dispose();
            }
            return _iExeCount;
        }

        /// <summary>
        /// 采用Odbc方式驱动 执行增，删，改操作 x86平台
        /// </summary>
        /// <param name="p_strSql">操作的sql</param>
        /// <param name="p_dictParam">字典参数</param>
        /// <param name="cmd">cmd</param>
        /// <returns>返回结果</returns>
        static public int OdbcExecuteNonQuery(string p_strSql, Dictionary<string, string> p_dictParam)
        {
            int _iExeCount = 0;
            m_odbcConn = new OdbcConnection(m_strConnectionString);
            m_odbcCmd = m_odbcConn.CreateCommand();
            m_odbcConn.Open();
            OdbcChangeSelectCommand(p_strSql, p_dictParam, ref m_odbcCmd);
            try
            {
                _iExeCount = m_odbcCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                WriteLog(exp, p_strSql);
                _iExeCount = -1;
            }
            finally
            {
                m_odbcConn.Dispose();
                m_odbcCmd.Dispose();
            }
            return _iExeCount;
        }


        /// <summary>
        /// 采用.net1.1 里的oracleclient驱动 执行增，删，改操作
        /// </summary>
        /// <param name="p_strSql">操作的sql</param>
        /// <param name="p_dictParam">字典参数</param>
        /// <param name="cmd">cmd</param>
        /// <returns>返回结果</returns>
        static public int OraExecuteNonQuery(string p_strSql, Dictionary<string, string> p_dictParam)
        {
            int _iExeCount = 0;
            m_oraConn = new OracleConnection(m_strConnectionString);
            m_oraCmd = m_oraConn.CreateCommand();
            m_oraConn.Open();
            OraChangeSelectCommand(p_strSql, p_dictParam, ref m_oraCmd);
            try
            {
                _iExeCount = m_oraCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                WriteLog(exp, p_strSql);
                _iExeCount = -1;
            }
            finally
            {
                m_oraConn.Dispose();
                m_oraCmd.Dispose();
            }
            return _iExeCount;
        }

        /// <summary>
        /// 里的sqlclient驱动 执行增，删，改操作
        /// </summary>
        /// <param name="p_strSql">操作的sql</param>
        /// <param name="p_dictParam">字典参数</param>
        /// <param name="cmd">cmd</param>
        /// <returns>返回结果</returns>
        static public int SqlExecuteNonQuery(string p_strSql, Dictionary<string, string> p_dictParam)
        {
            int _iExeCount = 0;
            m_sqlConn = new SqlConnection(m_strConnectionString);
            m_sqlCmd = m_sqlConn.CreateCommand();
            m_sqlConn.Open();
            ChangeSelectCommand(p_strSql, p_dictParam, ref m_sqlCmd);
            try
            {
                _iExeCount = m_sqlCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                WriteLog(exp, p_strSql);
                _iExeCount = -1;
            }
            finally
            {
                m_sqlConn.Dispose();
                m_sqlCmd.Dispose();
            }
            return _iExeCount;
        }

        /// <summary>
        /// 通过APP.config文件获取连接数据库字符串
        /// </summary>
        /// <returns></returns>
        public static void SetConnectionString()
        {
            //string _strDBType = ConfigurationManager.AppSettings["DBType"];
            //if ("Oracle" == _strDBType)
            //{
            //    m_strConnectionString = ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString;
            //}
            //else if ("SQLServer" == _strDBType)
            //{
            //    m_strConnectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
            //}
            //else if ("MySQL" == _strDBType)
            //{
            //    m_strConnectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
            //}
            //else
            //{
            //    MessageBox.Show("未设置[DBType]或数据库类型不是[Oracle][SQLServer][MySQL]!");
            //}
            m_strConnectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        /// <summary>
        /// 替换sql语句参数，并给cmd赋值,绑定变量
        /// </summary>
        /// <param name="p_strSql">sql语句</param>
        /// <param name="p_dictParam">参数字典</param>
        /// <param name="p_oleCmd">cmd</param>
        /// <returns>返回是否替换参数成功</returns>
        public static void OraChangeSelectCommand(string p_strSql, Dictionary<string, string> p_dictParam, ref OracleCommand p_oraCmd)
        {

            p_oraCmd.Parameters.Clear();
            string sqltxt = p_strSql;
            int nIndex = sqltxt.IndexOf(':');
            while (-1 != nIndex)
            {
                if (nIndex > -1)
                {
                    foreach (object obj in p_dictParam.Keys)
                    {
                        string strParm = ":" + obj.ToString();
                        int n = sqltxt.IndexOf(strParm);
                        if (nIndex == sqltxt.IndexOf(strParm, nIndex))
                        {
                            string values;
                            p_dictParam.TryGetValue(obj.ToString(), out values);
                            //p_oleCmd.Parameters.Add(new OleDbParameter(strParm, values));
                            p_oraCmd.Parameters.Add(strParm, OleDbType.VarChar).Value = values;

                        }
                    }
                }
                if (sqltxt.Length > nIndex)
                {
                    nIndex = sqltxt.IndexOf(':', nIndex + 1);
                }
                else
                    nIndex = -1;
            }
            p_oraCmd.CommandText = sqltxt;
        }

        ///// <summary>
        ///// 替换sql语句参数，并给cmd赋值,绑定变量
        ///// </summary>
        ///// <param name="p_strSql">sql语句</param>
        ///// <param name="p_dictParam">参数字典</param>
        ///// <param name="p_oleCmd">cmd</param>
        ///// <returns>返回是否替换参数成功</returns>
        public static void OleChangeSelectCommand(string p_strSql, SortedDictionary<string, string> p_dictParam, ref OleDbCommand p_oleCmd)
        {
            p_oleCmd.Parameters.Clear();
            foreach (object obj in p_dictParam.Keys)
            {
                string strParm =   obj.ToString();
                string values;
                p_dictParam.TryGetValue(obj.ToString(), out values);
                p_oleCmd.Parameters.Add(strParm, OleDbType.VarChar).Value = values;
            }
            p_oleCmd.CommandText = p_strSql;
        }


        /// <summary>
        /// 替换sql语句参数，并给cmd赋值,绑定变量
        /// </summary>
        /// <param name="p_strSql">sql语句</param>
        /// <param name="p_dictParam">参数字典</param>
        /// <param name="p_dbCmd">cmd</param>
        /// <returns>返回是否替换参数成功</returns>
        static public bool ChangeSelectCommand(string p_strSql, Dictionary<string, string> p_dictParam, ref SqlCommand p_sqlCmd)
        {

            p_sqlCmd.Parameters.Clear();
            string sqltxt = p_strSql;
            int nIndex = sqltxt.IndexOf(':');
            while (-1 != nIndex)
            {
                if (nIndex > -1)
                {
                    foreach (object obj in p_dictParam.Keys)
                    {
                        string strParm = ":" + obj.ToString();
                        int n = sqltxt.IndexOf(strParm);
                        if (nIndex == sqltxt.IndexOf(strParm, nIndex))
                        {
                            string values;
                            p_dictParam.TryGetValue(obj.ToString(), out values);
                            p_sqlCmd.Parameters.Add(new SqlParameter(strParm, values));

                        }
                    }
                }
                if (sqltxt.Length > nIndex)
                {
                    nIndex = sqltxt.IndexOf(':', nIndex + 1);
                }
                else
                    nIndex = -1;
            }
            p_sqlCmd.CommandText = sqltxt;
            return true;
        }

        /// <summary>
        /// 替换sql语句参数，并给cmd赋值,绑定变量
        /// </summary>
        /// <param name="p_strSql">sql语句</param>
        /// <param name="p_dictParam">参数字典</param>
        /// <param name="p_oraCmd">cmd</param>
        /// <returns>返回是否替换参数成功</returns>
        static public bool ChangeSelectCommand(string p_strSql, Dictionary<string, string> p_dictParam, ref OracleCommand p_oraCmd)
        {

            p_oraCmd.Parameters.Clear();
            string sqltxt = p_strSql;
            int nIndex = sqltxt.IndexOf(':');
            while (-1 != nIndex)
            {
                if (nIndex > -1)
                {
                    foreach (object obj in p_dictParam.Keys)
                    {
                        string strParm = ":" + obj.ToString();
                        int n = sqltxt.IndexOf(strParm);
                        if (nIndex == sqltxt.IndexOf(strParm, nIndex))
                        {
                            string values;
                            p_dictParam.TryGetValue(obj.ToString(), out values);
                            p_oraCmd.Parameters.Add(new OracleParameter(strParm, values));

                        }
                    }
                }
                if (sqltxt.Length > nIndex)
                {
                    nIndex = sqltxt.IndexOf(':', nIndex + 1);
                }
                else
                    nIndex = -1;
            }
            p_oraCmd.CommandText = sqltxt;
            return true;
        }
        #endregion

        #region 检查是否为空
        public static DataTable CheckNULL(DataTable source)
        {
            if (source == null)
            {
                MessageBox.Show("数据源为空");
                return null;
            }
            else
            {
                return source;
            }
        }
        #endregion

        #region 动态生成菜单
        /// <summary>
        /// 动态创建菜单
        /// </summary>
        private void CreateMenu(MenuStrip MainMenuStrip, Form parient_form)
        {
            //定义一个主菜单
            MenuStrip mainMenu = new MenuStrip();
            DataSet ds = new DataSet();
            //从XML中读取数据。数据结构后面详细讲一下。
            ds.ReadXml(@"..\..\Menu.xml");
            DataView dv = ds.Tables[0].DefaultView;
            //通过DataView来过滤数据首先得到最顶层的菜单
            dv.RowFilter = "ParentItemID=0";
            for (int i = 0; i < dv.Count; i++)
            {
                //创建一个菜单项
                ToolStripMenuItem topMenu = new ToolStripMenuItem();
                //给菜单赋Text值。也就是在界面上看到的值。
                topMenu.Text = dv[i]["Text"].ToString();
                //如果是有下级菜单则通过CreateSubMenu方法来创建下级菜单
                if (Convert.ToInt16(dv[i]["IsModule"]) == 1)
                {
                    //以ref的方式将顶层菜单传递参数，因为他可以在赋值后再回传。－－也许还有更好的方法^_^.
                    CreateSubMenu(ref topMenu, Convert.ToInt32(dv[i]["ItemID"]), ds.Tables[0]);
                }
                //显示应用程序中已打开的 MDI 子窗体列表的菜单项
                mainMenu.MdiWindowListItem = topMenu;
                //将递归附加好的菜单加到菜单根项上。
                mainMenu.Items.Add(topMenu);
            }
            mainMenu.Dock = DockStyle.Top;
            //将窗体的MainMenuStrip梆定为mainMenu.
            MainMenuStrip = mainMenu;
            //这句很重要。如果不写这句菜单将不会出现在主窗体中。
            parient_form.Controls.Add(mainMenu);
        }

        /// <summary>
        /// 创建子菜单
        /// </summary>
        /// <param name="topMenu">父菜单项</param>
        /// <param name="ItemID">父菜单的ID</param>
        /// <param name="dt">所有菜单数据集</param>
        private void CreateSubMenu(ref ToolStripMenuItem topMenu, int ItemID, DataTable dt)
        {
            DataView dv = new DataView(dt);
            //过滤出当前父菜单下在所有子菜单数据(仅为下一层的)
            dv.RowFilter = "ParentItemID=" + ItemID.ToString();

            for (int i = 0; i < dv.Count; i++)
            {
                //创建子菜单项
                ToolStripMenuItem subMenu = new ToolStripMenuItem();
                subMenu.Text = dv[i]["Text"].ToString();
                //如果还有子菜单则继续递归加载。
                if (Convert.ToInt16(dv[i]["IsModule"]) == 1)
                {
                    //递归调用
                    CreateSubMenu(ref subMenu, Convert.ToInt32(dv[i]["ItemID"]), dt);
                }
                else
                {
                    //扩展属性可以加任何想要的值。这里用formName属性来加载窗体。
                    subMenu.Tag = dv[i]["FormName"].ToString();
                    //给没有子菜单的菜单项加事件。
                    subMenu.Click += new EventHandler(subMenu_Click);
                }
                if (dv[i]["ImageName"].ToString().Length > 0)
                {
                    //设置菜单项前面的图票为16X16的图片文件。
                    Image img = Image.FromFile(@"..\..\Image\" + dv[i]["ImageName"].ToString());
                    subMenu.Image = img;
                    subMenu.Image.Tag = dv[i]["ImageName"].ToString();
                }
                //将菜单加到顶层菜单下。
                topMenu.DropDownItems.Add(subMenu);
            }
        }

        /// <summary>
        /// 菜单单击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void subMenu_Click(object sender, EventArgs e)
        {
            //tag属性在这里有用到。
            //string formName = ((ToolStripMenuItem)sender).Tag.ToString();
            //CreateFormInstance(formName);
        }

        /// <summary>
        /// 创建form实例。
        /// </summary>
        /// <param name="formName">form的类名</param>
        private void CreateFormInstance(Form form, string formName)
        {
            bool flag = false;
            //遍历主窗口上的所有子菜单
            for (int i = 0; i < form.MdiChildren.Length; i++)
            {
                //如果所点的窗口被打开则重新激活
                if (form.MdiChildren[i].Tag.ToString().ToLower() == formName.ToLower())
                {
                    form.MdiChildren[i].Activate();
                    form.MdiChildren[i].Show();
                    form.MdiChildren[i].WindowState = FormWindowState.Normal;
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                //如果不存在则用反射创建form窗体实例。
                Assembly asm = Assembly.Load("Fastyou.BookShop.Win");//程序集名
                object frmObj = asm.CreateInstance("Fastyou.BookShop.Win." + formName);//程序集+form的类名。
                Form frms = (Form)frmObj;
                //tag属性要重新写一次，否则在第二次的时候取不到。原因还不清楚。有知道的望告知。
                frms.Tag = formName.ToString();
                frms.MdiParent = form;
                frms.Show();
            }
        }
        #endregion

        #region 文本框显示
        /// <summary>
        /// 通过选取人本，返回相应值
        /// </summary>
        /// <param name="item">显示文本</param>
        /// <returns>对应值</returns>
        public static string returnSelectItemValue(string source, string item)
        {
            string result = "";
            XmlDocument doc = new XmlDocument();
            doc.Load(@"..\..\SELECTITEM.xml");
            XmlElement root = null;
            root = doc.DocumentElement;
            XmlNodeList xmlnodelist = null;
            xmlnodelist = root.SelectNodes("/dataset/" + source + "[itemtext = '" + item + "']/itemvalue");
            //xmlnodelist = root.SelectNodes("//itemtext[@name='" + item + "']/itemvalue");
            if (xmlnodelist.Count == 0)
            {
                return "";
            }
            result = xmlnodelist[0].InnerText;
            return result;
        }

        public static DataTable getComboxDatasource(string item, ComboBox cmb)
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("itemtext");
            dt.Columns.Add(dc);
            XmlDocument doc = new XmlDocument();
            doc.Load(@"..\..\SELECTITEM.xml");
            XmlElement root = null;
            root = doc.DocumentElement;
            XmlNodeList mxlnode = null;
            mxlnode = root.SelectNodes("/dataset/" + item + "/itemtext");
            foreach (var value in mxlnode)
            {
                DataRow dr = dt.NewRow();
                dr["itemtext"] = value.ToString();
                cmb.Items.Add(value.ToString());
            }
            return dt;
        }

        public static DataTable getComboxDatasource(string item)
        {
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn("itemtext");
            dt.Columns.Add(dc);
            XmlDocument doc = new XmlDocument();
            doc.Load(@"..\..\SELECTITEM.xml");
            XmlElement root = null;
            root = doc.DocumentElement;
            XmlNodeList mxlnode = null;
            mxlnode = root.SelectNodes("/dataset/" + item + "/itemtext");
            foreach (XmlNode value in mxlnode)
            {
                DataRow dr = dt.NewRow();
                dr["itemtext"] = value.InnerText.ToString();
                dt.Rows.Add(dr);
                //uc.Items.Add(value.ToString());
            }
            return dt;
        }
        #endregion

    }
}
