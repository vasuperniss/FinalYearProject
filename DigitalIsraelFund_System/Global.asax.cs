using DigitalIsraelFund_System.DataBase.Managers;
using System;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalIsraelFund_System
{
    public class MvcApplication : HttpApplication
    {
        private static CacheItemRemovedCallback OnCacheRemove = null;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            AddTask("EmailCheck", 5);
        }

        private void AddTask(string name, int seconds)
        {
            OnCacheRemove = new CacheItemRemovedCallback(CacheItemRemoved);
            HttpRuntime.Cache.Insert(name, seconds, null,
                DateTime.Now.AddSeconds(seconds), Cache.NoSlidingExpiration,
                CacheItemPriority.NotRemovable, OnCacheRemove);
        }

        public void CacheItemRemoved(string k, object v, CacheItemRemovedReason r)
        {
            // do stuff here if it matches our taskname, like WebRequest
            switch (k)
            {
                case "EmailCheck":
                    EmailManager.Manager.CheckForNewData();
                    break;
            }
            // re-add our task so it recurs
            AddTask(k, Convert.ToInt32(v));
        }
    }
}
