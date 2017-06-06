using DigitalIsraelFund_System.Models;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
    public class HomeController : Controller
    {
        
        [HttpGet]
        public ActionResult Page(string page)
        {
            if (page == "" || page == null)
                page = "Index";
            ViewData["Page"] = page;
            return View("StaticPage", this.GetPage(page));
        }

        [NonAction]
        private StaticPage GetPage(string filename)
        {
            var dataFile = Server.MapPath("~/App_Data/Pages/" + filename + ".json");
            string json = System.IO.File.ReadAllText(@dataFile);
            return StaticPage.LoadJson(json);
        }
    }
}