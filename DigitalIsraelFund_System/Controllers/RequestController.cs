using DigitalIsraelFund_System.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Controllers
{
    public class RequestController : Controller
    {
        public JsonResult ValidateField(string value, string type)
        {
            return Json(new { Success = TypeValidator.Validator.Validate(value, type) }, JsonRequestBehavior.AllowGet);
        }
    }
}