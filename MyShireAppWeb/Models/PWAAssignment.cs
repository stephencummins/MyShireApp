using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShireAppWeb.Models
{
    public class PWAAssignment
    {
        public string ProjectName { get; set; }
        public string taskName { get; set; }
        public Guid taskID { get; set; }
        public Guid AssignmentID { get; set; }
        public Guid ProjectID { get; set; }
        public string ID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string RecevedDate { get; set; }
        public string LastModifiedDate { get; set; }
        //  public string taskID { get; set; }
        // public string ResourceEmail { get; set; }
        public Dictionary<string, string> customFields = new Dictionary<string, string>();
    }
}
