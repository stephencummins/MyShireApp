using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShireAppWeb.Models
{
    public class PWAProject
    {
        public string ProjectName { get; set; }
        public string ProjectID { get; set; }

    }
    public class ProjectDetails : PWAProject
    {
        public string Local_Launch_Request { get; set; }
        public string LaunchTemplate { get; set; }
        public string Franchise { get; set; }
        public string MyProperty { get; set; }
        public string Brand { get; set; }
        public string Indication { get; set; }
        public string Region { get; set; }
        public string Cluster { get; set; }
        public string Country { get; set; }
        public string Launch_Priority { get; set; }
        public DateTime SubmitPR_HTAdossier_date { get; set; }
        public DateTime SubmitPR_HTAdossier_Response_date { get; set; }
        public DateTime Regulatory_Submission_date { get; set; }
        public DateTime Regulatory_Approval_date { get; set; }
        public DateTime Commercial_Laucnh_date { get; set; }
        public DateTime Trade_Stock_Available_date { get; set; }
        public double Cumulative_Sales { get; set; }
        public string IPPDateListItemID { get; set; }
        public string Local_Launch_Leader { get; set; }
        public string Country_General_Manager { get; set; }

        public List<PWAResources> Project_Team { get; set; }

        //Global Meeting Dates
        public DateTime GPS_Launch_Activation { get; set; }
        public DateTime Launch_Management_GPS_Review_L42M_date { get; set; }
        public DateTime Launch_Management_GPS_Review_L36M_date { get; set; }
        public DateTime Launch_Management_GPS_Review_L30M_date { get; set; }
        public DateTime Go_NoGo_GPS_L24_date { get; set; }
        public DateTime Launch_Management_GPS_Review_L12_date { get; set; }

        //Local Meeting Dates
        public DateTime Country_Launch_Activation { get; set; }
        public DateTime Launch_Management_Country_Review_L18M_date { get; set; }
        public DateTime Launch_Management_Country_Review_L12M_date { get; set; }
        public DateTime Launch_Management_Country_Review_L6M_date { get; set; }
        public DateTime Go_NoGo_date_Country_Review_L6M { get; set; }
    }

    public class ProjectLunchProvisionDetails : ProjectDetails
    {
        public string Incremental_Commercial_FTEs { get; set; }
        public string Incremental_Non_Commerical_FTEs { get; set; }
        public string Incremental_Global_Support_FTEs { get; set; }
        public double NPV { get; set; }
        public double Public_Price { get; set; }
        public double Actual_Price { get; set; }

    }

    public class ProjectLunchUpdateDetails : ProjectDetails
    {
        public string Executive_Summary { get; set; }
    }
}