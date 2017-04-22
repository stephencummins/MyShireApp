using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyShireAppWeb.Models
{
    public class LaunchFormDefaultValues
    {
        //Lookup fields \

        public List<EPTInfo> LaunchTemplates { get; set; }
        public List<CustomFieldEntryInfo> Brands { get; set; }
        public List<CustomFieldEntryInfo> Countries { get; set; }
        public List<CustomFieldEntryInfo> Indications { get; set; }
        public List<CustomFieldEntryInfo> Regions { get; set; }
        public List<CustomFieldEntryInfo> Clusters { get; set; }
        public List<CustomFieldEntryInfo> Franchises { get; set; }
        public List<PWAResources> PWA_Resources { get; set; }
        
        //public List<CountryWaveMapping> CountryWaveMapping { get; set; }
        //public List<BrandCategoryMapping> BrandCategoryMapping { get; set; }
        //public List<StartLaunchMapping> StarLaunchMapping { get; set; }
        //public List<IPPMilestoneDates> IPPMileStoneDates { get; set; }

    }
}