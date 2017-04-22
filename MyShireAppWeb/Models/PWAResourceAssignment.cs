using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MyShireAppWeb.Models
{
    [DataContract]
    public class PWAResourceAssignment
    {
        /// <summary>
        /// Project Name
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ProjectName { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Status { get; set; }

        /// <summary>
        /// ProjectOrTask
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ProjectOrTask { get; set; }

        /// <summary>
        /// taskName
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string taskName { get; set; }

        /// <summary>
        /// taskID
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Guid TaskID { get; set; }

        /// <summary>
        /// AssignmentID
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Guid AssignmentID { get; set; }

        /// <summary>
        /// ProjectID
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Guid ProjectID { get; set; }

        /// <summary>
        /// ID
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ID { get; set; }

        /// <summary>
        /// StartDate
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// EndDate
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// RecevedDate
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string RecevedDate { get; set; }

        /// <summary>
        /// Function
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Function { get; set; }

        /// <summary>
        /// LaunchPriority
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string LaunchPriority { get; set; }

        /// <summary>
        /// Organisation
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Organisation { get; set; }

        /// <summary>
        /// LastModifiedDate
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string LastModifiedDate { get; set; }
        //  public string taskID { get; set; }
        // public string ResourceEmail { get; set; }
        //public Dictionary<string, string> customFields = new Dictionary<string, string>();
    }

}