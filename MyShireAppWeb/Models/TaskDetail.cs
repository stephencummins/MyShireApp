using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShireAppWeb.Models
{
    public class TaskDetail
    {
        public string taskName { get; set; }

        public Guid ProjectUID { get; set; }

        public Guid TaskID { get; set; }

        public string Status { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string LaunchPriority { get; set; }

        public string Organisation { get; set; }

        public Guid AssignmentID { get; set; }

        public string Function { get; set; }

    }
}
