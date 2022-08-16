using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class RequestStep
    {
        [Key]
        public int request_step_id { get; set; }
        public int request_id { get; set; }
        public Request Request { get; set; }
        public int step_id { get; set; }
        public Step Step { get; set; }
        public DateTime date_begin { get; set; }
        public DateTime? date_end { get; set; }
        public int user_id { get; set; }
        public User User { get; set; }
        public string comment { get; set; } = null;
    }
}