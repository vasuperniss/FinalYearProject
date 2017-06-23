using DigitalIsraelFund_System.Models;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Filters
{
    public class GovExpFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!this.Validate(filterContext))
                filterContext.Result = new RedirectResult("~/Home/Page");
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!this.Validate(filterContext))
                filterContext.Result = new RedirectResult("~/Home/Page");
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (!this.Validate(filterContext))
                filterContext.Result = new RedirectResult("~/Home/Page");
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (!this.Validate(filterContext))
                filterContext.Result = new RedirectResult("~/Home/Page");
        }

        private bool Validate(ControllerContext context)
        {
            UserData user = (UserData)context.HttpContext.Session["user"];
            if (user != null)
            {
                if (user.Type.ToLower() == "momhee")
                    return true;
            }
            return false;
        }
    }
}