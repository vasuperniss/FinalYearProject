using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DigitalIsraelFund_System.Filters
{
    public abstract class BaseFilter : ActionFilterAttribute
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
            // get the user from Session from the Request context
            UserData user = (UserData)context.HttpContext.Session["user"];
            if (user != null)
            {
                // get [abstractly] the allowed users for this filter
                List<string> allowedUsers = getAllowedUsers();
                foreach (var userType in allowedUsers)
                    if (user.Type.ToLower() == userType.ToLower())
                        // the user is allowed
                        return true;
            }
            // not allowd
            return false;
        }

        protected abstract List<string> getAllowedUsers();
    }
}