using DigitalIsraelFund_System.Models;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Filters
{
    public class AdminFilter : ActionFilterAttribute
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
            var action = (string)context.RouteData.Values["action"];
            if (user != null)
            {
                if (user.Type.ToLower() == "momhee" &&
                    (action == "RequestsManage" || action == "SearchRequestBy"))
                    return true;
                if (user.Type.ToLower() == "admin")
                    return true;
            }
            return false;
        }
    }
}