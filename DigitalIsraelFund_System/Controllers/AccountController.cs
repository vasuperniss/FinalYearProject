using DigitalIsraelFund_System.DataBase;
using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Models;
using System;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
    public class AccountController : Controller
    {

        [HttpPost]
        public JsonResult Login(string email, string password)
        {
            TypeValidator v = TypeValidator.Validator;
            // validate the expected type of the login fields
            if (!v.Validate(email, "Email") || !v.Validate(password, "Letters"))
                return Json(new { Success = false, ErrMsg = "פורמט שדות שגוי" }, JsonRequestBehavior.AllowGet);
            UserData user = UserManager.Manager.GetIfCorrect(email, password);
            // check is user is allowed
            if (!UserManager.Manager.isUserAllowed(email))
                return Json(new { Success = false, ErrMsg = "החשבון הנ\"ל חסום מפעולות למשך 5 דקות." }, JsonRequestBehavior.AllowGet);
            Session["user"] = user;
            if (user == null)
            {
                // loging credentials are incorrect
                int badAttempts = UserManager.Manager.addIncorrectAttempt(email);
                bool isBlocked = !UserManager.Manager.isUserAllowed(email);
                if (!isBlocked)
                    return Json(new
                    {
                        Success = false,
                        ErrMsg = "הסיסמה שהזנת אינה נכונה. נותרו "
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
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            // clear the Session
            this.Session["user"] = null;
            return RedirectToAction("Page", "Home");
        }

        [HttpPost]
        public JsonResult KeepMeAlive()
        {
            // access the Session to keep it alive
            this.Session["Heartbeat"] = DateTime.Now;
            return Json(new { Success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}