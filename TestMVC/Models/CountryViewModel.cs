using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestMVC.Models
{
    public class CountryViewModel
    {
        public int CountryId { get; set; }

        public string CountryName { get; set; }

        public int RegionId { get; set; }

        public string RegionName { get; set; }
    }
}