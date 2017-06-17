using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
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
        public ActionResult MomhimManage(string page, string resultsPerPage, string orderBy, string isDesc,
            string fname , string lname, string email, string office_name)
        {
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
            int pageNum, resultsPerPageNum;
            bool isDescBool;
            if (!bool.TryParse(isDesc, out isDescBool)) isDescBool = false;
            if (!int.TryParse(page, out pageNum)) pageNum = 1;
            if (!int.TryParse(resultsPerPage, out resultsPerPageNum)) resultsPerPageNum = 10;
            string isDescString = "";
            if (isDescBool) isDescString = " DESC";
            string where = "";
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