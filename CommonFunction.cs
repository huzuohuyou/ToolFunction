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

namespace ToolFunction
{
    public class CommonFunction
    {
        #region ����
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
        public static SqlConnection m_sqlConn = null;
        public static SqlCommand m_sqlCmd = null;
        public static SqlTransaction m_sqlTrans = null;
        #endregion

        #region ���캯��
        public CommonFunction()
        {

        }
        #endregion

        #region ��������������string����
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

        #region ������־
        /// <summary>
        /// ͳһ��������־������Ժ�Ͳ���ÿ��������дtry...catch�ˣ�Ŀǰ����Ҫ��ȡ����ֵ�ķ���֧�ֻ�������
        /// </summary>
        /// <param name="obj">������</param>
        /// <param name="mymethod">������</param>
        /// <param name="param">���������б�</param>
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
        /// ��FileStreamд�ļ�
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
                ////����ַ�����
                //charData = str.ToCharArray();
                ////��ʼ���ֽ�����
                //byData = new byte[charData.Length];
                //���ַ�����ת��Ϊ��ȷ���ֽڸ�ʽ
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
        /// ��¼������Ϣ
        /// </summary>
        /// <param name="mess">������Ϣ</param>
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
        /// д��־��Ϣ
        /// </summary>
        /// <param name="mess">���Ի���Ϣ</param>
        public static void WriteLog(string mess)
        {
            WriteLog(new Exception(), mess);
        }
        /// <summary>
        /// д��־��Ϣ
        /// </summary>
        /// <param name="p_expEx">�쳣��Ϣ</param>
        /// <param name="p_strMess">���Ի���Ϣ</param>
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
        /// û����־�ļ�������־�ļ�
        /// </summary>
        /// <param name="doc">xml�ļ�</param>
        public static void CreateLogFile(XmlDocument doc)
        {
            XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "GB2312", null);
            doc.AppendChild(dec);
            XmlNode root = doc.CreateElement("ϵͳ������־");
            doc.AppendChild(root);
            doc.Save(Application.StartupPath + @"\" + Application.ProductName + "Log.xml");
        }
        /// <summary>
        /// �ɹ�����Log�ļ������ӽڵ���־��Ϣ
        /// </summary>
        /// <param name="doc">�����xml�ļ�</param>
        /// <param name="ex">�쳣��Ϣ</param>
        /// <param name="mess">������Ի���Ϣ</param>
        private static void AppendLogMessage(XmlDocument doc, Exception ex, string mess)
        {
            try
            {
                //������־�ļ�
                doc.Load(Application.StartupPath + @"\" + Application.ProductName + "Log.xml");
                //�����ڵ�(һ��)
                XmlNode root = doc.SelectSingleNode("ϵͳ������־");
                //�����ڵ㣨������
                XmlNode node = doc.CreateElement("Log");
                //�����ڵ㣨������
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

        #region ������usercontrol�İ�
        /// <summary>
        /// �򵥵Ĵ������á���Ϊÿ����ʾ����Ĵ��붼��һ���ġ�
        /// </summary>
        /// <param name="f">��������</param>
        /// <param name="uc">��������</param>
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
        /// ����һ���û��ؼ���һ��1024*768�Ĵ�����
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
        /// ��panel���������ӿؼ�
        /// </summary>
        /// <param name="p">����panel</param>
        /// <param name="uc">��ʾ��usercontrol</param>
        public static void AddForm3(Panel p, UserControl uc)
        {
            p.Controls.Clear();
            p.Controls.Add(uc);
            uc.Dock = DockStyle.Fill;

        }
        #endregion

        #region �ȴ�����
        /// <summary>
        /// ��tabcontrol���翨Ƭ��
        /// </summary>
        /// <param name="tc">ָ����tabcontrol</param>
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
        /// ���ô�����ʽ�������ؼ����뵽������
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
        /// ����һ���µ��̣߳���������
        /// </summary>
        public void WaitingThreadStart()
        {
            t = new Thread(new ThreadStart(ShowWaiting));
            t.Start();
        }
        /// <summary>
        /// ��ֹ����
        /// </summary>
        public void WaitingThreadStop()
        {
            t.Abort();
        }
        #endregion

        #region EXCEL��datatable�໥ת��
        /// <summary>
        /// �����ݼ�������Ϊexcel
        /// </summary>
        /// <param name="name">����excel������</param>
        /// <param name="ds">�����������ݼ�</param>
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
            GC.Collect();//�������� 
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

            //������¼  
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
            GC.Collect();//�������� 
        }

        #endregion

        #region XML��dataset�໥ת��
        /// <summary>
        /// ��xml���������ַ���ת��ΪDataSet
        /// </summary>
        /// <param name="xmlData">�ַ���</param>
        /// <returns>����dataset����</returns>
        public static DataSet ConvertXMLToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                //��streamװ�ص�XmlTextReader
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
        ///  ��xml�ļ�ת��ΪDataSet
        /// </summary>
        /// <param name="xmlFile">xml�ļ���ַ</param>
        /// <returns>dataset����</returns>
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
                //��streamװ�ص�XmlTextReader
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
        ///  ��DataSetת��Ϊxml�����ַ���
        /// </summary>
        /// <param name="xmlDS">dataset����</param>
        /// <returns>xml�ַ���</returns>
        public static string ConvertDataSetToXML(DataSet xmlDS)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;

            try
            {
                stream = new MemoryStream();
                //��streamװ�ص�XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);

                //��WriteXml����д���ļ�.
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
        /// �����ݼ�ת��ΪptNameָ���ĸ�ʽ�洢��xmlFile·����
        /// </summary>
        /// <param name="xmlDS">��Ҫת��������</param>
        /// <param name="xmlFile">���·��</param>
        /// <param name="sqlgetlayout">��ȡ��ʽ</param>
        /// <param name="doc">xml�ļ�</param>
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
        /// ��xml���ݼ�תת��Ϊxml�ļ�
        /// </summary>
        /// <param name="dsxml">���ݼ�</param>
        /// <param name="xmlFile">·��</param>
        /// <returns>�����ַ���</returns>
        public static string ConverDataSetToXMLFile(DataSet dsxml, string xmlFile)
        {
            try
            {
                StringBuilder strxml = new StringBuilder(ConvertDataSetToXML(dsxml));
                StreamWriter sw = new StreamWriter(xmlFile);
                strxml.Replace("NewDataSet", "��һ��");
                strxml.Replace("ds", "�ڶ���");
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
        /// ��DataSetת��Ϊxml�ļ�
        /// </summary>
        /// <param name="xmlDS">dtaset����</param>
        /// <param name="xmlFile">�ļ����·��</param>
        public static void ConvertDataSetToXMLFile(DataSet xmlDS, string xmlFile)
        {
            MemoryStream stream = null;
            XmlTextWriter writer = null;

            try
            {
                stream = new MemoryStream();
                //��streamװ�ص�XmlTextReader
                writer = new XmlTextWriter(stream, Encoding.Unicode);

                //��WriteXml����д���ļ�.
                xmlDS.WriteXml(writer);
                int count = (int)stream.Length;
                byte[] arr = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(arr, 0, count);

                //����Unicode������ı�
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

        #region �������ݿ�

        /// <summary>
        /// ��MongoCursorת��ΪDataTable��δʵ�֡�
        /// </summary>
        /// <param name="mc">����MongoCursor</param>
        /// <returns>���ɵ�DataTable</returns>
        public static DataTable ConverMongoCursorToDataTable(MongoCursor mc)
        {
            DataTable dt = new DataTable();
            //�еĳ�ʼ��
            foreach (string column in mc)
            {
                
            }
            //������
            foreach (BsonDocument item in mc)
            {
                
            }
            return dt;
        
        }

        /// <summary>
        /// ��ѯMongoDB
        /// </summary>
        /// <param name="collenction">��������</param>
        /// <param name="qd">QueryCocument����</param>
        /// <returns>���ؽ��</returns>
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
                //����ȫ��
                //result = col.FindAllAs<BsonDocument>();
                //��������
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
        /// MongoDb��������
        /// </summary>
        /// <param name="collenction">��������</param>
        /// <param name="bd">�����������</param>
        /// <returns>�������ݷ��صĽ��</returns>
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
        /// OleDb���� ִ�в�ѯ���� x86ƽ̨
        /// </summary>
        /// <param name="p_strSql">��ѯsql���</param>
        /// <param name="p_dicDictionary">�ֵ����</param>
        /// <param name="p_strTablename">������datatable����</param>
        /// <param name="cmd">cmd</param>
        /// <returns>���ر�</returns>
        static public DataTable OleExecuteBySQL(string p_strSql, Dictionary<string, string> p_dicDictionary, string p_strTablename)
        {
            if ("" == m_strConnectionString)
            {
                MessageBox.Show("δ�������ݿ������ַ�����");
            }
            DataTable _dtTable = new DataTable(p_strTablename);
            m_oleConn = new OleDbConnection(m_strConnectionString);
            m_oleCmd = m_oleConn.CreateCommand();
            m_oleConn.Open();
            ChangeSelectCommand(p_strSql, p_dicDictionary, ref m_oleCmd);
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
        /// sqlclient���� ִ�в�ѯ����
        /// </summary>
        /// <param name="p_strSql">��ѯsql���</param>
        /// <param name="p_dicDictionary">�ֵ����</param>
        /// <param name="p_strTablename">������datatable����</param>
        /// <param name="cmd">cmd</param>
        /// <returns>���ر�</returns>
        static public DataTable SqlExecuteBySQL(string p_strSql, Dictionary<string, string> p_dicDictionary, string p_strTablename)
        {
            if ("" == m_strConnectionString)
            {
                MessageBox.Show("δ�������ݿ������ַ�����");
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
        /// ִ�в�ѯ����
        /// </summary>
        /// <param name="p_strSql">��ѯsql���</param>
        /// <param name="p_dicDictionary">�ֵ����</param>
        /// <param name="p_strTablename">������datatable����</param>
        /// <param name="cmd">cmd</param>
        /// <returns>���ر�</returns>
        static public DataTable OraExecuteBySQL(string p_strSql, Dictionary<string, string> p_dicDictionary, string p_strTablename)
        {
            if ("" == m_strConnectionString)
            {
                MessageBox.Show("δ�������ݿ������ַ�����");
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
        /// ��������
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
        /// ֹͣ����
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
        /// �ڷ����ⲿ����������Ϊ�������롣����������ֶ��ύ����(������)
        /// </summary>
        /// <param name="p_strSql">ִ�е�sql</param>
        /// <param name="dictionary">�����ֵ�</param>
        /// <returns>Ӱ��Ľ������</returns>
        static public int OleExecuteTrans(string p_strSql, Dictionary<string, string> p_dictParam)
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
            ChangeSelectCommand(p_strSql, p_dictParam, ref m_oleCmd);
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
        /// �ڷ����ⲿ����������Ϊ�������롣����������ֶ��ύ����(������)
        /// </summary>
        /// <param name="p_strSql">ִ�е�sql</param>
        /// <param name="dictionary">�����ֵ�</param>
        /// <returns>Ӱ��Ľ������</returns>
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
            ChangeSelectCommand(p_strSql, p_dictParam, ref m_oraCmd);
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
        /// ����OleDb��ʽ���� ִ������ɾ���Ĳ��� x86ƽ̨
        /// </summary>
        /// <param name="p_strSql">������sql</param>
        /// <param name="p_dictParam">�ֵ����</param>
        /// <param name="cmd">cmd</param>
        /// <returns>���ؽ��</returns>
        static public int OleExecuteNonQuery(string p_strSql, Dictionary<string, string> p_dictParam)
        {
            int _iExeCount = 0;
            m_oleConn = new OleDbConnection(m_strConnectionString);
            m_oleCmd = m_oleConn.CreateCommand();
            m_oleConn.Open();
            ChangeSelectCommand(p_strSql, p_dictParam, ref m_oleCmd);
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
        /// ����.net1.1 ���oracleclient���� ִ������ɾ���Ĳ���
        /// </summary>
        /// <param name="p_strSql">������sql</param>
        /// <param name="p_dictParam">�ֵ����</param>
        /// <param name="cmd">cmd</param>
        /// <returns>���ؽ��</returns>
        static public int OraExecuteNonQuery(string p_strSql, Dictionary<string, string> p_dictParam)
        {
            int _iExeCount = 0;
            m_oraConn = new OracleConnection(m_strConnectionString);
            m_oraCmd = m_oraConn.CreateCommand();
            m_oleConn.Open();
            ChangeSelectCommand(p_strSql, p_dictParam, ref m_oraCmd);
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
        /// ���sqlclient���� ִ������ɾ���Ĳ���
        /// </summary>
        /// <param name="p_strSql">������sql</param>
        /// <param name="p_dictParam">�ֵ����</param>
        /// <param name="cmd">cmd</param>
        /// <returns>���ؽ��</returns>
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
        /// ͨ��APP.config�ļ���ȡ�������ݿ��ַ���
        /// </summary>
        /// <returns></returns>
        public static void SetConnectionString()
        {
            string _strDBType = ConfigurationManager.AppSettings["DBType"];
            if ("Oracle" == _strDBType)
            {
                m_strConnectionString = ConfigurationManager.ConnectionStrings["Oracle"].ConnectionString;
            }
            else if ("SQLServer" == _strDBType)
            {
                m_strConnectionString = ConfigurationManager.ConnectionStrings["SQLServer"].ConnectionString;
            }
            else if ("MySQL" == _strDBType)
            {
                m_strConnectionString = ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString;
            }
            else
            {
                MessageBox.Show("δ����[DBType]�����ݿ����Ͳ���[Oracle][SQLServer][MySQL]!");
            }
           
        }

        /// <summary>
        /// �滻sql������������cmd��ֵ,�󶨱���
        /// </summary>
        /// <param name="p_strSql">sql���</param>
        /// <param name="p_dictParam">�����ֵ�</param>
        /// <param name="p_oleCmd">cmd</param>
        /// <returns>�����Ƿ��滻�����ɹ�</returns>
        static public bool ChangeSelectCommand(string p_strSql, Dictionary<string, string> p_dictParam, ref OleDbCommand p_oleCmd)
        {

            p_oleCmd.Parameters.Clear();
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
                            p_oleCmd.Parameters.Add(new OleDbParameter(strParm, values));

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
            p_oleCmd.CommandText = sqltxt;
            return true;
        }

        /// <summary>
        /// �滻sql������������cmd��ֵ,�󶨱���
        /// </summary>
        /// <param name="p_strSql">sql���</param>
        /// <param name="p_dictParam">�����ֵ�</param>
        /// <param name="p_dbCmd">cmd</param>
        /// <returns>�����Ƿ��滻�����ɹ�</returns>
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
        /// �滻sql������������cmd��ֵ,�󶨱���
        /// </summary>
        /// <param name="p_strSql">sql���</param>
        /// <param name="p_dictParam">�����ֵ�</param>
        /// <param name="p_oraCmd">cmd</param>
        /// <returns>�����Ƿ��滻�����ɹ�</returns>
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

        #region ����Ƿ�Ϊ��
        public static DataTable CheckNULL(DataTable source)
        {
            if (source == null)
            {
                MessageBox.Show("����ԴΪ��");
                return null;
            }
            else
            {
                return source;
            }
        }
        #endregion

        #region ��̬���ɲ˵�
        /// <summary>
        /// ��̬�����˵�
        /// </summary>
        private void CreateMenu(MenuStrip MainMenuStrip, Form parient_form)
        {
            //����һ�����˵�
            MenuStrip mainMenu = new MenuStrip();
            DataSet ds = new DataSet();
            //��XML�ж�ȡ���ݡ����ݽṹ������ϸ��һ�¡�
            ds.ReadXml(@"..\..\Menu.xml");
            DataView dv = ds.Tables[0].DefaultView;
            //ͨ��DataView�������������ȵõ����Ĳ˵�
            dv.RowFilter = "ParentItemID=0";
            for (int i = 0; i < dv.Count; i++)
            {
                //����һ���˵���
                ToolStripMenuItem topMenu = new ToolStripMenuItem();
                //���˵���Textֵ��Ҳ�����ڽ����Ͽ�����ֵ��
                topMenu.Text = dv[i]["Text"].ToString();
                //��������¼��˵���ͨ��CreateSubMenu�����������¼��˵�
                if (Convert.ToInt16(dv[i]["IsModule"]) == 1)
                {
                    //��ref�ķ�ʽ������˵����ݲ�������Ϊ�������ڸ�ֵ���ٻش�������Ҳ�����и��õķ���^_^.
                    CreateSubMenu(ref topMenu, Convert.ToInt32(dv[i]["ItemID"]), ds.Tables[0]);
                }
                //��ʾӦ�ó������Ѵ򿪵� MDI �Ӵ����б��Ĳ˵���
                mainMenu.MdiWindowListItem = topMenu;
                //���ݹ鸽�ӺõĲ˵��ӵ��˵������ϡ�
                mainMenu.Items.Add(topMenu);
            }
            mainMenu.Dock = DockStyle.Top;
            //�������MainMenuStrip��ΪmainMenu.
            MainMenuStrip = mainMenu;
            //������Ҫ�������д���˵�������������������С�
            parient_form.Controls.Add(mainMenu);
        }

        /// <summary>
        /// �����Ӳ˵�
        /// </summary>
        /// <param name="topMenu">���˵���</param>
        /// <param name="ItemID">���˵���ID</param>
        /// <param name="dt">���в˵����ݼ�</param>
        private void CreateSubMenu(ref ToolStripMenuItem topMenu, int ItemID, DataTable dt)
        {
            DataView dv = new DataView(dt);
            //���˳���ǰ���˵����������Ӳ˵�����(��Ϊ��һ���)
            dv.RowFilter = "ParentItemID=" + ItemID.ToString();

            for (int i = 0; i < dv.Count; i++)
            {
                //�����Ӳ˵���
                ToolStripMenuItem subMenu = new ToolStripMenuItem();
                subMenu.Text = dv[i]["Text"].ToString();
                //��������Ӳ˵�������ݹ���ء�
                if (Convert.ToInt16(dv[i]["IsModule"]) == 1)
                {
                    //�ݹ����
                    CreateSubMenu(ref subMenu, Convert.ToInt32(dv[i]["ItemID"]), dt);
                }
                else
                {
                    //��չ���Կ��Լ��κ���Ҫ��ֵ��������formName���������ش��塣
                    subMenu.Tag = dv[i]["FormName"].ToString();
                    //��û���Ӳ˵��Ĳ˵�����¼���
                    subMenu.Click += new EventHandler(subMenu_Click);
                }
                if (dv[i]["ImageName"].ToString().Length > 0)
                {
                    //���ò˵���ǰ���ͼƱΪ16X16��ͼƬ�ļ���
                    Image img = Image.FromFile(@"..\..\Image\" + dv[i]["ImageName"].ToString());
                    subMenu.Image = img;
                    subMenu.Image.Tag = dv[i]["ImageName"].ToString();
                }
                //���˵��ӵ�����˵��¡�
                topMenu.DropDownItems.Add(subMenu);
            }
        }

        /// <summary>
        /// �˵������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void subMenu_Click(object sender, EventArgs e)
        {
            //tag�������������õ���
            //string formName = ((ToolStripMenuItem)sender).Tag.ToString();
            //CreateFormInstance(formName);
        }

        /// <summary>
        /// ����formʵ����
        /// </summary>
        /// <param name="formName">form������</param>
        private void CreateFormInstance(Form form, string formName)
        {
            bool flag = false;
            //�����������ϵ������Ӳ˵�
            for (int i = 0; i < form.MdiChildren.Length; i++)
            {
                //�������Ĵ��ڱ��������¼���
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
                //������������÷��䴴��form����ʵ����
                Assembly asm = Assembly.Load("Fastyou.BookShop.Win");//������
                object frmObj = asm.CreateInstance("Fastyou.BookShop.Win." + formName);//����+form��������
                Form frms = (Form)frmObj;
                //tag����Ҫ����дһ�Σ������ڵڶ��ε�ʱ��ȡ������ԭ�򻹲��������֪��������֪��
                frms.Tag = formName.ToString();
                frms.MdiParent = form;
                frms.Show();
            }
        }
        #endregion

        #region �ı�����ʾ
        /// <summary>
        /// ͨ��ѡȡ�˱���������Ӧֵ
        /// </summary>
        /// <param name="item">��ʾ�ı�</param>
        /// <returns>��Ӧֵ</returns>
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