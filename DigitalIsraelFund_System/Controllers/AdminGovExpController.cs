using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Filters;
using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
    [AdminGovExpFilter]
    public class AdminGovExpController : Controller
    {
        [HttpGet]
        public ActionResult RequestsManage(string page, string resultsPerPage, string orderBy, string isDesc)
        {
            // get the user
            UserData user = (UserData)this.Session["user"];
            // get the params for searching
            string file_number = Request.Params["file_number"],
                comp_name = Request.Params["comp_name"],
                status = Request.Params["status"],
                momhee_name = Request.Params["momhee_name"],
                madaan_momhee = Request.Params["madaan_momhee"],
                submiter_name = Request.Params["submiter_name"],
                mashov = Request.Params["mashov"];
            // get the params for page number and for ordering
            int pageNum, resultsPerPageNum;
            bool isDescBool;
            if (!bool.TryParse(isDesc, out isDescBool)) isDescBool = false;
            if (!int.TryParse(page, out pageNum)) pageNum = 1;
            if (!int.TryParse(resultsPerPage, out resultsPerPageNum)) resultsPerPageNum = 10;
            string isDescString = "";
            if (isDescBool) isDescString = " DESC";
            string where = "true";
            // set up the WHERE string with all searches
            if (user.Type.ToLower() == "momhee")
                where = "momhee_id='" + user.Id + "'";
            if (file_number != null && !file_number.Contains("'")) where += " and file_number LIKE '%" + file_number + "%'";
            if (comp_name != null && !comp_name.Contains("'")) where += " and comp_name LIKE '%" + comp_name + "%'";
            if (status != null && !status.Contains("'")) where += " and status LIKE '%" + status + "%'";
            if (user.Type.ToLower() == "admin")
                if (momhee_name != null && !momhee_name.Contains("'")) where += " and CONCAT_WS(' ', fname, lname) LIKE '%" + momhee_name + "%'";
            if (madaan_momhee != null && !madaan_momhee.Contains("'")) where += " and madaan_momhee LIKE '%" + madaan_momhee + "%'";
            if (submiter_name != null && !submiter_name.Contains("'")) where += " and submiter_name LIKE '%" + submiter_name + "%'";
            if (mashov != null && !mashov.Contains("'")) where += " and mashov LIKE '%" + mashov + "%'";

            // load the table and number of requests
            var table = RequestManager.Manager.GetAllWhere(where, orderBy + isDescString, pageNum, resultsPerPageNum);
            var count = RequestManager.Manager.Count(where);
            return View(new TableResult { Table = table, NumPages = (int)System.Math.Ceiling((double)count / resultsPerPageNum),
                isDesc = isDescBool, Page = pageNum, ResultsPerPage = resultsPerPageNum, OrderBy = orderBy });
        }

        [HttpPost]
        public JsonResult SearchRequestBy(string searchVal, string searchBy)
        {
            // get the user
            UserData user = (UserData)this.Session["user"];
            // validate all fields and check if in the allowed fields
            if (searchVal.Contains("'") ||
                (searchBy != "momhee_name" && searchBy != "file_number"
                 && searchBy != "comp_name" && searchBy != "status"
                 && searchBy != "madaan_momhee" && searchBy != "submiter_name"
                 && searchBy != "mashov") || 
                 (searchBy == "momhee_name" && user.Type.ToLower() != "admin"))
                return Json(new { Success = false, ErrMsg = "השדות לא בפורמט הנכון" }, JsonRequestBehavior.AllowGet);
            if (searchBy == "momhee_name") searchBy = "CONCAT_WS(' ', fname, lname)";
            // if momhee, give only results related to him
            var where = user.Type.ToLower() == "momhee" ? "momhee_id='" + user.Id + "' and " : "";
            // get the searched field options
            var listResult = RequestManager.Manager.GetFieldWhere(searchBy, where + searchBy + " LIKE '%" + searchVal + "%'", searchBy, 1, 5);
            return Json(new { Success = true, List = listResult }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFilesForRequest(string file_number)
        {
            // get the user
            UserData user = (UserData)this.Session["user"];
            if (user.Type.ToLower() == "momhee")
                // check if the momhee has excess to this request
                if (!RequestManager.Manager.IsRequestAllowedForMomhee(user.Id, file_number))
                    return Json(new { Success = false, ErrMsg = "פעולה לא חוקית" }, JsonRequestBehavior.AllowGet);
            // load the files of the request
            var listResult = RequestManager.Manager.GetFilesForRequest(file_number);
            return Json(new { Success = true, List = listResult }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult DownloadFile(string fileN, string file)
        {
            // get the user
            UserData user = (UserData)this.Session["user"];
            if (user.Type.ToLower() == "momhee")
                // check if the momhee has excess to this request
                if (!RequestManager.Manager.IsRequestAllowedForMomhee(user.Id, fileN))
                    return null;
            var filename = fileN + "_" + file;
            return File(Server.MapPath("~/App_Data/RequestFiles/" + filename), System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        [HttpGet]
        public ActionResult ViewMashov(string file_number, string form_ver)
        {
            // get the user
            UserData user = (UserData)this.Session["user"];
            if (user.Type.ToLower() == "momhee")
                // check if the momhee has excess to this request
                if (!RequestManager.Manager.IsRequestAllowedForMomhee(user.Id, file_number))
                    return RedirectToAction("RequestsManage", "AdminGovExp");
            // load the mashov
            var dataFile = Server.MapPath("~/App_Data/Mashovs/mashov_" + file_number + ".json");
            string json = System.IO.File.ReadAllText(@dataFile);
            Dictionary<string, string> values = FormValues.LoadJson(json).Values;
            // load the form of the mashov
            var mashovFile = Server.MapPath("~/App_Data/Forms/MashovForm_v_" + form_ver + ".xml");
            PostedForm pR = new PostedForm(FormManager.Manager.Load(mashovFile).FormComponents[0], values);
            // load extra data of the request
            string where = "file_number='" + file_number + "'";
            ViewData["request"] = RequestManager.Manager.GetAllWhere(where, null, 1, 1)[0];
            ViewData["names"] = RequestManager.Manager.GetAllColNames();

            return View(pR);
        }
    }
}