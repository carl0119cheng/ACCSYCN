using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DataEaiApi.Models
{
    public class Process
    {
        public static string WriteDataFileDept(string fileName, DtDeptList dtDeptList, out bool isSuccess)
        {
            isSuccess = false;
            string strRe;
            try
            {
                WriteLog("Info", "Dept 資料開始解析");
                if (dtDeptList != null || dtDeptList.DtDepts.Count == 0)
                {
                    // web.config參數，接收資料的檔案儲存路徑
                    string fileDir = Path.Combine(ConfigurationManager.AppSettings["DataFilePath"].ToString(), fileName);

                    try
                    {
                        if (!Directory.Exists(fileDir))
                        {
                            Directory.CreateDirectory(fileDir);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog("Error", "Dept 錯誤訊息：" + ex.ToString());
                        strRe = "Dept 建立資料夾失敗：" + ex;
                        return strRe;
                    }

                    StringBuilder filePathSb = new StringBuilder();
                    filePathSb.Append(fileDir);
                    filePathSb.Append("/");
                    filePathSb.Append(fileName);
                    filePathSb.Append("-");
                    filePathSb.Append(dtDeptList.City);
                    filePathSb.Append(".txt");
                    string filePath = filePathSb.ToString();
                    if (System.IO.File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    StreamWriter sw = File.AppendText(filePath);
                    foreach (DtDept dt in dtDeptList.DtDepts)
                    {
                        StringBuilder dtEmplStr = new StringBuilder();
                        foreach (var prop in dt.GetType().GetProperties())
                        {
                            if (prop.GetValue(dt, null) != null)
                            {
                                dtEmplStr.Append(prop.GetValue(dt, null));
                            }           
                            dtEmplStr.Append("\t");
                        }

                        if (dtEmplStr.Length > 0)
                        {
                            sw.WriteLine(dtEmplStr);
                        }
                    }

                    sw.Flush();
                    sw.Close();
                    sw.Dispose();

                    isSuccess = true;
                    strRe = "Dept 資料成功寫入";
                }
                else
                {
                    strRe = "Dept 沒有資料可以寫入";
                    WriteLog("Info", "Dept 沒有資料可以寫入");
                }

                WriteLog("Info", "Dept 資料解析結束");
            }
            catch (Exception ex)
            {
                WriteLog("Error", "Dept 錯誤訊息：" + ex.ToString());
                strRe = "Dept 資料寫入失敗：" + ex;
            }
            return strRe;
        }

        public static string WriteDataFileEmpl(string fileName, DtEmplList dtEmplList, out bool isSuccess)
        {
            isSuccess = false;
            string strRe;
            try
            {
                WriteLog("Info", "Empl 資料開始解析");
                if (dtEmplList != null || dtEmplList.DtEmpls.Count == 0)
                {
                    // web.config參數，接收資料的檔案儲存路徑
                    string fileDir = Path.Combine(ConfigurationManager.AppSettings["DataFilePath"].ToString(), fileName);

                    try
                    {
                        if (!Directory.Exists(fileDir))
                        {
                            Directory.CreateDirectory(fileDir);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog("Error", "Empl 錯誤訊息：" + ex.ToString());
                        strRe = "Empl 建立資料夾失敗：" + ex;
                        return strRe;
                    }

                    StringBuilder filePathSb = new StringBuilder();
                    filePathSb.Append(fileDir);
                    filePathSb.Append("/");
                    filePathSb.Append(fileName);
                    filePathSb.Append("-");
                    filePathSb.Append(dtEmplList.City);
                    filePathSb.Append(".txt");
                    string filePath = filePathSb.ToString();
                    if (System.IO.File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    StreamWriter sw = File.AppendText(filePath);
                    foreach (DtEmpl dt in dtEmplList.DtEmpls)
                    {
                        StringBuilder dtEmplStr = new StringBuilder();
                        foreach (var prop in dt.GetType().GetProperties())
                        {
                            if (prop.GetValue(dt, null) != null)
                            {
                                dtEmplStr.Append(prop.GetValue(dt, null));
                            }
                            dtEmplStr.Append("\t");
                        }

                        if (dtEmplStr.Length > 0)
                        {
                            sw.WriteLine(dtEmplStr);
                        }
                    }

                    sw.Flush();
                    sw.Close();
                    sw.Dispose();

                    isSuccess = true;
                    strRe = "Empl 資料成功寫入";
                }
                else
                {
                    strRe = "Empl 沒有資料可以寫入";
                }

                WriteLog("Info", "Empl 資料解析結束");
            }
            catch (Exception ex)
            {
                WriteLog("Error", "Empl 錯誤訊息：" + ex.ToString());
                strRe = "Empl 資料寫入失敗：" + ex;
            }
            return strRe;
        }

        public static string WriteDataFileEmplMap(string fileName, DtEmplMapList dtEmplMapList, out bool isSuccess)
        {
            isSuccess = false;
            string strRe;
            try
            {
                WriteLog("Info", "EmplMap 資料開始解析");
                if (dtEmplMapList != null || dtEmplMapList.DtEmplMaps.Count == 0)
                {
                    // web.config參數，接收資料的檔案儲存路徑
                    string fileDir = Path.Combine(ConfigurationManager.AppSettings["DataFilePath"].ToString(), fileName);

                    try
                    {
                        if (!Directory.Exists(fileDir))
                        {
                            Directory.CreateDirectory(fileDir);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog("Error", "EmplMap 錯誤訊息：" + ex.ToString());
                        strRe = "EmplMap 建立資料夾失敗：" + ex;
                        return strRe;
                    }

                    StringBuilder filePathSb = new StringBuilder();
                    filePathSb.Append(fileDir);
                    filePathSb.Append("/");
                    filePathSb.Append(fileName);
                    filePathSb.Append("-");
                    filePathSb.Append(dtEmplMapList.City);
                    filePathSb.Append(".txt");
                    string filePath = filePathSb.ToString();
                    if (System.IO.File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    StreamWriter sw = File.AppendText(filePath);
                    foreach (DtEmplMap dt in dtEmplMapList.DtEmplMaps)
                    {
                        StringBuilder dtEmplStr = new StringBuilder();
                        foreach (var prop in dt.GetType().GetProperties())
                        {
                            if (prop.GetValue(dt, null) != null)
                            {
                                dtEmplStr.Append(prop.GetValue(dt, null));
                            }
                            dtEmplStr.Append("\t");
                        }

                        if (dtEmplStr.Length > 0)
                        {
                            sw.WriteLine(dtEmplStr);
                        }
                    }

                    sw.Flush();
                    sw.Close();
                    sw.Dispose();

                    isSuccess = true;
                    strRe = "EmplMap 資料成功寫入";
                }
                else
                {
                    strRe = "EmplMap 沒有資料可以寫入";
                    WriteLog("Info", "EmplMap 沒有資料可以寫入");
                }

                WriteLog("Info", "EmplMap 資料解析結束");
            }
            catch (Exception ex)
            {
                WriteLog("Error", "EmplMap 錯誤訊息：" + ex.ToString());
                strRe = "EmplMap 資料寫入失敗：" + ex;
            }
            return strRe;
        }

        public static void WriteLog(string LogType, string LogMess)
        {
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            string fileDir = ConfigurationManager.AppSettings["LogPath"].ToString();
            string filePath = fileDir + fileName;
            string strLog = LogType + "  " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "  " + LogMess;

            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            StreamWriter sw = File.AppendText(filePath);
            sw.WriteLine(strLog);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }
}