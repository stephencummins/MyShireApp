using Microsoft.ProjectServer.Client;
using Microsoft.SharePoint.Client;
using System;

namespace MyShireAppWeb
{
    public class ContextHelper
    {
        public static ProjectContext GetClientContext(Uri url)
        {
            string contextToken = TokenHelper.GetContextTokenFromRequest(System.Web.HttpContext.Current.Request);
            string hostWeb = System.Web.HttpContext.Current.Request["SPHostUrl"];

            if (!string.IsNullOrEmpty(contextToken))
                System.Web.HttpContext.Current.Session["ctx"] = contextToken;
            else
                contextToken = (string)System.Web.HttpContext.Current.Session["ctx"];

            if (!string.IsNullOrEmpty(hostWeb))
                System.Web.HttpContext.Current.Session["host"] = hostWeb;
            else
                hostWeb = (string)System.Web.HttpContext.Current.Session["host"];

            return TokenHelper.GetProjectContextWithContextToken(hostWeb, contextToken, url.Authority);
        }
    }
}