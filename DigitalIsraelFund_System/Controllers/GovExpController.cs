using DigitalIsraelFund_System.DataBase;
using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
    public class GovExpController : Controller
    {

        [HttpGet]
        public ActionResult EditSelf()
        {
            return View((UserData)this.Session["user"]);
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
                    bool isSuccess = UserManager.Manager.Change(user, newValues);
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
        public JsonResult EditPersonalInfo(string password, string fname, string lname, string email)
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
                    bool isSuccess = UserManager.Manager.Change(user, newValues);
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