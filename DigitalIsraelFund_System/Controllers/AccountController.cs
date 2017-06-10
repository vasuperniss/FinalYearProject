using DigitalIsraelFund_System.DataBase.Managers;
using DigitalIsraelFund_System.Models;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
    public class AccountController : Controller
    {

        [HttpPost]
        public JsonResult Login(string email, string password)
        {
            UserData user = UserManager.GetIfCorrect(email, password);
            Session["user"] = user;
            return Json(new { Success = user != null }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            this.Session["user"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}