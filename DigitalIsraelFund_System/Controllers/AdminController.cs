using DigitalIsraelFund_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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


    }
}