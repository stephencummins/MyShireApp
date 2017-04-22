using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShireAppWeb.Models
{
    public class CountryWaveMapping
    {
        public string CountryName { get; set; }
        public string Wave { get; set; }
    }

    public class BrandCategoryMapping
    {
        public string BrandName { get; set; }
        public string Category { get; set; }
    }

    public class StartLaunchMapping
    {
        public string BrandName { get; set; }
        public string CountryName { get; set; }
        public string StarLaunch { get; set; }
    }

    public class LaunchPriorityRequest
    {
        public string BrandName { get; set; }
        public string CountryName { get; set; }

    }

    public class IPPMilestoneDateRequest
    {
        public string BrandName { get; set; }
        public string CountryName { get; set; }
        public string Indication { get; set; }

    }


}