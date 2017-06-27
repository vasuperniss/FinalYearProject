using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Models;
using DigitalIsraelFund_System.Filters;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using DigitalIsraelFund_System.DataBase;

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
            // get the params for searching
            string fname = Request.Params["fname"],
                lname = Request.Params["lname"],
                email = Request.Params["email"],
                office_name = Request.Params["office_name"];
            // get the params for page number and for ordering
            int pageNum, resultsPerPageNum;
            bool isDescBool = false;
            if (orderBy != "" && !bool.TryParse(isDesc, out isDescBool)) isDescBool = false;
            if (!int.TryParse(page, out pageNum)) pageNum = 1;
            if (!int.TryParse(resultsPerPage, out resultsPerPageNum)) resultsPerPageNum = 10;
            string isDescString = "";
            if (isDescBool) isDescString = " DESC";
            // set up the WHERE string with all searches
            string where = "type='momhee'";
            if (fname != null && !fname.Contains("'")) where += " and fname LIKE '%" + fname + "%'";
            if (lname != null && !lname.Contains("'")) where += " and lname LIKE '%" + lname + "%'";
            if (email != null && !email.Contains("'")) where += " and email LIKE '%" + email + "%'";
            if (office_name != null && !office_name.Contains("'")) where += " and office_name LIKE '%" + office_name + "%'";
            // load the table
            var table = UserManager.Manager.GetAllWhere(where, orderBy + isDescString, pageNum, resultsPerPageNum);
            // get the number of users
            var count = UserManager.Manager.Count(where);
            // add the offices into View Data
            ViewData["offices"] = OfficeManager.Manager.GetAll();
            return View(new TableResult { Table = table, NumPages = (int)System.Math.Ceiling((double)count / resultsPerPageNum),
                            isDesc = isDescBool, Page = pageNum, ResultsPerPage = resultsPerPageNum, OrderBy = orderBy });
        }

        [HttpPost]
        public JsonResult EditMomhee(string id, string fname, string lname, string email, string office, string phone, string cellPhone)
        {
            var vald = TypeValidator.Validator;
            // validate all fields
            if (!vald.Validate(id, "Integer") || !vald.Validate(fname, "Letters")
                || !vald.Validate(lname, "Letters") || !vald.Validate(email, "Email")
                || !vald.Validate(office, "Integer") || !vald.Validate(phone, "Phone")
                || !vald.Validate(cellPhone, "Phone"))
                return Json(new { Success = false, ErrMsg = "השדות לא בפורמט הנכון" }, JsonRequestBehavior.AllowGet);
            // attempt to edit the momhee
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
            // validate all fields and check if in the allowed fields
            if (searchVal.Contains("'") ||
                (searchBy != "fname" && searchBy != "lname"
                 && searchBy != "email" && searchBy != "office_name"))
                return Json(new { Success = false, ErrMsg = "השדות לא בפורמט הנכון" }, JsonRequestBehavior.AllowGet);
            // get the searched field options
            var listResult = UserManager.Manager.GetFieldWhere(searchBy, "type='momhee' and "
                + searchBy + " LIKE '%" + searchVal + "%'", searchBy, 1, 5);
            return Json(new { Success = true, List = listResult }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddNewMomhee()
        {
            // add offices to the adding of new momhim for picking an office
            ViewData["offices"] = OfficeManager.Manager.GetAll();
            return View();
        }

        [HttpPost]
        public JsonResult AddNewMomhee(string fname, string lname, string email, string password, string office, string phone, string cellPhone)
        {
            var vald = TypeValidator.Validator;
            // validate all fields
            if (!vald.Validate(fname, "Letters") || !vald.Validate(lname, "Letters")
                || !vald.Validate(email, "Email") || !vald.Validate(office, "Integer")
                || !vald.Validate(phone, "Phone") || !vald.Validate(cellPhone, "Phone"))
                return Json(new { Success = false, ErrMsg = "השדות לא בפורמט הנכון" }, JsonRequestBehavior.AllowGet);
            // attempt to mail the momhee
            var success = UserManager.Manager.Add(email, password, fname, lname, "momhee", office, phone, cellPhone);
            if (success)
            {
                var title = "צורפת למערכת של ישראל דיגיטלית";
                var body = "הסיסמה הראשונית שלך היא : " + password;
                EmailManager.Manager.SendMail(email, title, body);
            }
            // attempt to add the momhee to the data base
            return Json(new { Success = success },
                JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OfficesManage()
        {
            return View(new TableResult() { Table = OfficeManager.Manager.GetAll() });
        }

        [HttpPost]
        public JsonResult OfficesManage(string officeName)
        {
            var vald = TypeValidator.Validator;
            // validate all fields
            if (!vald.Validate(officeName, "Letters"))
                return Json(new { Success = false, ErrMsg = "השדה לא בפורמט הנכון" }, JsonRequestBehavior.AllowGet);
            // attempt to add the office to the data base
            return Json(new { Success = OfficeManager.Manager.Add(officeName) }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public void JoinMomheeAndRequest(string file_number, string momhee_id, string momhee_email)
        {
            var vald = TypeValidator.Validator;
            // validate all fields
            if (!vald.Validate(file_number, "Integer") || !vald.Validate(momhee_id, "Integer"))
                Response.Redirect("/Admin/RequestsManage");
            // connect the momhee to the request
            var newVals = new Dictionary<string, string>();
            newVals["momhee_id"] = momhee_id;
            RequestManager.Manager.Change(file_number, newVals);
            EmailManager.Manager.SendMail(momhee_email, "שובצת לבקשה חדשה", "התבצע שיבוץ שלך לבקשה מס' " + file_number);
            // redirect back to requests manage
            Response.Redirect("/AdminGovExp/RequestsManage");
        }

        [HttpGet]
        public ActionResult MadanMomhimManage(string page, string resultsPerPage, string orderBy, string isDesc)
        {
            // get the params for searching
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
            // get the params for page number and for ordering
            int pageNum, resultsPerPageNum;
            bool isDescBool;
            if (!bool.TryParse(isDesc, out isDescBool)) isDescBool = false;
            if (!int.TryParse(page, out pageNum)) pageNum = 1;
            if (!int.TryParse(resultsPerPage, out resultsPerPageNum)) resultsPerPageNum = 10;
            string isDescString = "";
            if (isDescBool) isDescString = " DESC";
            // set up the WHERE string with all searches
            string where = "true";
            if (file_number != null && !file_number.Contains("'")) where += " and file_number LIKE '%" + file_number + "%'";
            if (comp_name != null && !comp_name.Contains("'")) where += " and comp_name LIKE '%" + comp_name + "%'";
            if (comp_number != null && !comp_number.Contains("'")) where += " and comp_number LIKE '%" + comp_number + "%'";
            if (status != null && !status.Contains("'")) where += " and status LIKE '%" + status + "%'";
            if (status_date != null && !status_date.Contains("'")) where += " and status_date LIKE '%" + status_date + "%'";
            if (request_date != null && !request_date.Contains("'")) where += " and request_date LIKE '%" + request_date + "%'";
            if (name != null && !name.Contains("'")) where += " and name LIKE '%" + name + "%'";
            if (head_field != null && !head_field.Contains("'")) where += " and head_field LIKE '%" + head_field + "%'";
            if (tester_phone != null && !tester_phone.Contains("'")) where += " and tester_phone LIKE '%" + tester_phone + "%'";
            if (tester_email != null && !tester_email.Contains("'")) where += " and tester_email LIKE '%" + tester_email + "%'";
            if (phone != null && !phone.Contains("'")) where += " and phone LIKE '%" + phone + "%'";
            if (cellphone != null && !cellphone.Contains("'")) where += " and cellphone LIKE '%" + cellphone + "%'";
            // load the table and the number of rows
            var table = MadaanMomhimManager.Manager.GetAllWhere(where, orderBy + isDescString, pageNum, resultsPerPageNum);
            var count = MadaanMomhimManager.Manager.Count(where);
            return View(new TableResult { Table = table, NumPages = (int)System.Math.Ceiling((double)count / resultsPerPageNum),
                isDesc = isDescBool, Page = pageNum,  ResultsPerPage = resultsPerPageNum, OrderBy = orderBy });
        }

        [HttpPost]
        public JsonResult SearchMadanMonheeBy(string searchVal, string searchBy)
        {
            // validate all fields and check if in the allowed fields
            if (searchVal.Contains("'") ||
                (searchBy != "file_number" && searchBy != "comp_name" && searchBy != "comp_number"
                 && searchBy != "status" && searchBy != "status_date" && searchBy != "request_date"
                 && searchBy != "name" && searchBy != "head_field" && searchBy != "tester_phone"
                 && searchBy != "tester_email" && searchBy != "phone" && searchBy != "cellphone"))
                return Json(new { Success = false, ErrMsg = "השדות לא בפורמט הנכון" }, JsonRequestBehavior.AllowGet);
            // get the searched field options
            var listResult = MadaanMomhimManager.Manager.GetFieldWhere(searchBy, searchBy + " LIKE '%" + searchVal + "%'", searchBy, 1, 5);
            return Json(new { Success = true, List = listResult }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoadMadanMomhimTableFromExcel(HttpPostedFileBase file)
        {
            Models.Settings sett = Models.Settings.GetSettings();
            // save the excel file to app data
            string filePath = file.FileName;
            var saveTo = Server.MapPath("~/App_Data/Excels/" + filePath);
            file.SaveAs(saveTo);
            // read the excel
            var table = ExcelManager.Manager.LoadTableFromExcel(saveTo);
            // attempt to add or update the madaan momhim data base with the excel table
            MadaanMomhimManager.Manager.AddOrUpdate(table, sett);

            return RedirectToAction("MadanMomhimManage");
        }

        [HttpPost]
        public ActionResult LoadRequestsTableFromExcel(HttpPostedFileBase file)
        {
            Models.Settings sett = Models.Settings.GetSettings();
            // save the excel file to app data
            string filePath = file.FileName;
            var saveTo = Server.MapPath("~/App_Data/Excels/" + filePath);
            file.SaveAs(saveTo);
            // read the excel
            var table = ExcelManager.Manager.LoadTableFromExcel(saveTo);
            // attempt to add or update the requests data base with the excel table
            RequestManager.Manager.AddOrUpdate(table, sett);

            return RedirectToAction("RequestsManage", "AdminGovExp");
        }

        [HttpGet]
        public ActionResult OpenForChanges(string file_number, string form_ver)
        {
            // load the mashov
            var dataFile = Server.MapPath("~/App_Data/Mashovs/mashov_" + file_number + ".json");
            string json = System.IO.File.ReadAllText(@dataFile);

            // save the mashov as Temp
            dataFile = Server.MapPath("~/App_Data/Mashovs/temp_" + file_number + ".json");
            System.IO.File.WriteAllText(@dataFile, json);
            // link the mashov temp to the requests data base row of this request
            RequestManager.Manager.UpdateMashov(file_number, "temp_" + file_number, form_ver);

            return RedirectToAction("RequestsManage", "AdminGovExp");
        }
    }
}