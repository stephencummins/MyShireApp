using MyShireAppWeb.Models;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace MyShireAppWeb.Utilities
{
    public class SharePointUtility
    {

        private void GetListItems_NotInuse()
        {
            string URL = "Site URL/_api/web/lists/getByTitle(List Name')/items";

            HttpWebRequest endpointRequest = (HttpWebRequest)HttpWebRequest.Create(URL);

            endpointRequest.Method = "GET";
            endpointRequest.Accept = "application/json;odata=verbose";
            //NetworkCredential cred = new System.Net.NetworkCredential("username", "password", "domain");
            endpointRequest.Credentials = ContextUtility.GetOnlineCredentials();
            HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse();
            try
            {
                WebResponse webResponse = endpointRequest.GetResponse();
                Stream webStream = webResponse.GetResponseStream();
                StreamReader responseReader = new StreamReader(webStream);
                string response = responseReader.ReadToEnd();
                JObject jobj = JObject.Parse(response);
                JArray jarr = (JArray)jobj["d"]["results"];
                foreach (JObject j in jarr)
                {
                    Console.WriteLine(j["Title"] + " " + j["Body"]);
                }

                responseReader.Close();
                Console.ReadLine();


            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message); Console.ReadLine();
            }
        }

        public Dictionary<string, string> GetLaunchFields()
        {
            Dictionary<string, string> FieldsInfo = new Dictionary<string, string>();

            using (var context = ContextUtility.GetPWASiteClientContext())
            {
                // Assume the web has a list named "Announcements". 
                List announcementsList = context.Web.Lists.GetByTitle("Launch Fields");

                // This creates a CamlQuery that has a RowLimit of 100, and also specifies Scope="RecursiveAll" 
                // so that it grabs all list items, regardless of the folder they are in. 
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                  + "<Query>"
                  + "<Or><Eq><FieldRef Name='Launch_x0020_Activity' /><Value Type='Text'>Launch Provision</Value></Eq><Eq><FieldRef Name='Launch_x0020_Activity' /><Value Type='Text'>Both</Value></Eq></Or>"
                  + "</Query>"
                  + "<ViewFields>"
                  + "  <FieldRef Name='Title' /><FieldRef Name='Model_x0020_Property_x0020_Name' />"
                  + "</ViewFields>"
                  + "</View>";


                ListItemCollection items = announcementsList.GetItems(query);

                // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                context.Load(items);
                context.ExecuteQuery();
                foreach (ListItem listItem in items)
                {


                    if (listItem["Title"] != null && listItem["Model_x0020_Property_x0020_Name"] != null)
                    {
                        FieldsInfo.Add(listItem["Model_x0020_Property_x0020_Name"].ToString(), listItem["Title"].ToString());
                    }

                }
            }

            return FieldsInfo;
        }

        public Dictionary<string, string> GetLaunchFields(string CustomFieldType)
        {
            Dictionary<string, string> FieldsInfo = new Dictionary<string, string>();

            using (var context = ContextUtility.GetPWASiteClientContext())
            {
                List announcementsList = context.Web.Lists.GetByTitle("Launch Fields");
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                  + "<Query>"
                  + "<Eq><FieldRef Name='Custom_x0020_Field_x0020_Type' /><Value Type='Text'>" + CustomFieldType + "</Value></Eq>"
                  + "</Query>"
                  + "<ViewFields>"
                  + "  <FieldRef Name='Title' /><FieldRef Name='Model_x0020_Property_x0020_Name' />"
                  + "</ViewFields>"
                  + "</View>";

                ListItemCollection items = announcementsList.GetItems(query);

                // Retrieve all items in the ListItemCollection from List.GetItems(Query). 
                context.Load(items);
                context.ExecuteQuery();
                foreach (ListItem listItem in items)
                {
                    if (listItem["Title"] != null && listItem["Model_x0020_Property_x0020_Name"] != null)
                    {
                        FieldsInfo.Add(listItem["Model_x0020_Property_x0020_Name"].ToString(), listItem["Title"].ToString());
                    }
                }
            }

            return FieldsInfo;
        }

        public bool UpdateIPPMileStoneDate(string Brand, string Country, string Indication, IPPMilestoneDates MileStoneDates, int ItemId)
        {
            using (var context = ContextUtility.GetPWASiteClientContext())
            {
                try
                {
                    List MileStonelist = context.Web.Lists.GetByTitle("IPPMilestones");
                    ListItem _item = MileStonelist.GetItemById(ItemId);

                    _item["SubmitP_x0026_R_x002f_HTAdossier"] = MileStoneDates.SubmitPR_HTAdossier_date;
                    _item["P_x0026_R_x002f_HTAdossierRespon"] = MileStoneDates.SubmitPR_HTAdossier_Response_date;
                    _item["RegulatorySubmission"] = MileStoneDates.Regulatory_Submission_date;
                    _item["RegulatoryApproval"] = MileStoneDates.Regulatory_Approval_date;
                    _item["CommercialLaunch"] = MileStoneDates.Commercial_Laucnh_date;
                    _item["SupplyReady_x003a_TradeStockAvai"] = MileStoneDates.Trade_Stock_Available_date;
                    _item.Update();
                    context.ExecuteQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public bool UpdateIPPMileStoneDate(IPPMilestoneDates MileStoneDates, int ItemId)
        {
            using (var context = ContextUtility.GetLaunchSiteClientCotext())
            {
                try
                {
                    List MileStonelist = context.Web.Lists.GetByTitle("IPP Milestones");
                    ListItem _item = MileStonelist.GetItemById(ItemId);

                    _item["SubmitP_x0026_R_x002f_HTAdossier"] = MileStoneDates.SubmitPR_HTAdossier_date;
                    _item["P_x0026_R_x002f_HTAdossierRespon"] = MileStoneDates.SubmitPR_HTAdossier_Response_date;
                    _item["RegulatorySubmission"] = MileStoneDates.Regulatory_Submission_date;
                    _item["RegulatoryApproval"] = MileStoneDates.Regulatory_Approval_date;
                    _item["CommercialLaunch"] = MileStoneDates.Commercial_Laucnh_date;
                    _item["SupplyReady_x003a_TradeStockAvai"] = MileStoneDates.Trade_Stock_Available_date;
                    _item.Update();
                    context.ExecuteQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public List<BrandCategoryMapping> GetBrandCategoryMapping()
        {
            List<BrandCategoryMapping> FieldsInfo = new List<BrandCategoryMapping>();
            using (ClientContext context = ContextUtility.GetLaunchSiteClientCotext())
            {
                List _List = context.Web.Lists.GetByTitle("Brand Category Mapping");
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                  + "<Query>"
                  + "</Query>"
                  + "<ViewFields>"
                  + " <FieldRef Name='Brand' /> <FieldRef Name='Categories' />"
                  + "</ViewFields>"
                  + "</View>";

                ListItemCollection items = _List.GetItems(query);

                context.Load(items);
                context.ExecuteQuery();

                foreach (ListItem listItem in items)
                {
                    if (listItem["Brand"] != null)
                    {
                        BrandCategoryMapping BCM = new BrandCategoryMapping();
                        BCM.BrandName = listItem["Brand"].ToString();
                        BCM.Category = listItem["Categories"].ToString();

                        FieldsInfo.Add(BCM);

                    }
                }
            }
            return FieldsInfo;
        }

        public List<CountryWaveMapping> GetCountryWaveMapping()
        {
            List<CountryWaveMapping> FieldsInfo = new List<CountryWaveMapping>();

            using (ClientContext context = ContextUtility.GetLaunchSiteClientCotext())
            {
                List _List = context.Web.Lists.GetByTitle("Country Wave Mapping");
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                  + "<Query>"
                  + "</Query>"
                  + "<ViewFields>"
                  + " <FieldRef Name='Country' /> <FieldRef Name='Wave' />"
                  + "</ViewFields>"
                  + "</View>";

                ListItemCollection items = _List.GetItems(query);

                context.Load(items);
                context.ExecuteQuery();

                foreach (ListItem listItem in items)
                {
                    if (listItem["Country"] != null)
                    {
                        CountryWaveMapping CVM = new CountryWaveMapping();
                        CVM.CountryName = listItem["Country"].ToString();
                        CVM.Wave = listItem["Wave"].ToString();

                        FieldsInfo.Add(CVM);

                    }
                }
            }
            return FieldsInfo;
        }

        public List<StartLaunchMapping> GetStartLaunchMapping()
        {
            List<StartLaunchMapping> FieldsInfo = new List<StartLaunchMapping>();

            using (ClientContext context = ContextUtility.GetLaunchSiteClientCotext())
            {
                List _List = context.Web.Lists.GetByTitle("Star Launches");
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                  + "<Query>"
                  + "</Query>"
                  + "<ViewFields>"
                  + " <FieldRef Name='Country' /> <FieldRef Name='Brand' /> <FieldRef Name='StarLaunches' />"
                  + "</ViewFields>"
                  + "</View>";

                ListItemCollection items = _List.GetItems(query);

                context.Load(items);
                context.ExecuteQuery();

                foreach (ListItem listItem in items)
                {
                    if (listItem["Country"] != null && listItem["Brand"] != null)
                    {
                        StartLaunchMapping CVM = new StartLaunchMapping();
                        CVM.CountryName = listItem["Country"].ToString();
                        CVM.BrandName = listItem["Brand"].ToString();
                        CVM.StarLaunch = listItem["StarLaunches"].ToString();

                        FieldsInfo.Add(CVM);

                    }
                }
            }
            return FieldsInfo;
        }

        public List<IPPMilestoneDates> GetIPPMileStoneDates()
        {
            List<IPPMilestoneDates> IPPDates = new List<IPPMilestoneDates>();
            try
            {
                using (var context = ContextUtility.GetLaunchSiteClientCotext())
                {
                    List IPPMileStone = context.Web.Lists.GetByTitle("IPP Milestones");
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = "<View>"
                      + "<Query>"
                      + "</Query>"
                      + "</View>";

                    ListItemCollection items = IPPMileStone.GetItems(query);
                    context.Load(items);
                    context.ExecuteQuery();
                    ;
                    foreach (ListItem listItem in items)
                    {
                        IPPMilestoneDates milestonesDates = new IPPMilestoneDates();
                        milestonesDates.BrandName = listItem["Brand"].ToString();
                        milestonesDates.CountryName = listItem["Country"].ToString();
                        milestonesDates.SubmitPR_HTAdossier_date = Convert.ToDateTime(listItem["SubmitP_x0026_R_x002f_HTAdossier"].ToString());
                        milestonesDates.SubmitPR_HTAdossier_Response_date = Convert.ToDateTime(listItem["P_x0026_R_x002f_HTAdossierRespon"].ToString());
                        milestonesDates.Regulatory_Submission_date = Convert.ToDateTime(listItem["RegulatorySubmission"].ToString());
                        milestonesDates.Regulatory_Approval_date = Convert.ToDateTime(listItem["RegulatoryApproval"].ToString());
                        milestonesDates.Commercial_Laucnh_date = Convert.ToDateTime(listItem["CommercialLaunch"].ToString());
                        milestonesDates.Trade_Stock_Available_date = Convert.ToDateTime(listItem["SupplyReady_x003a_TradeStockAvai"].ToString());
                        IPPDates.Add(milestonesDates);
                    }

                }



            }
            catch (Exception ex)
            {

            }
            return IPPDates;
        }

        public IPPMilestoneDates GetIPPMileStoneDates(string brand, string country)
        {
            IPPMilestoneDates milestonesDates = new IPPMilestoneDates();
            try
            {
                using (var context = ContextUtility.GetLaunchSiteClientCotext())
                {
                    List IPPMileStone = context.Web.Lists.GetByTitle("IPP Milestones");
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = "<View>"
                      + "<Query>"
                      + "<Where>"
                        + "<And>"
                          + "<Eq>"
                            + "<FieldRef Name='Brand' />"
                            + "<Value Type='Text'>" + brand + "</Value>"
                          + "</Eq>"
                          + "<Eq>"
                               + "<FieldRef Name='Country'/>"
                               + "<Value Type='Text'>" + country + "</Value>"
                           + "</Eq>"
                        + "</And>"
                        + "</Where>"
                      + "</Query>"
                      + "</View>";

                    ListItemCollection items = IPPMileStone.GetItems(query);
                    context.Load(items);
                    context.ExecuteQuery();

                    if (items.Count > 0)
                    {
                        ListItem listItem = items.FirstOrDefault();
                        milestonesDates.ListItemID = listItem.Id.ToString();
                        milestonesDates.BrandName = listItem["Brand"].ToString();
                        milestonesDates.CountryName = listItem["Country"].ToString();
                        milestonesDates.SubmitPR_HTAdossier_date = Convert.ToDateTime(listItem["SubmitP_x0026_R_x002f_HTAdossier"].ToString());
                        milestonesDates.SubmitPR_HTAdossier_Response_date = Convert.ToDateTime(listItem["P_x0026_R_x002f_HTAdossierRespon"].ToString());
                        milestonesDates.Regulatory_Submission_date = Convert.ToDateTime(listItem["RegulatorySubmission"].ToString());
                        milestonesDates.Regulatory_Approval_date = Convert.ToDateTime(listItem["RegulatoryApproval"].ToString());
                        milestonesDates.Commercial_Laucnh_date = Convert.ToDateTime(listItem["CommercialLaunch"].ToString());
                        milestonesDates.Trade_Stock_Available_date = Convert.ToDateTime(listItem["SupplyReady_x003a_TradeStockAvai"].ToString());
  
                        
                    }                  
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return milestonesDates;

        }

        public string GetCountryWave(string CountryName)
        {
            string Wave = "";

            using (ClientContext context = ContextUtility.GetLaunchSiteClientCotext())
            {
                List _List = context.Web.Lists.GetByTitle("Country Wave Mapping");
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                  + "<Query>"
                   + "<Where>"
                          + "<Eq>"
                            + "<FieldRef Name='Country' />"
                            + "<Value Type='Text'>" + CountryName + "</Value>"
                          + "</Eq>"
                        + "</Where>"
                  + "</Query>"
                  + "<ViewFields>"
                  + " <FieldRef Name='Country' /> <FieldRef Name='Wave' />"
                  + "</ViewFields>"
                  + "</View>";

                ListItemCollection items = _List.GetItems(query);

                context.Load(items);
                context.ExecuteQuery();

                if (items.Count > 0)
                {
                    Wave = items.FirstOrDefault()["Wave"].ToString();
                }
            }
            return Wave;
        }

        public string GetBrandCategory(string BrandName)
        {
            string Category = "";

            using (ClientContext context = ContextUtility.GetLaunchSiteClientCotext())
            {
                List _List = context.Web.Lists.GetByTitle("Brand Category Mapping");
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                  + "<Query>"
                   + "<Where>"
                          + "<Eq>"
                            + "<FieldRef Name='Brand' />"
                            + "<Value Type='Text'>" + BrandName + "</Value>"
                          + "</Eq>"
                        + "</Where>"
                  + "</Query>"
                  + "<ViewFields>"
                  + " <FieldRef Name='Brand' /> <FieldRef Name='Categories' />"
                  + "</ViewFields>"
                  + "</View>";

                ListItemCollection items = _List.GetItems(query);

                context.Load(items);
                context.ExecuteQuery();

                if (items.Count > 0)
                {
                    Category = items.FirstOrDefault()["Categories"].ToString();
                }
            }
            return Category;
        }

        public bool GetStarLaunch(string BrandName, string CountryName)
        {
            bool startLaunch = false;

            using (ClientContext context = ContextUtility.GetLaunchSiteClientCotext())
            {
                List _List = context.Web.Lists.GetByTitle("Star Launches");
                CamlQuery query = new CamlQuery();
                query.ViewXml = "<View>"
                  + "<Query>"
                   + "<Where>"
                        + "<And>"
                          + "<Eq>"
                            + "<FieldRef Name='Brand' />"
                            + "<Value Type='Text'>" + BrandName + "</Value>"
                          + "</Eq>"
                          + "<Eq>"
                               + "<FieldRef Name='Country'/>"
                               + "<Value Type='Text'>" + CountryName + "</Value>"
                           + "</Eq>"
                        + "</And>"
                        + "</Where>"
                  + "</Query>"
                  + "<ViewFields>"
                  + " <FieldRef Name='Brand' /> <FieldRef Name='Category' /><FieldRef Name='StarLaunches' />"
                  + "</ViewFields>"
                  + "</View>";

                ListItemCollection items = _List.GetItems(query);

                context.Load(items);
                context.ExecuteQuery();

                if (items.Count > 0)
                {
                    startLaunch = Convert.ToBoolean(items.FirstOrDefault()["StarLaunches"].ToString());
                }
            }
            return startLaunch;
        }
    }
}