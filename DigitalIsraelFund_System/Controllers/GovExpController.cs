using DigitalIsraelFund_System.DataBase;
using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Filters;
using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
    [GovExpFilter]
    public class GovExpController : Controller
    {

        [HttpGet]
        public ActionResult EditSelf()
        {
            ViewData["offices"] = OfficeManager.Manager.GetAll();
            return View((UserData)this.Session["user"]);
        }

        [HttpGet]
        public ActionResult AddMashov(string file_number, string isContinue, string form_ver)
        {
            // check if the momhee has this request
            UserData user = (UserData)this.Session["user"];
            if (!RequestManager.Manager.IsRequestAllowedForMomhee(user.Id, file_number))
                Response.Redirect("AdminGovExp/RequestManage");
            // add extra data of the request for the mashov page
            string where = "file_number='" + file_number + "'";
            ViewData["request"] = RequestManager.Manager.GetAllWhere(where, null, 1, 1)[0];
            ViewData["names"] = RequestManager.Manager.GetAllColNames();

            var temp = new Dictionary<string, string>();
            // check if continue
            if (isContinue != null && isContinue.ToLower() == "true")
            {
                // load the save file
                var dataFile = Server.MapPath("~/App_Data/Mashovs/temp_" + file_number + ".json");
                string json = System.IO.File.ReadAllText(@dataFile);
                temp = FormValues.LoadJson(json).Values;
            }
            else
            {
                Settings sett = Settings.GetSettings();
                temp["file_version"] = sett.MashovVersion.ToString();
            }
            // load the mashov form
            var mashovFile = Server.MapPath("~/App_Data/Forms/MashovForm_v_" + temp["file_version"] + ".xml");
            // add extra data for "pull from" fields
            temp["file_number"] = file_number;
            temp["gov_exp_name"] = user.Name;
            temp["misrad_name"] = user.Office;
            ViewData["temp"] = temp;
            FormComponent mashovForm = FormManager.Manager.Load(mashovFile);
            return View(mashovForm.FormComponents[0]);
        }

        [HttpPost]
        public ActionResult AddMashov(FormValues fTV)
        {
            // check if the momhee has this request
            UserData user = (UserData)this.Session["user"];
            if (!RequestManager.Manager.IsRequestAllowedForMomhee(user.Id, fTV.Values["file_number"]))
                Response.Redirect("AdminGovExp/RequestManage");

            // check if submit or save
            var isSubmit = !fTV.Values.ContainsKey("isSave") || fTV.Values["isSave"] == "false" ? true : false;

            // save the mashov as Json file
            string jsonPath = isSubmit ? "mashov_" : "temp_";
            jsonPath += fTV.Values["file_number"];
            var dataFile = Server.MapPath("~/App_Data/Mashovs/" + jsonPath + ".json");
            string json = fTV.GetJson();
            System.IO.File.WriteAllText(@dataFile, json);
            // link the mashov to the requests data base row of this request
            RequestManager.Manager.UpdateMashov(fTV.Values["file_number"], jsonPath, fTV.Values["file_version"]);
            return RedirectToAction("../AdminGovExp/RequestsManage");
        }

        [HttpPost]
        public JsonResult EditPassword(string oldPass, string newPass)
        {
            TypeValidator v = TypeValidator.Validator;
            UserData user = (UserData)this.Session["user"];
            // check if the user is not blocked from password related actions
            if (!UserManager.Manager.isUserAllowed(user.Email))
            {
                return Json(new { Success = false, ErrMsg = "החשבון הנ\"ל חסום מפעולות למשך 5 דקות." }, JsonRequestBehavior.AllowGet);
            }
            // check the passwords for type
            if (v.Validate(oldPass, "Password") && v.Validate(newPass, "Password"))
            {
                UserData passTest = UserManager.Manager.GetIfCorrect(user.Email, oldPass);
                if (passTest != null && passTest.Id == user.Id)
                {
                    // the old password is correct
                    Dictionary<string, string> newValues = new Dictionary<string, string>();
                    newValues["password"] = newPass;
                    bool isSuccess = UserManager.Manager.Change(user.Id, newValues);
                    return Json(new { Success = isSuccess, ErrMsg = "שגיאה בשרת." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // the old password is NOT correct
                    int badAttempts = UserManager.Manager.addIncorrectAttempt(user.Email);
                    bool isBlocked = !UserManager.Manager.isUserAllowed(user.Email);
                    if (!isBlocked)
                        return Json(new
                        {
                            Success = false,
                            ErrMsg = "הסיסמה הישנה שהזנת אינה נכונה. נותרו "
                        + (3 - badAttempts) + " מס' הזנות לא נכונות של הסיסמה עד לחסימה"
                        }, JsonRequestBehavior.AllowGet);
                    else
                        this.Session["user"] = null;
                        return Json(new
                        {
                            Success = false,
                            ErrMsg = "החשבון נחסם למשך 5 דקות"
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, ErrMsg = "אחת הסיסמאות שהזנן אינה בםורמט הנכון." }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditPersonalInfo(string password, string fname, string lname, string email,
            string office, string phone, string cellPhone)
        {
            TypeValidator v = TypeValidator.Validator;
            UserData user = (UserData)this.Session["user"];
            // check if the user is not blocked from password related actions
            if (!UserManager.Manager.isUserAllowed(user.Email))
            {
                return Json(new { Success = false, ErrMsg = "החשבון הנ\"ל חסום מפעולות למשך 5 דקות." }, JsonRequestBehavior.AllowGet);
            }
            // validate the fields for type
            if (v.Validate(password, "Password") && v.Validate(fname, "Letters")
                && v.Validate(lname, "Letters") && v.Validate(email, "Email")
                && v.Validate(office, "Integer") && v.Validate(phone, "Phone")
                && v.Validate(cellPhone, "Phone"))
            {
                UserData passTest = UserManager.Manager.GetIfCorrect(user.Email, password);
                if (passTest != null && passTest.Id == user.Id)
                {
                    // the old password is correct
                    Dictionary<string, string> newValues = new Dictionary<string, string>();
                    newValues["fname"] = fname;
                    newValues["lname"] = lname;
                    newValues["email"] = email;
                    newValues["office"] = office;
                    newValues["cell_phone"] = cellPhone;
                    newValues["phone"] = phone;
                    bool isSuccess = UserManager.Manager.Change(user.Id, newValues);
                    if (isSuccess)
                    {
                        // update the user in the Session
                        this.Session["user"] = UserManager.Manager.GetIfCorrect(email, password);
                    }
                    return Json(new { Success = isSuccess, ErrMsg = "שגיאה בשרת." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int badAttempts = UserManager.Manager.addIncorrectAttempt(user.Email);
                    bool isBlocked = !UserManager.Manager.isUserAllowed(user.Email);
                    this.Session["user"] = null;
                    if (!isBlocked)
                        return Json(new
                        {
                            Success = false,
                            ErrMsg = "הסיסמה הישנה שהזנת אינה נכונה. נותרו "
                        + (3 - badAttempts) + " מס' הזנות לא נכונות של הסיסמה עד לחסימה"
                        }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new
                        {
                            Success = false,
                            ErrMsg = "החשבון נחסם למשך 5 דקות"
                        }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { Success = false, ErrMsg = "אחד מהשדות שהזנן אינו בפורנט הנכון." }, JsonRequestBehavior.AllowGet);
        }
    }
}