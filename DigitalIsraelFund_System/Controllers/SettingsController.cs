using DigitalIsraelFund_System.Filters;
using DigitalIsraelFund_System.Models;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;

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
            // check if file is a good form

            Models.Settings sett = Models.Settings.GetSettings();
            // add the file to the possible forms
            var newVer = sett.PossibleMashovVersions[sett.PossibleMashovVersions.Count - 1] + 1;
            string tempPath = "MashovForm_Temp.xml";
            string filePath = "MashovForm_v_" + newVer + ".xml";

            // save as temp
            var saveTo = Server.MapPath("~/App_Data/Forms/" + tempPath);
            file.SaveAs(saveTo);
            // check if the file can be read
            try
            {
                string xmlNode = System.IO.File.ReadAllText(saveTo);
                XmlReader xmlReader = XmlReader.Create(new StringReader(xmlNode));
                FormComponent loadedForm = new FormComponent(xmlReader);
                xmlReader.Close();
            }
            catch
            {
                // failed to intepret the file
                return RedirectToAction("Settings");
            }

            // add the mashov file
            saveTo = Server.MapPath("~/App_Data/Forms/" + filePath);
            file.SaveAs(saveTo);
            // save the settings
            sett.PossibleMashovVersions.Add(newVer);
            sett.Save();

            return RedirectToAction("Settings");
        }
    }
}