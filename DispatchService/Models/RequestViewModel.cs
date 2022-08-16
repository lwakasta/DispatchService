using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class RequestViewModel
    {
        public int request_id { get; set; }
        public string division { get; set; }
        public string street { get; set; }
        public string house { get; set; }
        public string flat { get; set; }
        public string category { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public DateTime date_begin { get; set; }
        public DateTime? date_end { get; set; }
        public DateTime due_date { get; set; }
        public int days_in_work { get; set; }
        public int due_date_days { get; set; }


        //public TimeSpan time_remain;
        public string color;
        public string border;

        public string step_name { get; set; }
        public DateTime step_date_begin { get; set; }
        public DateTime? step_date_end { get; set; }
        public string implemented_by { get; set; }
        public string user_role { get; set; }
        public string comment { get; set; }
    }
}