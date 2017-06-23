using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Models;
using DigitalIsraelFund_System.Filters;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
    [AdminFilter]
    public class AdminController : Controller
    {

        [HttpPost]
        public JsonResult PageUpdate(string page, string title, string html)
        {
            this.SavePage(page, new StaticPage() { Title = title, HtmlContent = html });
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        private void SavePage(string filename, StaticPage page)
        {
            var dataFile = Server.MapPath("~/App_Data/Pages/" + filename + ".json");
            string json = page.GetJson();
            System.IO.File.WriteAllText(@dataFile, json);
        }

        [HttpGet]
        public ActionResult MomhimManage(string page, string resultsPerPage, string orderBy, string isDesc)
        {
            string fname = Request.Params["fname"],
                lname = Request.Params["lname"],
                email = Request.Params["email"],
                office_name = Request.Params["office_name"];
            int pageNum, resultsPerPageNum;
            bool isDescBool = false;
            if (orderBy != "" && !bool.TryParse(isDesc, out isDescBool)) isDescBool = false;
            if (!int.TryParse(page, out pageNum)) pageNum = 1;
            if (!int.TryParse(resultsPerPage, out resultsPerPageNum)) resultsPerPageNum = 10;
            string isDescString = "";
            if (isDescBool) isDescString = " DESC";
            string where = "type='momhee'";
            if (fname != null) where += " and fname LIKE '%" + fname + "%'";
            if (lname != null) where += " and lname LIKE '%" + lname + "%'";
            if (email != null) where += " and email LIKE '%" + email + "%'";
            if (office_name != null) where += " and office_name LIKE '%" + office_name + "%'";

            var table = UserManager.Manager.GetAllWhere(where, orderBy + isDescString, pageNum, resultsPerPageNum);
            var count = UserManager.Manager.Count("type='momhee'");
            ViewData["offices"] = OfficeManager.Manager.GetAll();
            return View(new TableResult { Table = table, NumPages = (int)System.Math.Ceiling((double)count / resultsPerPageNum),
                            isDesc = isDescBool, Page = pageNum,
                            ResultsPerPage = resultsPerPageNum, OrderBy = orderBy });
        }

        [HttpPost]
        public JsonResult EditMomhee(string id, string fname, string lname, string email, string office, string phone, string cellPhone)
        {
            Dictionary<string, string> newValues = new Dictionary<string, string>();
            newValues["fname"] = fname;
            newValues["lname"] = lname;
            newValues["email"] = email;
            newValues["office"] = office;
            newValues["phone"] = phone;
            newValues["cell_phone"] = cellPhone;
            return Json(new { Success = UserManager.Manager.Change(id, newValues), ErrMsg = "נכשל." },
                JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SearchUserBy(string searchVal, string searchBy)
        {
            var table = UserManager.Manager.GetFieldWhere(searchBy, "type='momhee' and "
                + searchBy + " LIKE '%" + searchVal + "%'", searchBy, 1, 5);
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

        [HttpGet]
        public ActionResult AddNewMomhee()
        {
            ViewData["offices"] = OfficeManager.Manager.GetAll();
            return View();
        }

        [HttpPost]
        public ActionResult AddNewMomhee(string fname, string lname, string email, string password, string office, string phone, string cellPhone)
        {
            UserManager.Manager.Add(email, password, fname, lname, "momhee", office, phone, cellPhone);
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OfficesManage()
        {
            return View(new TableResult() { Table = OfficeManager.Manager.GetAll() });
        }

        [HttpPost]
        public JsonResult OfficesManage(string officeName)
        {
            return Json(new { Success = OfficeManager.Manager.Add(officeName) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult RequestsManage(string page, string resultsPerPage, string orderBy, string isDesc)
        {
            UserData user = (UserData)this.Session["user"];
            string file_number = Request.Params["file_number"],
                comp_name = Request.Params["comp_name"],
                status = Request.Params["status"],
                momhee_name = Request.Params["momhee_name"],
                madaan_momhee = Request.Params["madaan_momhe"],
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

        [HttpGet]
        public void JoinMomheeAndRequest(string file_number, string momhee_id)
        {
            var newVals = new Dictionary<string, string>();
            newVals["momhee_id"] = momhee_id;
            RequestManager.Manager.Change(file_number, newVals);
            Response.Redirect("/Admin/RequestsManage");
        }
    }
}