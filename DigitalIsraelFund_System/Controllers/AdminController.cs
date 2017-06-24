using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Models;
using DigitalIsraelFund_System.Filters;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;

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

        [HttpPost]
        public JsonResult SearchMadanMonheeBy(string searchVal, string searchBy)
        {
            var table = MadaanMomhimManager.Manager.GetFieldWhere(searchBy, searchBy + " LIKE '%" + searchVal + "%'", searchBy, 1, 5);
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

        [HttpGet]
        public ActionResult Settings()
        {
            var dataFile = Server.MapPath("~/App_Data/Settings.json");
            string json = System.IO.File.ReadAllText(@dataFile);
            return View(Models.Settings.LoadJson(json));
        }

        [HttpPost]
        public ActionResult ChangeMashovVer(string version)
        {
            var dataFile = Server.MapPath("~/App_Data/Settings.json");
            string json = System.IO.File.ReadAllText(@dataFile);
            Models.Settings sett = Models.Settings.LoadJson(json);

            sett.MashovVersion = int.Parse(version);
            json = sett.GetJson();
            System.IO.File.WriteAllText(@dataFile, json);

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public ActionResult AddMashovFile(HttpPostedFileBase file)
        {
            var dataFile = Server.MapPath("~/App_Data/Settings.json");
            string json = System.IO.File.ReadAllText(@dataFile);
            Models.Settings sett = Models.Settings.LoadJson(json);

            var newVer = sett.PossibleMashovVersions[sett.PossibleMashovVersions.Count - 1] + 1;
            string filePath = "MashovForm_v_" + newVer + ".xml";
            var saveTo = Server.MapPath("~/App_Data/Forms/" + filePath);
            file.SaveAs(saveTo);

            sett.PossibleMashovVersions.Add(newVer);
            json = sett.GetJson();
            System.IO.File.WriteAllText(@dataFile, json);

            return RedirectToAction("Settings");
        }

        [HttpGet]
        public ActionResult MadanMomhimManage(string page, string resultsPerPage, string orderBy, string isDesc)
        {
            string file_number = Request.Params["file_number"],
                comp_number = Request.Params["comp_number"],
                comp_name = Request.Params["comp_name"],
                status = Request.Params["status"],
                status_date = Request.Params["status_date"],
                request_date = Request.Params["request_date"],
                name = Request.Params["name"],
                head_field = Request.Params["head_field"],
                tester_phone = Request.Params["tester_phone"],
                tester_email = Request.Params["tester_email"],
                phone = Request.Params["phone"],
                cellphone = Request.Params["cellphone"];

            int pageNum, resultsPerPageNum;
            bool isDescBool;
            if (!bool.TryParse(isDesc, out isDescBool)) isDescBool = false;
            if (!int.TryParse(page, out pageNum)) pageNum = 1;
            if (!int.TryParse(resultsPerPage, out resultsPerPageNum)) resultsPerPageNum = 10;
            string isDescString = "";
            if (isDescBool) isDescString = " DESC";
            string where = "true";
            where += " and file_number LIKE '%" + file_number + "%'";
            where += " and comp_name LIKE '%" + comp_name + "%'";
            where += " and comp_number LIKE '%" + comp_number + "%'";
            where += " and status LIKE '%" + status + "%'";
            where += " and status_date LIKE '%" + status_date + "%'";
            where += " and request_date LIKE '%" + request_date + "%'";
            where += " and name LIKE '%" + name + "%'";
            where += " and head_field LIKE '%" + head_field + "%'";
            where += " and tester_phone LIKE '%" + tester_phone + "%'";
            where += " and tester_email LIKE '%" + tester_email + "%'";
            where += " and phone LIKE '%" + phone + "%'";
            where += " and cellphone LIKE '%" + cellphone + "%'";

            var table = MadaanMomhimManager.Manager.GetAllWhere(where, orderBy + isDescString, pageNum, resultsPerPageNum);
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
        public ActionResult LoadMadanMomhimTableFromExcel(HttpPostedFileBase file2)
        {
            var dataFile = Server.MapPath("~/App_Data/Settings.json");
            string json = System.IO.File.ReadAllText(@dataFile);
            Models.Settings sett = Models.Settings.LoadJson(json);

            string filePath = file2.FileName;
            var saveTo = Server.MapPath("~/App_Data/Excels/" + filePath);
            file2.SaveAs(saveTo);

            var table = ExcelManager.Manager.LoadTableFromExcel(saveTo);
            MadaanMomhimManager.Manager.AddOrUpdate(table, sett);

            return RedirectToAction("MadanMomhimManage");
        }
    }
}