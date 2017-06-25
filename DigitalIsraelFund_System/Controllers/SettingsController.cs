using DigitalIsraelFund_System.Filters;
using System.Web;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
    [AdminFilter]
    public class SettingsController : Controller
    {
        [HttpGet]
        public ActionResult Settings()
        {
            return View(Models.Settings.GetSettings());
        }

        [HttpPost]
        public ActionResult ChangeMashovVer(string version)
        {
            // load the settings, update the mashov version and save the settings
            Models.Settings sett = Models.Settings.GetSettings();
            sett.MashovVersion = int.Parse(version);
            sett.Save();

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public ActionResult AddMashovFile(HttpPostedFileBase file)
        {
            Models.Settings sett = Models.Settings.GetSettings();
            // add the file to the possible forms
            var newVer = sett.PossibleMashovVersions[sett.PossibleMashovVersions.Count - 1] + 1;
            string filePath = "MashovForm_v_" + newVer + ".xml";
            var saveTo = Server.MapPath("~/App_Data/Forms/" + filePath);
            file.SaveAs(saveTo);
            // save the settings
            sett.PossibleMashovVersions.Add(newVer);
            sett.Save();

            return RedirectToAction("Settings");
        }
    }
}