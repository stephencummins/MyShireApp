using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShireAppWeb.Models
{
    public class IPPMilestoneDates
    {
        public string ListItemID { get; set; }
        public string BrandName { get; set; }
        public string CountryName { get; set; }
        public DateTime SubmitPR_HTAdossier_date { get; set; }
        public DateTime SubmitPR_HTAdossier_Response_date { get; set; }
        public DateTime Regulatory_Submission_date { get; set; }
        public DateTime Regulatory_Approval_date { get; set; }
        public DateTime Commercial_Laucnh_date { get; set; }
        public DateTime Trade_Stock_Available_date { get; set; }
    }
}