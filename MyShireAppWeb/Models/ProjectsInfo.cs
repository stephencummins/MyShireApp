using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MyShireAppWeb.Models
{    
    [DataContract]
    public class ProjectsInfo
    {
        /// <summary>
        /// Project Name
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ProjectName { get; set; }

        /// <summary>
        /// Project Uid
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ProjectUid { get; set; }

        /// <summary>
        /// Project Site URL
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ProjectSiteUrl { get; set; }
    }

}
