using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteCityData
{
    internal class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            WriteData();
        }

        public static void WriteData()
        {
            logger.Info("程式開始執行");
            WriteLog("Info", "程式開始執行");
            // config參數，接收資料的檔案儲存路徑
            string fileDir = ConfigurationManager.AppSettings["DataFilePath"].ToString();
            string backUpDir = ConfigurationManager.AppSettings["BackUpFilePath"].ToString();
            string backUpFolder = Path.Combine(backUpDir, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            string connStr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;
            try
            {
                //新增備份資料夾
                if (!Directory.Exists(backUpDir))
                {
                    Directory.CreateDirectory(backUpDir);
                }

                Directory.CreateDirectory(backUpFolder);

                //刪除超過七天的備份
                string[] fileArr = Directory.GetDirectories(backUpDir);
                if (fileArr.Length > 0)
                {
                    for (int i = 0; i < fileArr.Length; i++)
                    {
                        string path = fileArr[i];
                        string dirName = Path.GetFileNameWithoutExtension(path);
                        DateTime date = DateTime.ParseExact(dirName, "yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
                        if (date.AddDays(7) < DateTime.Now)
                        {
                            Directory.Delete(Path.Combine(backUpDir, dirName), true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error", "新增備份資料夾時發生錯誤:" + ex.Message);
            }

            try
            {
                #region DEPT

                WriteLog("Info", "開始新增資料至 DEPT");
                string deptDir = Path.Combine(fileDir, "DEPT");
                //List<DtDept> deptLst = new List<DtDept>();
                //bool hasRow = false;

                string deptSQL = string.Empty;
                if (Directory.Exists(deptDir))
                {
                    string[] fileArr = Directory.GetFiles(deptDir);
                    if (fileArr.Length > 0)
                    {
                        for (int i = 0; i < fileArr.Length; i++)
                        {
                            string path = fileArr[i];
                            string fileName = Path.GetFileName(path);
                            string fileNameWithoutEx = Path.GetFileNameWithoutExtension(path);
                            string city = fileNameWithoutEx.Substring(fileNameWithoutEx.IndexOf("-") + 1);
                            string data = File.ReadAllText(path);

                            if (data.Length > 0)
                            {
                                //data = data.Replace("\r\n", "\n");
                                string[] stringSeparators = new string[] { "\r\n" };
                                string[] row = data.Split(stringSeparators, StringSplitOptions.None);
                                DataTable tbDEPT = new DataTable();
                                DataColumn CID = new DataColumn("CID");
                                DataColumn DEPTNO = new DataColumn("DEPTNO");
                                DataColumn DEPTNAME = new DataColumn("DEPTNAME");
                                DataColumn INDEPTNO = new DataColumn("INDEPTNO");
                                DataColumn GRADE = new DataColumn("GRADE");
                                DataColumn ADDR = new DataColumn("ADDR");
                                DataColumn X = new DataColumn("X");
                                DataColumn Y = new DataColumn("Y");
                                DataColumn LDATE = new DataColumn("LDATE");
                                DataColumn LUSER = new DataColumn("LUSER");
                                DataColumn DELETED = new DataColumn("DELETED");
                                tbDEPT.Columns.Add(CID);
                                tbDEPT.Columns.Add(DEPTNO);
                                tbDEPT.Columns.Add(DEPTNAME);
                                tbDEPT.Columns.Add(INDEPTNO);
                                tbDEPT.Columns.Add(GRADE);
                                tbDEPT.Columns.Add(ADDR);
                                tbDEPT.Columns.Add(X);
                                tbDEPT.Columns.Add(Y);
                                tbDEPT.Columns.Add(LDATE);
                                tbDEPT.Columns.Add(LUSER);
                                tbDEPT.Columns.Add(DELETED);
                                for (int s = 0; s < row.Count(); s++)
                                {
                                    if (row[s].Equals(string.Empty))
                                    {
                                        continue;
                                    }

                                    string[] item = row[s].Split('\t');
                                    DtDept dept = new DtDept();
                                    string strX = "0";
                                    string strY = "0";
                                    if (!string.IsNullOrEmpty(item[5].ToString())) {
                                        strX = item[5].ToString();
                                    }
                                    if (!string.IsNullOrEmpty(item[6].ToString()))
                                    {
                                        strY = item[6].ToString();
                                    }

                                    DataRow row1 = tbDEPT.NewRow();
                                    row1["CID"] = city;
                                    row1["DEPTNO"] = item[0];
                                    row1["DEPTNAME"] = item[1];
                                    row1["INDEPTNO"] = item[2];
                                    row1["GRADE"] = item[3];
                                    row1["ADDR"] = item[4];
                                    row1["X"] = strX;
                                    row1["Y"] = strY;
                                    row1["LDATE"] = item[7];
                                    row1["LUSER"] = item[8];
                                    row1["DELETED"] = item[9];
                                    tbDEPT.Rows.Add(row1);
                                }

                                if (tbDEPT.Rows.Count > 0)
                                {
                                        string deleteSQL = string.Format(@"DELETE FROM DEPT_forOracle WHERE CID = '{0}';", city);
                                        ExecuteDeleteCommand(deleteSQL);

                                    using (SqlConnection conn = new SqlConnection(connStr))
                                    {
                                        conn.Open();
                                        using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn))
                                        {
                                            //設定一個批次量寫入多少筆資料
                                            sqlBC.BatchSize = 1000;

                                            //設定逾時的秒數
                                            sqlBC.BulkCopyTimeout = 60;

                                            //設定要寫入的資料庫
                                            sqlBC.DestinationTableName = "DEPT_forOracle";

                                            //對應資料行
                                            sqlBC.ColumnMappings.Add("CID", "CID");
                                            sqlBC.ColumnMappings.Add("DEPTNO", "DEPTNO");
                                            sqlBC.ColumnMappings.Add("DEPTNAME", "DEPTNAME");
                                            sqlBC.ColumnMappings.Add("INDEPTNO", "INDEPTNO");
                                            sqlBC.ColumnMappings.Add("GRADE", "GRADE");
                                            sqlBC.ColumnMappings.Add("ADDR", "ADDR");
                                            sqlBC.ColumnMappings.Add("X", "X");
                                            sqlBC.ColumnMappings.Add("Y", "Y");
                                            sqlBC.ColumnMappings.Add("LDATE", "LDATE");
                                            sqlBC.ColumnMappings.Add("LUSER", "LUSER");
                                            sqlBC.ColumnMappings.Add("DELETED", "DELETED");
                                            //開始寫入
                                            sqlBC.WriteToServer(tbDEPT);
                                        }
                                    }

                                    File.Move(path, Path.Combine(backUpFolder, fileName));

  
                                }
                            }
                        }
                    }
                }
                WriteLog("Info", "結束新增資料至 DEPT");
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog("Error", "新增至 DEPT 資料庫時發生錯誤:" + ex.Message);
            }

            try
            {
                #region EMPL
                WriteLog("Info", "開始新增資料至 EMPL");
                string emplDir = Path.Combine(fileDir, "EMPL");
                //List<DtEmpl> emplLst = new List<DtEmpl>();
                //bool hasRow = false;

                string emplSQL = string.Empty;
                if (Directory.Exists(emplDir))
                {
                    string[] fileArr = Directory.GetFiles(emplDir);
                    if (fileArr.Length > 0)
                    {
                        for (int i = 0; i < fileArr.Length; i++)
                        {
                            string path = fileArr[i];
                            string fileName = Path.GetFileName(path);
                            string fileNameWithoutEx = Path.GetFileNameWithoutExtension(path);
                            string city = fileNameWithoutEx.Substring(fileNameWithoutEx.IndexOf("-") + 1);
                            string data = File.ReadAllText(path);
                            if (data.Length > 0)
                            {
                                data = data.Replace("\r\n", "\n");
                                string[] row = data.Split('\n');

                                DataTable tbEMPL = new DataTable();
                                DataColumn USERID = new DataColumn("USERID");
                                DataColumn PWD = new DataColumn("PWD");
                                DataColumn NAME = new DataColumn("NAME");
                                DataColumn COUNTRY = new DataColumn("COUNTRY");
                                DataColumn DEPTNO = new DataColumn("DEPTNO");
                                DataColumn JOBTITLE = new DataColumn("JOBTITLE");
                                DataColumn JOBTYPE = new DataColumn("JOBTYPE");
                                DataColumn ONDUTY = new DataColumn("ONDUTY");
                                DataColumn EMAIL = new DataColumn("EMAIL");
                                DataColumn PRETEL = new DataColumn("PRETEL");
                                DataColumn TEL = new DataColumn("TEL");
                                DataColumn LASTTEL = new DataColumn("LASTTEL");
                                DataColumn CELLPHONE = new DataColumn("CELLPHONE");
                                DataColumn LDATE = new DataColumn("LDATE");
                                DataColumn LUSER = new DataColumn("LUSER");
                                DataColumn ROLEID = new DataColumn("ROLEID");
                                DataColumn DELETED = new DataColumn("DELETED");
                                DataColumn NEWDEPTNO = new DataColumn("NEWDEPTNO");
                                DataColumn NEWROLEID = new DataColumn("NEWROLEID");
                                DataColumn CHECKROLE = new DataColumn("CHECKROLE");
                                DataColumn MEMID = new DataColumn("MEMID");
                                tbEMPL.Columns.Add(USERID);
                                tbEMPL.Columns.Add(PWD);
                                tbEMPL.Columns.Add(NAME);
                                tbEMPL.Columns.Add(COUNTRY);
                                tbEMPL.Columns.Add(DEPTNO);
                                tbEMPL.Columns.Add(JOBTITLE);
                                tbEMPL.Columns.Add(JOBTYPE);
                                tbEMPL.Columns.Add(ONDUTY);
                                tbEMPL.Columns.Add(EMAIL);
                                tbEMPL.Columns.Add(PRETEL);
                                tbEMPL.Columns.Add(TEL);
                                tbEMPL.Columns.Add(LASTTEL);
                                tbEMPL.Columns.Add(CELLPHONE);
                                tbEMPL.Columns.Add(LDATE);
                                tbEMPL.Columns.Add(LUSER);
                                tbEMPL.Columns.Add(ROLEID);
                                tbEMPL.Columns.Add(DELETED);
                                tbEMPL.Columns.Add(NEWDEPTNO);
                                tbEMPL.Columns.Add(NEWROLEID);
                                tbEMPL.Columns.Add(CHECKROLE);
                                tbEMPL.Columns.Add(MEMID);

                                for (int s = 0; s < row.Count(); s++)
                                {
                                    if (row[s].Equals(string.Empty))
                                    {
                                        continue;
                                    }

                                    string[] item = row[s].Split('\t');
                                    DataRow row1 = tbEMPL.NewRow();
                                    row1["USERID"] = item[0];
                                    row1["PWD"] = item[1];
                                    row1["NAME"] = item[2];
                                    row1["COUNTRY"] = item[3];
                                    row1["DEPTNO"] = item[4];
                                    row1["JOBTITLE"] = item[5];
                                    row1["JOBTYPE"] = item[6];
                                    row1["ONDUTY"] = item[7];
                                    row1["EMAIL"] = item[8];
                                    row1["PRETEL"] = item[9];
                                    row1["TEL"] = item[10];
                                    row1["LASTTEL"] = item[11];
                                    row1["CELLPHONE"] = item[12];
                                    row1["LDATE"] = item[13];
                                    row1["LUSER"] = item[14];
                                    row1["ROLEID"] = item[15];
                                    row1["DELETED"] = item[16];
                                    row1["NEWDEPTNO"] = item[17];
                                    row1["NEWROLEID"] = item[18];
                                    row1["CHECKROLE"] = item[19];
                                    row1["MEMID"] = item[20];
                                    tbEMPL.Rows.Add(row1);
                                }

                                if (tbEMPL.Rows.Count > 0)
                                {
                                       string deleteSQL = string.Format(@"DELETE FROM EMPL_forOracle WHERE COUNTRY = '{0}';", city);
                                        ExecuteDeleteCommand(deleteSQL);
                                    using (SqlConnection conn = new SqlConnection(connStr))
                                    {
                                        conn.Open();
                                        using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn))
                                        {
                                            //設定一個批次量寫入多少筆資料
                                            sqlBC.BatchSize = 1000;

                                            //設定逾時的秒數
                                            sqlBC.BulkCopyTimeout = 60;

                                            //設定要寫入的資料庫
                                            sqlBC.DestinationTableName = "EMPL_forOracle";

                                            //對應資料行
                                            sqlBC.ColumnMappings.Add("USERID", "USERID");
                                            sqlBC.ColumnMappings.Add("PWD", "PWD");
                                            sqlBC.ColumnMappings.Add("NAME", "NAME");
                                            sqlBC.ColumnMappings.Add("COUNTRY", "COUNTRY");
                                            sqlBC.ColumnMappings.Add("DEPTNO", "DEPTNO");
                                            sqlBC.ColumnMappings.Add("JOBTITLE", "JOBTITLE");
                                            sqlBC.ColumnMappings.Add("JOBTYPE", "JOBTYPE");
                                            sqlBC.ColumnMappings.Add("ONDUTY", "ONDUTY");
                                            sqlBC.ColumnMappings.Add("EMAIL", "EMAIL");
                                            sqlBC.ColumnMappings.Add("PRETEL", "PRETEL");
                                            sqlBC.ColumnMappings.Add("TEL", "TEL");
                                            sqlBC.ColumnMappings.Add("LASTTEL", "LASTTEL");
                                            sqlBC.ColumnMappings.Add("CELLPHONE", "CELLPHONE");
                                            sqlBC.ColumnMappings.Add("LDATE", "LDATE");
                                            sqlBC.ColumnMappings.Add("LUSER", "LUSER");
                                            sqlBC.ColumnMappings.Add("ROLEID", "ROLEID");
                                            sqlBC.ColumnMappings.Add("DELETED", "DELETED");
                                            sqlBC.ColumnMappings.Add("NEWDEPTNO", "NEWDEPTNO");
                                            sqlBC.ColumnMappings.Add("NEWROLEID", "NEWROLEID");
                                            sqlBC.ColumnMappings.Add("CHECKROLE", "CHECKROLE");
                                            sqlBC.ColumnMappings.Add("MEMID", "MEMID");

                                            //開始寫入
                                            sqlBC.WriteToServer(tbEMPL);
                                        }
                                    }
                                    File.Move(path, Path.Combine(backUpFolder, fileName));
                                }
                            }
                        }
                    }
                }
                WriteLog("Info", "結束新增資料至 EMPL");
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog("Error", "新增至 EMPL 資料庫時發生錯誤:" + ex.Message);
            }

            try
            {
                #region EMPL_MAPPING

                WriteLog("Info", "開始新增資料至 EMPL_MAPPING");
                string emplMapDir = Path.Combine(fileDir, "EMPL_MAPPING");
                List<DtEmplMap> emplMapLst = new List<DtEmplMap>();
                bool hasRow = false;

                string emplMapSQL = string.Empty;
                if (Directory.Exists(emplMapDir))
                {
                    string[] fileArr = Directory.GetFiles(emplMapDir);
                    if (fileArr.Length > 0)
                    {
                        for (int i = 0; i < fileArr.Length; i++)
                        {
                            string path = fileArr[i];
                            string fileName = Path.GetFileName(path);
                            string fileNameWithoutEx = Path.GetFileNameWithoutExtension(path);
                            string city = fileNameWithoutEx.Substring(fileNameWithoutEx.IndexOf("-") + 1);
                            string data = File.ReadAllText(path);
                            if (data.Length > 0)
                            {
                                DataTable tbEmplMapping = new DataTable();
                                DataColumn COUNTRY = new DataColumn("COUNTRY");
                                DataColumn USERID = new DataColumn("USERID");
                                DataColumn NDPPC_UID = new DataColumn("NDPPC_UID");
                                DataColumn LDATE = new DataColumn("LDATE");
                                DataColumn LUSER = new DataColumn("LUSER");
                                tbEmplMapping.Columns.Add(COUNTRY);
                                tbEmplMapping.Columns.Add(USERID);
                                tbEmplMapping.Columns.Add(NDPPC_UID);
                                tbEmplMapping.Columns.Add(LDATE);
                                tbEmplMapping.Columns.Add(LUSER);

                                data = data.Replace("\r\n", "\n");
                                string[] row = data.Split('\n');

                                for (int s = 0; s < row.Count(); s++)
                                {
                                    if (row[s].Equals(string.Empty))
                                    {
                                        continue;
                                    }

                                    string[] item = row[s].Split('\t');
                                    DataRow row1 = tbEmplMapping.NewRow();
                                    row1["COUNTRY"] = item[0];
                                    row1["USERID"] = item[1];
                                    row1["NDPPC_UID"] = item[2];
                                    row1["LDATE"] = item[3];
                                    row1["LUSER"] = item[4];
                                    tbEmplMapping.Rows.Add(row1);
                                }

                                if (tbEmplMapping.Rows.Count > 0)
                                {
                                        string deleteSQL = string.Format(@"DELETE FROM EMPL_MAPPING_forOracle WHERE COUNTRY = '{0}';", city);
                                        ExecuteDeleteCommand(deleteSQL);

                                    using (SqlConnection conn = new SqlConnection(connStr))
                                    {
                                        conn.Open();
                                        using (SqlBulkCopy sqlBC = new SqlBulkCopy(conn))
                                        {
                                            //設定一個批次量寫入多少筆資料
                                            sqlBC.BatchSize = 1000;

                                            //設定逾時的秒數
                                            sqlBC.BulkCopyTimeout = 60;

                                            //設定要寫入的資料庫
                                            sqlBC.DestinationTableName = "EMPL_MAPPING_forOracle";

                                            //對應資料行
                                            sqlBC.ColumnMappings.Add("COUNTRY", "COUNTRY");
                                            sqlBC.ColumnMappings.Add("USERID", "USERID");
                                            sqlBC.ColumnMappings.Add("NDPPC_UID", "NDPPC_UID");
                                            sqlBC.ColumnMappings.Add("LDATE", "LDATE");
                                            sqlBC.ColumnMappings.Add("LUSER", "LUSER");

                                            //開始寫入
                                            sqlBC.WriteToServer(tbEmplMapping);
                                        }
                                    }

                                    File.Move(path, Path.Combine(backUpFolder, fileName));
                                }
                            }
                        }
                    }
                }

                WriteLog("Info", "結束新增資料至 EMPL_MAPPING");

                #endregion
            }
            catch (Exception ex)
            {
                WriteLog("Error", "新增至 EMPL_MAPPING 資料庫時發生錯誤:" + ex.Message);
            }
        }

        /// <summary>
        /// 執行sql指令碼不回傳
        /// </summary>
        /// <param name="sql">新增sql</param>
        public static void ExecuteInsertCommand(string sql)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

                //建立資料庫連線
                SqlConnection con = new SqlConnection(connStr);

                //啟用連線
                con.Open();

                //SQL查詢指令
                SqlCommand cmd = new SqlCommand(sql, con);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.InsertCommand = new SqlCommand(sql, con);
                adapter.InsertCommand.ExecuteNonQuery();

                //關閉連線
                if (adapter != null) { adapter.Dispose(); }
                if (cmd != null) { cmd.Dispose(); }
                if (con != null) { con.Close(); }

            }
            catch (Exception ex)
            {
                WriteLog("Error", "ExecuteQuery()發生錯誤:" + ex.Message);
            };
        }

        /// <summary>
        /// 執行sql指令碼不回傳
        /// </summary>
        /// <param name="sql">刪除sql</param>
        public static void ExecuteDeleteCommand(string sql)
        {
            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

                //建立資料庫連線
                SqlConnection con = new SqlConnection(connStr);

                //啟用連線
                con.Open();

                //SQL查詢指令
                SqlCommand cmd = new SqlCommand(sql, con);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.DeleteCommand = new SqlCommand(sql, con);
                adapter.DeleteCommand.ExecuteNonQuery();

                //關閉連線
                if (adapter != null) { adapter.Dispose(); }
                if (cmd != null) { cmd.Dispose(); }
                if (con != null) { con.Close(); }

            }
            catch (Exception ex)
            {
                WriteLog("Error", "ExecuteQuery()發生錯誤:" + ex.Message);
            };
        }

        /// <summary>
        /// 寫log
        /// </summary>
        /// <param name="LogType"></param>
        /// <param name="LogMess"></param>
        public static void WriteLog(string LogType, string LogMess)
        {
            switch (LogType) {
                case "Info":
                    logger.Info(LogMess);
                    break;
                case "Error":
                    logger.Error(LogMess);
                    break;
            }
            //string FileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            //string str = Directory.GetCurrentDirectory();

            //try
            //{
            //    if (!Directory.Exists(str + "\\Log"))
            //    {
            //        Directory.CreateDirectory(str + "\\Log");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(string.Format("Failed to create directory on {0}: ", str + "\\Log"));
            //    Console.WriteLine(ex.Message);
            //    return;
            //}


            //string FilePath = str + "\\Log\\" + FileName;
            //string strLog = LogType + "  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + LogMess;

            //StreamWriter sw = File.AppendText(FilePath);            
            //sw.WriteLine(strLog);
            //Console.WriteLine(strLog);
            //sw.Flush();
            //sw.Close();
            //sw.Dispose();
        }
    }
}
