using DigitalIsraelFund_System.DataBase;
using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Filters;
using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Xml;

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
        public ActionResult AddMashov(string file_number)
        {
            string where = "file_number='" + file_number + "'";
            ViewData["request"] = RequestManager.Manager.GetAllWhere(where, null, 1, 1)[0];
            ViewData["names"] = RequestManager.Manager.GetAllColNames();

            var requestFile = Server.MapPath("~/App_Data/Forms/MashovForm_v_0.xml");
            string xmlNode = System.IO.File.ReadAllText(@requestFile);
            XmlReader xmlReader = XmlReader.Create(new StringReader(xmlNode));
            FormComponent requestForm = new FormComponent(xmlReader);
            xmlReader.Close();

            ViewData["postToController"] = "../GovExp/AddMashov";
            ViewData["sendBtnTitle"] = "שלח משוב";
            ViewData["fileNumberRequest"] = file_number;
            ViewData["nameGovExp"] = ((UserData)this.Session["user"]).Name;
            ViewData["officeGovExp"] = ((UserData)this.Session["user"]).Office;
            return View("../Home/Form2", requestForm.FormComponents[0]);
        }

        [HttpPost]
        public ActionResult AddMashov(FormValues fTV)
        {
            string jsonPath = "mashov_" + fTV.Values["file_number"];
            var dataFile = Server.MapPath("~/App_Data/Mashovs/" + jsonPath + ".json");
            string json = fTV.GetJson();
            System.IO.File.WriteAllText(@dataFile, json);

            RequestManager.Manager.UpdateMashov(fTV.Values["file_number"], jsonPath);
            return RedirectToAction("~/Admin/RequestsManage");
        }

        [HttpPost]
        public JsonResult EditPassword(string oldPass, string newPass)
        {
            TypeValidator v = TypeValidator.Validator;
            UserData user = (UserData)this.Session["user"];
            if (!UserManager.Manager.isUserAllowed(user.Email))
            {
                return Json(new { Success = false, ErrMsg = "החשבון הנ\"ל חסום מפעולות למשך 5 דקות." }, JsonRequestBehavior.AllowGet);
            }
            if (v.Validate(oldPass, "Password") && v.Validate(newPass, "Password"))
            {
                UserData passTest = UserManager.Manager.GetIfCorrect(user.Email, oldPass);
                if (passTest != null && passTest.Id == user.Id)
                {
                    Dictionary<string, string> newValues = new Dictionary<string, string>();
                    newValues["password"] = newPass;
                    bool isSuccess = UserManager.Manager.Change(user.Id, newValues);
                    return Json(new { Success = isSuccess, ErrMsg = "שגיאה בשרת." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
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
        public JsonResult EditPersonalInfo(string password, string fname, string lname, string email, string office)
        {
            TypeValidator v = TypeValidator.Validator;
            UserData user = (UserData)this.Session["user"];
            if (!UserManager.Manager.isUserAllowed(user.Email))
            {
                return Json(new { Success = false, ErrMsg = "החשבון הנ\"ל חסום מפעולות למשך 5 דקות." }, JsonRequestBehavior.AllowGet);
            }
            if (v.Validate(password, "Password") && v.Validate(fname, "Letters")
                && v.Validate(lname, "Letters") && v.Validate(email, "Email"))
            {
                UserData passTest = UserManager.Manager.GetIfCorrect(user.Email, password);
                if (passTest != null && passTest.Id == user.Id)
                {
                    Dictionary<string, string> newValues = new Dictionary<string, string>();
                    newValues["fname"] = fname;
                    newValues["lname"] = lname;
                    newValues["email"] = email;
                    newValues["office"] = office;
                    bool isSuccess = UserManager.Manager.Change(user.Id, newValues);
                    if (isSuccess)
                    {
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