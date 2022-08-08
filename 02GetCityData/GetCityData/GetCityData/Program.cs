using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Text;

namespace GetCityData
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GetData();
        }

        public static void GetData()
        {
            WriteLog("Info", "程式開始執行");

            bool connectSuccess = true;
            DtDeptList dtDeptList = new DtDeptList();
            dtDeptList.City = ConfigurationManager.AppSettings["City"];

            DtEmplList dtEmplList = new DtEmplList();
            dtEmplList.City = ConfigurationManager.AppSettings["City"];

            DtEmplMapList dtEmplMapList = new DtEmplMapList();
            dtEmplMapList.City = ConfigurationManager.AppSettings["City"];

            try
            {
                string sql = @"SELECT DEPTNO ,DEPTNAME ,INDEPTNO ,GRADE ,ADDR ,X ,Y ,LDATE ,LUSER ,DELETED FROM DEPT";
                DataTable dtDept = ExecuteQuery(sql);
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
                        //string SORTING = "";
                        //if (row["SORTING"] != DBNull.Value)
                        //{
                        //    SORTING = row["SORTING"].ToString();
                        //}

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
                            DELETED = DELETED
                            //,SORTING = SORTING,
                            //ISSAVED = ISSAVED
                        };
                        dtDeptList.DtDepts.Add(deptPara);
                    }

                }
                else
                {
                    connectSuccess = false;
                }
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
                    if (dtDeptList.DtDepts != null && dtDeptList.DtDepts.Count > 0)
                    {
                        string webAPIUrl = Path.Combine(ConfigurationManager.AppSettings["WebAPIUrl"], "SteDept");
                        string sendData = JsonConvert.SerializeObject(dtDeptList);
                        HttpWebRequest httpRquest = (HttpWebRequest)WebRequest.Create(webAPIUrl);
                        httpRquest.Method = "POST";
                        httpRquest.ContentType = "application/json;charset=utf-8";
                        byte[] dataArray = Encoding.UTF8.GetBytes(sendData);
                        Stream requestStream = null;
                        if (!string.IsNullOrEmpty(sendData))
                        {
                            requestStream = httpRquest.GetRequestStream();
                            requestStream.Write(dataArray, 0, dataArray.Length);
                            requestStream.Close();
                        }

                        HttpWebResponse response = (HttpWebResponse)httpRquest.GetResponse();
                        string strStatusCode = response.StatusCode.ToString();

                        if (strStatusCode == "OK")
                        {
                            WriteLog("Info", "資料庫 DEPT 資料已傳送");
                        }
                        else
                        {
                            WriteLog("Error", "從資料庫 DEPT 取得資料時發生錯誤:" + response.ToString());
                        }
                        response.Close();
                    }
                    else
                    {
                        WriteLog("Info", "資料庫 DEPT 沒有資料");
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
                string sql = @"SELECT USERID ,PWD ,NAME ,COUNTRY ,DEPTNO ,JOBTITLE ,JOBTYPE ,
ONDUTY ,EMAIL ,PRETEL ,TEL ,LASTTEL ,CELLPHONE ,LDATE ,LUSER ,
ROLEID ,DELETED ,NEWDEPTNO ,NEWROLEID ,CHECKROLE ,MEMID FROM EMPL ";
                DataTable dtEmpl = ExecuteQuery(sql);
                if (dtEmpl != null)
                {
                    dtEmplList.DtEmpls = new System.Collections.Generic.List<DtEmpl>();
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
                            //IDNO = IDNO,
                            //IDNOKEY = IDNOKEY,
                            MEMID = MEMID
                        };
                        dtEmplList.DtEmpls.Add(emplPara);
                    }
                }
                else
                {
                    connectSuccess = false;
                }
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
                    WriteLog("Info", "EMPL開始建立傳送通道");
                    if (dtEmplList.DtEmpls != null && dtEmplList.DtEmpls.Count > 0)
                    {
                        string webAPIUrl = Path.Combine(ConfigurationManager.AppSettings["WebAPIUrl"], "SteEmpl");
                        WriteLog("Info", "webAPIUrl : " + webAPIUrl.ToString());
                        string sendData = JsonConvert.SerializeObject(dtEmplList);
                        HttpWebRequest httpRquest = (HttpWebRequest)WebRequest.Create(webAPIUrl);
                        int Timeouts = 300 * 1000;
                        httpRquest.Timeout = Timeouts;
                        httpRquest.Method = "POST";
                        httpRquest.ContentType = "application/json;charset=utf-8";
                        byte[] dataArray = Encoding.UTF8.GetBytes(sendData);
                        Stream requestStream = null;
                        if (!string.IsNullOrEmpty(sendData))
                        {
                            requestStream = httpRquest.GetRequestStream();
                            requestStream.Write(dataArray, 0, dataArray.Length);
                            requestStream.Close();
                        }
                        WriteLog("info", "EMPL開始建立傳送");
                        HttpWebResponse response = (HttpWebResponse)httpRquest.GetResponse();
                        string strStatusCode = response.StatusCode.ToString();
                        WriteLog("info", "EMPL傳送結果："+ strStatusCode.ToString());
                        if (strStatusCode == "OK")
                        {
                            WriteLog("Info", "資料庫 Empl 資料已傳送");
                        }
                        else
                        {
                            WriteLog("Error", "從資料庫 Empl 取得資料時發生錯誤:" + response.ToString());
                        }
                        response.Close();
                    }
                    else
                    {
                        WriteLog("Info", "資料庫 Empl 沒有資料");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error", "於資料庫 Empl 儲存檔案時發生錯誤:" + ex.Message);
                return;
            }

            connectSuccess = true;
            try
            {
                string sql = @"SELECT COUNTRY ,USERID ,NDPPC_UID ,LDATE ,LUSER FROM EMPL_MAPPING";
                DataTable dtEmplMap = ExecuteQuery(sql);
                if (dtEmplMap != null)
                {
                    dtEmplMapList.DtEmplMaps = new System.Collections.Generic.List<DtEmplMap>();
                    foreach (DataRow row in dtEmplMap.Rows)
                    {
                        string COUNTRY = "";
                        if (row["COUNTRY"] != DBNull.Value)
                        {
                            COUNTRY = row["COUNTRY"].ToString();
                        }
                        string USERID = "";
                        if (row["USERID"] != DBNull.Value)
                        {
                            USERID = row["USERID"].ToString();
                        }

                        string NDPPC_UID = "";
                        if (row["NDPPC_UID"] != DBNull.Value)
                        {
                            NDPPC_UID = row["NDPPC_UID"].ToString();
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


                        DtEmplMap emplMapPara = new DtEmplMap()
                        {
                            COUNTRY = COUNTRY,
                            USERID = USERID,
                            NDPPC_UID = NDPPC_UID,
                            LDATE = LDATE,
                            LUSER = LUSER,                       
                        };
                        dtEmplMapList.DtEmplMaps.Add(emplMapPara);
                    }
                }
                else
                {
                    connectSuccess = false;
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error", "從資料庫 EMPL_MAPPING 取得資料時發生錯誤:" + ex.Message);
                connectSuccess = false;
            }

            try
            {
                if (connectSuccess)
                {
                    if (dtEmplMapList.DtEmplMaps != null && dtEmplMapList.DtEmplMaps.Count > 0)
                    {
                        string webAPIUrl = Path.Combine(ConfigurationManager.AppSettings["WebAPIUrl"], "SteEmplMapping");
                        string sendData = JsonConvert.SerializeObject(dtEmplMapList);
                        HttpWebRequest httpRquest = (HttpWebRequest)WebRequest.Create(webAPIUrl);
                        httpRquest.Method = "POST";
                        httpRquest.ContentType = "application/json;charset=utf-8";
                        byte[] dataArray = Encoding.UTF8.GetBytes(sendData);
                        Stream requestStream = null;
                        if (!string.IsNullOrEmpty(sendData))
                        {
                            requestStream = httpRquest.GetRequestStream();
                            requestStream.Write(dataArray, 0, dataArray.Length);
                            requestStream.Close();
                        }

                        HttpWebResponse response = (HttpWebResponse)httpRquest.GetResponse();
                        string strStatusCode = response.StatusCode.ToString();

                        if (strStatusCode == "OK")
                        {
                            WriteLog("Info", "資料庫 EMPL_MAPPING 資料已傳送");
                        }
                        else
                        {
                            WriteLog("Error", "從資料庫 EMPL_MAPPING 取得資料時發生錯誤:" + response.ToString());
                        }
                        response.Close();
                    }
                    else
                    {
                        WriteLog("Info", "資料庫 EMPL_MAPPING 沒有資料");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error", "於資料庫 Empl 儲存檔案時發生錯誤:" + ex.Message);
                return;
            }

            WriteLog("Info", "程式執行完畢");
        }

        /// <summary>
        /// 執行sql指令碼並回傳DataTable
        /// </summary>
        /// <param name="sql">查詢sql</param>
        /// <returns>查詢結果之DataTable</returns>
        public static DataTable ExecuteQuery(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                //資料庫連線字串
                string connStr = ConfigurationManager.ConnectionStrings["conn"].ConnectionString;

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
