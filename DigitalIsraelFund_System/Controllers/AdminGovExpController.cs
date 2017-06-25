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
            UserData user = (UserData)this.Session["user"];
            string file_number = Request.Params["file_number"],
                comp_name = Request.Params["comp_name"],
                status = Request.Params["status"],
                momhee_name = Request.Params["momhee_name"],
                madaan_momhee = Request.Params["madaan_momhee"],
                submiter_name = Request.Params["submiter_name"],
                mashov = Request.Params["mashov"];
            int pageNum, resultsPerPageNum;
            bool isDescBool;
            if (!bool.TryParse(isDesc, out isDescBool)) isDescBool = false;
            if (!int.TryParse(page, out pageNum)) pageNum = 1;
            if (!int.TryParse(resultsPerPage, out resultsPerPageNum)) resultsPerPageNum = 10;
            string isDescString = "";
            if (isDescBool) isDescString = " DESC";
            string where = "true";
            if (user.Type.ToLower() == "momhee")
                where = "momhee_id='" + user.Id + "'";
            where += " and file_number LIKE '%" + file_number + "%'";
            where += " and comp_name LIKE '%" + comp_name + "%'";
            where += " and status LIKE '%" + status + "%'";
            if (user.Type.ToLower() == "admin")
                where += " and CONCAT_WS(' ', fname, lname) LIKE '%" + momhee_name + "%'";
            where += " and madaan_momhee LIKE '%" + madaan_momhee + "%'";
            where += " and submiter_name LIKE '%" + submiter_name + "%'";
            where += " and mashov LIKE '%" + mashov + "%'";

            var table = RequestManager.Manager.GetAllWhere(where, orderBy + isDescString, pageNum, resultsPerPageNum);
            var count = RequestManager.Manager.Count("");
            return View(new TableResult
            {
                Table = table,
                NumPages = (int)System.Math.Ceiling((double)count / resultsPerPageNum),
                isDesc = isDescBool,
                Page = pageNum,
                ResultsPerPage = resultsPerPageNum,
                OrderBy = orderBy
            });
        }

        [HttpPost]
        public JsonResult SearchRequestBy(string searchVal, string searchBy)
        {
            if (searchBy == "momhee_name") searchBy = "CONCAT_WS(' ', fname, lname)";
            var table = RequestManager.Manager.GetFieldWhere(searchBy, searchBy + " LIKE '%" + searchVal + "%'", searchBy, 1, 5);
            List<string> results = new List<string>();
            if (table != null)
            {
                foreach (Dictionary<string, string> row in table)
                {
                    results.Add(row[searchBy]);
                }
            }
            return Json(new { Success = true, List = results }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetFilesForRequest(string file_number)
        {
            var table = RequestManager.Manager.GetFilesForRequest(file_number);
            List<string> results = new List<string>();
            if (table != null)
            {
                foreach (Dictionary<string, string> row in table)
                {
                    results.Add(row["path"]);
                }
            }
            return Json(new { Success = true, List = results }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public FileResult DownloadFile(string fileN, string file)
        {
            var filename = fileN + "_" + file;
            return File(Server.MapPath("~/App_Data/RequestFiles/" + filename), System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        [HttpGet]
        public ActionResult ViewMashov(string file_number, string form_ver)
        {
            var dataFile = Server.MapPath("~/App_Data/Mashovs/mashov_" + file_number + ".json");
            string json = System.IO.File.ReadAllText(@dataFile);
            Dictionary<string, string> values = FormValues.LoadJson(json).Values;

            var mashovFile = Server.MapPath("~/App_Data/Forms/MashovForm_v_" + form_ver + ".xml");
            PostedForm pR = new PostedForm(FormManager.Manager.Load(mashovFile).FormComponents[0], values);

            string where = "file_number='" + file_number + "'";
            ViewData["request"] = RequestManager.Manager.GetAllWhere(where, null, 1, 1)[0];
            ViewData["names"] = RequestManager.Manager.GetAllColNames();

            return View("../Home/PostedForm", pR);
        }
    }
}