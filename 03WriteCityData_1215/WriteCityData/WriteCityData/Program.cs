using System;
using System.Collections.Generic;
using System.Configuration;
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
        static void Main(string[] args)
        {
            WriteData();
        }

        public static void WriteData()
        {
            WriteLog("Info", "程式開始執行");
            // config參數，接收資料的檔案儲存路徑
            string fileDir = ConfigurationManager.AppSettings["DataFilePath"].ToString();
            string backUpDir = ConfigurationManager.AppSettings["BackUpFilePath"].ToString();
            string backUpFolder = Path.Combine(backUpDir, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            
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
                            Directory.Delete(Path.Combine(backUpDir, dirName));
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
                List<DtDept> deptLst = new List<DtDept>();
                bool hasRow = false;

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
                                data = data.Replace("\r\n", "\n");
                                string[] row = data.Split('\n');

                                for (int s = 0; s < row.Count(); s++)
                                {
                                    if (row[s].Equals(string.Empty))
                                    {
                                        continue;
                                    }

                                    string[] item = row[s].Split('\t');
                                    DtDept dept = new DtDept();
                                    dept.DEPTNO = item[0];
                                    dept.DEPTNAME = item[1];
                                    dept.INDEPTNO = item[2];
                                    dept.GRADE = item[3];
                                    dept.ADDR = item[4];
                                    dept.X = item[5];
                                    dept.Y = item[6];
                                    dept.LDATE = item[7];
                                    dept.LUSER = item[8];
                                    dept.DELETED = item[9];
                                    deptLst.Add(dept);
                                }

                                if (deptLst.Count > 0)
                                {
                                    foreach (var item in deptLst)
                                    {
                                        string value = string.Format(@"INSERT DEPT_forOracle
(CID ,DEPTNO ,DEPTNAME ,INDEPTNO ,GRADE ,ADDR ,X ,Y ,LDATE ,LUSER ,DELETED)
VALUES
('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, {7}, '{8}', '{9}', '{10}');
",
                                            city, item.DEPTNO, item.DEPTNAME, item.INDEPTNO, item.GRADE, item.ADDR, item.X, item.Y, item.LDATE, item.LUSER, item.DELETED);
                                        deptSQL += value;
                                        hasRow = true;
                                    }

                                    if (hasRow)
                                    {
                                        string deleteSQL = string.Format(@"DELETE FROM DEPT_forOracle WHERE CID = '{0}';", city);
                                        ExecuteDeleteCommand(deleteSQL);

                                        deptSQL = deptSQL.Substring(0, deptSQL.Length - 1);
                                        ExecuteInsertCommand(deptSQL);

                                        File.Move(path, Path.Combine(backUpFolder, fileName));

                                    }
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
                List<DtEmpl> emplLst = new List<DtEmpl>();
                bool hasRow = false;

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

                                for (int s = 0; s < row.Count(); s++)
                                {
                                    if (row[s].Equals(string.Empty))
                                    {
                                        continue;
                                    }

                                    string[] item = row[s].Split('\t');
                                    DtEmpl empl = new DtEmpl();
                                    empl.USERID = item[0];
                                    empl.PWD = item[1];
                                    empl.NAME = item[2];
                                    empl.COUNTRY = item[3];
                                    empl.DEPTNO = item[4];
                                    empl.JOBTITLE = item[5];
                                    empl.JOBTYPE = item[6];
                                    empl.ONDUTY = item[7];
                                    empl.EMAIL = item[8];
                                    empl.PRETEL = item[9];
                                    empl.TEL = item[10];
                                    empl.LASTTEL = item[11];
                                    empl.CELLPHONE = item[12];
                                    empl.LDATE = item[13];
                                    empl.LUSER = item[14];
                                    empl.ROLEID = item[15];
                                    empl.DELETED = item[16];
                                    empl.NEWDEPTNO = item[17];
                                    empl.NEWROLEID = item[18];
                                    empl.CHECKROLE = item[19];
                                    empl.MEMID = item[20];
                                    emplLst.Add(empl);
                                }

                                if (emplLst.Count > 0)
                                {
                                    foreach (var item in emplLst)
                                    {
                                        string value = string.Format(@"INSERT EMPL_forOracle
(USERID ,PWD ,NAME ,COUNTRY ,DEPTNO ,JOBTITLE ,JOBTYPE ,ONDUTY ,EMAIL ,PRETEL ,TEL ,LASTTEL ,CELLPHONE ,LDATE ,LUSER ,ROLEID ,DELETED ,NEWDEPTNO ,NEWROLEID ,CHECKROLE ,MEMID)
VALUES
('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}');
",
                                            item.USERID, item.PWD, item.NAME, item.COUNTRY, item.DEPTNO, item.JOBTITLE, item.JOBTYPE, item.ONDUTY,
                                            item.EMAIL, item.PRETEL, item.TEL, item.LASTTEL, item.CELLPHONE, item.LDATE, item.LUSER, item.ROLEID,
                                            item.DELETED, item.NEWDEPTNO, item.NEWROLEID, item.CHECKROLE, item.MEMID);
                                        emplSQL += value;
                                        hasRow = true;
                                    }

                                    if (hasRow)
                                    {
                                        string deleteSQL = string.Format(@"DELETE FROM EMPL_forOracle WHERE COUNTRY = '{0}';", city);
                                        ExecuteDeleteCommand(deleteSQL);

                                        emplSQL = emplSQL.Substring(0, emplSQL.Length - 1);
                                        ExecuteInsertCommand(emplSQL);

                                        File.Move(path, Path.Combine(backUpFolder, fileName));
                                    }
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
                                data = data.Replace("\r\n", "\n");
                                string[] row = data.Split('\n');

                                for (int s = 0; s < row.Count(); s++)
                                {
                                    if (row[s].Equals(string.Empty))
                                    {
                                        continue;
                                    }

                                    string[] item = row[s].Split('\t');
                                    DtEmplMap emplMap = new DtEmplMap();
                                    emplMap.COUNTRY = item[0];
                                    emplMap.USERID = item[1];
                                    emplMap.NDPPC_UID = item[2];
                                    emplMap.LDATE = item[3];
                                    emplMap.LUSER = item[4];
                                    emplMapLst.Add(emplMap);
                                }

                                if (emplMapLst.Count > 0)
                                {
                                    foreach (var item in emplMapLst)
                                    {
                                        string value = string.Format(@"INSERT EMPL_MAPPING_forOracle
(COUNTRY ,USERID ,NDPPC_UID ,LDATE ,LUSER)
VALUES
('{0}', '{1}', '{2}', '{3}', '{4}');
",
                                            item.COUNTRY, item.USERID, item.NDPPC_UID, item.LDATE, item.LUSER);
                                        emplMapSQL += value;
                                        hasRow = true;
                                    }

                                    if (hasRow)
                                    {
                                        string deleteSQL = string.Format(@"DELETE FROM EMPL_MAPPING_forOracle WHERE COUNTRY = '{0}';", city);
                                        ExecuteDeleteCommand(deleteSQL);

                                        emplMapSQL = emplMapSQL.Substring(0, emplMapSQL.Length - 1);
                                        ExecuteInsertCommand(emplMapSQL);

                                        File.Move(path, Path.Combine(backUpFolder, fileName));
                                    }
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
            string FileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            string str = Directory.GetCurrentDirectory();

            try
            {
                if (!Directory.Exists(str + "\\Log"))
                {
                    Directory.CreateDirectory(str + "\\Log");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed to create directory on {0}: ", str + "\\Log"));
                Console.WriteLine(ex.Message);
                return;
            }


            string FilePath = str + "\\Log\\" + FileName;
            string strLog = LogType + "  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + LogMess;

            StreamWriter sw = File.AppendText(FilePath);
            sw.WriteLine(strLog);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}
