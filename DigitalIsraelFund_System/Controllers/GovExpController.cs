using DigitalIsraelFund_System.Models;
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

            return null;
        }

        [HttpPost]
        public JsonResult EditPersonalInfo(string fname, string lname, string email)
        {

            return null;
        }
    }
}