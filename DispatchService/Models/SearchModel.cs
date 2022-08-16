using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class SearchModel
    {
        public string division { get; set; }
        public string street { get; set; }
        public string house { get; set; }
        public string flat { get; set; }
        public string[] status { get; set; } = new string[] { };
        public string[] type { get; set; } = new string[] { };
        public string category { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}