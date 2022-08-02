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
                                tbDEPT.Columns.Add("CID", typeof(string));
                                tbDEPT.Columns.Add("DEPTNO", typeof(string));
                                tbDEPT.Columns.Add("DEPTNAME", typeof(string));
                                tbDEPT.Columns.Add("INDEPTNO", typeof(string));
                                tbDEPT.Columns.Add("GRADE", typeof(string));
                                tbDEPT.Columns.Add("ADDR", typeof(string));
                                tbDEPT.Columns.Add("X", typeof(string));
                                tbDEPT.Columns.Add("Y", typeof(string));
                                tbDEPT.Columns.Add("LDATE", typeof(string));
                                tbDEPT.Columns.Add("LUSER", typeof(string));
                                tbDEPT.Columns.Add("DELETED", typeof(string));
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
                                tbEMPL.Columns.Add("USERID", typeof(string));
                                tbEMPL.Columns.Add("PWD", typeof(string));
                                tbEMPL.Columns.Add("NAME", typeof(string));
                                tbEMPL.Columns.Add("COUNTRY", typeof(string));
                                tbEMPL.Columns.Add("DEPTNO", typeof(string));
                                tbEMPL.Columns.Add("JOBTITLE", typeof(string));
                                tbEMPL.Columns.Add("JOBTYPE", typeof(string));
                                tbEMPL.Columns.Add("ONDUTY", typeof(string));
                                tbEMPL.Columns.Add("EMAIL", typeof(string));
                                tbEMPL.Columns.Add("PRETEL", typeof(string));
                                tbEMPL.Columns.Add("TEL", typeof(string));
                                tbEMPL.Columns.Add("LASTTEL", typeof(string));
                                tbEMPL.Columns.Add("CELLPHONE", typeof(string));
                                tbEMPL.Columns.Add("LDATE", typeof(string));
                                tbEMPL.Columns.Add("LUSER", typeof(string));
                                tbEMPL.Columns.Add("ROLEID", typeof(string));
                                tbEMPL.Columns.Add("DELETED", typeof(string));
                                tbEMPL.Columns.Add("NEWDEPTNO", typeof(string));
                                tbEMPL.Columns.Add("NEWROLEID", typeof(string));
                                tbEMPL.Columns.Add("CHECKROLE", typeof(string));
                                tbEMPL.Columns.Add("MEMID", typeof(string));

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
                                tbEmplMapping.Columns.Add("COUNTRY", typeof(string));
                                tbEmplMapping.Columns.Add("USERID", typeof(string));
                                tbEmplMapping.Columns.Add("NDPPC_UID", typeof(string));
                                tbEmplMapping.Columns.Add("LDATE", typeof(string));
                                tbEmplMapping.Columns.Add("LUSER", typeof(string));

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
        //public static void ExecuteInsertCommand(string sql)
        //{
        //    try
        //    {
        //        string connStr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

        //        //建立資料庫連線
        //        SqlConnection con = new SqlConnection(connStr);

        //        //啟用連線
        //        con.Open();

        //        //SQL查詢指令
        //        SqlCommand cmd = new SqlCommand(sql, con);

        //        SqlDataAdapter adapter = new SqlDataAdapter();
        //        adapter.InsertCommand = new SqlCommand(sql, con);
        //        adapter.InsertCommand.ExecuteNonQuery();

        //        //關閉連線
        //        if (adapter != null) { adapter.Dispose(); }
        //        if (cmd != null) { cmd.Dispose(); }
        //        if (con != null) { con.Close(); }

        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog("Error", "ExecuteQuery()發生錯誤:" + ex.Message);
        //    };
        //}

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
        }
    }
}
