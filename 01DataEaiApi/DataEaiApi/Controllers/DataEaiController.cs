using DataEaiApi.Models;
using Hackathon.Models;
using System.Web.Mvc;

namespace DataEaiApi.Controllers
{
    public class DataEaiController : Controller
    {
        // GET: DataEai
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SteDept(DtDeptList dtDeptList)
        {
            try
            {
                Process.WriteLog("Info", "SteDept開始");
                string fileName = "DEPT";
                string strResponse = Process.WriteDataFileDept(fileName, dtDeptList, out bool isSuccess);

                if (isSuccess)
                {
                    Process.WriteLog("Info", "SteDept結束：" + strResponse);
                    return Json(new ApiResult<string>(strResponse));
                }
                else
                {
                    Process.WriteLog("Info", "SteDept結束：" + strResponse);
                    return Json(new ApiError("0001", "錯誤訊息：" + strResponse));
                }
            }
            catch (System.Exception ex)
            {
                Process.WriteLog("Info", "SteDept結束，錯誤訊息：" + ex);
                return Json(new ApiError("0001", "錯誤訊息：" + ex));
            }
        }

        [HttpPost]
        public ActionResult SteEmpl(DtEmplList dtEmplList)
        {
            try
            {
                Process.WriteLog("Info", "SteEmpl開始");
                string fileName = "EMPL";
                string strResponse = Process.WriteDataFileEmpl(fileName, dtEmplList, out bool isSuccess);

                if (isSuccess)
                {
                    Process.WriteLog("Info", "SteEmpl結束：" + strResponse);
                    return Json(new ApiResult<string>(strResponse));
                }
                else
                {
                    Process.WriteLog("Info", "SteEmpl結束：" + strResponse);
                    return Json(new ApiError("0001", "錯誤訊息：" + strResponse));
                }
                
            }
            catch (System.Exception ex)
            {
                Process.WriteLog("Info", "SteEmpl結束，錯誤訊息：" + ex);
                return Json(new ApiError("0001", "錯誤訊息：" + ex));
            }
        }

        [HttpPost]
        public ActionResult SteEmplMapping(DtEmplMapList dtEmplMapList)
        {
            try
            {
                Process.WriteLog("Info", "SteEmplMapping開始");
                string fileName = "EMPL_MAPPING";
                string strResponse = Process.WriteDataFileEmplMap(fileName, dtEmplMapList, out bool isSuccess);

                if (isSuccess)
                {
                    Process.WriteLog("Info", "SteEmplMapping結束：" + strResponse);
                    return Json(new ApiResult<string>(strResponse));
                }
                else
                {
                    Process.WriteLog("Info", "SteEmplMapping結束：" + strResponse);
                    return Json(new ApiError("0001", "錯誤訊息：" + strResponse));
                }
            }
            catch (System.Exception ex)
            {
                Process.WriteLog("Info", "SteEmplMapping結束，錯誤訊息：" + ex);
                return Json(new ApiError("0001", "錯誤訊息：" + ex));
            }
        }
    }


}