using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestMVC.Models
{
    public class Select2PagedResult
    {
        public int Total { get; set; }
        public List<clsList> Results { get; set; }
    }
}