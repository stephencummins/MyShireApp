using Microsoft.ProjectServer.Client;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;

namespace MyShireAppWeb.Utilities
{
    public static class ContextUtility
    {
        private static string PWAUrl = "";
        private static string UserName = "";
        private static string Password = "";
        private static string LaunchSiteUrl = "";

        static ContextUtility()
        {
            PWAUrl = "https://shirepharma.sharepoint.com/sites/cepwadev";
            LaunchSiteUrl = "https://shirepharma.sharepoint.com/sites/cedev";
            UserName = "lchan-c@shire.com";
            Password = "Oneshire!!2";
        }

        /// <summary>
        /// Returns the Project Context
        /// </summary>
        /// <returns>Project Context</returns>
        public static ProjectContext GetProjectContext()
        {
            return CreateProjectContext(PWAUrl, GetOnlineCredentials());
        }

        /// <summary>
        /// Returns PWA site Client Context
        /// </summary>
        /// <returns>Client Context</returns>
        public static ClientContext GetPWASiteClientContext()
        {
            return CreateClientContext(PWAUrl, GetOnlineCredentials());
        }

        /// <summary>
        /// Returns Launch Site Client Context
        /// </summary>
        /// <returns>Client Context</returns>
        public static ClientContext GetLaunchSiteClientCotext()
        {
            return CreateClientContext(LaunchSiteUrl, GetOnlineCredentials());
        }

        /// <summary>
        /// Creates & Returns the SharePoint Credentials
        /// </summary>
        /// <param name="UserName">O365 Login Email</param>
        /// <param name="Password">O3656 Login Password</param>
        /// <returns>SharePoint Credentials</returns>
        public static SharePointOnlineCredentials GetOnlineCredentials()
        {
            SecureString secpassword = new SecureString();
            foreach (char c in Password.ToCharArray()) secpassword.AppendChar(c);
            return new SharePointOnlineCredentials(UserName, secpassword);
        }

        /// <summary>
        /// Creates the Project Context using provided URL & SharePoint Credentials
        /// </summary>
        /// <param name="PWAUrl">PWA Site URL</param>
        /// <param name="Credentials">SharePoint Credentials For the Site</param>
        /// <returns>Project Context</returns>
        private static ProjectContext CreateProjectContext(string PWAUrl, SharePointOnlineCredentials Credentials)
        {
            ProjectContext projectcontext = new ProjectContext(PWAUrl);
            projectcontext.Credentials = Credentials;
            return projectcontext;
        }

        /// <summary>
        /// Creates the Clinet Context using provided URL & SharePoint Credentials
        /// </summary>
        /// <param name="SiteUrl">SharePoint Site URL</param>
        /// <param name="Credentials">SharePoint Credentials For the Site</param>
        /// <returns>Client Context</returns>
        private static ClientContext CreateClientContext(string SiteUrl, SharePointOnlineCredentials Credentials)
        {
            ClientContext clientcontext = new ClientContext(SiteUrl);
            clientcontext.Credentials = Credentials;
            return clientcontext;
        }
    }
}