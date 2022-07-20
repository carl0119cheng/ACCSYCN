using Newtonsoft.Json;
using NLog;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MoveCityData
{
    internal class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            GetAndSendData();
        }

        public static void GetAndSendData()
        {
            WriteLog("Info", "程式開始執行");

            //資料庫連線字串
            string conStrFrom = ConfigurationManager.ConnectionStrings["conStrFrom"].ConnectionString;
            string conStrTo = ConfigurationManager.ConnectionStrings["conStrTo"].ConnectionString;

            DtDeptList dtDeptList = new DtDeptList();
            dtDeptList.City = ConfigurationManager.AppSettings["City"];

            DtEmplList dtEmplList = new DtEmplList();
            dtEmplList.City = ConfigurationManager.AppSettings["City"];

            DtEmplMapList dtEmplMapList = new DtEmplMapList();
            dtEmplMapList.City = ConfigurationManager.AppSettings["City"];

            bool connectSuccess = true;
            try
            {
                WriteLog("Info", "開始撈取 DEPT 資料");

                string sql = @"SELECT DEPTNO ,DEPTNAME ,INDEPTNO ,GRADE ,ADDR ,X ,Y ,LDATE ,LUSER ,DELETED,SORTING,SORTING1 FROM DEPT";
                DataTable dtDept = ExecuteQuery(sql, conStrFrom);
                if (dtDept != null)
                {
                    dtDeptList.DtDepts = new List<DtDept>();
                    foreach (DataRow row in dtDept.Rows)
                    {
                        string DEPTNO = "";
                        if (row["DEPTNO"] != DBNull.Value)
                        {
                            DEPTNO = row["DEPTNO"].ToString();
                        }

                        string DEPTNAME = "";
                        if (row["DEPTNAME"] != DBNull.Value)
                        {
                            DEPTNAME = row["DEPTNAME"].ToString();
                        }

                        string INDEPTNO = "";
                        if (row["INDEPTNO"] != DBNull.Value)
                        {
                            INDEPTNO = row["INDEPTNO"].ToString();
                        }

                        string GRADE = "";
                        if (row["GRADE"] != DBNull.Value)
                        {
                            GRADE = row["GRADE"].ToString();
                        }

                        string ADDR = "";
                        if (row["ADDR"] != DBNull.Value)
                        {
                            ADDR = row["ADDR"].ToString();
                        }

                        string X = "0";
                        if (row["X"] != DBNull.Value)
                        {
                            X = row["X"].ToString();
                        }

                        string Y = "0";
                        if (row["Y"] != DBNull.Value)
                        {
                            Y = row["Y"].ToString();
                        }

                        string LDATE = "";
                        if (row["LDATE"] != DBNull.Value)
                        {
                            LDATE = row["LDATE"].ToString();
                        }

                        string LUSER = "";
                        if (row["LUSER"] != DBNull.Value)
                        {
                            LUSER = row["LUSER"].ToString();
                        }

                        string DELETED = "";
                        if (row["DELETED"] != DBNull.Value)
                        {
                            DELETED = row["DELETED"].ToString();
                        }

                        //資料庫中沒有的欄位
                        string SORTING = "";
                        if (row["SORTING"] != DBNull.Value)
                        {
                            SORTING = row["SORTING"].ToString();
                        }
                        string SORTING1 = "";
                        if (row["SORTING1"] != DBNull.Value)
                        {
                            SORTING1 = row["SORTING1"].ToString();
                        }

                        //string ISSAVED = "";
                        //if (row["ISSAVED"] != DBNull.Value)
                        //{
                        //    ISSAVED = row["ISSAVED"].ToString();
                        //}

                        DtDept deptPara = new DtDept()
                        {
                            DEPTNO = DEPTNO,
                            DEPTNAME = DEPTNAME,
                            INDEPTNO = INDEPTNO,
                            GRADE = GRADE,
                            ADDR = ADDR,
                            X = X,
                            Y = Y,
                            LDATE = LDATE,
                            LUSER = LUSER,
                            DELETED = DELETED,
                            SORTING = SORTING,
                            SORTING1 = SORTING1,
                            //ISSAVED = ISSAVED
                        };
                        dtDeptList.DtDepts.Add(deptPara);
                    }

                }
                else
                {
                    connectSuccess = false;
                    WriteLog("Info", "撈取 DEPT 資料失敗: 無資料或執行失敗");
                }

                WriteLog("Info", "結束撈取 DEPT 資料");
            }
            catch (Exception ex)
            {
                WriteLog("Error", "從資料庫 DEPT 取得資料時發生錯誤:" + ex.Message);
                connectSuccess = false;
            }

            try
            {
                if (connectSuccess)
                {
                    if (dtDeptList.DtDepts.Count > 0)
                    {
                        WriteLog("Info", "開始更新 DEPT 資料");

                        bool hasRow = false;
                        string deptSQL = string.Empty;
                        foreach (var item in dtDeptList.DtDepts)
                        {
                            string value = string.Format(@"INSERT INTO DEPT
(DEPTNO ,DEPTNAME ,INDEPTNO ,GRADE ,ADDR ,X ,Y ,LDATE ,LUSER ,DELETED,SORTING,SORTING1)
VALUES
('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}','{10}','{11}');
",
                                item.DEPTNO, item.DEPTNAME, item.INDEPTNO, item.GRADE, item.ADDR, item.X, item.Y, item.LDATE, item.LUSER, item.DELETED,item.SORTING,item.SORTING1);
                            deptSQL += value;
                            hasRow = true;
                        }

                        if (hasRow)
                        {
                            string DateTimeDay = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string BKsql = @"SELECT * into DEPT_"+ DateTimeDay + " FROM DEPT";

                            ExecuteDeleteCommand(BKsql, conStrTo);

                            string deleteSQL = @"TRUNCATE TABLE DEPT ;";
                            ExecuteDeleteCommand(deleteSQL, conStrTo);

                            deptSQL = deptSQL.Substring(0, deptSQL.Length - 1);
                          
                            ExecuteInsertCommand(deptSQL, conStrTo);
                        }

                        WriteLog("Info", "結束更新 DEPT 資料");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error", "於資料庫 DEPT 儲存檔案時發生錯誤:" + ex.Message);
                return;
            }

            connectSuccess = true;
            try
            {
                WriteLog("Info", "開始撈取 EMPL 資料");

                string sql = @"SELECT USERID ,PWD ,NAME ,COUNTRY ,DEPTNO ,JOBTITLE ,JOBTYPE ,
ONDUTY ,EMAIL ,PRETEL ,TEL ,LASTTEL ,CELLPHONE ,LDATE ,LUSER ,
ROLEID ,DELETED ,NEWDEPTNO ,NEWROLEID ,CHECKROLE ,RADIOKIND1, 
                            RADIOKIND2, IDNO, MEMID, ALIVETIME, BLOCKTIME, ERRORTIMES FROM EMPL";
                DataTable dtEmpl = ExecuteQuery(sql, conStrFrom);
                if (dtEmpl != null)
                {
                    dtEmplList.DtEmpls = new List<DtEmpl>();
                    foreach (DataRow row in dtEmpl.Rows)
                    {
                        string USERID = "";
                        if (row["USERID"] != DBNull.Value)
                        {
                            USERID = row["USERID"].ToString();
                        }

                        string PWD = "";
                        if (row["PWD"] != DBNull.Value)
                        {
                            PWD = row["PWD"].ToString();
                        }

                        string NAME = "";
                        if (row["NAME"] != DBNull.Value)
                        {
                            NAME = row["NAME"].ToString();
                        }

                        string COUNTRY = "";
                        if (row["COUNTRY"] != DBNull.Value)
                        {
                            COUNTRY = row["COUNTRY"].ToString();
                        }

                        string DEPTNO = "";
                        if (row["DEPTNO"] != DBNull.Value)
                        {
                            DEPTNO = row["DEPTNO"].ToString();
                        }

                        string JOBTITLE = "";
                        if (row["JOBTITLE"] != DBNull.Value)
                        {
                            JOBTITLE = row["JOBTITLE"].ToString();
                        }

                        string JOBTYPE = "";
                        if (row["JOBTYPE"] != DBNull.Value)
                        {
                            JOBTYPE = row["JOBTYPE"].ToString();
                        }

                        string ONDUTY = "";
                        if (row["ONDUTY"] != DBNull.Value)
                        {
                            ONDUTY = row["ONDUTY"].ToString();
                        }

                        string EMAIL = "";
                        if (row["EMAIL"] != DBNull.Value)
                        {
                            EMAIL = row["EMAIL"].ToString();
                        }

                        string PRETEL = "";
                        if (row["PRETEL"] != DBNull.Value)
                        {
                            PRETEL = row["PRETEL"].ToString();
                        }

                        string TEL = "";
                        if (row["TEL"] != DBNull.Value)
                        {
                            TEL = row["TEL"].ToString();
                        }

                        string LASTTEL = "";
                        if (row["LASTTEL"] != DBNull.Value)
                        {
                            LASTTEL = row["LASTTEL"].ToString();
                        }

                        string CELLPHONE = "";
                        if (row["CELLPHONE"] != DBNull.Value)
                        {
                            CELLPHONE = row["CELLPHONE"].ToString();
                        }

                        string LDATE = "";
                        if (row["LDATE"] != DBNull.Value)
                        {
                            LDATE = row["LDATE"].ToString();
                        }

                        string LUSER = "";
                        if (row["LUSER"] != DBNull.Value)
                        {
                            LUSER = row["LUSER"].ToString();
                        }

                        string ROLEID = "";
                        if (row["ROLEID"] != DBNull.Value)
                        {
                            ROLEID = row["ROLEID"].ToString();
                        }

                        string DELETED = "";
                        if (row["DELETED"] != DBNull.Value)
                        {
                            DELETED = row["DELETED"].ToString();
                        }

                        string NEWDEPTNO = "";
                        if (row["NEWDEPTNO"] != DBNull.Value)
                        {
                            NEWDEPTNO = row["NEWDEPTNO"].ToString();
                        }

                        string NEWROLEID = "";
                        if (row["NEWROLEID"] != DBNull.Value)
                        {
                            NEWROLEID = row["NEWROLEID"].ToString();
                        }

                        string CHECKROLE = "";
                        if (row["CHECKROLE"] != DBNull.Value)
                        {
                            CHECKROLE = row["CHECKROLE"].ToString();
                        }

                        string RADIOKIND1 = "";
                        if (row["RADIOKIND1"] != DBNull.Value)
                        {
                            RADIOKIND1 = row["RADIOKIND1"].ToString();
                        }

                        string RADIOKIND2 = "";
                        if (row["RADIOKIND2"] != DBNull.Value)
                        {
                            RADIOKIND2 = row["RADIOKIND2"].ToString();
                        }
                        string IDNO = "";
                        if (row["IDNO"] != DBNull.Value)
                        {
                            IDNO = row["IDNO"].ToString();
                        }
                        //資料庫中沒有的欄位
                        //string IDNO = "";
                        //if (row["IDNO"] != DBNull.Value)
                        //{
                        //    IDNO = row["IDNO"].ToString();
                        //}

                        //string IDNOKEY = "";
                        //if (row["IDNOKEY"] != DBNull.Value)
                        //{
                        //    IDNO = row["IDNOKEY"].ToString();
                        //}

                        string MEMID = "";
                        if (row["MEMID"] != DBNull.Value)
                        {
                            MEMID = row["MEMID"].ToString();
                        }

                        string ALIVETIME = "";
                        if (row["ALIVETIME"] != DBNull.Value)
                        {
                            ALIVETIME = row["ALIVETIME"].ToString();
                        }

                        string BLOCKTIME = "";
                        if (row["BLOCKTIME"] != DBNull.Value)
                        {
                            BLOCKTIME = row["BLOCKTIME"].ToString();
                        }
                        float ERRORTIMES = 0;
                        if (row["ERRORTIMES"] != DBNull.Value)
                        {
                            float.TryParse(row["ERRORTIMES"].ToString(), out ERRORTIMES);
                        }

                        DtEmpl emplPara = new DtEmpl()
                        {
                            USERID = USERID,
                            PWD = PWD,
                            NAME = NAME,
                            COUNTRY = COUNTRY,
                            DEPTNO = DEPTNO,
                            JOBTITLE = JOBTITLE,
                            JOBTYPE = JOBTYPE,
                            ONDUTY = ONDUTY,
                            EMAIL = EMAIL,
                            PRETEL = PRETEL,
                            TEL = TEL,
                            LASTTEL = LASTTEL,
                            CELLPHONE = CELLPHONE,
                            LDATE = LDATE,
                            LUSER = LUSER,
                            ROLEID = ROLEID,
                            DELETED = DELETED,
                            NEWDEPTNO = NEWDEPTNO,
                            NEWROLEID = NEWROLEID,
                            CHECKROLE = CHECKROLE,

                            RADIOKIND1= RADIOKIND1,
                            RADIOKIND2= RADIOKIND2,
                            IDNO = IDNO,
                            //IDNOKEY = IDNOKEY,
                            MEMID = MEMID,
                            ALIVETIME= ALIVETIME,
                            BLOCKTIME= BLOCKTIME,
                            ERRORTIMES= ERRORTIMES
                        };
                        dtEmplList.DtEmpls.Add(emplPara);
                    }
                }
                else
                {
                    connectSuccess = false;
                    WriteLog("Info", "撈取 EMPL 資料失敗: 無資料或執行失敗");
                }

                WriteLog("Info", "結束撈取 EMPL 資料");
            }
            catch (Exception ex)
            {
                WriteLog("Error", "從資料庫 EMPL 取得資料時發生錯誤:" + ex.Message);
                connectSuccess = false;
            }

            try
            {
                if (connectSuccess)
                {
                    if (dtEmplList.DtEmpls.Count > 0)
                    {
                        WriteLog("Info", "開始更新 EMPL 資料");

                        bool hasRow = false;
                        string emplSQL = string.Empty;
                        foreach (var item in dtEmplList.DtEmpls)
                        {
                            string value = string.Format(@"INSERT EMPL
(USERID ,PWD ,NAME ,COUNTRY ,DEPTNO ,JOBTITLE ,JOBTYPE ,ONDUTY ,EMAIL ,PRETEL ,TEL ,LASTTEL ,CELLPHONE ,LDATE ,LUSER ,ROLEID ,DELETED ,NEWDEPTNO ,NEWROLEID ,CHECKROLE,RADIOKIND1,RADIOKIND2,IDNO,MEMID,ALIVETIME,BLOCKTIME,ERRORTIMES)
VALUES
('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}','{21}','{22}','{23}','{24}','{25}',{26});
",
                                item.USERID, item.PWD, item.NAME, item.COUNTRY, item.DEPTNO, item.JOBTITLE, item.JOBTYPE, item.ONDUTY,
                                item.EMAIL, item.PRETEL, item.TEL, item.LASTTEL, item.CELLPHONE, item.LDATE, item.LUSER, item.ROLEID,
                                item.DELETED, item.NEWDEPTNO, item.NEWROLEID, item.CHECKROLE, item.RADIOKIND1, item.RADIOKIND2, item.IDNO, item.MEMID, item.ALIVETIME, item.BLOCKTIME, item.ERRORTIMES);

                            emplSQL += value;
                            hasRow = true;
                        }

                        if (hasRow)
                        {
                            string DateTimeDay = DateTime.Now.ToString("yyyyMMddHHmmss");
                            string BKsql = @"SELECT * into EMPL_" + DateTimeDay + " FROM DEPT";

                            ExecuteDeleteCommand(BKsql, conStrTo);

                            string deleteSQL = @"TRUNCATE TABLE EMPL;";
                            ExecuteDeleteCommand(deleteSQL, conStrTo);

                            emplSQL = emplSQL.Substring(0, emplSQL.Length - 1);
                            ExecuteInsertCommand(emplSQL, conStrTo);
                        }

                        WriteLog("Info", "結束更新 EMPL 資料");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error", "於資料庫 EMPL 儲存檔案時發生錯誤:" + ex.Message);
                return;
            }

            connectSuccess = true;
//            try
//            {
//                WriteLog("Info", "開始撈取 EMPL_MAPPING 資料");

//                string sql = @"SELECT COUNTRY ,USERID ,NDPPC_UID ,LDATE ,LUSER FROM EMPL_MAPPING";
//                DataTable dtEmplMap = ExecuteQuery(sql, conStrFrom);
//                if (dtEmplMap != null)
//                {
//                    dtEmplMapList.DtEmplMaps = new List<DtEmplMap>();
//                    foreach (DataRow row in dtEmplMap.Rows)
//                    {
//                        string COUNTRY = "";
//                        if (row["COUNTRY"] != DBNull.Value)
//                        {
//                            COUNTRY = row["COUNTRY"].ToString();
//                        }
//                        string USERID = "";
//                        if (row["USERID"] != DBNull.Value)
//                        {
//                            USERID = row["USERID"].ToString();
//                        }

//                        string NDPPC_UID = "";
//                        if (row["NDPPC_UID"] != DBNull.Value)
//                        {
//                            NDPPC_UID = row["NDPPC_UID"].ToString();
//                        }

//                        string LDATE = "";
//                        if (row["LDATE"] != DBNull.Value)
//                        {
//                            LDATE = row["LDATE"].ToString();
//                        }

//                        string LUSER = "";
//                        if (row["LUSER"] != DBNull.Value)
//                        {
//                            LUSER = row["LUSER"].ToString();
//                        }


//                        DtEmplMap emplMapPara = new DtEmplMap()
//                        {
//                            COUNTRY = COUNTRY,
//                            USERID = USERID,
//                            NDPPC_UID = NDPPC_UID,
//                            LDATE = LDATE,
//                            LUSER = LUSER,
//                        };
//                        dtEmplMapList.DtEmplMaps.Add(emplMapPara);
//                    }
//                }
//                else
//                {
//                    connectSuccess = false;
//                    WriteLog("Info", "撈取 EMPL_MAPPING 資料失敗: 無資料或執行失敗");
//                }

//                WriteLog("Info", "結束撈取 EMPL_MAPPING 資料");
//            }
//            catch (Exception ex)
//            {
//                WriteLog("Error", "從資料庫 EMPL_MAPPING 取得資料時發生錯誤:" + ex.Message);
//                connectSuccess = false;
//            }

//            try
//            {
//                if (connectSuccess)
//                {
//                    if (dtEmplMapList.DtEmplMaps.Count > 0)
//                    {
//                        WriteLog("Info", "開始更新 EMPL_MAPPING 資料");

//                        bool hasRow = false;
//                        string emplMapSQL = string.Empty;
//                        foreach (var item in dtEmplMapList.DtEmplMaps)
//                        {
//                            string value = string.Format(@"INSERT EMPL_MAPPING_forOracle
//(COUNTRY ,USERID ,NDPPC_UID ,LDATE ,LUSER)
//VALUES
//('{0}', '{1}', '{2}', '{3}', '{4}');
//",
//                                item.COUNTRY, item.USERID, item.NDPPC_UID, item.LDATE, item.LUSER);
//                            emplMapSQL += value;
//                            hasRow = true;
//                        }

//                        if (hasRow)
//                        {
//                            string deleteSQL = string.Format(@"DELETE FROM EMPL_MAPPING_forOracle WHERE COUNTRY = '{0}';", dtEmplMapList.City);
//                            ExecuteDeleteCommand(deleteSQL, conStrTo);

//                            emplMapSQL = emplMapSQL.Substring(0, emplMapSQL.Length - 1);
//                            ExecuteInsertCommand(emplMapSQL, conStrTo);
//                        }

//                        WriteLog("Info", "結束更新 EMPL_MAPPING 資料");
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                WriteLog("Error", "於資料庫 EMPL_MAPPING 儲存檔案時發生錯誤:" + ex.Message);
//                return;
//            }

            WriteLog("Info", "程式執行完畢");
        }

        /// <summary>
        /// 執行sql指令碼並回傳DataTable
        /// </summary>
        /// <param name="sql">查詢sql</param>
        /// <returns>查詢結果之DataTable</returns>
        public static DataTable ExecuteQuery(string sql, string connStr)
        {
            DataTable dt = new DataTable();

            try
            {
                //建立資料庫連線
                OracleConnection conn_oracle = new OracleConnection(connStr);

                //啟用連線
                conn_oracle.Open();

                //SQL查詢指令
                OracleCommand cmdOracle = new OracleCommand(sql, conn_oracle);

                //執行查詢指令 傳回 DataTable
                using (OracleDataAdapter dataAdapter = new OracleDataAdapter())
                {
                    dataAdapter.SelectCommand = cmdOracle;
                    dataAdapter.Fill(dt);
                }

                //關閉連線
                if (conn_oracle != null)
                {
                    conn_oracle.Close();
                    conn_oracle.Dispose();
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error", "ExecuteQuery()發生錯誤:" + ex.Message);
            }

            return dt;
        }

        /// <summary>
        /// 執行sql指令碼不回傳
        /// </summary>
        /// <param name="sql">新增sql</param>
        public static void ExecuteInsertCommand(string sql, string connStr)
        {
            WriteLog("Info", "sql :" + sql);
            try
            {
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
        public static void ExecuteDeleteCommand(string sql, string connStr)
        {
            try
            {
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
