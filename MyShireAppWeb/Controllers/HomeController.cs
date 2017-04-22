using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyShireAppWeb.Utilities;
using MyShireAppWeb.Models;

namespace MyShireAppWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            User spUser = null;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    ViewBag.UserName = spUser.Title;
                }
            }

            //Project Context
            using (var projectContext = ContextHelper.GetClientContext(Request.Url))
            {
                // Query the Project Context
                //projectContext.Load(projectContext.Projects);
                // projectContext.ExecuteQuery();

                //projectContext.Load(projectContext.Web, web => web.Title);
                //projectContext.ExecuteQuery();
                //ViewBag.Message = projectContext.Web.Title;

                PWAUtility pwaUtil = new PWAUtility();

                List<PWAAssignment> PWA_ResourceAssignment = pwaUtil.ResourceAssignments(projectContext);
                ViewBag.Message = "Hello, I work!";
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
