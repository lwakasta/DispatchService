using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class Step
    {
        [Key]
        public int step_id { get; set; }
        public string step_name { get; set; }
        public ICollection<RequestStep> RequestSteps { get; set; } = new List<RequestStep>();
    }
}